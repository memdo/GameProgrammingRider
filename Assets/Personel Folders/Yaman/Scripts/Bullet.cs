using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Damage")]
    public int damageToDeal = 1; // Damage this bullet inflicts
    public string targetTag = "Enemy"; // Tag the turret GameObject should have

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has the target tag (e.g., "Enemy")
        if (collision.CompareTag(targetTag))
        {
            // Try to get the Health component from the collided object
            Health healthComponent = collision.GetComponent<Health>();

            if (healthComponent != null)
            {
                // Deal damage to the turret
                healthComponent.TakeDamage(damageToDeal);
            }

            // Destroy the bullet after hitting something (optional, depending on your game)
            Destroy(gameObject);
        }
    }
    
    // Use OnCollisionEnter2D instead if your bullets have a Rigidbody2D and Collider2D set to non-trigger
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // ... same logic as above but use collision.gameObject for the object
    // }
}