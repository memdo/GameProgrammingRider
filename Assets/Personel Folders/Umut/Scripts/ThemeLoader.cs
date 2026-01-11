using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ThemeLoader : MonoBehaviour
{
    // Artýk tam yol deðil, Resources içindeki yolunu yazýyoruz
    [Header("Configuration")]
    [SerializeField] private string themesPath = "Themes";

    [System.Serializable]
    public class ThemePriceConfig
    {
        public string folderName;
        public int price;
    }

    [Header("Price Settings")]
    [SerializeField] private List<ThemePriceConfig> themePrices;

    public Theme[] LoadAllThemes()
    {
        List<Theme> themes = new List<Theme>();

        // --- DEÐÝÞÝKLÝK BURADA ---
        // Resources klasöründeki "Themes" altýndaki her þeyi yükle
        // Ancak Resources.LoadAll klasörleri deðil dosyalarý getirir.
        // Bu yüzden TextAsset veya Sprite olarak tarayýp klasör isimlerini çýkaracaðýz.

        // Yöntem: Themes klasöründeki tüm Sprite'larý çekip klasörlerine göre gruplayalým.
        Sprite[] allSprites = Resources.LoadAll<Sprite>(themesPath);

        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogError($"ThemeLoader: Resources/{themesPath} altýnda hiç resim bulunamadý!");
            return themes.ToArray();
        }

        // Klasör isimlerini (Tema adlarýný) bul
        // Örnek yol: "Themes/a/1" -> Klasör: "a"
        HashSet<string> foundThemeNames = new HashSet<string>();

        foreach (var sprite in allSprites)
        {
            // Unity editörde veya buildde Resources yolu klasör hiyerarþisini korur mu?
            // En güvenli yöntem: Manuel tanýmlý klasör listesi veya bilinen klasörleri elle check etmek.
            // Ancak otomatik olsun istiyoruz.

            // Resources'ta klasör listeleme komutu yoktur. 
            // Bu yüzden küçük bir hile yapacaðýz:
            // "Themes/a", "Themes/b" gibi klasörlerinin olduðunu VARSAYACAÐIZ. 
            // Bunu yapmanýn en kolay yolu Inspector'dan klasör isimlerini girmek 
            // ya da Fiyat Listesindeki isimleri kullanmaktýr.
        }

        // --- 2. YÖNTEM (DAHA KOLAY) ---
        // Fiyat Listesine yazdýðýn isimleri REFERANS alarak yükleyelim.
        // Böylece hem fiyatý hem klasörü biliriz.
        // Eðer fiyat listesinde yoksa yüklenmez (veya manuel bir liste daha ekleyebilirsin).

        if (themePrices == null || themePrices.Count == 0)
        {
            Debug.LogWarning("ThemeLoader: Theme Prices listesi boþ! Android için buraya tema isimlerini girmelisin.");
            return themes.ToArray();
        }

        foreach (var config in themePrices)
        {
            string folderName = config.folderName;
            Theme theme = LoadThemeFromResources(folderName);

            if (theme != null && theme.IsValid())
            {
                theme.price = config.price;
                themes.Add(theme);
            }
        }

        themes = themes.OrderBy(t => t.themeFolderName).ToList();
        return themes.ToArray();
    }

    public Theme LoadThemeFromResources(string folderName)
    {
        Theme theme = new Theme(folderName.ToUpper(), folderName);

        // Örn: Themes/a/a
        string basePath = $"{themesPath}/{folderName}";

        // 1. Kapak Resmi
        theme.representationSprite = Resources.Load<Sprite>($"{basePath}/{folderName}");
        if (theme.representationSprite == null) return null; // Kapak yoksa yükleme

        // 2. Parallax
        List<Sprite> parallaxLayers = new List<Sprite>();
        int i = 1;
        while (true)
        {
            Sprite s = Resources.Load<Sprite>($"{basePath}/{i}");
            if (s == null) break;
            parallaxLayers.Add(s);
            i++;
        }
        if (parallaxLayers.Count == 0) return null;
        theme.parallaxLayers = parallaxLayers.ToArray();

        // 3. Sky & Cloud
        theme.skySprite = Resources.Load<Sprite>($"{basePath}/sky");

        List<Sprite> clouds = new List<Sprite>();
        int c = 1;
        while (true)
        {
            Sprite s = Resources.Load<Sprite>($"{basePath}/cloud{c}");
            if (s == null) break;
            clouds.Add(s);
            c++;
        }
        theme.cloudLayers = clouds.ToArray();

        return theme;
    }
}