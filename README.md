# CrmYonetimSistemi 🚀

`CrmYonetimSistemi`, KOBİ'ler ve küçük işletmeler için müşteri ilişkileri, ürün, satış, ödeme ve gider yönetimi gibi temel ticari operasyonları tek bir yerden yönetmeyi sağlayan kapsamlı bir web uygulamasıdır. ASP.NET Core MVC kullanılarak geliştirilmiştir.

---

## 🌟 Temel Özellikler

* **Güvenli Kimlik Doğrulama ve Yetkilendirme:**
    * ASP.NET Core Identity altyapısı ile güvenli kullanıcı girişi.
    * **Admin** ve **User** olmak üzere iki farklı kullanıcı rolü.

* **Yönetici Paneli:**
    * Admin rolüne sahip kullanıcılar için özel yönetim paneli.
    * Yeni kullanıcı oluşturma (kullanıcı adı ve şifre ile).
    * Mevcut kullanıcıların şifrelerini değiştirme.
    * Kullanıcılara "Admin" veya "User" rolü atama/değiştirme.
    * Kullanıcı silme.

* **Ana Kontrol Paneli (Dashboard):**
    * Toplam müşteri sayısı, ürün sayısı, yapılan satış adedi gibi genel istatistiklerin anlık takibi.
    * Son yapılan satışlar ve ödemelerin hızlı bir görünümü.

* **Modüler Yönetim (CRUD İşlemleri):**
    * **Müşteri Yönetimi:** Yeni müşteri ekleme, bilgilerini düzenleme ve müşteri bazlı satış/ödeme geçmişini görüntüleme.
    * **Ürün Yönetimi:** Ürün ekleme, stok ve fiyat bilgilerini güncelleme.
    * **Satış Yönetimi:** Yeni satış kaydı oluşturma, satış detaylarını görüntüleme ve düzenleme.
    * **Gider Yönetimi:** İşletme giderlerini kaydetme ve listeleme.
    * **Ödeme Yönetimi:** Müşterilerden alınan ödemeleri kaydetme ve takip etme.

* **Proforma Fatura Oluşturma:**
    * Yapılan satışlar için dinamik olarak PDF formatında proforma fatura oluşturma. (QuestPDF kütüphanesi ile)

---

## 🛠️ Kullanılan Teknolojiler ve Kütüphaneler

* **Platform:** .NET 9 ve ASP.NET Core MVC
* **Dil:** C#
* **Veritabanı:** Microsoft SQL Server
* **ORM:** Entity Framework Core
* **Kimlik Doğrulama:** ASP.NET Core Identity
* **UI Kütüphaneleri:** Bootstrap, jQuery, jQuery Validation
* **PDF Oluşturma:** QuestPDF

---

## 🚀 Kurulum ve Başlarken

Projeyi yerel makinenizde kurmak ve çalıştırmak için aşağıdaki adımları izleyin.

### Ön Koşullar

* **Visual Studio 2022 (veya üzeri):** ".NET web geliştirme" iş yükü kurulu olmalıdır.
* **.NET 9 SDK (veya üzeri):** Projenin TargetFramework'ü ile uyumlu SDK.
* **Microsoft SQL Server:** Herhangi bir sürüm (ücretsiz "Developer" veya "Express" sürümü tavsiye edilir).

### Uygulama Yapılandırması

1.  **Projeyi Klonlayın veya İndirin:**
    ```bash
    git clone [https://github.com/KULLANICI_ADINIZ/CrmYonetimSistemi.git](https://github.com/KULLANICI_ADINIZ/CrmYonetimSistemi.git)
    ```
2.  **Projeyi Visual Studio'da Açın:**
    * `CrmYonetimSistemi.sln` dosyasına çift tıklayarak projeyi açın.

3.  **Veritabanı Bağlantısını Yapılandırma:**
    * `appsettings.json` dosyasını açın.
    * `ConnectionStrings` bölümündeki `DefaultConnection` değerini kendi SQL Server sunucunuza göre güncelleyin.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=SUNUCU_ADINIZ;Database=CrmYonetimSistemiDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"
    }
    ```
    * **`SUNUCU_ADINIZ`**: SQL Server sunucunuzun adıdır (Örn: `DESKTOP-ABC\SQLEXPRESS` veya `localhost`).

4.  **İlk Admin Şifresini Belirleme:**
    * `Data/DbSeeder.cs` dosyasını açın.
    * `superAdminPassword` değişkenine ilk admin kullanıcısı için güçlü bir şifre atayın.
    ```csharp
    // LÜTFEN BU ŞİFREYİ GÜÇLÜ BİR ŞİFRE İLE DEĞİŞTİRİN!
    string superAdminPassword = "Password123!";
    ```

5.  **Veritabanını Oluşturma:**
    * Visual Studio'da **Tools -> NuGet Package Manager -> Package Manager Console** menüsünü açın.
    * Açılan konsola aşağıdaki komutu yazarak veritabanını ve tabloları oluşturun:
    ```powershell
    Update-Database
    ```

### Uygulamayı Çalıştırma

1.  Tüm yapılandırmalar tamamlandıktan sonra, Visual Studio'da `F5` tuşuna basarak projeyi başlatın.
2.  Uygulama ilk kez çalıştığında, `DbSeeder` mekanizması `Admin` ve `User` rollerini ve `DbSeeder.cs` dosyasında şifresini belirlediğiniz `superadmin` kullanıcısını otomatik olarak oluşturacaktır.

---

### ⚠️ Kurulum Sonrası Önemli Adımlar (GÜVENLİK)

`superadmin` kullanıcısı başarıyla oluşturulduktan sonra, bu tek seferlik kurulum işlemini devre dışı bırakmak **güvenlik ve performans için çok önemlidir.**

1.  **`Program.cs` dosyasını açın.**
2.  `// --- SEEDER ÇAĞRISI ---` bloğunun altındaki seeder'ı çağıran satırı bulun.
3.  Bu satırı **yorum satırı yapın veya silin**:

    ```csharp
    // ESKİ HALİ:
    // await DbSeeder.SeedRolesAndSuperAdminAsync(services);

    // YENİ HALİ:
    // await DbSeeder.SeedRolesAndSuperAdminAsync(services); // Bu satırı devre dışı bırakın.
    ```
Bu işlemi yaptıktan sonra uygulama her başladığında yeniden kullanıcı oluşturmaya çalışmaz ve kodunuzda gereksiz bir işlem kalmaz.

---

## 📋 Uygulama Kullanım Adımları

1.  Uygulama açıldığında "Giriş Yap" sayfasına gidin.
2.  **Kullanıcı Adı:** `superadmin`
3.  **Şifre:** `DbSeeder.cs` dosyasında belirlediğiniz şifre ile giriş yapın.
4.  Giriş yaptıktan sonra, üst menüdeki **"Kullanıcı Yönetimi"** linkinden yeni kullanıcılar oluşturabilir ve rollerini yönetebilirsiniz.
5.  Diğer menü linklerini kullanarak müşteri, ürün, satış, gider ve ödeme kayıtları oluşturmaya başlayabilirsiniz.

---

## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına bakın.
