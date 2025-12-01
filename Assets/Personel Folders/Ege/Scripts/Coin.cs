using UnityEngine;

public class Coin : MonoBehaviour
{
    // The value of this coin
    public int coinValue = 10;

    // CHANGED: Use OnTriggerEnter2D for 2D colliders
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is the player/car
        if (other.CompareTag("Player"))
        {
            // 1. Tell the GameManager to add the value
            GameManager.Instance.AddCoins(coinValue); 

            // 2. NEW: Play the coin collection sound effect
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayCoinSound(); 
            }

            // 3. Destroy the coin object so it disappears from the scene
            Destroy(gameObject);
        }
    }
}