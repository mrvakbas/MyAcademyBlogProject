using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.Admin.ViewComponents
{
    public class _AdminScriptsPartial : ViewComponent
    {
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}