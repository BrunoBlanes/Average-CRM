using System;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class Disable2faModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<Disable2faModel> logger;

		public Disable2faModel(
			UserManager<ApplicationUser> userManager,
			ILogger<Disable2faModel> logger)
		{
			this.userManager = userManager;
			this.logger = logger;
		}

		[TempData]
		public string StatusMessage { get; set; }

		public async Task<IActionResult> OnGet()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			if (!await userManager.GetTwoFactorEnabledAsync(user))
			{
				throw new InvalidOperationException($"Cannot disable 2FA for user with ID '{userManager.GetUserId(User)}' as it's not currently enabled.");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			IdentityResult? disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
			if (!disable2faResult.Succeeded)
			{
				throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{userManager.GetUserId(User)}'.");
			}

			logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userManager.GetUserId(User));
			StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
			return RedirectToPage("./TwoFactorAuthentication");
		}
	}
}