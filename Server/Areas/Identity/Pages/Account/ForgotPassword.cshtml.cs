using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ForgotPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IEmailSender emailSender;

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
		{
			this.userManager = userManager;
			this.emailSender = emailSender;
			Email = string.Empty;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				ApplicationUser? user = await userManager.FindByEmailAsync(Email);

				// Don't reveal that the user does not exist or is not confirmed
				if (user is null || !await userManager.IsEmailConfirmedAsync(user))
					return RedirectToPage("./ForgotPasswordConfirmation");

				// Generates and encodes the confirmation code
				var code = await userManager.GeneratePasswordResetTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page(
					"/Account/ResetPassword",
					pageHandler: null,
					values: new { area = "Identity", code },
					protocol: Request.Scheme);

				// Send the code to the user's email address
				await emailSender.SendEmailAsync(
					Email,
					"Reset Password",
					$"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
				return RedirectToPage("./ForgotPasswordConfirmation");
			}

			return Page();
		}
	}
}