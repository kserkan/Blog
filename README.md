Bu proje, modern bir **Blog Platformu** geliştirmek amacıyla **ASP.NET Core MVC** mimarisi kullanılarak oluşturulmuştur. Kullanıcıların yazı yazabildiği, yorum yapabildiği, etiketler ve kategoriler üzerinden filtreleme yapabildiği, yönetici onay sistemi içeren tam işlevsel bir içerik yönetim sistemidir. Ayrıca **OpenAI API** entegrasyonu ile yorumların yapay zeka tarafından analiz edilmesi sağlanır.

**Yayında Görüntüle:**  
[http://167.71.46.191:5000/blogs](http://167.71.46.191:5000/blogs)

---

## 🚀 Kullanılan Teknolojiler

- **ASP.NET Core 8 MVC**
- **Entity Framework Core (Code First)**
- **MySQL** (Docker ile konteyner üzerinde)
- **OpenAI API** (GPT-3.5 ile yorum analizi)
- **Bootstrap 5**, **jQuery**, **Toastr**, **Select2**
- **TinyMCE** (Zengin metin düzenleyici)
- **AJAX** tabanlı form işlemleri
- **Cookie Authentication** (Rol bazlı yetkilendirme)

---

## 🔐 Özellikler

### 👥 Kullanıcı İşlemleri
- Kayıt olma, giriş yapma, çıkış yapma
- Rol kontrolü (Admin / Kullanıcı)
- Profil görüntüleme ve düzenleme
- Profil fotoğrafı güncelleme

### 📰 Yazı Yönetimi
- Yazı oluşturma (Başlık, içerik, görsel, kategori, etiket)
- TinyMCE editörü ile zengin metin
- Yazı listesi, kategori ve etikete göre filtreleme
- Yönetici onay sistemi
- Görüntülenme sayısı takibi

### 💬 Yorumlar
- AJAX ile hızlı yorum ekleme, düzenleme ve silme
- Cevap (reply) sistemi ile iç içe yorumlar
- Kullanıcıya özel yorum listesi
- OpenAI ile yapay zeka analizi (uygunsuzluk tespiti)
- Uygunsuz yorumda admin’e bildirim gönderimi

### 🛎️ Bildirim Sistemi
- Kullanıcılara yazı onayı, yorum ve uygunsuz içerik bildirimi
- Navbar’da okunmamış bildirim göstergesi
- Bildirimi tıklayınca okundu olarak işaretleme
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
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MySQL](https://www.mysql.com/) (veya Docker)
- OpenAI API Key

### 2. Ortam Değişkenleri (.env)
```env
OPENAI_API_KEY=sk-xxxxxx

3. Terminal Üzerinden Kurulum

dotnet restore
dotnet ef database update
dotnet run

Eğer Docker ile çalışacaksanız docker-compose.yml üzerinden veritabanını ayağa kaldırabilirsiniz.


---

🧠 OpenAI Entegrasyonu

Yorumlar OpenAIService sınıfı üzerinden GPT-3.5 modeline gönderilir ve şu sistem mesajı ile analiz edilir:

> “Kısa ve öz analiz yap. Yorum eğer hakaret, küfür, nefret, spam içeriyorsa sadece 'uygunsuz' yaz. Aksi halde 'uygun' yaz.”



Sonuç "uygunsuz" ise:

Yorum veritabanına kaydedilmez

Admin’e bildirim gönderilir

Kullanıcı bilgilendirilir



---

🌍 Yayın

Projenin canlı versiyonuna aşağıdaki adresten erişebilirsiniz:

http://167.71.46.191:5000/blogs


---

📬 İletişim

Kerim Serkan Şahin
📧 kerimserkann@gmail.com
🎓 Konya Teknik Üniversitesi - Yazılım Mühendisliği


---

📄 Lisans

Bu proje MIT Lisansı ile lisanslanmıştır.

---
