Bu proje, modern bir blog platformu geliştirmek amacıyla ASP.NET Core MVC mimarisi kullanılarak oluşturulmuştur. Kullanıcıların yazı yazabildiği, yorum yapabildiği, etiketler ve kategoriler üzerinden filtreleme yapabildiği, yönetici onay sistemi içeren tam işlevsel bir içerik yönetim sistemidir. Ayrıca OpenAI API entegrasyonu ile yorumların yapay zeka tarafından analiz edilmesi sağlanır.

---

## 🚀 Kullanılan Teknolojiler

- ASP.NET Core 8 MVC
- Entity Framework Core (Code First)
- MySQL & Docker
- OpenAI API (Yorum analizi)
- Bootstrap 5 & jQuery & Toastr & Select2
- TinyMCE (Zengin metin düzenleyici)
- AJAX tabanlı form işlemleri
- Cookie Authentication (Rol bazlı yetkilendirme)

---

## 🔐 Özellikler

### 👥 Kullanıcı İşlemleri
- Kayıt olma, giriş yapma, çıkış yapma
- Rol kontrolü (Admin / Kullanıcı)
- Profil görüntüleme ve düzenleme
- Profil fotoğrafı güncelleme

### 📰 Yazı Yönetimi
- Yazı oluşturma (başlık, içerik, görsel, kategori, etiket)
- TinyMCE editörü
- Yazıların listelenmesi, kategorilere göre filtrelenmesi
- Etiket bazlı filtreleme (query string üzerinden)
- Yazı onay sistemi (Yönetici onayı gerektirir)
- Görüntülenme sayısı takibi

### 💬 Yorumlar
- AJAX ile yorum ekleme, düzenleme ve silme
- Cevap (reply) sistemi
- Kullanıcıya özel yorum listesi
- Yorumların yapay zeka (OpenAI) tarafından uygunsuzluk kontrolü
- Uygunsuz yorum girişiminde admin’e bildirim

### 🛎️ Bildirim Sistemi
- Kullanıcılara yorum, onay ve uygunsuz içerik bildirimleri
- Navbar’da okunmamış bildirim sayısı
- Bildirimi tıklayınca okundu olarak işaretlenme
- Tüm bildirimleri listeleme

---

## 📁 Proje Yapısı

- `Entity/` → Veri modelleri (Post, Comment, User, Tag, Category)
- `Data/` → EF Core context, repository arayüzleri ve sınıfları
- `Controllers/` → MVC controller sınıfları
- `Views/` → Razor View sayfaları (Listeler, Formlar, Partiallar)
- `ViewComponents/` → Son yazılar, tag listesi bileşenleri
- `wwwroot/` → Statik dosyalar (css, js, görseller)
- `Services/` → `OpenAIService` ile GPT analiz mantığı

---

## ⚙️ Kurulum ve Çalıştırma

### 1. Gereksinimler
- .NET 8 SDK
- MySQL (veya Docker ile MySQL Container)
- OpenAI API Key

### 2. .env Dosyası (Server ortamı için)
```
OPENAI_API_KEY=sk-xxxxxx
```

### 3. Terminal Komutları
```bash
dotnet restore
dotnet ef database update
dotnet run
```

---

## 🧠 OpenAI Entegrasyonu

Yorumlar `OpenAIService` sınıfı üzerinden GPT-3.5 modeline gönderilir ve şu sistem mesajı kullanılır:

> "Kısa ve öz analiz yap. Yorum eğer hakaret, küfür, nefret, spam içeriyorsa sadece 'uygunsuz' yaz. Aksi halde 'uygun' yaz."

Gelen sonuç "uygunsuz" içeriyorsa yorum reddedilir ve admin’e bildirim gönderilir.

---

## 📬 İletişim

**Kerim Serkan Şahin**  
📧 kerimserkann@gmail.com  
🌐 Konya Teknik Üniversitesi - Yazılım Mühendisliği

---

## 📄 Lisans
Bu proje MIT lisansı ile lisanslanmıştır. Daha fazla bilgi için LICENSE dosyasına göz atabilirsiniz.
