using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DotNetEnv;
namespace Blog.Services
{
    public class OpenAIService
    {
        

        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public OpenAIService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _apiKey = configuration["OpenAI:ApiKey"];
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> AnalyzeCommentAsync(string comment)
        {
            var request = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Kısa ve öz analiz yap. Yorum eğer hakaret, küfür, nefret, spam içeriyorsa sadece 'uygunsuz' yaz. Aksi halde 'uygun' yaz." },
                    new { role = "user", content = comment }
                }
            };
            // - Eğer "hata" ise, model düzgün cevap verememiştir (örneğin API hatası, düşük içerik kalitesi vs.)
            // - Eğer "uygunsuz" kelimesini içeriyorsa, içerik yorum olarak eklenmemelidir.
            // Bu kontrol, OpenAI tarafında modeli Türkçe içeriklerde "uygunsuz" anahtar kelimesini dönecek şekilde eğitmem/ayarlamam sayesinde yapılabiliyor.
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❗ OpenAI HATASI: " + result);
                    Console.WriteLine("OpenAI İsteği => " + comment);
                    Console.WriteLine("HTTP DURUM: " + response.StatusCode);



                    return "hata";
                }

                using var json = JsonDocument.Parse(result);
                var answer = json.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
                Console.WriteLine("OpenAI İsteği => " + comment);
                Console.WriteLine("OpenAI API YANITI => " + answer);

                Console.WriteLine("🧠 AI YANITI: " + answer);
                return answer?.Trim().ToLower() ?? "hata";
            }

            catch (Exception ex)
            {
                Console.WriteLine("⚠️ OpenAIService JSON parse hatası: " + ex.Message);
                return "hata";
            }
        }
    }
}
