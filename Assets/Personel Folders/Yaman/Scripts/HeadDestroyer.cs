using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadDestroyer : MonoBehaviour
{
    // Tag on the ground layer (make sure your ground has this tag)
    public string groundTag = "Ground";

    // Reference to the main player script to call the DestroyPlayer method
    private RiderLikeController_Full playerController;

    void Awake()
    {
        // Get a reference to the main player script on the parent object
        playerController = GetComponentInParent<RiderLikeController_Full>();
        
        if (playerController == null)
        {
            Debug.LogError("HeadDestroyer must be a child of an object with RiderLikeController_Full!");
        }
        
        // Ensure a collider is present for collision detection
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("HeadCheckPoint needs a Collider2D (like a BoxCollider2D) set to be a Trigger!");
        }
    }

    // Use OnTriggerEnter2D if the HeadCheckPoint's collider is set to 'Is Trigger'
    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckForCrash(other.gameObject);
    }
    
    // Use OnCollisionEnter2D if the HeadCheckPoint's collider is NOT a Trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckForCrash(collision.gameObject);
    }

    private void CheckForCrash(GameObject other)
    {
        // Check if the object we hit is the ground
        if (other.CompareTag(groundTag))
        {
            // Now, we must check the impact direction.
            // We only want to crash if we are moving INTO the ground, typically upwards or stalled,
            // or if the impact is violent enough (e.g., angular velocity is high).
            
            // For a simple check, we can rely on the trigger/collision occurring
            // while assuming the player is rotating or airborne (i.e., not grounded).
            
            // To be more precise, we could check if the player's y-velocity is close to zero or negative, 
            // but for a simple "Rider-like" crash, hitting the top collider is enough.

            playerController.DestroyPlayer();
            SceneManager.LoadScene("asd");
        }
    }
}