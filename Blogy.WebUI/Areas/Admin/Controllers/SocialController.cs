using Blogy.Business.Services.SocialService;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = Roles.Admin)]
	public class SocialController(ISocialService _socialService) : Controller
	{
		public async Task<IActionResult> Index()
		{
			var social = await _socialService.GetAllAsync();	
			return View(social);
		}
	}
}
