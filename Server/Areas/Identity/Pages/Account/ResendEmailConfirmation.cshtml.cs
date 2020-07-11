using System.Text;
using System.Text.Encodings.Web;
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
	public class ResendEmailConfirmationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IEmailSender emailSender;

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
		{
			Email = string.Empty;
			this.userManager = userManager;
			this.emailSender = emailSender;
		}

		public static void OnGet()
		{

		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

            ApplicationUser? user = await userManager.FindByEmailAsync(Email);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
				return Page();
			}

			var userId = await userManager.GetUserIdAsync(user);
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { userId, code },
				protocol: Request.Scheme);
			await emailSender.SendEmailAsync(
				Email,
				"Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

			ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
			return Page();
		}
	}
}
