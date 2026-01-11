using UnityEngine;
using UnityEngine.SceneManagement; // ⚠️ MUST have this to load scenes!

public class MainMenuUI : MonoBehaviour
{
    // This function takes a text parameter so you can reuse it for ALL buttons
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}