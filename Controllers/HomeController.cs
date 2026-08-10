using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogYonetimPaneli.Models;

namespace BlogYonetimPaneli.Controllers
{
    // Anasayfa, gizlilik ve genel hata sayfalarını yöneten controller.
    // Proje dotnet new mvc ile oluşturulduğunda varsayılan olarak gelir.
    public class HomeController : Controller
    {
        // Loglama servisi; hata/olay kaydı tutmak için kullanılır (constructor injection).
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: Home/Index
        // Uygulama ilk açıldığında karşılanan anasayfayı gösterir
        // (Program.cs'teki varsayılan route Posts/Index olsa da, / adresine
        // doğrudan gidildiğinde bu aksiyon devreye girer).
        public IActionResult Index()
        {
            return View();
        }

        // GET: Home/Privacy
        // Şablondan gelen basit, statik bir gizlilik politikası sayfası.
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Error
        // Uygulamada beklenmeyen bir hata oluştuğunda kullanıcıya gösterilen
        // genel hata sayfası. Program.cs içinde production ortamında
        // UseExceptionHandler("/Home/Error") ile buraya yönlendirme yapılır.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // RequestId, hatayı GitHub/log kayıtlarında izlemek için kullanılan
            // benzersiz bir istek kimliğidir.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
