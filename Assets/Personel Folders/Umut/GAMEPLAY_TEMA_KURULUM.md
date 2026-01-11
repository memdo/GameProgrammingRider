# Gameplay Sahnesine Tema Desteği Ekleme

## Sorun
Tema seçim ekranında seçilen parallax teması, gameplay sahnesinde (New Scene.unity) görünmüyor.

## Çözüm: ThemeAwareParallaxController Ekleme

### Adım 1: Parallax Container Oluştur

1. **New Scene.unity** sahnesini aç
2. Hierarchy'de sağ tık → **Create Empty**
3. İsim: `ParallaxContainer`
4. Position: (0, 0, 0) veya kameranın arkasında bir yere

### Adım 2: ThemeAwareParallaxController Ekle

1. `ParallaxContainer` GameObject'ini seç
2. Inspector'da **Add Component**
3. `ThemeAwareParallaxController` ara ve ekle

### Adım 3: Ayarları Yapılandır

Inspector'da şu ayarları yap:

**ThemeAwareParallaxController:**
- ✅ **Load From Selected Theme**: AÇIK (enabled)
- **Parallax Speed**: `0.03` (ayarlayabilirsin)
- **Layer Z Spacing**: `1.0` (katmanlar arası mesafe)
- **Layer Scale**: `(3, 3, 1)` (sprite boyutuna göre ayarla)

### Adım 4: Test Et

1. **Önce tema seçim sahnesini oynat:**
   - ThemeSelectionScene'i aç
   - Bir tema seç (örn: Theme A)
   - **SELECT** butonuna tıkla

2. **Sonra gameplay sahnesini oynat:**
   - New Scene.unity'yi aç
   - Play'e bas
   - Seçtiğin tema parallax olarak görünmeli!

## Önemli Notlar

### Tema Seçimi Akışı
```
1. ThemeSelectionScene → Tema seç → SELECT
2. SelectedThemeManager tema kaydeder
3. Gameplay Scene → ThemeAwareParallaxController tema yükler
4. Parallax katmanları otomatik oluşturulur
```

### Eğer Tema Görünmüyorsa

**Kontrol Et:**
1. ✅ Tema seçim sahnesinde SELECT butonuna bastın mı?
2. ✅ Console'da "SelectedThemeManager: Theme 'X' selected" mesajı var mı?
3. ✅ ThemeAwareParallaxController'da "Load From Selected Theme" açık mı?
4. ✅ Kamera doğru konumda mı? (parallax katmanları görebiliyor mu?)

**Console Mesajları:**
- ✅ İyi: "ThemeAwareParallaxController: Loading theme 'A' with 7 layers"
- ❌ Sorun: "ThemeAwareParallaxController: No theme selected yet"

### Katman Ayarları

Eğer parallax katmanları çok büyük/küçük görünüyorsa:

- **Layer Scale** değerini değiştir:
  - Çok büyük → `(2, 2, 1)` veya `(1, 1, 1)`
  - Çok küçük → `(4, 4, 1)` veya `(5, 5, 1)`

- **Layer Z Spacing** değerini değiştir:
  - Daha fazla derinlik → `2.0` veya `3.0`
  - Daha az derinlik → `0.5`

### Parallax Hızı Ayarlama

- **Parallax Speed**: `0.01` - `0.05` arası
  - Yavaş parallax → `0.01`
  - Hızlı parallax → `0.05`

## Alternatif: Manuel Test

Eğer tema seçmeden test etmek istersen:

1. ThemeAwareParallaxController'da:
   - `Load From Selected Theme` → KAPAT
2. ParallaxContainer'a manuel olarak child GameObject'ler ekle
3. Her birine SpriteRenderer ekle ve sprite ata
4. Z pozisyonlarını ayarla (0, 1, 2, 3...)

## Salih'in Sahnesinden Fark

Salih'in version B sahnesinde zaten ParallaxController var.
Ege'nin New Scene sahnesinde parallax yoktu, bu yüzden sıfırdan ekliyoruz.

Her iki sahnede de ThemeAwareParallaxController kullanabilirsin!
