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



1.  **Projeyi Klonlayın veya İndirin:**

```bash

    git clone [https://github.com/KULLANICI_ADINIZ/CrmYonetimSistemi.git](https://github.com/KULLANICI_ADINIZ/CrmYonetimSistemi.git)

```

2.  **Projeyi Visual Studio'da Açın:**

    * `CrmYonetimSistemi.sln` dosyasına çift tıklayarak projeyi açın.



3.  **Veritabanı Bağlantısını Yapılandırma:**

    * `appsettings.json` dosyasını açın.

    * `ConnectionStrings` bölümündeki `DefaultConnection` değerini kendi SQL Server sunucunuza göre güncelleyin.



```json

    "ConnectionStrings": {

      "DefaultConnection": "Server=SUNUCU_ADINIZ;Database=CrmYonetimSistemiDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"

    }

```

    * **`SUNUCU_ADINIZ`**: SQL Server sunucunuzun adıdır (Örn: `DESKTOP-ABC\SQLEXPRESS` veya `localhost`).



4.  **İlk Admin Şifresini Belirleme:**

    * `Data/DbSeeder.cs` dosyasını açın.

    * `superAdminPassword` değişkenine ilk admin kullanıcısı için güçlü bir şifre atayın.

```csharp

    // LÜTFEN BU ŞİFREYİ GÜÇLÜ BİR ŞİFRE İLE DEĞİŞTİRİN!

    string superAdminPassword = "Password123!";

```



5.  **Veritabanını Oluşturma:**

    * Visual Studio'da **Tools -> NuGet Package Manager -> Package Manager Console** menüsünü açın.

    * Açılan konsola aşağıdaki komutu yazarak veritabanını ve tabloları oluşturun:

```powershell

    Update-Database

```



### Uygulamayı Çalıştırma



1.  Tüm yapılandırmalar tamamlandıktan sonra, Visual Studio'da `F5` tuşuna basarak projeyi başlatın.

2.  Uygulama ilk kez çalıştığında, `DbSeeder` mekanizması `Admin` ve `User` rollerini ve `DbSeeder.cs` dosyasında şifresini belirlediğiniz `superadmin` kullanıcısını otomatik olarak oluşturacaktır.



---



### ⚠️ Kurulum Sonrası Önemli Adımlar (GÜVENLİK)



`superadmin` kullanıcısı başarıyla oluşturulduktan sonra, bu tek seferlik kurulum işlemini devre dışı bırakmak **güvenlik ve performans için çok önemlidir.**



1.  **`Program.cs` dosyasını açın.**

2.  `// --- SEEDER ÇAĞRISI ---` bloğunun altındaki seeder'ı çağıran satırı bulun.

3.  Bu satırı **yorum satırı yapın veya silin**:



 ```csharp

    // ESKİ HALİ:

     await DbSeeder.SeedRolesAndSuperAdminAsync(services);



    // YENİ HALİ:

    // await DbSeeder.SeedRolesAndSuperAdminAsync(services); // Bu satırı devre dışı bırakın ya da silin.

 ```

Bu işlemi yaptıktan sonra uygulama her başladığında yeniden kullanıcı oluşturmaya çalışmaz ve kodunuzda gereksiz bir işlem kalmaz.



---



## 📋 Uygulama Kullanım Adımları



1.  Uygulama açıldığında "Giriş Yap" sayfasına gidin.

2.  **Kullanıcı Adı:** `DbSeeder.cs` dosyasında belirlediğiniz kullanıcı adı ile giriş yapın.

3.  **Şifre:** `DbSeeder.cs` dosyasında belirlediğiniz şifre ile giriş yapın.

4.  Giriş yaptıktan sonra, üst menüdeki **"Kullanıcı Yönetimi"** linkinden yeni kullanıcılar oluşturabilir ve rollerini yönetebilirsiniz.

5.  Diğer menü linklerini kullanarak müşteri, ürün, satış, gider ve ödeme kayıtları oluşturmaya başlayabilirsiniz.



---



## 📷 Uygulama Görselleri

**Kontrol Paneli**

<img width="1920" height="717" alt="Dashboard" src="https://github.com/user-attachments/assets/52a2c0b3-dad3-4d96-a2b5-415bd57f9b7a" />

**Kullanıcı Yönetimi**

<img width="1920" height="513" alt="UserManagement" src="https://github.com/user-attachments/assets/cce6727c-3107-4dff-b256-4c2f705c54bb" />

**Yeni Ürün Alımı**

<img width="1516" height="883" alt="BuyingNewItem" src="https://github.com/user-attachments/assets/a3cc0cca-63a4-4125-90dd-38f062ccc86a" />

**Ürün Alım Geçmişi**

<img width="1915" height="370" alt="BuyingItemHistory" src="https://github.com/user-attachments/assets/53af42f5-8dd9-43d3-879b-9bdc865880ed" />

**Ürün (stok) Yönetimi**

<img width="1920" height="407" alt="ItemManagement" src="https://github.com/user-attachments/assets/2d6fee68-f790-4e82-a53c-32504cbbe996" />

**Gider Listesi**

<img width="1920" height="420" alt="ExpensesList" src="https://github.com/user-attachments/assets/32a5e19b-8050-4937-b15a-13aec0b6c339" />

**Satış Kaydı Ekleme**

<img width="1920" height="722" alt="NewSale" src="https://github.com/user-attachments/assets/cb28be3a-dd27-4c4b-b929-33253e01faec" />

**Ödeme Kayıtları**

<img width="1920" height="442" alt="TestPayment" src="https://github.com/user-attachments/assets/07e7c797-20fe-4ff9-bf49-e84526851965" />

**Proforma Çıktısı**

<img width="656" height="923" alt="Proforma" src="https://github.com/user-attachments/assets/d917e4e5-b0b0-497c-b418-c6d570168a63" />



---



## 📄 Lisans



Bu proje MIT Lisansı altında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına bakın.
