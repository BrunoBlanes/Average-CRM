using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using CRM.Server.Models;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<RegisterModel> logger;
		private readonly IEmailSender emailSender;

		[BindProperty]
		public InputModel Input { get; set; }
		public string? ReturnUrl { get; set; }
		public IList<AuthenticationScheme>? ExternalLogins { get; private set; }

		public RegisterModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender)
		{
			this.logger = logger;
			Input = new InputModel();
			this.emailSender = emailSender;
			this.userManager = userManager;
			this.signInManager = signInManager;
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
				var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, CPF = Input.CPF };
				IdentityResult? result = await userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					// Generates the user account confirmation code
					logger.LogInformation($"User {Input.Email} created a new account with password.");
					var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = user.Id, code, returnUrl },
						protocol: Request.Scheme);

					// TODO: Generate proper email body
					// Sends a confirmation email to the user
					await emailSender.SendEmailAsync(Input.Email,
						"Confirm your email",
						$"Please confirm your account by <a href='{ HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					// Redirect to the account confirmation page
					return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
				}

				foreach (IdentityError? error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
