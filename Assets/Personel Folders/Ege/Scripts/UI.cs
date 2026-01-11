using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false); // Close settings if open

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // ----- BUTTON FUNCTIONS -----

    public void OnResumeButton()
    {
        ResumeGame();
    }

    public void OnSettingsButton()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBackFromSettingsButton()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void OnMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("main menu"); // Set to your menu scene name
    }
}
