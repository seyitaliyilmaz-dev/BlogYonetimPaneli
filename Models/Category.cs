using System.ComponentModel.DataAnnotations;

namespace BlogYonetimPaneli.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
