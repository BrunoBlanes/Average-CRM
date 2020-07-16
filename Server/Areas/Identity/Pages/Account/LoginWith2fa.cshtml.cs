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
	public class LoginWith2faModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ILogger<LoginWith2faModel> logger;

		[Required]
		[BindProperty]
		[DataType(DataType.Text)]
		[Display(Name = "Authenticator code")]
		[StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		public string TwoFactorCode { get; set; }

		[BindProperty]
		[Display(Name = "Remember this machine")]
		public bool RememberMachine { get; set; }
		public bool RememberMe { get; set; }
		public string? ReturnUrl { get; set; }

		public LoginWith2faModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginWith2faModel> logger)
		{
			this.logger = logger;
			this.signInManager = signInManager;
			TwoFactorCode = string.Empty;
		}

		public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
		{
			// Ensure the user has gone through the username & password screen first
			ApplicationUser? user = await signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user is null) throw new InvalidOperationException($"Unable to load two-factor authentication user.");
			ReturnUrl = returnUrl;
			RememberMe = rememberMe;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
		{
			if (ModelState.IsValid)
			{

				ApplicationUser? user = await signInManager.GetTwoFactorAuthenticationUserAsync();
				if (user is null) throw new InvalidOperationException($"Unable to load two-factor authentication user.");
				var authenticatorCode = TwoFactorCode
					.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
					.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
				Microsoft.AspNetCore.Identity.SignInResult? result = await signInManager.TwoFactorAuthenticatorSignInAsync(
					authenticatorCode,
					rememberMe,
					RememberMachine);

				if (result.Succeeded)
				{
					logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
					return LocalRedirect(returnUrl);
				}

				else if (result.IsLockedOut)
				{
					logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
					return RedirectToPage("./Lockout");
				}

				else
				{
					logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
					ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
					return Page();
				}
			}

			return Page();
		}
	}
}
