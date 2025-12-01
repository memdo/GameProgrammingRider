using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCheck : MonoBehaviour
{
    // The name tag of the ground object
    private const string GROUND_TAG = "Ground"; 
    
    // Check for collision with the ground (using the 2D physics system)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the head collided with the Ground object
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            // You might add logic here to check the impact angle or force if you want more control,
            // but for a basic game over, simple collision is enough.

            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over: Driver Head Touched Ground!");

        // 1. Save the coins collected during this run
        // Ensure GameManager.Instance is initialized before calling EndRun
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndRun();
        }
        
        // 2. Load the main menu scene or display a Game Over UI
        SceneManager.LoadScene("main menu"); // Assuming your main menu scene is named "Main Menu"
        
        // OR simply display a Game Over UI panel if you prefer to stay in the scene
        // gameOverUIPanel.SetActive(true); 
    }
}