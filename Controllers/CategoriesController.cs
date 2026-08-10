using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogYonetimPaneli.Data;
using BlogYonetimPaneli.Models;

namespace BlogYonetimPaneli.Controllers
{
    // Kategorilerle ilgili tüm CRUD işlemlerini yöneten controller.
    // Sınıfın tamamı [Authorize] ile korunur; yalnızca Index ve Details
    // aksiyonları [AllowAnonymous] ile herkese açılır.
    [Authorize]
    public class CategoriesController : Controller
    {
        // Veritabanına erişim için DbContext, constructor injection ile alınır.
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        // Tüm kategorileri listeler. Herkes (giriş yapmasa bile) görebilir.
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        // Tek bir kategorinin detayını ve ona bağlı yazıları gösterir.
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Include(c => c.Posts): kategoriye bağlı yazılar da birlikte çekilir (eager loading).
            var category = await _context.Categories
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        // Boş kategori ekleme formunu gösterir.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // Formdan gelen veriyi doğrular ve veritabanına kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF (siteler arası istek sahteciliği) saldırılarına karşı korur
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            if (ModelState.IsValid) // [Required], [StringLength] gibi kurallar sağlanmış mı?
            {
                _context.Add(category);
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına yazar
                return RedirectToAction(nameof(Index));
            }
            // Doğrulama başarısızsa, kullanıcının girdiği verilerle formu tekrar gösterir.
            return View(category);
        }

        // GET: Categories/Edit/5
        // Düzenlenecek kategoriyi bulup formu doldurur.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        // Formdan gelen güncellenmiş veriyi kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Kayıt, kaydetme sırasında başka biri/işlem tarafından
                    // silinmişse anlamlı bir NotFound döner, aksi halde hatayı fırlatır.
                    if (!_context.Categories.Any(e => e.Id == category.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        // Silme onay sayfasını gösterir (silme işlemini henüz yapmaz).
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        // Kullanıcı onay verdikten sonra gerçek silme işlemini yapar.
        // ActionName("Delete") sayesinde GET Delete ile aynı URL'i paylaşır.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
