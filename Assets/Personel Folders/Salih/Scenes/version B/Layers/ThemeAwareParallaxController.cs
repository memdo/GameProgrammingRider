using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enhanced parallax controller that loads selected theme sprites dynamically.
/// Implements a structured layering system: 
/// 1. Static Sky (Background)
/// 2. Constant Drift Clouds (Middle)
/// 3. Camera-Relative Parallax (Foreground)
/// </summary>
public class ThemeAwareParallaxController : MonoBehaviour
{
    Transform cam;              // Main Camera
    Vector3 camStartPos;

    [Header("Parallax Settings")]
    [Range(0.01f, 0.1f)]
    public float parallaxSpeed = 0.05f;
    [SerializeField] private float layerZSpacing = 1.0f;
    [SerializeField] private Vector3 layerScale = new Vector3(3f, 3f, 1f);

    [Header("Cloud Settings")]
    [SerializeField] private float cloudScrollSpeed = 0.08f; // Slightly faster base speed
    [SerializeField] private float cloudSpeedVariation = 0.3f; // More variation
    [SerializeField] private float cloudBaseZ = 50f;
    
    [Header("Theme Loading")]
    [SerializeField] private bool loadFromSelectedTheme = true;
    
    // Internal State
    private GameObject[] backgrounds;
    private Material[] mat;
    private float[] backSpeed;
    
    private GameObject skyGO;
    private Material skyMat;
    
    private GameObject[] clouds;
    private Material[] cloudMat;

    private ThemeLoader themeLoader;
    private float cloudScrollOffset = 0f;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        if (loadFromSelectedTheme)
        {
            if (!LoadSelectedTheme())
            {
                Debug.LogWarning("ThemeAwareParallaxController: No theme selected, falling back.");
            }
        }
    }

    private bool LoadSelectedTheme()
    {
        Theme themeToLoad = null;
        
        // 1. Check session selection
        if (SelectedThemeManager.Instance.HasSelectedTheme())
        {
            themeToLoad = SelectedThemeManager.Instance.GetSelectedTheme();
        }
        else
        {
            // 2. Check PlayerPrefs
            string savedThemeName = SelectedThemeManager.Instance.GetSavedThemeName();
            if (!string.IsNullOrEmpty(savedThemeName))
            {
                themeToLoad = LoadThemeByName(savedThemeName);
            }
            
            // 3. Default to 'a'
            if (themeToLoad == null) themeToLoad = LoadThemeByName("a");
        }
        
        if (themeToLoad == null || !themeToLoad.IsValid()) return false;
        
        ClearExistingLayers();
        
        // --- CREATE LAYERS IN ORDER ---
        
        // 1. Sky (Absolute Back)
        if (themeToLoad.skySprite != null) CreateSkyLayer(themeToLoad);
        
        // 2. Parallax (Main Layers)
        CreateParallaxLayers(themeToLoad);
        
        // 3. Clouds (In Front of Sky, Behind Parallax)
        if (themeToLoad.cloudLayers != null && themeToLoad.cloudLayers.Length > 0)
        {
            CreateCloudLayers(themeToLoad);
        }
        
        return true;
    }

    private Theme LoadThemeByName(string themeFolderName)
    {
        if (themeLoader == null)
        {
            themeLoader = GetComponent<ThemeLoader>() ?? gameObject.AddComponent<ThemeLoader>();
        }
        
        // Android Fix: Use Resources.Load directly instead of LoadAllThemes which fails on Android
        return themeLoader.LoadThemeFromResources(themeFolderName);
    }

    private void CreateSkyLayer(Theme theme)
    {
        skyGO = new GameObject($"Sky_{theme.themeFolderName}");
        skyGO.transform.SetParent(transform);
        skyGO.transform.localPosition = new Vector3(0, 0, 100f); // Very far back
        
        MeshFilter mf = skyGO.AddComponent<MeshFilter>();
        MeshRenderer mr = skyGO.AddComponent<MeshRenderer>();
        mf.mesh = CreateQuadMesh();
        
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;
        skyGO.transform.localScale = new Vector3(screenWidth * layerScale.x, screenHeight * layerScale.y, 1f);
        
        skyMat = new Material(Shader.Find("Unlit/Transparent"));
        skyMat.mainTexture = theme.skySprite.texture;
        mr.material = skyMat;
        mr.sortingOrder = -500; // Furthest render order
    }

    private void CreateParallaxLayers(Theme theme)
    {
        int count = theme.parallaxLayers.Length;
        backgrounds = new GameObject[count];
        mat = new Material[count];
        backSpeed = new float[count];
        
        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject($"Layer_{i + 1}");
            go.transform.SetParent(transform);
            
            // Z-Depth: 0 (near) to N (far)
            float zPos = i * layerZSpacing;
            go.transform.localPosition = new Vector3(0, 0, zPos);
            
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mf.mesh = CreateQuadMesh();
            
            float screenHeight = Camera.main.orthographicSize * 2f;
            float screenWidth = screenHeight * Camera.main.aspect;
            go.transform.localScale = new Vector3(screenWidth * layerScale.x, screenHeight * layerScale.y, 1f);
            
            Material material = new Material(Shader.Find("Unlit/Transparent"));
            Texture2D tex = theme.parallaxLayers[i].texture;
            tex.wrapMode = TextureWrapMode.Repeat;
            material.mainTexture = tex;
            mr.material = material;
            
            mr.sortingOrder = -i; // Nearest layers at order 0, further at -1, -2...
            
            backgrounds[i] = go;
            mat[i] = material;
        }
        
        CalculateParallaxSpeeds(count);
    }

    private void CreateCloudLayers(Theme theme)
    {
        int count = theme.cloudLayers.Length;
        clouds = new GameObject[count];
        cloudMat = new Material[count];
        
        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject($"Cloud_{i + 1}");
            go.transform.SetParent(transform);
            
            // Middle Depth: Between Sky (100) and Parallax (0-20)
            float zPos = cloudBaseZ - (i * 1.0f);
            go.transform.localPosition = new Vector3(0, 0, zPos);
            
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mf.mesh = CreateQuadMesh();
            
            float screenHeight = Camera.main.orthographicSize * 2f;
            float screenWidth = screenHeight * Camera.main.aspect;
            go.transform.localScale = new Vector3(screenWidth * layerScale.x, screenHeight * layerScale.y, 1f);
            
            Material material = new Material(Shader.Find("Unlit/Transparent"));
            Texture2D tex = theme.cloudLayers[i].texture;
            tex.wrapMode = TextureWrapMode.Repeat;
            material.mainTexture = tex;
            mr.material = material;
            
            mr.sortingOrder = -200 - i; // Behind parallax
            
            clouds[i] = go;
            cloudMat[i] = material;
        }
    }

    private void CalculateParallaxSpeeds(int count)
    {
        if (count <= 0) return;
        if (count == 1) { backSpeed[0] = 1.0f; return; }

        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        for (int i = 0; i < count; i++)
        {
            float z = backgrounds[i].transform.localPosition.z;
            if (z < minZ) minZ = z;
            if (z > maxZ) maxZ = z;
        }

        for (int i = 0; i < count; i++)
        {
            float z = backgrounds[i].transform.localPosition.z;
            float t = (z - minZ) / (maxZ - minZ);
            // Near (0) = speed 1.0, Far (N) = speed 0.2
            backSpeed[i] = Mathf.Lerp(1.0f, 0.2f, t);
        }
    }

    private Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(-0.5f, -0.5f, 0), new Vector3(0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 0) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        mesh.RecalculateNormals();
        return mesh;
    }

    private void ClearExistingLayers()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    void LateUpdate()
    {
        // Container follows Camera
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);

        // 1. Move Parallax (Character-Relative Displacement)
        float distX = cam.position.x - camStartPos.x;
        if (backgrounds != null)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                if (mat[i] != null)
                {
                    float offset = distX * (backSpeed[i] * parallaxSpeed);
                    mat[i].SetTextureOffset("_MainTex", new Vector2(offset, 0f));
                }
            }
        }

        // 2. Move Clouds (Constant Camera-Relative Drift)
        if (clouds != null)
        {
            for (int i = 0; i < clouds.Length; i++)
            {
                if (cloudMat[i] != null)
                {
                    // Drift independent of character speed
                    float layerVar = 1.0f + (i * cloudSpeedVariation);
                    float offset = Time.time * cloudScrollSpeed * layerVar;
                    cloudMat[i].SetTextureOffset("_MainTex", new Vector2(offset, 0f));
                }
            }
        }
    }
}
