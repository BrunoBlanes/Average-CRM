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
		public ApplicationUser ApplicationUser { get; set; }

		[Required]
		[EmailAddress]
		[ProtectedPersonalData]
		[Display(Prompt = "Email")]
		public string Email
		{
			get => ApplicationUser.Email;
			set => ApplicationUser.Email = value;
		}

		[Required]
		[CpfValidation]
		[Display(Prompt = "CPF")]
		[StringLength(14, ErrorMessage = "The {0} must be exactly 11 characters long.", MinimumLength = 14)]
		public string CPF
		{
			get => ApplicationUser.CPF;
			set => ApplicationUser.CPF = value;
		}

		[BindProperty]
		[DataType(DataType.Password)]
		[Display(Prompt = "Password")]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		public string Password { get; set; }

		[BindProperty]
		[DataType(DataType.Password)]
		[Display(Prompt = "Confirm password")]
		[Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public RegisterModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<RegisterModel> logger, ISmtpService smtpService)
		{
			this.logger = logger;
			Password = string.Empty;
			ConfirmPassword = string.Empty;
			this.smtpService = smtpService;
			this.userManager = userManager;
			this.signInManager = signInManager;
			ApplicationUser = new ApplicationUser();
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
				ApplicationUser.UserName = ApplicationUser.Email;
				IdentityResult result = await userManager.CreateAsync(ApplicationUser, Password);

				if (result.Succeeded)
				{
					// Generates the user account confirmation code
					logger.LogInformation($"User {ApplicationUser.Email} created a new account with password.");
					string code = await userManager.GenerateEmailConfirmationTokenAsync(ApplicationUser);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					string callbackUrl = Url.Page("/Account/ConfirmEmail", null, new
					{
						area = "Identity",
						userId = ApplicationUser.Id,
						code,
						returnUrl
					}, Request.Scheme);

					// Sends a confirmation email to the user
					await smtpService.SendAccountConfirmationEmailAsync(callbackUrl, ApplicationUser);

					// Redirect to the account confirmation page
					return RedirectToPage("RegisterConfirmation", new { email = ApplicationUser.Email, returnUrl });
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
