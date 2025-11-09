using AutoMapper;
using Blogy.Business.DTOs.BlogDtos;
using Blogy.Business.DTOs.CommentDtos;
using Blogy.Business.Services.BlogServices;
using Blogy.Business.Services.CategoryServices;
using Blogy.Business.Services.CommentService;
using Blogy.DataAccess.Context;
using Blogy.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Blogy.WebUI.Controllers
{
	public class BlogController(IBlogService _blogService,
		ICategoryService _categoryService,
		IMapper _mapper,
		UserManager<AppUser> _userManager,
		ICommentService _commentService) : Controller
	{
		public async Task<IActionResult> Index(int page = 1, int pageSize = 2)
		{
			var blogs = await _blogService.GetAllAsync();
			var values = new PagedList<ResultBlogDto>(blogs.AsQueryable(), page, pageSize);
			return View(values);
		}
		public async Task<IActionResult> GetBlogsByCategory(int id)
		{
			var category = await _categoryService.GetByIdAsync(id);
			ViewBag.categoryName = category.Name;
			var blogs = await _blogService.GetBlogsByCategoryIdAsync(id);
			return View(blogs);
		}

		public async Task<IActionResult> BlogDetails(int id)
		{
			var blog = await _blogService.GetSingleByIdAsync(id);
			return View(blog);
		}


		[HttpPost]
		public async Task<IActionResult> CreateComment(CreateCommentDto createCommentDto, int id)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Index");
			}

			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			createCommentDto.UserId = user.Id;
			var blogs = await _blogService.GetSingleByIdAsync(id);
			createCommentDto.BlogId = blogs.Id;

			//Toxic Bert Api Analizi
			using (var client = new HttpClient())
			{
				var apiKey = "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";
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

				try
				{
					var translateRequestBody = new
					{
						inputs = createCommentDto.Content
					};

					var translateJson = JsonSerializer.Serialize(translateRequestBody);
					var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");

					var translateResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
					var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

					string englishText = createCommentDto.Content;
					if (translateResponseString.TrimStart().StartsWith("["))
					{
						var translateDoc = JsonDocument.Parse(translateResponseString);
						englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
					}

					var toxicRequestBody = new
					{
						inputs = englishText
					};

					var toxicJson = JsonSerializer.Serialize(toxicRequestBody);
					var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");
					var toxicResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/unitary/toxic-bert", toxicContent);
					var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();
					bool isToxic = false;

					if (toxicResponseString.TrimStart().StartsWith("["))
					{
						var toxicDoc = JsonDocument.Parse(toxicResponseString);
						foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
						{
							string label = item.GetProperty("label").GetString();
							double score = item.GetProperty("score").GetDouble();

							if ((label.Contains("toxic") || label.Contains("insult") || label.Contains("obscene")) && score > 0.5)
							{
								isToxic = true;
								break;
							}
						}
						createCommentDto.CommentStatus = isToxic ? "Toksik Yorum" : "Yorum Onaylandı";
					}
					if (string.IsNullOrEmpty(createCommentDto.CommentStatus))
					{
						createCommentDto.CommentStatus = "Yorum Onaylandı";
					}
				}
				catch (Exception ex)
				{
					createCommentDto.CommentStatus = "Onay Bekliyor";
				}

				await _commentService.CreateAsync(createCommentDto);
				return RedirectToAction("BlogDetails", "Blog", new { id = createCommentDto.BlogId });
			}
		}
	}
}
//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