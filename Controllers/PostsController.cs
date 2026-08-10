using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogYonetimPaneli.Data;
using BlogYonetimPaneli.Models;

namespace BlogYonetimPaneli.Controllers
{
    // Blog yazılarıyla ilgili tüm CRUD işlemlerini yöneten controller.
    // Sınıfın tamamı [Authorize] ile korunur; Index ve Details herkese açık.
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        // Tüm yazıları, kategorileriyle birlikte en yeniden eskiye sıralı listeler.
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.Category)       // İlişkili kategori bilgisini de getirir
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // GET: Posts/Details/5
        // Tek bir yazının detayını gösterir; içerik burada Markdown'dan HTML'e çevrilir.
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // GET: Posts/Create
        // Yeni yazı formunu, kategori seçim listesiyle birlikte hazırlar.
        public IActionResult Create()
        {
            // SelectList, Category tablosundaki kayıtları <select> elemanına dönüştürür.
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // Formdan gelen yeni yazıyı doğrular ve kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,CategoryId,IsPublished")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now; // Oluşturulma tarihi sunucu tarafında set edilir
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Doğrulama hatası varsa, kategori listesi tekrar doldurulup form yeniden gösterilir.
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        // Düzenlenecek yazıyı bulup formu, seçili kategoriyle birlikte doldurur.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // Güncellenmiş yazı verisini kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CategoryId,IsPublished,CreatedAt")] Post post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    post.UpdatedAt = DateTime.Now; // Güncellenme tarihi burada set edilir
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Posts.Any(e => e.Id == post.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Delete/5
        // Silme onay sayfasını gösterir.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Posts/Delete/5
        // Onay sonrası gerçek silme işlemini gerçekleştirir.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
