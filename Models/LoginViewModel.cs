using System.ComponentModel.DataAnnotations;

namespace BlogYonetimPaneli.Models
{
    // Giriş (Login) formundan gelen verileri taşımak için kullanılan
    // view model. Doğrudan veritabanı tablosuna karşılık gelmez,
    // sadece form <-> controller arasında veri taşır.
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)] // View'da input'un type="password" olmasını sağlar
        public string Password { get; set; } = string.Empty;

        // İşaretlenirse oturum çerezi tarayıcı kapansa bile kalıcı olur.
        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
