using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class GenerateRecoveryCodesModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<GenerateRecoveryCodesModel> logger;

		[TempData]
		public IEnumerable<string>? RecoveryCodes { get; set; }

		[TempData]
		public string? StatusMessage { get; set; }

		public GenerateRecoveryCodesModel(
			UserManager<ApplicationUser> userManager,
			ILogger<GenerateRecoveryCodesModel> logger)
		{
			this.logger = logger;
			this.userManager = userManager;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);

			if (isTwoFactorEnabled is not true)
			{
				var userId = await userManager.GetUserIdAsync(user);
				throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
			var userId = await userManager.GetUserIdAsync(user);
			
			if (isTwoFactorEnabled is not true)
			{
				throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
			}

			RecoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
			logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
			StatusMessage = "You have generated new recovery codes.";
			return RedirectToPage("./ShowRecoveryCodes");
		}
	}
}