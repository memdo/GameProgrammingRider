# Tema Sistemi - Hızlı Başlangıç Rehberi

## Sistem Nasıl Çalışıyor?

```
[Tema Seçim Ekranı] → [Tema Seç + SELECT] → [Gameplay] → [Tema Görünür]
```

## 1. Tema Seçim Ekranı Oluştur

Unity menüsünde:
```
Tools > Create Theme Selection Scene
```

Bu otomatik olarak oluşturur:
- ✅ Sol/Sağ navigasyon butonları
- ✅ Tema önizleme görseli
- ✅ SELECT butonu
- ✅ MAIN MENU butonu

## 2. Ana Menüye Tema Butonu Ekle

**gameui.unity** sahnesinde:

1. Tema butonunu oluştur/seç
2. Button → onClick eventinde:
   - `MainMenuUI.LoadScene("ThemeSelectionScene")`
3. **Build Settings'e ekle:**
   - `File > Build Settings`
   - ThemeSelectionScene'i listeye ekle

## 3. Gameplay Sahnesine Tema Desteği Ekle

**New Scene.unity** (veya oyun sahnesi) içinde:

1. **Boş GameObject oluştur:**
   - İsim: `ParallaxContainer`

2. **Component ekle:**
   - `ThemeAwareParallaxController`

3. **Ayarları yap:**
   - ✅ Load From Selected Theme: AÇIK
   - Parallax Speed: `0.03`
   - Layer Z Spacing: `1.0`
   - Layer Scale: `(3, 3, 1)`

## 4. Test Et!

### Adım 1: Tema Seç
1. ThemeSelectionScene'i oynat
2. Sol/Sağ butonlarla tema seç
3. **SELECT** butonuna tıkla

### Adım 2: Oyunu Oynat
1. Gameplay sahnesini oynat
2. Seçtiğin tema parallax olarak görünmeli!

## Sorun Giderme

### Tema Seçim Ekranı Açılmıyor
- ❌ ThemeSelectionScene Build Settings'de değil
- ✅ `File > Build Settings` → Sahneyi ekle

### Gameplay'de Tema Görünmüyor
- ❌ SELECT butonuna basmadın
- ❌ ThemeAwareParallaxController yok
- ❌ "Load From Selected Theme" kapalı
- ✅ Önce tema seç, sonra gameplay'i oynat

### Console Hataları
- "No theme selected" → Önce tema seçim ekranında SELECT'e bas
- "ThemeLoader: Could not load" → Themes klasörü doğru konumda değil

## Dosya Konumları

```
Assets/
├── Personel Folders/
│   ├── Umut/
│   │   ├── Themes/          ← Tema klasörleri (a, b, c, d)
│   │   ├── Scenes/
│   │   │   └── ThemeSelectionScene.unity
│   │   └── Scripts/
│   │       ├── Theme.cs
│   │       ├── ThemeLoader.cs
│   │       ├── ThemeSelectionUI.cs
│   │       ├── SelectedThemeManager.cs
│   │       └── ThemeAwareParallaxController.cs (Salih klasöründe de var)
│   ├── Ege/
│   │   └── NEW/
│   │       ├── gameui.unity          ← Ana menü
│   │       └── New Scene.unity       ← Gameplay
│   └── Salih/
│       └── Scenes/version B/
│           └── Layers/
│               └── ThemeAwareParallaxController.cs
```

## Hızlı Komutlar

| Ne Yapacaksan | Komut |
|---------------|-------|
| Tema seçim sahnesi oluştur | `Tools > Create Theme Selection Scene` |
| Build Settings aç | `File > Build Settings` |
| Sahne yükle (kod) | `SceneManager.LoadScene("ThemeSelectionScene")` |

## Daha Fazla Bilgi

- **INTEGRATION_GUIDE.md** - Detaylı entegrasyon rehberi
- **GAMEPLAY_TEMA_KURULUM.md** - Gameplay sahnesine tema ekleme
- **TEMA_BUTONU_REHBER.md** - Ana menüye buton ekleme
- **walkthrough.md** - Sistem mimarisi ve teknik detaylar
