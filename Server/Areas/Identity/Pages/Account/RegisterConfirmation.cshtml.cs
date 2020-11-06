using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterConfirmationModel : PageModel
	{
		public RegisterConfirmationModel()
		{

		}

		public IActionResult OnGet(string email, string? returnUrl = null)
		{
			return email is null ? RedirectToPage("Index") : (IActionResult)Page();
		}
	}
}