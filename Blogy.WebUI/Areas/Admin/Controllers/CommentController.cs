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

namespace Blogy.WebUI.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
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
            var comment = await _commentService.GetAllAsync();  
            return View(comment);   
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

		public async Task<IActionResult> CommentStatusChangeToToxic(int id)
		{
			var value = _context.Comments.Find(id);
			value.CommentStatus = "Toksik Yorum";
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> CommentStatusChangeToPassive(int id)
		{
			var value = _context.Comments.Find(id);
			value.CommentStatus = "Yorum Kaldırıldı";
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> CommentStatusChangeToActive(int id)
		{
			var value = _context.Comments.Find(id);
			value.CommentStatus = "Yorum Onaylandı";
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> DeleteComment(int id)
		{
            await _commentService.DeleteAsync(id);
			return RedirectToAction("Index");
		}
	}
}
