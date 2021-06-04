using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
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
	public class ForgotPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ISmtpService smtpService;

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		public ForgotPasswordModel(UserManager<ApplicationUser> userManager, ISmtpService smtpService)
		{
			this.userManager = userManager;
			this.smtpService = smtpService;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				ApplicationUser? user = await userManager.FindByEmailAsync(Email);

				// Don't reveal that the user does not exist or is not confirmed
				if (user is null || !await userManager.IsEmailConfirmedAsync(user))
				{
					return RedirectToPage("./ForgotPasswordConfirmation");
				}

				// For more information on how to enable account confirmation and password reset please 
				// visit https://go.microsoft.com/fwlink/?LinkID=532713
				// Generates and encodes the confirmation code
				var code = await userManager.GeneratePasswordResetTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page("/Account/ResetPassword",
					null,
					new { area = "Account", code },
					Request.Scheme);

				// Send the code to the user's email address
				await smtpService.SendEmailAsync(Email,
					"Reset Password",
					$"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
				return RedirectToPage("./ForgotPasswordConfirmation");
			}

			return Page();
		}
	}
}
