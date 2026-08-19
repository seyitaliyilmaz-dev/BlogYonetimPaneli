# Blog Yönetim Paneli

Kullanıcıların kayıt olup giriş yaparak blog yazıları oluşturabildiği, Markdown ile biçimlendirebildiği ve kategorilere ayırabildiği bir blog yönetim paneli. ASP.NET Core MVC ile geliştirilmiştir.

## Özellikler

- **CRUD işlemleri** — yazı ve kategori ekleme, listeleme, düzenleme, silme
- **Markdown destekli içerik editörü** — yazı içeriği Markdown olarak yazılır, Markdig ile HTML'e dönüştürülüp görüntülenir
- **Giriş / Kayıt sistemi** — ASP.NET Core Identity ile kimlik doğrulama
- **Kategori ilişkilendirmesi** — her yazı bir kategoriye bağlıdır (Post ↔ Category)
- **Serbest kategori girişi** — kategori yazarken serbestçe girilir; aynı isim varsa mevcut kategori kullanılır, yoksa otomatik oluşturulur
- **Sahiplik kontrolü ve Admin rolü** — her kullanıcı yalnızca kendi yazısını düzenleyip silebilir; Admin rolündeki kullanıcılar tüm yazıları ve kategorileri yönetebilir
- Özgün, kütüphane şablonundan bağımsız arayüz tasarımı

## Kullanılan Teknolojiler

- ASP.NET Core MVC (.NET)
- Entity Framework Core (SQL Server)
- ASP.NET Core Identity (roller dahil)
- Markdig (Markdown → HTML dönüşümü)
- Bootstrap + özel CSS

## Projeyi Çalıştırma

### Gereksinimler

- [.NET SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express veya tam sürüm — bağlantı dizesine göre)

### Adımlar

1. Depoyu klonlayın:

```bash
   git clone https://github.com/seyitaliyilmaz-dev/BlogYonetimPaneli.git
   cd BlogYonetimPaneli
```

2. Bağımlılıkları geri yükleyin:

```bash
   dotnet restore
```

3. `appsettings.json` içindeki `DefaultConnection` bağlantı dizesini kendi SQL Server örneğinize göre düzenleyin.

4. Veritabanını oluşturun:

```bash
   dotnet ef database update
```

   > `dotnet ef` komutu bulunamazsa önce şunu çalıştırın: `dotnet tool install --global dotnet-ef`

5. Uygulamayı başlatın:

```bash
   dotnet run
```

6. Terminalde görünen adresi tarayıcıda açın (örn. `http://localhost:5280`).

### Admin Kullanıcı Tanımlama

Uygulama ilk çalıştırıldığında "Admin" rolü otomatik olarak oluşturulur. Bir kullanıcıyı admin yapmak için, kayıt olduktan sonra veritabanında `AspNetUserRoles` tablosuna ilgili kullanıcı ve Admin rolü Id'lerini eşleştiren bir kayıt eklemeniz gerekir.

## Kullanım

1. Anasayfada **Kayıt Ol** ile bir hesap oluşturun.
2. Giriş yaptıktan sonra **Yeni Yazı** ile Markdown formatında içerik yazın, bir kategori adı girin.
3. Yazı detay sayfasında Markdown içeriğin HTML'e dönüştürülmüş halini görün.
4. Kendi yazılarınızı düzenleyip silebilirsiniz; başka kullanıcıların yazılarına yalnızca Admin erişebilir.

## Proje Yapısı
