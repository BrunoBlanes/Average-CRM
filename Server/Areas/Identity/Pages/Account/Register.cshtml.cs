using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRM.Core.Attributes;
using CRM.Core.Models;
using CRM.Server.Interfaces;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<RegisterModel> logger;
		private readonly ISmtpService smtpService;

		public string? ReturnUrl { get; set; }
		public ICollection<AuthenticationScheme>? ExternalLogins { get; private set; }

		[BindProperty]
		public ApplicationUser AppUser { get; set; }

		public RegisterModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<RegisterModel> logger, ISmtpService smtpService)
		{
			this.logger = logger;
			this.smtpService = smtpService;
			this.userManager = userManager;
			this.signInManager = signInManager;
			AppUser = new ApplicationUser();
		}

		// Page load
		public async Task OnGetAsync(string? returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		// Submit new user button click
		public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				// Creates the new user and it's identity
				AppUser.UserName = AppUser.Email;
				IdentityResult result = await userManager.CreateAsync(AppUser, AppUser.Password);

				if (result.Succeeded)
				{
					// Generates the user account confirmation code
					logger.LogInformation($"User {AppUser.Email} created a new account with password.");
					string code = await userManager.GenerateEmailConfirmationTokenAsync(AppUser);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					string callbackUrl = Url.Page("/Account/ConfirmEmail", null, new
					{
						area = "Identity",
						userId = AppUser.Id,
						code,
						returnUrl
					}, Request.Scheme);

					// Sends a confirmation email to the user
					await smtpService.SendAccountConfirmationEmailAsync(callbackUrl, AppUser);

					// Redirect to the account confirmation page
					return RedirectToPage("RegisterConfirmation", new { email = AppUser.Email, returnUrl });
				}

				foreach (IdentityError? error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
