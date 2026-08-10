namespace BlogYonetimPaneli.Models
{
    // Hata sayfasında gösterilecek isteğe bağlı bilgiyi taşıyan basit view model.
    public class ErrorViewModel
    {
        // Hangi HTTP isteğinin hataya sebep olduğunu belirten kimlik.
        public string? RequestId { get; set; }

        // RequestId doluysa true döner; view'da "Request ID: ..." satırının
        // gösterilip gösterilmeyeceğine bu karar verir.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
