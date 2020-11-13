using System.Text;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Server.Areas.Account.Pages
{
	[AllowAnonymous]
	public class ConfirmEmailChangeModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		[TempData]
		public string? StatusMessage { get; set; }

		public ConfirmEmailChangeModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
		{
			if (userId is null || email is null || code is null)
			{
				return RedirectToPage("/Index");
			}

			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userId}'.");
			}

			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			IdentityResult? result = await userManager.ChangeEmailAsync(user, email, code);

			if (result.Succeeded is not true)
			{
				StatusMessage = "Error changing email.";
				return Page();
			}

			// TODO: Separate email from user name
			// In our UI email and user name are one and the same, so when we update the email
			// we need to update the user name.
			IdentityResult? setUserNameResult = await userManager.SetUserNameAsync(user, email);

			if (setUserNameResult.Succeeded is not true)
			{
				StatusMessage = "Error changing user name.";
				return Page();
			}

			await signInManager.RefreshSignInAsync(user);
			StatusMessage = "Thank you for confirming your email change.";
			return Page();
		}
	}
}
