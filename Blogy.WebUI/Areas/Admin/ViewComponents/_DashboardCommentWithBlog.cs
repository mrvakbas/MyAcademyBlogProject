using Blogy.DataAccess.Context;
using Blogy.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogy.WebUI.Areas.Admin.ViewComponents
{
    public class _DashboardCommentWithBlog(AppDbContext _context) : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var blogCount = await _context.Blogs.CountAsync();
			var commentCount = await _context.Comments.CountAsync();

			var result = await _context.Blogs
				.Include(b => b.Comments)
				.Select(b => new CommentCountWithBlogViewModel
				{
					BlogId = b.Id,
					BlogTitle = b.Title,
					CommentCount = b.Comments.Count()
				})
				.ToListAsync();


			return View(result);
		}
	}
}
