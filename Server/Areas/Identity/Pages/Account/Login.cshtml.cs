using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
	public class LoginModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ApplicationDbContext context;
		private readonly ILogger<LoginModel> logger;

		[BindProperty]
		public ICollection<AuthenticationScheme>? ExternalLogins { get; private set; }

		[TempData]
		public string? ErrorMessage { get; set; }
		public string? ReturnUrl { get; set; }

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		[Required]
		[BindProperty]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		public string Password { get; set; }

		[BindProperty]
		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }

		public LoginModel(SignInManager<ApplicationUser> signInManager,
			ApplicationDbContext context,
			ILogger<LoginModel> logger)
		{
			this.logger = logger;
			this.context = context;
			this.signInManager = signInManager;
			Password = string.Empty;
			Email = string.Empty;
		}

		public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
		{
			if (string.IsNullOrEmpty(ErrorMessage) is not true)
			{
				ModelState.AddModelError(string.Empty, ErrorMessage ?? string.Empty);
			}

			Setting settings = await context.Settings.FindAsync(1);
			if (settings is null || settings.FirstRun is true)
			{
				return LocalRedirect("~/Setup");
			}

			returnUrl ??= Url.Content("~/");

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
			ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			ReturnUrl = returnUrl;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				// Try to login
				Microsoft.AspNetCore.Identity.SignInResult? result = await signInManager.PasswordSignInAsync(
					Email,
					Password,
					RememberMe,
					lockoutOnFailure: true);

				// Sucessful login
				if (result.Succeeded)
				{
					logger.LogInformation("User logged in.");
					return LocalRedirect(returnUrl);
				}

				// Redirect to two factor auth
				if (result.RequiresTwoFactor)
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe });

				// Redirect t lockout
				if (result.IsLockedOut)
				{
					logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}

				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}