using Blogy.Business.DTOs.BlogDtos;
using Blogy.Business.Services.BlogServices;
using Blogy.Business.Services.CategoryServices;
using Blogy.DataAccess.Context;
using Blogy.Entity.Entities;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Blogy.WebUI.Areas.Writer.Controllers
{
	[Area(Roles.Writer)]
	[Authorize(Roles = Roles.Writer)]
	public class BlogController(IBlogService _blogService, ICategoryService _categoryService, UserManager<AppUser> _userManager, AppDbContext _context) : Controller
	{
		private async Task GetCategoriesAsync()
		{
			var categories = await _categoryService.GetAllAsync();

			ViewBag.categories = (from category in categories
								  select new SelectListItem
								  {
									  Text = category.CategoryName,
									  Value = category.Id.ToString(),
								  }).ToList();
		}
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var blogs = _context.Blogs.Include(y => y.Category).Where(x => x.WriterId == user.Id).ToList();
			return View(blogs);
		}


		public async Task<IActionResult> CreateBlog()
		{
			await GetCategoriesAsync();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateBlog(CreateBlogDto blogDto, int id)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			blogDto.WriterId = user.Id;
			if (!ModelState.IsValid)
			{
				await GetCategoriesAsync();
				return View(blogDto);
			}

			await _blogService.CreateAsync(blogDto);
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> DeleteBlog(int id)
		{
			await _blogService.DeleteAsync(id);
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> UpdateBlog(int id)
		{
			await GetCategoriesAsync();
			var blog = await _blogService.GetByIdAsync(id);
			return View(blog);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateBlog(UpdateBlogDto updateBlogDto)
		{
			if (!ModelState.IsValid)
			{
				await GetCategoriesAsync();
				return View(updateBlogDto);
			}
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			updateBlogDto.WriterId = user.Id;
			await _blogService.UpdateAsync(updateBlogDto);
			return RedirectToAction("Index");
		}

	}
}
