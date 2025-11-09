using Blogy.Business.DTOs.AboutDtos;
using Blogy.Business.Services.AboutService;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = Roles.Admin)]
	public class AboutController(IAboutService _aboutService) : Controller
    {
        public async Task<IActionResult> Index()
        {
			var values = await _aboutService.GetAllAsync();
			return View(values);
		}

		[HttpGet]
		public IActionResult CreateAbout()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateAbout(CreateAboutDto createAboutDto)
		{
			await _aboutService.CreateAsync(createAboutDto);
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> UpdateAbout(int id)
		{
			var value = await _aboutService.GetByIdAsync(id);
			return View(value);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAbout(UpdateAboutDto updateAboutDto)
		{
			await _aboutService.UpdateAsync(updateAboutDto);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> DeleteAbout(int id)
		{
			await _aboutService.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}

	}
}
