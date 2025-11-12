using Blogy.DataAccess.Context;
using Blogy.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.Admin.ViewComponents
{
    public class _DashboardCategoryWithBlogCountComponentPartial(AppDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
			var values = _context.Categories.Select(c => new CategorysBlogCountViewModel
			{
				CategoryName = c.Name,
				BlogCount = c.Blogs.Count(),
				Id = c.Id,
			}).ToList();

			return View(values);
		}
    }
}
