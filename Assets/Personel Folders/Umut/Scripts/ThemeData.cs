using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData", menuName = "Game/Theme Data", order = 1)]
public class ThemeData : ScriptableObject
{
    [Header("Theme Information")]
    public string themeName;
    
    [Header("Visual Assets")]
    [Tooltip("The main representation image shown in the theme selection screen")]
    public Sprite representationSprite;
    
    [Tooltip("Parallax layers ordered from nearest (index 0) to farthest")]
    public Sprite[] parallaxLayers;
    
    [Header("Optional Settings")]
    public Color themeColor = Color.white;
    
    /// <summary>
    /// Validates that the theme has all required data
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(themeName) && 
               representationSprite != null && 
               parallaxLayers != null && 
               parallaxLayers.Length > 0;
    }
    
    /// <summary>
    /// Gets the number of parallax layers in this theme
    /// </summary>
    public int GetLayerCount()
    {
        return parallaxLayers != null ? parallaxLayers.Length : 0;
    }
}
