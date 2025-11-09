using Blogy.Business.DTOs.ContactDtos;
using Blogy.Business.Services.ContactService;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Controllers
{
    public class ContactsController(IContactService _contactService) : Controller
    {
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public PartialViewResult CreateContact()
		{
			return PartialView();
		}

		[HttpPost]
		public async Task<IActionResult> CreateContact(CreateContactDto createContactDto)
		{
			await _contactService.CreateAsync(createContactDto);
			return RedirectToAction(nameof(Index));
		}
	}
}
