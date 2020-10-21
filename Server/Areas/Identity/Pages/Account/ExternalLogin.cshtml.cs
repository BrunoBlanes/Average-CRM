using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ExternalLoginModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<ExternalLoginModel> logger;

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }
		public string? ProviderDisplayName { get; set; }
		public string? ReturnUrl { get; set; }

		[TempData]
		public string? ErrorMessage { get; set; }

		public ExternalLoginModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ILogger<ExternalLoginModel> logger)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
			Email = string.Empty;
		}

		public IActionResult OnGetAsync()
		{
			return RedirectToPage("./Login");
		}

		public IActionResult OnPost(string provider, string? returnUrl = null)
		{
			// Request a redirect to the external login provider.
			string? redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
			AuthenticationProperties? properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return new ChallengeResult(provider, properties);
		}

		public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
		{
			returnUrl ??= Url.Content("~/");

			if (remoteError is not null)
			{
				ErrorMessage = $"Error from external provider: {remoteError}";
				return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
			}

			ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync();

			if (info is null)
			{
				ErrorMessage = "Error loading external login information.";
				return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
			}

			// Sign in the user with this external login provider if the user already has a login.
			Microsoft.AspNetCore.Identity.SignInResult? result = await signInManager.ExternalLoginSignInAsync(
				info.LoginProvider,
				info.ProviderKey,
				isPersistent: false,
				bypassTwoFactor: true);

			if (result.Succeeded)
			{
				logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity?.Name, info.LoginProvider);
				return LocalRedirect(returnUrl);
			}

			if (result.IsLockedOut)
			{
				return RedirectToPage("./Lockout");
			}
			else
			{
				// If the user does not have an account, then ask the user to create an account.
				ReturnUrl = returnUrl;
				ProviderDisplayName = info.ProviderDisplayName;
				if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
					Email = info.Principal.FindFirstValue(ClaimTypes.Email);
				return Page();
			}
		}

		public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			// Get the information about the user from the external login provider
			ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync();

			if (info is null)
			{
				ErrorMessage = "Error loading external login information during confirmation.";
				return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
			}

			if (ModelState.IsValid)
			{
				// Create a new user from external login provider
				var user = new ApplicationUser { UserName = Email, Email = Email };
				IdentityResult? result = await userManager.CreateAsync(user);

				if (result.Succeeded)
				{
					result = await userManager.AddLoginAsync(user, info);

					if (result.Succeeded)
					{
						// Redirects to account confirmation page
						logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
						return RedirectToPage("./RegisterConfirmation", new { Email });
					}
				}

				foreach (IdentityError? error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}

			ProviderDisplayName = info.ProviderDisplayName;
			ReturnUrl = returnUrl;
			return Page();
		}
	}
}