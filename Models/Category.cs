using System.ComponentModel.DataAnnotations;

namespace BlogYonetimPaneli.Models
{
    // Blog yazılarının gruplandığı kategori modeli.
    // Veritabanındaki "Categories" tablosuna karşılık gelir.
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Bir kategoriye ait tüm yazıları tutan koleksiyon.
        // Bire-çok ilişkinin "çok" tarafı (bir kategori, birden fazla yazıya sahip olabilir).
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
