using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogYonetimPaneli.Data;
using BlogYonetimPaneli.Models;

namespace BlogYonetimPaneli.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Giriş yapmış kullanıcının kimliğine (Id, rol vb.) erişmek için eklendi.
        private readonly UserManager<IdentityUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            // "Oluşturan" sütununda e-posta gösterebilmek için, yazılardaki
            // AuthorId'lere karşılık gelen kullanıcılar tek seferde çekiliyor.
            var authorIds = posts.Select(p => p.AuthorId).Where(id => id != null).Distinct().ToList();
            var authors = await _context.Users
                .Where(u => authorIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);
            ViewData["Authors"] = authors;

            // View'da "bu benim yazım mı" kontrolü yapabilmek için mevcut kullanıcının Id'si gönderiliyor.
            ViewData["CurrentUserId"] = _userManager.GetUserId(User);

            return View(posts);
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            // Yazar e-postasını gösterebilmek için ayrıca sorgulanıyor.
            if (post.AuthorId != null)
            {
                var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == post.AuthorId);
                ViewData["AuthorEmail"] = author?.Email;
            }

            ViewData["CurrentUserId"] = _userManager.GetUserId(User);

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // categoryName: kullanıcının serbestçe yazdığı kategori adı.
        // Aynı isimde kategori zaten varsa o kullanılır, yoksa otomatik oluşturulur.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,IsPublished")] Post post, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ViewData["CategoryError"] = "Kategori adı zorunludur.";
                ViewData["CategoryName"] = categoryName;
                return View(post);
            }

            if (ModelState.IsValid)
            {
                var trimmedName = categoryName.Trim();

                // Büyük/küçük harf farkı gözetmeden var olan kategori aranır.
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == trimmedName.ToLower());

                if (category == null)
                {
                    category = new Category { Name = trimmedName };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync(); // Id'sinin oluşması için önce kaydedilir
                }

                post.CategoryId = category.Id;
                post.CreatedAt = DateTime.Now;
                // Yazının sahibi, o an giriş yapmış kullanıcı olarak kaydediliyor.
                post.AuthorId = _userManager.GetUserId(User);

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryName"] = categoryName;
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();

            // Sadece yazının sahibi ya da Admin rolündeki kullanıcı düzenleyebilir.
            var currentUserId = _userManager.GetUserId(User);
            if (post.AuthorId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            ViewData["CategoryName"] = post.Category?.Name;
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,IsPublished,CreatedAt,AuthorId")] Post post, string categoryName)
        {
            if (id != post.Id) return NotFound();

            // Veritabanındaki gerçek kaydı çekip sahiplik kontrolünü buradan yapıyoruz
            // (formdan gelen AuthorId'ye güvenmek güvenlik açığı olurdu).
            var existingPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existingPost == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (existingPost.AuthorId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ViewData["CategoryError"] = "Kategori adı zorunludur.";
                ViewData["CategoryName"] = categoryName;
                return View(post);
            }

            if (ModelState.IsValid)
            {
                var trimmedName = categoryName.Trim();
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == trimmedName.ToLower());

                if (category == null)
                {
                    category = new Category { Name = trimmedName };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                }

                post.CategoryId = category.Id;
                // Orijinal yazarın değişmemesi için veritabanındaki değer korunur.
                post.AuthorId = existingPost.AuthorId;

                try
                {
                    post.UpdatedAt = DateTime.Now;
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

            ViewData["CategoryName"] = categoryName;
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (post.AuthorId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (post.AuthorId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
