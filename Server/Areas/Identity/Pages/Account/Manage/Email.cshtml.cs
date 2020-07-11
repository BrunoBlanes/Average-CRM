using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Server.Areas.Identity.Pages.Account.Manage
{
	public partial class EmailModel : PageModel
	{
		private readonly IEmailSender emailSender;
		private readonly UserManager<ApplicationUser> userManager;

		public EmailModel(
			UserManager<ApplicationUser> userManager,
			IEmailSender emailSender)
		{
			this.emailSender = emailSender;
			this.userManager = userManager;
			NewEmail = string.Empty;
		}

		public string? Username { get; set; }

		public string? Email { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[TempData]
		public string? StatusMessage { get; set; }

		[Required]
		[EmailAddress]
		[BindProperty]
		[Display(Name = "New email")]
		public string NewEmail { get; set; }

		private async Task LoadAsync(ApplicationUser user)
		{
			NewEmail = Email = await userManager.GetEmailAsync(user);
			IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
		}

		public async Task<IActionResult> OnGetAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostChangeEmailAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			string? email = await userManager.GetEmailAsync(user);

			if (NewEmail != email)
			{
				string? userId = await userManager.GetUserIdAsync(user);
				string? code = await userManager.GenerateChangeEmailTokenAsync(user, NewEmail);
				string? callbackUrl = Url.Page(
					"/Account/ConfirmEmailChange",
					pageHandler: null,
					values: new { userId, email = NewEmail, code },
					protocol: Request.Scheme);
				await emailSender.SendEmailAsync(
					NewEmail,
					"Confirm your email",
					$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
				StatusMessage = "Confirmation link to change email sent. Please check your email.";
				return RedirectToPage();
			}

			StatusMessage = "Your email is unchanged.";
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostSendVerificationEmailAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			string? userId = await userManager.GetUserIdAsync(user);
			string? email = await userManager.GetEmailAsync(user);
			string? code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			string? callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { area = "Identity", userId, code },
				protocol: Request.Scheme);
			await emailSender.SendEmailAsync(
				email,
				"Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
			StatusMessage = "Verification email sent. Please check your email.";
			return RedirectToPage();
		}
	}
}