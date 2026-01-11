using UnityEngine;

/// <summary>
/// Singleton manager that persists the selected theme across scenes
/// </summary>
public class SelectedThemeManager : MonoBehaviour
{
    private static SelectedThemeManager instance;
    
    public static SelectedThemeManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SelectedThemeManager");
                instance = go.AddComponent<SelectedThemeManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    [Header("Current Selected Theme")]
    private Theme selectedTheme;
    
    [Header("Persistence Settings")]
    [SerializeField] private bool saveToPlayerPrefs = true;
    private const string THEME_PREFS_KEY = "SelectedThemeName";
    
    private void Awake()
    {
        // Ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Load saved theme if available
        if (saveToPlayerPrefs)
        {
            LoadThemeFromPrefs();
        }
    }
    
    /// <summary>
    /// Sets the selected theme
    /// </summary>
    public void SetSelectedTheme(Theme theme)
    {
        if (theme == null)
        {
            Debug.LogWarning("SelectedThemeManager: Attempted to set null theme");
            return;
        }
        
        selectedTheme = theme;
        Debug.Log($"SelectedThemeManager: Theme '{theme.themeName}' selected");
        
        // Save to PlayerPrefs if enabled
        if (saveToPlayerPrefs)
        {
            SaveThemeToPrefs(theme);
        }
    }
    
    /// <summary>
    /// Gets the currently selected theme
    /// </summary>
    public Theme GetSelectedTheme()
    {
        return selectedTheme;
    }
    
    /// <summary>
    /// Checks if a theme has been selected
    /// </summary>
    public bool HasSelectedTheme()
    {
        return selectedTheme != null && selectedTheme.IsValid();
    }
    
    /// <summary>
    /// Clears the selected theme
    /// </summary>
    public void ClearSelectedTheme()
    {
        selectedTheme = null;
        
        if (saveToPlayerPrefs)
        {
            PlayerPrefs.DeleteKey(THEME_PREFS_KEY);
            PlayerPrefs.Save();
        }
        
        Debug.Log("SelectedThemeManager: Theme selection cleared");
    }
    
    /// <summary>
    /// Saves theme name to PlayerPrefs
    /// </summary>
    private void SaveThemeToPrefs(Theme theme)
    {
        PlayerPrefs.SetString(THEME_PREFS_KEY, theme.themeFolderName);
        PlayerPrefs.Save();
        Debug.Log($"SelectedThemeManager: Saved theme '{theme.themeFolderName}' to PlayerPrefs");
    }
    
    /// <summary>
    /// Loads theme from PlayerPrefs (requires reloading theme data)
    /// </summary>
    private void LoadThemeFromPrefs()
    {
        if (PlayerPrefs.HasKey(THEME_PREFS_KEY))
        {
            string themeName = PlayerPrefs.GetString(THEME_PREFS_KEY);
            Debug.Log($"SelectedThemeManager: Found saved theme '{themeName}' in PlayerPrefs");
            
            // Note: The actual theme sprites will need to be reloaded by ThemeLoader
            // This just stores the theme name for reference
        }
    }
    
    /// <summary>
    /// Gets the saved theme folder name from PlayerPrefs
    /// </summary>
    public string GetSavedThemeName()
    {
        if (PlayerPrefs.HasKey(THEME_PREFS_KEY))
        {
            return PlayerPrefs.GetString(THEME_PREFS_KEY);
        }
        return null;
    }
}
