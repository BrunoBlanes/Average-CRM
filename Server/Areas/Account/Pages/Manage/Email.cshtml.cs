using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public partial class EmailModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ISmtpService smtpService;

		public string Username { get; set; }

		public string Email { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[Required]
		[EmailAddress]
		[BindProperty]
		[Display(Name = "New email")]
		public string NewEmail { get; set; }

		public EmailModel(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ISmtpService smtpService)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.smtpService = smtpService;
			NewEmail = string.Empty;
		}

		private async Task LoadAsync(ApplicationUser user)
		{
			NewEmail = Email = await userManager.GetEmailAsync(user);
			IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
		}

		public async Task<IActionResult> OnGetAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);

			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostChangeEmailAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			if (ModelState.IsValid)
			{
				var email = await userManager.GetEmailAsync(user);
				
				if (NewEmail != email)
				{
					var userId = await userManager.GetUserIdAsync(user);

					// Generates a new email confirmation code and encodes it
					var code = await userManager.GenerateChangeEmailTokenAsync(user, NewEmail);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page("/Account/ConfirmEmailChange",
						null,
						new { userId, email = NewEmail, code },
						Request.Scheme);

					// Sends an email to the user with the confirmation code
					await smtpService.SendEmailAsync(NewEmail,
						"Confirm your email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
					StatusMessage = "Confirmation link to change email sent. Please check your email.";
					return RedirectToPage();
				}

				StatusMessage = "Your email is unchanged.";
				return RedirectToPage();
			}

			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostSendVerificationEmailAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			if (ModelState.IsValid)
			{
				var userId = await userManager.GetUserIdAsync(user);
				var email = await userManager.GetEmailAsync(user);

				// Generates a new email confirmation code and encodes it
				var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page("/Account/ConfirmEmail",
					null,
					new { area = "Identity", userId, code },
					Request.Scheme);

				// Sends an email to the user with the confirmation code
				await smtpService.SendEmailAsync(email,
					"Confirm your email",
					$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
				StatusMessage = "Verification email sent. Please check your email.";
				return RedirectToPage();
			}

			await LoadAsync(user);
			return Page();
		}
	}
}