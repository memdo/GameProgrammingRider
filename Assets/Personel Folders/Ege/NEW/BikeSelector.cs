using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BikeSelector : MonoBehaviour
{
    [System.Serializable]
    public class BikeVisual
    {
        public Sprite body;
        public Sprite frontWheel;
        public Sprite backWheel;
        public int price;
    }

    [Header("UI References")]
    public Image bodyImage;
    public Image frontWheelImage;
    public Image backWheelImage;

    // YENİ: İki tane kilit resmi (Image olarak)
    public Image lockImage1;
    public Image lockImage2;

    public Text priceText;
    public Text totalCoinsText;

    [Header("Button Settings")]
    public Image actionButtonImage;
    public Sprite playSprite;
    public Sprite buySprite;

    [Header("Settings")]
    public string gameSceneName = "Game";
    public BikeVisual[] bikes;

    private int index = 0;

    void Start()
    {
        index = PlayerPrefs.GetInt("selectedBike", 0);

        // --- BU KISMI EKLE ---
        if (index >= bikes.Length)
        {
            index = 0; // Hata varsa başa dön
        }
        // ---------------------

        UpdateUI();
    }

    public void NextBike()
    {
        index++;
        if (index >= bikes.Length) index = 0;
        UpdateUI();
    }

    public void PrevBike()
    {
        index--;
        if (index < 0) index = bikes.Length - 1;
        UpdateUI();
    }

    void UpdateUI()
    {

        // --- BU KISMI EKLE (EN BAŞA) ---
        if (bikes == null || bikes.Length == 0) return; // Liste boşsa işlem yapma
        if (index >= bikes.Length) index = 0; // İndeks taşmışsa düzelt
        // -------------------------------

        // 1. Araç Görselleri (Eski kod buradan devam ediyor)

        // 1. Araç Görselleri
        BikeVisual b = bikes[index];
        bodyImage.sprite = b.body;
        frontWheelImage.sprite = b.frontWheel;
        backWheelImage.sprite = b.backWheel;

        // 2. Toplam Para
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        if (totalCoinsText != null) totalCoinsText.text = currentCoins.ToString();

        // 3. Kilit Durumu
        bool isUnlocked = IsBikeUnlocked(index);

        // --- YENİ: İki Kilit Resmini Kontrol Et ---
        if (lockImage1 != null) lockImage1.enabled = !isUnlocked;
        if (lockImage2 != null) lockImage2.enabled = !isUnlocked;
        // ------------------------------------------

        if (isUnlocked)
        {
            // AÇIK -> Play Butonu
            if (actionButtonImage != null && playSprite != null)
                actionButtonImage.sprite = playSprite;

            if (priceText != null) priceText.text = "";
        }
        else
        {
            // KİLİTLİ -> Satın Al Butonu
            if (actionButtonImage != null && buySprite != null)
                actionButtonImage.sprite = buySprite;

            if (priceText != null) priceText.text = b.price.ToString();
        }
    }

    public void OnActionClick()
    {
        if (IsBikeUnlocked(index))
        {
            SelectBike();
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            TryBuyBike(index);
        }
    }

    void TryBuyBike(int bikeIndex)
    {
        int cost = bikes[bikeIndex].price;
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);

        if (currentCoins >= cost)
        {
            currentCoins -= cost;
            PlayerPrefs.SetInt("TotalCoins", currentCoins);
            PlayerPrefs.SetInt("BikeUnlocked_" + bikeIndex, 1);
            PlayerPrefs.Save();

            Debug.Log("Araç satın alındı!");
            UpdateUI();
        }
        else
        {
            Debug.Log("Yetersiz Bakiye!");
        }
    }

    bool IsBikeUnlocked(int bikeIndex)
    {
        if (bikeIndex == 0) return true;
        return PlayerPrefs.GetInt("BikeUnlocked_" + bikeIndex, 0) == 1;
    }

    public void SelectBike()
    {
        PlayerPrefs.SetInt("selectedBike", index);
        PlayerPrefs.Save();
    }

    // --- TEST ARAÇLARI ---
    [ContextMenu("Reset All Purchases")]
    public void ResetPurchases()
    {
        for (int i = 1; i < bikes.Length; i++)
        {
            PlayerPrefs.DeleteKey("BikeUnlocked_" + i);
        }
        PlayerPrefs.Save();
        Debug.Log("Tüm satın almalar sıfırlandı!");
        if (Application.isPlaying) UpdateUI();
    }

    [ContextMenu("Add 1000 Coins")]
    public void AddCheatCoins()
    {
        int coins = PlayerPrefs.GetInt("TotalCoins", 0);
        coins += 1000;
        PlayerPrefs.SetInt("TotalCoins", coins);
        PlayerPrefs.Save();
        Debug.Log("1000 Coin eklendi. Yeni bakiye: " + coins);
        if (Application.isPlaying) UpdateUI();
    }
}