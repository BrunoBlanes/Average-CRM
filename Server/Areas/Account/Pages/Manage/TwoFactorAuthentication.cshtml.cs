using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class TwoFactorAuthenticationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		[BindProperty]
		public bool Is2faEnabled { get; set; }
		public bool IsMachineRemembered { get; set; }

		[TempData]
		public string? StatusMessage { get; set; }
		public bool HasAuthenticator { get; set; }
		public int RecoveryCodesLeft { get; set; }

		public TwoFactorAuthenticationModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<IActionResult> OnGet()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user is null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) is not null;
			Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
			IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
			RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPost()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user is null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			await signInManager.ForgetTwoFactorClientAsync();
			StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
			return RedirectToPage();
		}
	}
}