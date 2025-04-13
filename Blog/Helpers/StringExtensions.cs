namespace Blog.Helpers
{
    public static class StringExtensions
    {
        public static string ToSeoUrl(this string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            return text.ToLower()
                       .Replace(" ", "-")
                       .Replace("ç", "c")
                       .Replace("ğ", "g")
                       .Replace("ı", "i")
                       .Replace("ö", "o")
                       .Replace("ş", "s")
                       .Replace("ü", "u");
        }
    }
    public static class ExceptionHelper
    {
        public static InvalidOperationException EfContextNotFound()
        {
            return new InvalidOperationException("EF Context bulunamadı.");
        }
    }
}
