// using Blogy.Business.DTOs.ContactDtos;
// using Blogy.Business.Services.ContactService;
// using Microsoft.AspNetCore.Mvc;
// using System.Net.Http.Json;
// using System.Text.Json;
// using System.Text.Json.Serialization;
using Blogy.Business.DTOs.ContactDtos;
using Blogy.Business.Services.ContactService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace Blogy.WebUI.Controllers
{
	// ... (Gemini DTO'ları aynı kalır) ...
	public class Part
	{
		[JsonPropertyName("text")]
		public string Text { get; set; }
	}

	public class Content
	{
		[JsonPropertyName("parts")]
		public Part[] Parts { get; set; }
	}

	public class GeminiRequest
	{
		[JsonPropertyName("contents")]
		public Content[] Contents { get; set; }
	}

	public class Candidate
	{
		[JsonPropertyName("content")]
		public Content Content { get; set; }
	}

	public class GeminiResponse
	{
		[JsonPropertyName("candidates")]
		public Candidate[] Candidates { get; set; }
	}
	// --------------------------------------------------------

	public class ContactsController(
		IContactService _contactService,
		HttpClient _httpClient) : Controller
	{
		// **!!! SMTP Bilgileri - Kendi Bilgilerinizle Doldurmayı Unutmayın !!!**
		private const string SmtpHost = "smtp.gmail.com";
		private const int SmtpPort = 587;
		private const string SmtpUsername = "merve.akba54@gmail.com";
		private const string SmtpPassword = "eaxruurahdghexac";
		private const string AutoReplySubject = "Blogy - Mesajınız Alındı";

		// Gemini API Bilgileri
		private readonly string _geminiApiKey = "";
		private readonly string _geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

		public IActionResult Index() => View();

		[HttpGet]
		public PartialViewResult CreateContact() => PartialView();

		[HttpPost]
		public async Task<IActionResult> CreateContact(CreateContactDto createContactDto)
		{
			if (ModelState.IsValid)
			{
				string autoReply = string.Empty;

				// 1. Gemini API İşlemi (Otomatik Cevap Oluşturma)
				try
				{
					// Yeni ve daha kısa Prompt
					string prompt = $"""
                        Aşağıdaki Konu ve Mesaj içeriğine dayanarak, kullanıcının mesajı için teşekkür eden, 
                        mesajına atıfta bulunan ve kısa süre içinde kendisiyle iletişime geçileceğini belirten, 
                        nazik ve kısa bir otomatik cevap taslağı oluştur. Cevabınız sadece otomatik cevap metni içermelidir.
                        
                        Konu: {createContactDto.Subject}
                        Mesaj: {createContactDto.Message}
                        """;

					var requestBody = new GeminiRequest
					{
						Contents = new[]
						{
							new Content
							{
								Parts = new[] { new Part { Text = prompt } }
							}
						}
					};

					string fullUrl = $"{_geminiEndpoint}?key={_geminiApiKey}";

					var response = await _httpClient.PostAsJsonAsync(fullUrl, requestBody);
					response.EnsureSuccessStatusCode();

					var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();

					autoReply = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
								?? "Otomatik cevap oluşturulamadı ancak mesajınız elimize ulaştı.";
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Gemini API (HTTP) hatası: {ex.Message}");
					autoReply = "Mesajınız başarıyla alınmıştır. En kısa sürede size geri dönüş yapacağız. Anlayışınız için teşekkür ederiz.";
				}

				// 2. Oluşturulan cevabı DTO'ya atama ve Veritabanına kaydetme
				createContactDto.AutoReplyContent = autoReply;
				await _contactService.CreateAsync(createContactDto);

				// 3. KULLANICIYA OTOMATİK CEVAP E-POSTASI GÖNDERME
				try
				{
					using (var mail = new MailMessage())
					{
						mail.From = new MailAddress(SmtpUsername, "Blogy");
						mail.To.Add(createContactDto.Email);
						mail.Subject = AutoReplySubject;
						mail.Body = autoReply;
						mail.IsBodyHtml = false;

						using (var smtpClient = new SmtpClient(SmtpHost, SmtpPort))
						{
							smtpClient.UseDefaultCredentials = false;
							smtpClient.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);
							smtpClient.EnableSsl = true;

							await smtpClient.SendMailAsync(mail);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"E-posta gönderme hatası: {ex.Message}");
				}

				return RedirectToAction(nameof(Index));
			}

			return PartialView(createContactDto);
		}
	}
}