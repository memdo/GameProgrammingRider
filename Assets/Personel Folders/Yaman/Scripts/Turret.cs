using UnityEngine;

[RequireComponent(typeof(Health))] 
public class Turret : MonoBehaviour
{
    [Header("Targeting")]
    // **CRITICAL: Assign the Player's Transform here in the Inspector**
    public Transform target; 
    public float range = 10f; 
    public float rotationSpeed = 5f; 

    [Header("Shooting")]
    // **CRITICAL: Assign a child Transform for the bullet spawn point**
    public Transform firePoint; 
    public GameObject turretBulletPrefab; 
    public float bulletSpeed = 10f; 
    public float fireRate = 2f; 
    public float bulletLifetime = 5f; 
    
    private float nextFireTime = 0f;

    void Awake()
    {
        // 1. Safety Check for Target
        if (target == null)
        {
            // Try to find the player only once at the start
            var player = GameObject.FindObjectOfType<RiderLikeController_Full>();
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("TurretController: Player (RiderLikeController_Full) not found! Turret will be disabled.");
                enabled = false; // Disable the script if no target is found
                return;
            }
        }
    }

    void Update()
    {
        // Null check for target to prevent crashes
        if (target == null) 
        {
            // If the target was destroyed, stop trying to do work
            enabled = false;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= range)
        {
            AimAtTarget();
            ShootAtTarget();
        }
    }

    void AimAtTarget()
    {
        // Calculate the direction vector from the turret's position to the target's position
        Vector3 direction = target.position - transform.position;
        
        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Create the target rotation. We use -90 if the default "forward" for your turret 
        // sprite is facing up (which is common). If it faces right, use 0.
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // Smoothly rotate the turret using Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void ShootAtTarget()
    {
        // Null check for firePoint and prefab to prevent errors
        if (firePoint == null || turretBulletPrefab == null) 
        {
            Debug.LogError("TurretController: FirePoint or Bullet Prefab is not assigned!");
            return;
        }

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            
            GameObject b = Instantiate(turretBulletPrefab, firePoint.position, firePoint.rotation);
            
            Rigidbody2D rbBullet = b.GetComponent<Rigidbody2D>();

            // Ensure the bullet has an Rigidbody2D before trying to set its velocity
            if (rbBullet != null)
                // The bullet fires in the direction the turret is currently rotated (firePoint.right)
                rbBullet.linearVelocity = firePoint.right * bulletSpeed;
            else
                Debug.LogError("Turret Bullet Prefab is missing a Rigidbody2D!");
            
            Destroy(b, bulletLifetime);
        }
    }
}