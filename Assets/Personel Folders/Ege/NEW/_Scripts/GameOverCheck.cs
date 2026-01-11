using UnityEngine;
using UnityEngine.SceneManagement;





public class GameOverCheck : MonoBehaviour
{
    // The name tag of the ground object
    private const string GROUND_TAG = "Ground"; 
    private const string GROUND_TAG2 = "taban75";
    public GameObject turboButton;

    void Start()
    {
        // "BoostButton" ismimi butonun sahnedeki GER�EK ADIYLA de�i�tir.
        turboButton = GameObject.Find("BoostButton");
    }

    private void Awake()
   {
        Time.timeScale = 1f;  
   }

   // Check for collision with the ground (using the 2D physics system)
   private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the head collided with the Ground object
        if (collision.gameObject.CompareTag(GROUND_TAG) || collision.gameObject.CompareTag(GROUND_TAG2))
        {
            // You might add logic here to check the impact angle or force if you want more control,
            // but for a basic game over, simple collision is enough.

            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
{
    Debug.Log("Game Over: Driver Head Touched Ground!");

    // Save coins
    if (GameManager.Instance != null)
    {
        GameManager.Instance.EndRun();
    }

    // Slow motion
    Time.timeScale = 0.5f;

    // Trigger camera zoom
    CameraZoom camZoom = Camera.main.GetComponent<CameraZoom>();
    if (camZoom != null)
    {
        camZoom.StartZoom();
    }

    // Delay game over screen
    Invoke(nameof(LoadGameOver), 1f);
}


    private void LoadGameOver()
   {
        // 1. Canvas'� bul (Sahnedeki Canvas isminin "Canvas" oldu�undan emin ol)
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            // 2. Kapal� olan GameOverPanel'i bul
            Transform panel = canvas.transform.Find("GameOverPanel");
            Transform mainmenu = canvas.transform.Find("main menu");
            Transform retry = canvas.transform.Find("retry");
            Transform GameOverText = canvas.transform.Find("GameOverText");
            Transform MainMenuUI = canvas.transform.Find("MainMenuUI");
            Transform StatisticsText = canvas.transform.Find("StatisticsText");
            Transform StatisticsImage = canvas.transform.Find("StatisticsImage");


            if (panel != null)
            {
                turboButton.SetActive(false);
                panel.gameObject.SetActive(true); // Paneli a�
                mainmenu.gameObject.SetActive(true);
                retry.gameObject.SetActive(true);
                GameOverText.gameObject.SetActive(true);
                MainMenuUI.gameObject.SetActive(true);
                StatisticsText.gameObject.SetActive(true);
                StatisticsImage.gameObject.SetActive(true);
                Time.timeScale = 0f; // Oyunu tamamen dondur
            }
            else
            {
                Debug.LogError("GameOverPanel bulunamad�! �smini kontrol et.");
            }
        }
    }
}