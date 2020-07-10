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

		[BindProperty]
		public InputModel Input { get; set; }

		public Uri? ReturnUrl { get; set; }

		public IList<AuthenticationScheme>? ExternalLogins { get; private set; }

		public async Task OnGetAsync(Uri? returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(Uri? returnUrl = null)
		{
			returnUrl ??= new Uri(Url.Content("~/"));
			ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid && Input.CPF is not null)
			{
				var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, CPF = Input.CPF };
				IdentityResult? result = await userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					logger.LogInformation($"User {Input.Email} created a new account with password.");
					var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = user.Id, code, returnUrl },
						protocol: Request.Scheme);

					await emailSender.SendEmailAsync(Input.Email, "Confirm your email", $"Please confirm your account by <a href='{ HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					if (userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
					}

					else
					{
						await signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl.AbsoluteUri);
					}
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
