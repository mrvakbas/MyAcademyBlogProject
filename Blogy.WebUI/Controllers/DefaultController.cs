using Blogy.Business.Services.BlogServices;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Controllers
{
	public class DefaultController(IBlogService _blogService) : Controller
	{
		public async Task<IActionResult> Index()
		{
			var blogs = await _blogService.GetAllAsync();
			return View(blogs);
		}

	}
}
