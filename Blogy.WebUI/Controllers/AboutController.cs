using Blogy.Business.Services.AboutService;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Controllers
{
    public class AboutController(IAboutService _aboutService) : Controller
    {
        public async Task<IActionResult> Index()
        {
			var values = await _aboutService.GetAllAsync();
			return View(values);
		}
    }
}
