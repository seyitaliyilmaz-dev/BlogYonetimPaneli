using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogYonetimPaneli.Models;

namespace BlogYonetimPaneli.Controllers
{
    // Kullanıcı kayıt, giriş ve çıkış işlemlerini yöneten controller.
    // ASP.NET Core Identity'nin sağladığı SignInManager ve UserManager
    // servisleri üzerinden çalışır.
    public class AccountController : Controller
    {
        // Kullanıcının oturum açıp kapatmasını yöneten servis.
        private readonly SignInManager<IdentityUser> _signInManager;

        // Kullanıcı oluşturma, şifre kontrolü gibi işlemleri yöneten servis.
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Account/Register
        // Boş kayıt formunu gösterir.
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        // Yeni kullanıcı hesabı oluşturur ve otomatik olarak giriş yaptırır.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // E-posta hem kullanıcı adı hem de e-posta olarak kullanılıyor.
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };

            // CreateAsync şifreyi güvenli şekilde (hash'leyerek) saklar.
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kayıt başarılıysa kullanıcı ekstra bir giriş adımına gerek kalmadan içeri alınır.
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Posts");
            }

            // Identity'nin ürettiği hatalar (örn. "şifre çok kısa") forma yansıtılır.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Account/Login
        // returnUrl: korumalı bir sayfadan buraya yönlendirildiyse,
        // giriş sonrası geri dönülecek adresi taşır.
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        // Girilen e-posta/şifreyi doğrular, başarılıysa oturum açar.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // lockoutOnFailure: false -> yanlış denemelerde hesap kilitlenmez (staj/geliştirme için basit tutuldu).
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Kullanıcı korumalı bir sayfadan yönlendirildiyse oraya geri gönderilir.
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Posts");
            }

            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View(model);
        }

        // POST: Account/Logout
        // Oturumu kapatır ve anasayfaya yönlendirir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        // Yetkisi olmayan bir kullanıcı korumalı bir alana erişmeye çalışırsa gösterilir.
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
