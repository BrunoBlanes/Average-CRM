using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages
{
	[AllowAnonymous]
	public class RegisterConfirmationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;

		public RegisterConfirmationModel(UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
		{
			if (email is null)
			{
				return RedirectToPage("/Index");
			}

			ApplicationUser? user = await userManager.FindByEmailAsync(email);

			if (user is null)
			{
				return NotFound($"Unable to load user with email '{email}'.");
			}

			return Page();
		}
	}
}