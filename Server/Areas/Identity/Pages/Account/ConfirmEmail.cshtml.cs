using System.Text;
using System.Threading.Tasks;

using CRM.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ConfirmEmailModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;

		[TempData]
		public string? StatusMessage { get; set; }

		public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<IActionResult> OnGetAsync(string userId, string code)
		{
			if (userId is null || code is null) return RedirectToPage("/Index");

			// Gets the user from the database
			ApplicationUser? user = await userManager.FindByIdAsync(userId);

			// User id is incorrect
			if (user is null) return NotFound($"Unable to load user with ID '{userId}'.");

			// Confirm the account
			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			IdentityResult? result = await userManager.ConfirmEmailAsync(user, code);
			StatusMessage = result.Succeeded
				? "Thank you for confirming your email."
				: "Error confirming your email.";
			return Page();
		}
	}
}