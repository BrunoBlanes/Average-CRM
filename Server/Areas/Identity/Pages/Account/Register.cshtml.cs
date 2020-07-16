using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using CRM.Shared.Attributes;
using CRM.Shared.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
		private readonly IEmailSender emailSender;

		public string? ReturnUrl { get; set; }
		public IList<AuthenticationScheme>? ExternalLogins { get; private set; }

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		[Required]
		[CpfValidation]
		[BindProperty]
		[StringLength(14, ErrorMessage = "The {0} must be exactly 11 characters long.", MinimumLength = 14)]
		public string CPF { get; set; }

		[Required]
		[BindProperty]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		public string Password { get; set; }

		[BindProperty]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public RegisterModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender)
		{
			this.logger = logger;
			this.emailSender = emailSender;
			this.userManager = userManager;
			this.signInManager = signInManager;
			CPF = string.Empty;
			Email = string.Empty;
			Password = string.Empty;
			ConfirmPassword = string.Empty;
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
				var user = new ApplicationUser { UserName = Email, Email = Email, CPF = CPF };
				IdentityResult? result = await userManager.CreateAsync(user, Password);

				if (result.Succeeded)
				{
					// Generates the user account confirmation code
					logger.LogInformation($"User {Email} created a new account with password.");
					var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = user.Id, code, returnUrl },
						protocol: Request.Scheme);

					// TODO: Generate proper email body
					// TODO: Move code generation and email sender to two separate functions
					// Sends a confirmation email to the user
					await emailSender.SendEmailAsync(Email,
						"Confirm your email",
						$"Please confirm your account by <a href='{ HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					// Redirect to the account confirmation page
					return RedirectToPage("RegisterConfirmation", new { email = Email, returnUrl });
				}

				foreach (IdentityError? error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
