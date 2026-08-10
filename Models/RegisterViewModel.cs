using System.ComponentModel.DataAnnotations;

namespace BlogYonetimPaneli.Models
{
    // Kayıt (Register) formundan gelen verileri taşıyan view model.
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalı.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Password alanıyla aynı olup olmadığı [Compare] ile otomatik kontrol edilir.
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
