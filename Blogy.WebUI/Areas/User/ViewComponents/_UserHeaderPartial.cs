using Blogy.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.User.ViewComponents
{
    public class _UserHeaderPartial(UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.User = user.FirstName + " " + user.LastName;
			ViewBag.userImage = user.ImageUrl;
			ViewBag.userTitle = user.Title;
			return View();
        }
    }
}
