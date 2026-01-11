using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class ThemeSelectionSceneBuilder : EditorWindow
{
    [MenuItem("Tools/Create Theme Selection Scene")]
    public static void CreateThemeSelectionScene()
    {
        // Create new scene
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, 
            UnityEditor.SceneManagement.NewSceneMode.Single
        );
        
        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if it doesn't exist
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create Theme Display Image (Center)
        GameObject themeDisplayGO = new GameObject("ThemeDisplay");
        themeDisplayGO.transform.SetParent(canvasGO.transform, false);
        Image themeImage = themeDisplayGO.AddComponent<Image>();
        themeImage.preserveAspect = true;
        
        RectTransform themeRect = themeDisplayGO.GetComponent<RectTransform>();
        themeRect.anchorMin = new Vector2(0.5f, 0.5f);
        themeRect.anchorMax = new Vector2(0.5f, 0.5f);
        themeRect.anchoredPosition = Vector2.zero;
        themeRect.sizeDelta = new Vector2(800, 600);
        
        // Create Left Button
        GameObject leftButtonGO = new GameObject("LeftButton");
        leftButtonGO.transform.SetParent(canvasGO.transform, false);
        Button leftButton = leftButtonGO.AddComponent<Button>();
        Image leftButtonImage = leftButtonGO.AddComponent<Image>();
        leftButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        RectTransform leftRect = leftButtonGO.GetComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0f, 0.5f);
        leftRect.anchorMax = new Vector2(0f, 0.5f);
        leftRect.anchoredPosition = new Vector2(100, 0);
        leftRect.sizeDelta = new Vector2(100, 100);
        
        // Left Button Text
        GameObject leftTextGO = new GameObject("Text");
        leftTextGO.transform.SetParent(leftButtonGO.transform, false);
        Text leftText = leftTextGO.AddComponent<Text>();
        leftText.text = "<";
        leftText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        leftText.fontSize = 36;
        leftText.fontStyle = FontStyle.Bold;
        leftText.alignment = TextAnchor.MiddleCenter;
        leftText.color = Color.white;
        
        RectTransform leftTextRect = leftTextGO.GetComponent<RectTransform>();
        leftTextRect.anchorMin = Vector2.zero;
        leftTextRect.anchorMax = Vector2.one;
        leftTextRect.sizeDelta = Vector2.zero;
        
        // Create Right Button
        GameObject rightButtonGO = new GameObject("RightButton");
        rightButtonGO.transform.SetParent(canvasGO.transform, false);
        Button rightButton = rightButtonGO.AddComponent<Button>();
        Image rightButtonImage = rightButtonGO.AddComponent<Image>();
        rightButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        RectTransform rightRect = rightButtonGO.GetComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(1f, 0.5f);
        rightRect.anchorMax = new Vector2(1f, 0.5f);
        rightRect.anchoredPosition = new Vector2(-100, 0);
        rightRect.sizeDelta = new Vector2(100, 100);
        
        // Right Button Text
        GameObject rightTextGO = new GameObject("Text");
        rightTextGO.transform.SetParent(rightButtonGO.transform, false);
        Text rightText = rightTextGO.AddComponent<Text>();
        rightText.text = ">";
        rightText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rightText.fontSize = 36;
        rightText.fontStyle = FontStyle.Bold;
        rightText.alignment = TextAnchor.MiddleCenter;
        rightText.color = Color.white;
        
        RectTransform rightTextRect = rightTextGO.GetComponent<RectTransform>();
        rightTextRect.anchorMin = Vector2.zero;
        rightTextRect.anchorMax = Vector2.one;
        rightTextRect.sizeDelta = Vector2.zero;
        
        
        // Create Main Menu Button (Top-Left)
        GameObject mainMenuButtonGO = new GameObject("MainMenuButton");
        mainMenuButtonGO.transform.SetParent(canvasGO.transform, false);
        Button mainMenuButton = mainMenuButtonGO.AddComponent<Button>();
        Image mainMenuButtonImage = mainMenuButtonGO.AddComponent<Image>();
        mainMenuButtonImage.color = new Color(0.8f, 0.3f, 0.3f, 1f); // Red color
        
        RectTransform mainMenuRect = mainMenuButtonGO.GetComponent<RectTransform>();
        mainMenuRect.anchorMin = new Vector2(0f, 1f);
        mainMenuRect.anchorMax = new Vector2(0f, 1f);
        mainMenuRect.anchoredPosition = new Vector2(120, -50);
        mainMenuRect.sizeDelta = new Vector2(200, 60);
        
        // Main Menu Button Text
        GameObject mainMenuTextGO = new GameObject("Text");
        mainMenuTextGO.transform.SetParent(mainMenuButtonGO.transform, false);
        Text mainMenuText = mainMenuTextGO.AddComponent<Text>();
        mainMenuText.text = "MAIN MENU";
        mainMenuText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        mainMenuText.fontSize = 20;
        mainMenuText.fontStyle = FontStyle.Bold;
        mainMenuText.alignment = TextAnchor.MiddleCenter;
        mainMenuText.color = Color.white;
        
        RectTransform mainMenuTextRect = mainMenuTextGO.GetComponent<RectTransform>();
        mainMenuTextRect.anchorMin = Vector2.zero;
        mainMenuTextRect.anchorMax = Vector2.one;
        mainMenuTextRect.sizeDelta = Vector2.zero;
        
        
        // Add MainMenuUI component to handle button click
        GameObject mainMenuManagerGO = new GameObject("MainMenuManager");
        MainMenuUI mainMenuUI = mainMenuManagerGO.AddComponent<MainMenuUI>();
        
        // Connect button to MainMenuUI - load gameui scene (main UI)
        mainMenuButton.onClick.AddListener(() => mainMenuUI.LoadScene("gameui"));
        
        
        
        // Create Select Button
        GameObject selectButtonGO = new GameObject("SelectButton");
        selectButtonGO.transform.SetParent(canvasGO.transform, false);
        Button selectButton = selectButtonGO.AddComponent<Button>();
        Image selectButtonImage = selectButtonGO.AddComponent<Image>();
        selectButtonImage.color = new Color(0.2f, 0.7f, 0.3f, 1f); // Green color
        
        RectTransform selectRect = selectButtonGO.GetComponent<RectTransform>();
        selectRect.anchorMin = new Vector2(0.5f, 0.5f);
        selectRect.anchorMax = new Vector2(0.5f, 0.5f);
        selectRect.anchoredPosition = new Vector2(0, -400);
        selectRect.sizeDelta = new Vector2(200, 60);
        
        // Select Button Text
        GameObject selectTextGO = new GameObject("Text");
        selectTextGO.transform.SetParent(selectButtonGO.transform, false);
        Text selectText = selectTextGO.AddComponent<Text>();
        selectText.text = "SELECT";
        selectText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        selectText.fontSize = 24;
        selectText.fontStyle = FontStyle.Bold;
        selectText.alignment = TextAnchor.MiddleCenter;
        selectText.color = Color.white;
        
        RectTransform selectTextRect = selectTextGO.GetComponent<RectTransform>();
        selectTextRect.anchorMin = Vector2.zero;
        selectTextRect.anchorMax = Vector2.one;
        selectTextRect.sizeDelta = Vector2.zero;
        
        // Create ThemeSelectionManager
        GameObject managerGO = new GameObject("ThemeSelectionManager");
        ThemeSelectionUI themeSelectionUI = managerGO.AddComponent<ThemeSelectionUI>();
        
        // Use reflection to set private fields
        var themeDisplayField = typeof(ThemeSelectionUI).GetField("themeDisplayImage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var leftButtonField = typeof(ThemeSelectionUI).GetField("leftButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rightButtonField = typeof(ThemeSelectionUI).GetField("rightButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var selectButtonField = typeof(ThemeSelectionUI).GetField("selectButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        themeDisplayField?.SetValue(themeSelectionUI, themeImage);
        leftButtonField?.SetValue(themeSelectionUI, leftButton);
        rightButtonField?.SetValue(themeSelectionUI, rightButton);
        selectButtonField?.SetValue(themeSelectionUI, selectButton);
        
        // Note: ParallaxManager is not needed in theme selection scene
        // It will be used in the gameplay scene via ThemeAwareParallaxController
        
        
        // Mark scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        
        // Save scene
        string scenePath = "Assets/Personel Folders/Umut/Scenes/ThemeSelectionScene.unity";
        string sceneDir = Path.GetDirectoryName(scenePath);
        if (!Directory.Exists(sceneDir))
        {
            Directory.CreateDirectory(sceneDir);
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
        
        Debug.Log("Theme Selection Scene created successfully at: " + scenePath);
        Debug.Log("Themes will be auto-loaded from the Themes folder when you play the scene!");
        
        // Select the manager so user can configure it
        Selection.activeGameObject = managerGO;
    }
}
