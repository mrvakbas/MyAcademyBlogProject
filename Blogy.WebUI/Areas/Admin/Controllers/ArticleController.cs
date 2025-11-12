using Blogy.WebUI.Areas.Admin.Models;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json; // JsonDocument için gerekli
using System.Threading.Tasks;

namespace Blogy.WebUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = Roles.Admin)]
	public class ArticleController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		// Lütfen buraya kendi Gemini API Key'inizi yapıştırın
		private readonly string _geminiApiKey = "";
		private readonly string _geminiModel = "gemini-2.5-flash";

		public ArticleController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View(new ArticleCreateViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> Index(ArticleCreateViewModel model)
		{
			if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Topic))
			{
				ModelState.AddModelError("", "Lütfen bir konu giriniz.");
				return View(model);
			}

			if (string.IsNullOrWhiteSpace(_geminiApiKey) || _geminiApiKey.Contains("BURAYA_GEMINI_API_KEYINIZI_YAPISTIRIN"))
			{
				ViewBag.GeneratedArticle = "Lütfen controller içindeki '_geminiApiKey' değişkenine geçerli bir API anahtarı girin.";
				return View(model);
			}

			var client = _httpClientFactory.CreateClient();
			string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiModel}:generateContent?key={_geminiApiKey}";

			// 1. Sadeleştirme: Prompt'lar tek bir stringde birleştirildi (Rol kısıtlamasını aşmak için)
			string finalPrompt =
				$"Sistem Talimatı: Sen, Türkçe blog makaleleri yazan yardımsever, uzman ve akıcı bir asistansın. Yazıların detaylı, özgün ve akıcı olmalı.\n\n" +
				$"Kullanıcı İsteği: {model.Topic} hakkında, Giriş, Gelişme ve Sonuç bölümlerinden oluşan, en az 1000 karakter uzunluğunda detaylı bir blog makalesi yaz.";

			// 2. Sadeleştirme: Request body sadece 'user' rolü ile gönderildi
			var requestBody = new
			{
				contents = new[]
				{
					new
					{
						role = "user",
						parts = new[]
						{
							new { text = finalPrompt }
						}
					}
				},
				generationConfig = new
				{
					maxOutputTokens = 2048, // Uzun makale için
					temperature = 0.7
				}
			};

			var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

			try
			{
				var response = await client.PostAsync(url, content);
				var responseString = await response.Content.ReadAsStringAsync();

				if (!response.IsSuccessStatusCode)
				{
					ViewBag.GeneratedArticle = $"API isteği başarısız oldu: HTTP {response.StatusCode}. Yanıt: {responseString}";
					return View(model);
				}

				// 3. Sadeleştirme: JSON yanıtını dinamik olarak ayrıştırma (Helper class'lar kaldırıldı)
				string generatedText = "Yazı oluşturulamadı. (Model yanıtı boş geldi veya güvenlik filtresine takıldı).";

				try
				{
					using (JsonDocument document = JsonDocument.Parse(responseString))
					{
						// JSON yolunu kısaltmak için sorgulama yapıyoruz
						var candidate = document.RootElement
							.GetProperty("candidates")[0];

						var part = candidate
							.GetProperty("content")
							.GetProperty("parts")[0];

						generatedText = part.GetProperty("text").GetString();
					}
				}
				catch (System.Exception ex) when (ex is JsonException || ex is System.IndexOutOfRangeException)
				{
					// JSON yapısı beklenenden farklıysa veya eleman bulunamazsa hata yakalama
					ViewBag.GeneratedArticle = $"API yanıtı ayrıştırılamadı veya boş geldi. JSON Hata: {ex.Message}. Gelen Yanıt: {responseString}";
					return View(model);
				}

				ViewBag.GeneratedArticle = generatedText.Trim();

				return View(model);
			}
			catch (System.Exception ex)
			{
				ViewBag.GeneratedArticle = $"Sunucu tarafında hata oluştu: {ex.Message}";
				return View(model);
			}
		}
	}
}