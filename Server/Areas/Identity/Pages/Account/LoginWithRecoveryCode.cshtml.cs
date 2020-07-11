using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class LoginWithRecoveryCodeModel : PageModel
	{
		private readonly ILogger<LoginWithRecoveryCodeModel> logger;
		private readonly SignInManager<ApplicationUser> signInManager;

		[Required]
		[BindProperty]
		[DataType(DataType.Text)]
		[Display(Name = "Recovery Code")]
		public string RecoveryCode { get; set; }
		public string? ReturnUrl { get; set; }

		public LoginWithRecoveryCodeModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginWithRecoveryCodeModel> logger)
		{
			this.logger = logger;
			this.signInManager = signInManager;
			RecoveryCode = string.Empty;
		}

		public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
		{
			// Ensure the user has gone through the username & password screen first
			ApplicationUser? user = await signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user is null) throw new InvalidOperationException($"Unable to load two-factor authentication user.");
			ReturnUrl = returnUrl;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser? user = await signInManager.GetTwoFactorAuthenticationUserAsync();
				if (user is null) throw new InvalidOperationException($"Unable to load two-factor authentication user.");
				var recoveryCode = RecoveryCode.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
				Microsoft.AspNetCore.Identity.SignInResult? result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

				if (result.Succeeded)
				{
					logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
					return LocalRedirect(returnUrl ?? Url.Content("~/"));
				}

				if (result.IsLockedOut)
				{
					logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
					return RedirectToPage("./Lockout");
				}

				else
				{
					logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
					ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
					return Page();
				}
			}

			return Page();
		}
	}
}