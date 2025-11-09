using Blogy.Business.DTOs.UserDtos;
using Blogy.Entity.Entities;
using Blogy.WebUI.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blogy.WebUI.Controllers
{
	public class LoginController(SignInManager<AppUser> _signInManager, UserManager<AppUser> _userManager) : Controller
	{
		[HttpGet]
		public IActionResult Index(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(LoginDto model, string returnUrl = null)
		{
			var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

			if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			if (!result.Succeeded)
			{

				ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı");
				return View(model);
			}
			else
			{
				var user = await _userManager.FindByNameAsync(model.UserName);
				var roles = await _userManager.GetRolesAsync(user);

				if (roles.Contains("Admin"))
				{
					return RedirectToAction("Index", "Blog", new { area = "Admin" });
				}
				if (roles.Contains("Writer"))
				{
					return RedirectToAction("Index", "Blog", new { area = "Writer" });
				}
				if (roles.Contains("User"))
				{
					return RedirectToAction("Index", "Blog", new { area = "User" });
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Rol tanımlanmamış. Yöneticiyle iletişime geçiniz.");
				}
			}
			return View(model);
		}
	}
}
