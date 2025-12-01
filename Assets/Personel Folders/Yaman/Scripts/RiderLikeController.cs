using UnityEngine;
using UnityEngine.InputSystem; // new Input System

[RequireComponent(typeof(Rigidbody2D))]
public class RiderLikeController_Full : MonoBehaviour
{
    [Header("Destruction Check")] // New Header
    public Transform headCheckPoint; // Reference to the HeadCheckPoint GameObject
    [Header("Shooting")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 5f; // bullets per second (COOLDOWN: affects how quickly you can press 'A' again)
    public float bulletLifetime = 3f;
    float nextFireTime = 0f;

    // *** MODIFIED CACHE: Now stores if the 'A' key was pressed this frame ***
    bool _fireTriggeredCache = false;
    Vector2 _mouseWorldPositionCache;

    [Header("Forward Speed")]
    public float targetForwardSpeed = 5f;

    [Header("Flipping (mouse left hold)")]
    public float flipTorque = 2f;        // torque applied while holding left mouse
    public bool flipClockwise = true;      // choose sign of torque
    public float maxAngularVelocity = 2f; // degrees/sec clamp

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;
    public float groundBuffer = 0.05f; // small grace window to consider grounded

    [Header("Landing Snap")]
    public float snapRotationSpeed = 20f; // degrees/sec when snapping upright on ground

    [Header("Debug")]
    public bool debugInput = true;
    public bool debugPhysics = false;

    Rigidbody2D rb;
    float groundTimer = 0f;
    bool isGrounded = false;

    // cached input for FixedUpdate
    bool _leftHeldCache = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheckPoint == null) groundCheckPoint = transform;
    }

    void Update()
    {
        // Input reading
        bool leftHeld = false;
        // *** MODIFIED: Check for a single press of the 'A' key ***
        bool fireTriggered = false; 
        Vector2 screenMousePos = Vector2.zero;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        // MOUSE LEFT (pressed/held) - Used for flipping
        if (mouse != null)
        {
            leftHeld = mouse.leftButton.isPressed;
            screenMousePos = mouse.position.ReadValue();
        }
        else // Fallback to legacy input system
        {
            leftHeld = Input.GetMouseButton(0);
            screenMousePos = Input.mousePosition;
        }

        // 'A' KEY (pressed once) - Used for shooting
        if (keyboard != null)
            // *** NEW INPUT: Check for a single press this frame ***
            fireTriggered = keyboard.aKey.wasPressedThisFrame;
        else
            // *** NEW INPUT (Legacy): Check for a single press down ***
            fireTriggered = Input.GetKeyDown(KeyCode.A);
        
        // Calculate the world position of the mouse cursor (for aiming)
        if (Camera.main != null)
        {
            _mouseWorldPositionCache = Camera.main.ScreenToWorldPoint(screenMousePos);
        }
        else
        {
            Debug.LogError("Main Camera not found! Ensure your Camera is tagged 'MainCamera'.");
        }
        
        // debug input logs (optional)
        if (debugInput)
        {
            if (leftHeld) Debug.Log("[INPUT] Left Mouse held");
            if (fireTriggered) Debug.Log("[INPUT] 'A' key pressed once to shoot"); // MODIFIED Log
        }

        // cache input for FixedUpdate
        _leftHeldCache = leftHeld;
        _fireTriggeredCache = fireTriggered; // *** MODIFIED Cache ***
    }

    void FixedUpdate()
    {
        HandleShooting();

        CheckGrounded();

        HandleForwardSpeed();

        HandleFlip(_leftHeldCache);

        // Debug state
        if (debugPhysics)
            Debug.Log($"[PHYS] grounded={isGrounded} vel=({rb.linearVelocity.x:F2},{rb.linearVelocity.y:F2}) angVel={rb.angularVelocity:F1}");
    }

    public void DestroyPlayer()
    {
    // 1. Disable the controller and collider so it stops moving and colliding
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        enabled = false;
    
    // Optional: Get the BoxCollider2D on this object and disable it
    // GetComponent<BoxCollider2D>().enabled = false;

    // Optional: Add visual effects (like an explosion or particle effect)
    // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Debug.Log($"{gameObject.name} crashed head-first! Destroying...");
    
    // 2. Destroy the GameObject (or replace with a respawn system)
        Destroy(gameObject);
    }

    void HandleShooting()
    {
        if (firePoint == null || bulletPrefab == null)
            return;

        // Only shoot if the 'A' key was pressed this frame
        if (!_fireTriggeredCache) 
            return;

        // Cooldown check
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate); // Sets the cooldown time
            Shoot();
        }
    }

    void Shoot()
    {
        // Calculate direction from firePoint to mouse world position
        Vector3 direction = (_mouseWorldPositionCache - (Vector2)firePoint.position).normalized;

        // Calculate rotation (angle in Z-axis) from the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        // Spawn bullet with the calculated rotation
        GameObject b = Instantiate(bulletPrefab, firePoint.position, rotation);

        // Grab rb
        Rigidbody2D rbBullet = b.GetComponent<Rigidbody2D>();

        // Bullet moves forward in the direction of the mouse
        if (rbBullet != null)
            rbBullet.linearVelocity = direction * bulletSpeed;

        // Destroy after X seconds
        Destroy(b, bulletLifetime);
    }
    
    // -------------------------
    // FORWARD SPEED (keeps at least target speed)
    // -------------------------
    void HandleForwardSpeed()
    {
        // Removed: if (recentlyBoosted) return;
        
        Vector2 v = rb.linearVelocity;
        if (v.x < targetForwardSpeed)
            v.x = targetForwardSpeed;
        rb.linearVelocity = v;
    }

    // -------------------------
    // FLIP (left mouse hold while airborne)
    // -------------------------
    void HandleFlip(bool leftHeld)
    {
        if (!leftHeld) return;
        if (isGrounded) return; // only flip in air

        float sign = flipClockwise ? -1f : 1f; // Unity torque sign choice
        float torqueToApply = flipTorque * sign;

        // Apply torque (continuous while holding).
        rb.AddTorque(torqueToApply, ForceMode2D.Force);

        // clamp angular velocity so it doesn't spin out
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    // -------------------------
    // GROUND CHECK
    // -------------------------
    void CheckGrounded()
    {
        bool hit = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        if (hit)
        {
            groundTimer = groundBuffer;
            isGrounded = true;
        }
        else
        {
            if (groundTimer > 0f)
            {
                groundTimer -= Time.fixedDeltaTime;
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        // If grounded, gently snap upright and zero angular velocity
        if (isGrounded)
        {
            float current = rb.rotation;
            float desired = 0f;
            float maxRotate = snapRotationSpeed * Time.fixedDeltaTime;
            float next = Mathf.MoveTowardsAngle(current, desired, maxRotate);
            rb.MoveRotation(next);
            rb.angularVelocity = 0f;
        }
    }

    // -------------------------
    // Gizmos for debugging
    // -------------------------
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}