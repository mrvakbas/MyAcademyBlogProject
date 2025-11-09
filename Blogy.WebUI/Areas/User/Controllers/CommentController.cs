using Blogy.Business.DTOs.CommentDtos;
using Blogy.Business.Services.BlogServices;
using Blogy.Business.Services.CommentService;
using Blogy.DataAccess.Context;
using Blogy.Entity.Entities;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Blogy.WebUI.Areas.User.Controllers
{
	[Area(Roles.User)]
	[Authorize(Roles = Roles.User)]
	public class CommentController(ICommentService _commentService,
									AppDbContext _context,
									IBlogService _blogService,
									UserManager<AppUser> _userManager) : Controller
    {
		private async Task GetBlogs()
		{
			var blogs = await _blogService.GetAllAsync();
			ViewBag.Blogs = (from blog in blogs
							 select new SelectListItem
							 {
								 Text = blog.Title,
								 Value = blog.Id.ToString(),
							 }).ToList();
		}

		public async Task<IActionResult> Index()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var comments = _context.Comments.Include(y => y.Blog).Where(x => x.UserId == user.Id).ToList();
			return View(comments);
		}

		public async Task<IActionResult> CreateComment()
		{
			await GetBlogs();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateComment(CreateCommentDto createCommentDto)
		{
			await GetBlogs();
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			createCommentDto.UserId = user.Id;
			createCommentDto.CommentStatus = "Onay Bekliyor";
			await _commentService.CreateAsync(createCommentDto);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> DeleteComment(int id)
		{
			await _commentService.DeleteAsync(id);
			return RedirectToAction("Index");
		}
	}
}
