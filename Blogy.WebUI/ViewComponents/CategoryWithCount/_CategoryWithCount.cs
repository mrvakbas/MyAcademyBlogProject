using Blogy.DataAccess.Context;
using Blogy.WebUI.Models;
using Microsoft.AspNetCore.Mvc;


namespace Blogy.WebUI.ViewComponents.CategoryWithCount
{
	public class _CategoryWithCount(AppDbContext _context) : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var values = _context.Categories.Select(c => new CategoryBlogCountViewModel
			{
				CategoryName = c.Name,
				BlogCount = c.Blogs.Count(),
				Id = c.Id,
			}).ToList();

			return View(values);
		}
	}
}
