using UnityEngine;

/// <summary>
/// Runtime theme data class - loaded dynamically from the Themes folder
/// </summary>
[System.Serializable]
public class Theme
{
    public string themeName;
    public string themeFolderName;
    public Sprite representationSprite;
    public Sprite skySprite; // Optional sky background (sky.png)
    public Sprite[] parallaxLayers;
    public Sprite[] cloudLayers; // Cloud layers (cloud1.png, cloud2.png, etc.)

    public int price; // Temanýn Fiyatý

    public Theme(string name, string folderName)
    {
        this.themeName = name;
        this.themeFolderName = folderName;
    }
    
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
    
    /// <summary>
    /// Gets the number of cloud layers in this theme
    /// </summary>
    public int GetCloudCount()
    {
        return cloudLayers != null ? cloudLayers.Length : 0;
    }
}
