## 🎮 Game Events Documentation

Bu döküman, oyundaki tüm `IGameEvent` türlerinin:
- Nereden tetiklendiğini
- Hangi koşullarda çalıştığını
- Hangi sınıflar tarafından dinlendiğini ve bu sınıfların ne yaptığını

belgelemek amacıyla hazırlanmıştır.

---

### 📦 Event: `CurrencyChanged`
- **Tetiklenme Yeri:**
  - `RewardRequestHandler`
- **Tetiklenme Koşulu:**
  - Coin veya Life harcandığında veya kazanıldığında
- **Dinleyiciler ve Tepkileri:**
  - (Henüz dinleyici yok)
  - (İleride: UI güncellemesi, ses ve efektler eklenebilir)

---

### 📦 Event: `UIChangeAsyncEvent`
- **Tetiklenme Yeri:**
  - `UIManager`, `GameFlowController`, çeşitli panel kontrolcüleri
- **Tetiklenme Koşulu:**
  - Ekran (ScreenType) veya Overlay (OverlayType) geçişi gerektiğinde
- **Dinleyiciler ve Tepkileri:**
  - `UIManager`: İlgili paneli asenkron biçimde gösterir/gizler

---

### 📦 Event: `TransactionCompletedEvent`
- **Tetiklenme Yeri:**
  - `RewardRequestHandler`
- **Tetiklenme Koşulu:**
  - Bir ödül işlemi başarıyla tamamlandığında
- **Dinleyiciler ve Tepkileri:**
  - `UIManager`: Bazı geçiş veya UI güncellemelerini bu event ile başlatır
  - (İleride: Tek sorumlu tetikleyici sınıf ile merkezi organize edilebilir)

---

### 📦 Event: `UpdateTimerUIEvent`
- **Tetiklenme Yeri:**
  - HUD'da gösterilecek süre değiştiğinde (örn. zaman sayacı güncellenirken)
- **Tetiklenme Koşulu:**
  - Kalan süre değiştiğinde
- **Dinleyiciler ve Tepkileri:**
  - `HUDController` veya benzeri UI sınıfı: HUD üzerindeki zaman göstergesi güncellenir

---

### 📦 Event: `UpdateMoveCountUIEvent`
- **Tetiklenme Yeri:**
  - Oyuncu hamle yaptığında kalan hamle sayısı değiştiğinde
- **Tetiklenme Koşulu:**
  - `MoveManager` veya benzeri bir yerden hamle eksilince
- **Dinleyiciler ve Tepkileri:**
  - `HUDController`: Kalan hamle UI’sini günceller

---

### 📦 Event: `UpdateLifeUIEvent`
- **Tetiklenme Yeri:**
  - Oyuncu can kazandığında ya da kaybettiğinde
- **Tetiklenme Koşulu:**
  - `GameEconomy`, `RewardProcessor` vb. sınıflar içinde
- **Dinleyiciler ve Tepkileri:**
  - `HUDController`: Can göstergesi güncellenir

---

### 📦 Event: `UpdateCoinUIEvent`
- **Tetiklenme Yeri:**
  - Coin kazanıldığında veya harcandığında
- **Tetiklenme Koşulu:**
  - `RewardProcessor`, `GameEconomy` vb. sınıflar içinde
- **Dinleyiciler ve Tepkileri:**
  - `HUDController`: Coin göstergesi güncellenir

---

### 📦 Event: `ButtonClickedEvent`
- **Tetiklenme Yeri:**
  - UI butonlarının `OnClick()` metodlarından
- **Tetiklenme Koşulu:**
  - Herhangi bir butona tıklanması
- **Dinleyiciler ve Tepkileri:**
  - (Henüz dinleyici yok)
  - (İleride: Ses ve animasyon efektleri için kullanılabilir)

---

### 📦 Event: `RewardRequestedEvent`
- **Tetiklenme Yeri:**
  - Ödül butonlarından (örneğin: +coin, +life, joker butonları)
- **Tetiklenme Koşulu:**
  - Oyuncu reklam/ödül sisteminden bir hak istediğinde
- **Dinleyiciler ve Tepkileri:**
  - `RewardRequestProcessor`: Ödül işlemini başlatır

---

### 📦 Event: `RewardResultEvent`
- **Tetiklenme Yeri:**
  - `RewardRequestProcessor` (aynı sınıf hem dinleyici hem tetikleyici olabilir)
- **Tetiklenme Koşulu:**
  - Ödül tamamlandıktan sonra başarı/başarısızlık sonucu belirir
- **Dinleyiciler ve Tepkileri:**
  - (Henüz dinleyici yok)
  - (İleride: Ses/animasyon veya bilgi mesajları için kullanılabilir)

---

### 📦 Event: `InvalidFusionEvent`
- **Tetiklenme Yeri:**
  - `FusionProcessor`
- **Tetiklenme Koşulu:**
  - Geçersiz bir birleştirme yapılmaya çalışıldığında
- **Dinleyiciler ve Tepkileri:**
  - (İleride: Ses ve görsel hata bildirimi için kullanılabilir)

---

### 📦 Event: `WhiteFusionEvent`
- **Tetiklenme Yeri:**
  - `FusionProcessor`
- **Tetiklenme Koşulu:**
  - Başarılı birleştirme sonucunda beyaz renk elde edilirse
- **Dinleyiciler ve Tepkileri:**
  - (İleride: Özel efekt veya skor bildirimi için kullanılabilir)

---

