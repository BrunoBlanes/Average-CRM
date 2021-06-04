using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Server.Areas.Account.Pages
{
	[AllowAnonymous]
	public class ResendEmailConfirmationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ISmtpService smtpService;

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, ISmtpService smtpService)
		{
			Email = string.Empty;
			this.userManager = userManager;
			this.smtpService = smtpService;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				ApplicationUser? user = await userManager.FindByEmailAsync(Email);

				if (user is null)
				{
					ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
					return Page();
				}

				// Generates a new confirmation code and encodes it
				var userId = await userManager.GetUserIdAsync(user);
				var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page("/ConfirmEmail", null, new { userId, code, }, Request.Scheme);

				// Sends an email to the user with the account confirmation code
				await smtpService.SendAccountConfirmationEmailAsync(callbackUrl, user);
				ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
				return Page();
			}

			return Page();
		}
	}
}