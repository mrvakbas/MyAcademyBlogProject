using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.User.ViewComponents
{
	public class _UserSidebarPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
