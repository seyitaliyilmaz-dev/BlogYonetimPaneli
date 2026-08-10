using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogYonetimPaneli.Models
{
    // Bir blog yazısını temsil eden model sınıfı.
    // Veritabanındaki "Posts" tablosuna karşılık gelir.
    public class Post
    {
        // Birincil anahtar (primary key). EF Core, "Id" ismini
        // otomatik olarak PK olarak tanır.
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // Yazının asıl içeriği. Markdown formatında düz metin
        // olarak saklanır; ekranda gösterilirken MarkdownHelper
        // ile HTML'e dönüştürülür.
        [Required(ErrorMessage = "İçerik zorunludur.")]
        public string Content { get; set; } = string.Empty;

        // Yazı oluşturulduğu an otomatik atanır (controller'da set edilir).
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Yazı düzenlendiğinde doldurulur, hiç düzenlenmediyse null kalır.
        public DateTime? UpdatedAt { get; set; }

        // Yazının yayında mı yoksa taslak mı olduğunu belirtir.
        public bool IsPublished { get; set; } = false;

        // Category tablosuna referans veren yabancı anahtar (foreign key).
        [Required(ErrorMessage = "Kategori seçilmelidir.")]
        public int CategoryId { get; set; }

        // CategoryId üzerinden ilişkili Category nesnesine erişimi sağlayan
        // navigasyon özelliği (navigation property).
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // İleride "hangi kullanıcı yazdı" bilgisini tutmak için ayrılmış alan
        // (Identity kullanıcı Id'si buraya yazılabilir).
        public string? AuthorId { get; set; }
    }
}
