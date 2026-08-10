using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogYonetimPaneli.Data;

// Uygulamanın giriş noktası. Servislerin kaydedildiği ve
// HTTP istek hattının (pipeline) yapılandırıldığı yer burasıdır.
var builder = WebApplication.CreateBuilder(args);

// appsettings.json içindeki "DefaultConnection" bağlantı cümlesi okunuyor.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Entity Framework Core, SQL Server sürücüsüyle DI (Dependency Injection)
// konteynerine kaydediliyor. Controller'lar ApplicationDbContext'i
// constructor üzerinden bu sayede otomatik alabiliyor.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ASP.NET Core Identity (kullanıcı, rol, giriş/kayıt alt yapısı) ekleniyor.
// Şifre kuralları burada gevşetiliyor (proje/staj ortamı için pratik olsun diye).
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;          // Şifrede en az bir rakam zorunlu
    options.Password.RequiredLength = 6;            // Minimum şifre uzunluğu
    options.Password.RequireNonAlphanumeric = false; // Özel karakter zorunlu değil
    options.Password.RequireUppercase = false;       // Büyük harf zorunlu değil
})
    .AddEntityFrameworkStores<ApplicationDbContext>() // Identity verileri EF Core ile saklanır
    .AddDefaultTokenProviders();                      // Şifre sıfırlama vb. token üretimi için

// Giriş yapılmamış kullanıcı korumalı bir sayfaya girmeye çalışırsa
// yönlendirileceği sayfalar tanımlanıyor.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// MVC (Controller + View) desteği ekleniyor.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Geliştirme ortamında değilsek (yani yayında/production'da) genel hata
// sayfası gösterilir ve HSTS güvenlik başlığı eklenir.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS'e yönlendirir
app.UseStaticFiles();      // wwwroot altındaki CSS/JS/görsel gibi statik dosyaları sunar

app.UseRouting(); // Gelen isteğin hangi controller/action'a gideceğine karar verir

app.UseAuthentication(); // Kullanıcının kim olduğunu çözer (cookie okunur)
app.UseAuthorization();  // [Authorize] kurallarını uygular

// Varsayılan route şablonu: /Controller/Action/id
// Uygulama açıldığında ilk olarak Posts/Index çalışır.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");

app.Run(); // Uygulamayı ayağa kaldırır ve istek dinlemeye başlar
