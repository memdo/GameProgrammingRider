using UnityEngine;

/// <summary>
/// Manages parallax background layers and applies selected themes
/// </summary>
public class ParallaxManager : MonoBehaviour
{
    [Header("Parallax Settings")]
    [SerializeField] private Transform parallaxContainer;
    [SerializeField] private float layerZOffset = 1f;

    [Header("Layer Prefab (Optional)")]
    [SerializeField] private GameObject parallaxLayerPrefab;

    [Header("Theme Loading")]
    [SerializeField] private ThemeLoader themeLoader;

    private GameObject[] currentLayers;

    private void Start()
    {
        // 1. Seçili Temanın ismini öğren
        string themeName = "";

        if (SelectedThemeManager.Instance != null)
        {
            themeName = SelectedThemeManager.Instance.GetSavedThemeName();
        }

        // Varsayılan tema ismi (eğer kayıt yoksa)
        if (string.IsNullOrEmpty(themeName))
        {
            themeName = "a";
        }

        // 2. ThemeLoader'ı bul
        if (themeLoader == null) themeLoader = GetComponent<ThemeLoader>();
        if (themeLoader == null) themeLoader = FindObjectOfType<ThemeLoader>();

        // 3. Temayı Yükle ve Uygula
        if (themeLoader != null)
        {
            // --- ÖNEMLİ: ThemeLoader.cs içinde bu metodun PUBLIC olması lazım! ---
            Theme loadedTheme = themeLoader.LoadThemeFromResources(themeName);

            if (loadedTheme != null)
            {
                ApplyTheme(loadedTheme);
            }
            else
            {
                Debug.LogError($"ParallaxManager: '{themeName}' teması Resources'tan yüklenemedi!");
            }
        }
        else
        {
            // Loader yoksa eski yöntem
            if (SelectedThemeManager.Instance != null && SelectedThemeManager.Instance.GetSelectedTheme() != null)
            {
                ApplyTheme(SelectedThemeManager.Instance.GetSelectedTheme());
            }
        }
    }

    /// <summary>
    /// Applies a theme's parallax layers to the scene
    /// </summary>
    public void ApplyTheme(Theme theme)
    {
        if (theme == null || !theme.IsValid())
        {
            Debug.LogError("ParallaxManager: Cannot apply invalid theme");
            return;
        }

        ClearLayers();

        if (parallaxContainer == null)
        {
            GameObject containerGO = new GameObject("ParallaxContainer");
            parallaxContainer = containerGO.transform;
        }

        currentLayers = new GameObject[theme.parallaxLayers.Length];

        for (int i = 0; i < theme.parallaxLayers.Length; i++)
        {
            GameObject layerGO = CreateParallaxLayer(theme.parallaxLayers[i], i);
            layerGO.name = $"ParallaxLayer_{i + 1}";
            layerGO.transform.SetParent(parallaxContainer, false);

            Vector3 pos = layerGO.transform.localPosition;
            pos.z = i * layerZOffset;
            layerGO.transform.localPosition = pos;

            currentLayers[i] = layerGO;
        }

        // istersen buraya sky ekle...

        Debug.Log($"ParallaxManager: Applied theme '{theme.themeName}' with {theme.GetLayerCount()} layers");
    }

    private GameObject CreateParallaxLayer(Sprite sprite, int layerIndex)
    {
        GameObject layerGO;

        if (parallaxLayerPrefab != null)
        {
            layerGO = Instantiate(parallaxLayerPrefab);
        }
        else
        {
            layerGO = new GameObject();
            layerGO.AddComponent<SpriteRenderer>();
        }

        SpriteRenderer spriteRenderer = layerGO.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = -(layerIndex);
        }

        return layerGO;
    }

    private void ClearLayers()
    {
        if (currentLayers != null)
        {
            foreach (GameObject layer in currentLayers)
            {
                if (layer != null)
                    Destroy(layer);
            }
            currentLayers = null;
        }
    }

    private void OnDestroy()
    {
        ClearLayers();
    }
}