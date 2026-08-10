using Markdig;

namespace BlogYonetimPaneli.Helpers
{
    // Markdown metnini HTML'e çevirmek için kullanılan yardımcı (statik) sınıf.
    // Views/Posts/Details.cshtml içinde çağrılır.
    public static class MarkdownHelper
    {
        // Pipeline, Markdig'in hangi Markdown özelliklerini (tablo, kalın yazı,
        // liste vb.) destekleyeceğini belirler. Uygulama boyunca tek sefer
        // oluşturulup tekrar tekrar kullanılması performans açısından önemlidir.
        private static readonly MarkdownPipeline Pipeline =
            new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        // Verilen Markdown metnini güvenli şekilde HTML çıktısına dönüştürür.
        public static string ToHtml(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return string.Empty;

            return Markdown.ToHtml(markdown, Pipeline);
        }
    }
}
