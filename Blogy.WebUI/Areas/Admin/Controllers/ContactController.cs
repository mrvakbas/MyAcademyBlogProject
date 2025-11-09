using Blogy.Business.DTOs.ContactDtos;
using Blogy.Business.Services.ContactService;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = Roles.Admin)]
	public class ContactController(IContactService _contactService) : Controller
    {
		public async Task<IActionResult> Index()
		{
			var values = await _contactService.GetAllAsync();
			return View(values);
		}

		[HttpGet]
		public IActionResult CreateContact()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateContact(CreateContactDto createContactDto)
		{
			await _contactService.CreateAsync(createContactDto);
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> UpdateContact(int id)
		{
			var value = await _contactService.GetByIdAsync(id);
			return View(value);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateContact(UpdateContactDto updateContactDto)
		{
			await _contactService.UpdateAsync(updateContactDto);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> DeleteContact(int id)
		{
			await _contactService.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}
	}
}
