using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ThemeSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image themeDisplayImage;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button selectButton;

    // YENÝ: Toplam Parayý Gösteren Text
    [SerializeField] private Text totalCoinsDisplay;

    [Header("Select Button Sprites")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite buyButtonSprite;
    [SerializeField] private Sprite selectButtonSprite;
    [SerializeField] private Sprite selectedButtonSprite;

    [Header("Price & Lock Visuals")]
    [SerializeField] private Text priceText;
    [SerializeField] private GameObject coinIcon;
    [SerializeField] private GameObject lockIcon;

    [Header("Animation Settings")]
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Optional UI Elements")]
    [SerializeField] private Text themeNameText;

    [Header("Events")]
    public UnityEvent<Theme> OnThemeSelected;

    private Theme[] themes;
    private int currentThemeIndex = 0;
    private bool isTransitioning = false;
    private ThemeLoader themeLoader;

    private void Start()
    {
        themeLoader = GetComponent<ThemeLoader>();
        if (themeLoader == null) themeLoader = gameObject.AddComponent<ThemeLoader>();

        themes = themeLoader.LoadAllThemes();

        if (themes == null || themes.Length == 0) return;

        if (leftButton != null) leftButton.onClick.AddListener(OnLeftButtonClicked);
        if (rightButton != null) rightButton.onClick.AddListener(OnRightButtonClicked);
        if (selectButton != null) selectButton.onClick.AddListener(OnSelectButtonClicked);

        currentThemeIndex = FindSelectedThemeIndex();

        // Ýlk açýlýþta parayý ve temayý güncelle
        UpdateCoinDisplay();
        DisplayCurrentTheme(false);
    }

    private int FindSelectedThemeIndex()
    {
        // Önce kayýtlý isme bakalým
        string savedName = "";

        if (SelectedThemeManager.Instance != null)
        {
            savedName = SelectedThemeManager.Instance.GetSavedThemeName();
        }

        // Eðer kayýt yoksa varsayýlan (örneðin "a" veya "0_Default")
        if (string.IsNullOrEmpty(savedName)) savedName = "a";
        for (int i = 0; i < themes.Length; i++)
        {
            if (themes[i].themeFolderName == savedName)
            {
                // BULDUK! 
                // ÖNEMLÝ: Manager'a "Bak bu tema seçili, haberin olsun" diyoruz.
                // Böylece IsThemeSelected fonksiyonu 'true' dönecek ve buton yeþil olacak.
                SelectedThemeManager.Instance.SetSelectedTheme(themes[i]);
                return i;
            }
        }

        return 0;
    }

    private void OnLeftButtonClicked()
    {
        if (isTransitioning) return;
        currentThemeIndex--;
        if (currentThemeIndex < 0) currentThemeIndex = themes.Length - 1;
        DisplayCurrentTheme(true);
    }

    private void OnRightButtonClicked()
    {
        if (isTransitioning) return;
        currentThemeIndex++;
        if (currentThemeIndex >= themes.Length) currentThemeIndex = 0;
        DisplayCurrentTheme(true);
    }

    private void OnSelectButtonClicked()
    {
        Theme currentTheme = themes[currentThemeIndex];
        bool isUnlocked = IsThemeUnlocked(currentTheme);
        bool isSelected = IsThemeSelected(currentTheme);

        if (isSelected) return;

        if (isUnlocked)
        {
            SelectTheme(currentTheme);
        }
        else
        {
            TryBuyTheme(currentTheme);
        }
    }

    private void TryBuyTheme(Theme theme)
    {
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);

        if (currentCoins >= theme.price)
        {
            currentCoins -= theme.price;
            PlayerPrefs.SetInt("TotalCoins", currentCoins);
            PlayerPrefs.SetInt("ThemeUnlocked_" + theme.themeFolderName, 1);
            PlayerPrefs.Save();

            Debug.Log($"Tema '{theme.themeName}' satýn alýndý!");

            UpdateCoinDisplay(); // Parayý güncelle
            UpdateUIState(theme);
        }
        else
        {
            Debug.Log("Yetersiz Bakiye!");
        }
    }

    private void SelectTheme(Theme theme)
    {
        SelectedThemeManager.Instance.SetSelectedTheme(theme);
        OnThemeSelected?.Invoke(theme);
        UpdateUIState(theme);
        Debug.Log($"Tema '{theme.themeName}' seçildi.");
    }

    private void DisplayCurrentTheme(bool animate)
    {
        if (currentThemeIndex < 0 || currentThemeIndex >= themes.Length) return;
        Theme theme = themes[currentThemeIndex];

        if (animate) StartCoroutine(AnimateThemeTransition(theme));
        else ApplyTheme(theme);
    }

    private void ApplyTheme(Theme theme)
    {
        if (themeDisplayImage != null) themeDisplayImage.sprite = theme.representationSprite;
        if (themeNameText != null) themeNameText.text = theme.themeName;

        UpdateUIState(theme);
    }

    private void UpdateUIState(Theme theme)
    {
        bool isUnlocked = IsThemeUnlocked(theme);
        bool isSelected = IsThemeSelected(theme);

        if (isSelected)
        {
            // DURUM: SEÇÝLÝ
            if (buttonImage && selectedButtonSprite) buttonImage.sprite = selectedButtonSprite;

            if (priceText) priceText.text = "";
            if (coinIcon) coinIcon.SetActive(false);
            if (lockIcon) lockIcon.SetActive(false);
        }
        else if (isUnlocked)
        {
            // DURUM: AÇIK AMA SEÇÝLÝ DEÐÝL
            if (buttonImage && selectButtonSprite) buttonImage.sprite = selectButtonSprite;

            if (priceText) priceText.text = "";
            if (coinIcon) coinIcon.SetActive(false);
            if (lockIcon) lockIcon.SetActive(false);
        }
        else
        {
            // DURUM: KÝLÝTLÝ
            if (buttonImage && buyButtonSprite) buttonImage.sprite = buyButtonSprite;

            if (priceText) priceText.text = theme.price.ToString();
            if (coinIcon) coinIcon.SetActive(true);
            if (lockIcon) lockIcon.SetActive(true);
        }
    }

    private void UpdateCoinDisplay()
    {
        if (totalCoinsDisplay != null)
        {
            int coins = PlayerPrefs.GetInt("TotalCoins", 0);
            totalCoinsDisplay.text = coins.ToString();
        }
    }

    private bool IsThemeUnlocked(Theme theme)
    {
        if (theme.price == 0) return true;
        return PlayerPrefs.GetInt("ThemeUnlocked_" + theme.themeFolderName, 0) == 1;
    }

    private bool IsThemeSelected(Theme theme)
    {
        if (!SelectedThemeManager.Instance.HasSelectedTheme()) return false;
        return SelectedThemeManager.Instance.GetSelectedTheme().themeFolderName == theme.themeFolderName;
    }

    private IEnumerator AnimateThemeTransition(Theme newTheme)
    {
        isTransitioning = true;
        float elapsed = 0f;
        Color startColor = themeDisplayImage.color;

        while (elapsed < transitionDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsed / (transitionDuration / 2));
            themeDisplayImage.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);
            yield return null;
        }

        ApplyTheme(newTheme);

        elapsed = 0f;
        while (elapsed < transitionDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsed / (transitionDuration / 2));
            themeDisplayImage.color = Color.Lerp(new Color(startColor.r, startColor.g, startColor.b, 0), startColor, t);
            yield return null;
        }
        themeDisplayImage.color = startColor;
        isTransitioning = false;
    }

    // --- TEST ARAÇLARI ---
    [ContextMenu("Reset All Themes")]
    public void ResetAllThemes()
    {
        if (themes == null)
        {
            var loader = GetComponent<ThemeLoader>();
            if (loader != null) themes = loader.LoadAllThemes();
        }

        if (themes != null)
        {
            foreach (var t in themes)
            {
                if (t.price > 0)
                {
                    PlayerPrefs.DeleteKey("ThemeUnlocked_" + t.themeFolderName);
                }
            }
            PlayerPrefs.Save();
            Debug.Log("Tüm tema satýn alýmlarý sýfýrlandý!");
        }

        if (Application.isPlaying)
        {
            DisplayCurrentTheme(false);
        }
    }

    [ContextMenu("Add 1000 Coins")]
    public void AddCheatCoins()
    {
        int coins = PlayerPrefs.GetInt("TotalCoins", 0);
        coins += 1000;
        PlayerPrefs.SetInt("TotalCoins", coins);
        PlayerPrefs.Save();
        Debug.Log("1000 Coin Eklendi. Yeni Bakiye: " + coins);

        if (Application.isPlaying) UpdateCoinDisplay();
    }
}