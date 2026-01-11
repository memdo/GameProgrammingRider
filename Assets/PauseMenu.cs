using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseUI;
    public GameObject settingsUI;

    private void Start()
    {
        pauseUI.SetActive(false);
        settingsUI.SetActive(false);
        Time.timeScale = 1f; // Make sure game starts unpaused
    }

    // Called when pause button is pressed
    public void OpenPauseMenu()
    {
        Time.timeScale = 0f;      // ⏸ Pause game
        pauseUI.SetActive(true);
        settingsUI.SetActive(false);
    }

    // Continue Button
    public void ContinueGame()
    {
        Time.timeScale = 1f;      // ▶ Resume game
        pauseUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    // Settings Button
    public void OpenSettings()
    {
        pauseUI.SetActive(false);
        settingsUI.SetActive(true);
        // Time.timeScale stays 0 (game remains paused)
    }

    // Back Button in Settings
    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        pauseUI.SetActive(true);
        // Time.timeScale stays 0 (still paused)
    }

    // Restart Button
    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause before reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        // 1. IMPORTANT: Unpause the game time!
        // If you don't do this, the Main Menu might be frozen when it loads.
        Time.timeScale = 1f;

        // 2. Load your specific menu scene
        SceneManager.LoadScene("gameui");
    }
}
