using Blogy.Business.DTOs.AboutDtos;
using Blogy.Business.Services.AboutService;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Blogy.WebUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = Roles.Admin)]
	public class AboutController : Controller
	{
		private readonly IAboutService _aboutService;
		private readonly IConfiguration _configuration;

        public AboutController(IAboutService aboutService, IConfiguration configuration)
        {
            _aboutService = aboutService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
		{
			var values = await _aboutService.GetAllAsync();
			return View(values);
		}

		[HttpGet]
		public IActionResult CreateAbout()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateAbout(CreateAboutDto createAboutDto)
		{
			await _aboutService.CreateAsync(createAboutDto);
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> UpdateAbout(int id)
		{
			var value = await _aboutService.GetByIdAsync(id);
			return View(value);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAbout(UpdateAboutDto updateAboutDto)
		{
			await _aboutService.UpdateAsync(updateAboutDto);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> DeleteAbout(int id)
		{
			await _aboutService.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> GenerateAbout(string prompt)
		{
			if (string.IsNullOrWhiteSpace(prompt))
			{
				ViewData["Error"] = "Lütfen bir prompt girin.";
				var emptyList = await _aboutService.GetAllAsync() ?? new List<ResultAboutDto>();
				return View("Index", emptyList);
			}

			// 🔹 1. API Key'i appsettings.json'dan al
			var apiKey = _configuration["Gemini:ApiKey"];
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				ViewData["Error"] = "Gemini API key bulunamadı.";
				var listFailKey = await _aboutService.GetAllAsync() ?? new List<ResultAboutDto>();
				return View("Index", listFailKey);
			}

			// 🔹 2. Gemini API Endpoint'i (HuggingFace yerine)
			// Model adı (gemini-pro) ve API anahtarı URL'ye eklenir.
			var modelEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

			using var client = new HttpClient();

			// 🔹 3. Gemini için İstek Gövdesi (Request Body) Oluşturma
			// Gemini, HuggingFace'den farklı bir JSON yapısı bekler.
			var requestBody = new
			{
				contents = new[]
				{
					new
					{
						parts = new[]
						{
							new { text = prompt }
						}
					}
				}
			};

			var json = JsonSerializer.Serialize(requestBody);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync(modelEndpoint, content);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				ViewData["Error"] = $"Hakkımda yazısı oluşturulamadı. StatusCode: {response.StatusCode}. Detay: {errorContent}";
				var listFail = await _aboutService.GetAllAsync() ?? new List<ResultAboutDto>();
				return View("Index", listFail);
			}

			var responseString = await response.Content.ReadAsStringAsync();

			string generatedText;
			try
			{
				// 🔹 4. Gemini Yanıtını (Response) Ayrıştırma
				// Yanıt yapısı HuggingFace'den farklıdır.
				var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
				var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString, options);

				// Metin, 'candidates' dizisinin ilk elemanının içinde gizlidir.
				generatedText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

				if (string.IsNullOrWhiteSpace(generatedText))
				{
					// Bu durum, içeriğin güvenlik filtrelerine takılması vb. durumlarda oluşabilir.
					generatedText = "Yazı oluşturulamadı (İçerik alınamadı veya filtreye takıldı).";
				}
			}
			catch (Exception ex)
			{
				generatedText = $"Yanıt ayrıştırılamadı: {ex.Message}"; // fallback
			}

			ViewData["GeneratedText"] = generatedText;
			var values = await _aboutService.GetAllAsync() ?? new List<ResultAboutDto>();
			return View("Index", values);
		}

		// 🔹 5. Gemini Yanıtını (Response) Yakalamak için Gerekli Helper Class'lar
		// Bu sınıflar, API'den gelen karmaşık JSON yapısını C# nesnelerine dönüştürmek için gereklidir.
		private class GeminiResponse
		{
			public List<Candidate> Candidates { get; set; }
		}

		private class Candidate
		{
			public Content Content { get; set; }
		}

		private class Content
		{
			public List<Part> Parts { get; set; }
			public string Role { get; set; }
		}

		private class Part
		{
			public string Text { get; set; }
		}


	}
}