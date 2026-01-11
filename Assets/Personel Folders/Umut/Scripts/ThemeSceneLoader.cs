using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Helper script to load the Theme Selection scene from main menu
/// Attach to a GameObject and connect to your theme button's onClick event
/// </summary>
public class ThemeSceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads the Theme Selection scene
    /// Call this from your theme button's onClick event
    /// </summary>
    public void LoadThemeSelectionScene()
    {
        Debug.Log("Loading Theme Selection Scene...");
        SceneManager.LoadScene("ThemeSelectionScene");
    }
}
