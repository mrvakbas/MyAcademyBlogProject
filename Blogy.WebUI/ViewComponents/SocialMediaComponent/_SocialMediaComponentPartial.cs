using Blogy.Business.Services.SocialService;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.ViewComponents.SocialMediaComponent
{
    public class _SocialMediaComponentPartial(ISocialService _socialService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
			var social = await _socialService.GetAllAsync();
			return View(social);
        }
    }
}
