using Blogy.DataAccess.Context;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = $"{Roles.Admin}")]
	public class DashboardController(AppDbContext _context) : Controller
	{
		public IActionResult Index()
		{
			ViewBag.categoryCount = _context.Categories.Count();
			ViewBag.blogCount = _context.Blogs.Count();
			ViewBag.userCount = _context.Users.Count();
			return View();
		}
	}
}
