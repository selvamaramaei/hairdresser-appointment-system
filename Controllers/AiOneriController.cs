using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class AiOneriController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AiOneriController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new AiOneri();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessHairStyle(AiOneri model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-rapidapi-host", "hairstyle-changer.p.rapidapi.com");
            client.DefaultRequestHeaders.Add("x-rapidapi-key", "028f298abbmshdd13ab1627c15dcp150482jsnb47bc5a09a47");

            using var content = new MultipartFormDataContent();
            using var stream = model.Photo.OpenReadStream();
            content.Add(new StreamContent(stream), "image", model.Photo.FileName);
            content.Add(new StringContent(model.HairStyle.ToString()), "type");

            var response = await client.PostAsync("https://hairstyle-changer.p.rapidapi.com/huoshan/facebody/hairstyle", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası: {response.StatusCode}, Mesaj: {errorMessage}");
                ModelState.AddModelError(string.Empty, "API'den geçerli bir yanıt alınamadı. Lütfen tekrar deneyin.");
                return View("Index", model);
            }

            // API yanıtını işleme
            var resultImage = await response.Content.ReadAsByteArrayAsync();
            var base64Image = Convert.ToBase64String(resultImage);
            ViewBag.ResultImage = $"data:image/jpeg;base64,{base64Image}";

            // Sonuç görüntüsünü aynı sayfada göstermek için Index görünümüne dön
            return View("Index", model);
        }
    }
}
