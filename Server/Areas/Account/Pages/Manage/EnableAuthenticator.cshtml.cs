using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class EnableAuthenticatorModel : PageModel
	{
		private const string authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<EnableAuthenticatorModel> logger;
		private readonly UrlEncoder urlEncoder;

		[TempData]
		public IEnumerable<string>? RecoveryCodes { get; private set; }

		[TempData]
		public string? StatusMessage { get; set; }
		public string? SharedKey { get; set; }
		public string? AuthenticatorUri { get; set; }

		[Required]
		[BindProperty]
		[DataType(DataType.Text)]
		[Display(Name = "Verification Code")]
		[StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		public string Code { get; set; }

		public EnableAuthenticatorModel(
			UserManager<ApplicationUser> userManager,
			ILogger<EnableAuthenticatorModel> logger,
			UrlEncoder urlEncoder)
		{
			this.logger = logger;
			this.urlEncoder = urlEncoder;
			this.userManager = userManager;
			Code = string.Empty;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			await LoadSharedKeyAndQrCodeUriAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			if (ModelState.IsValid)
			{
				// Strip spaces and hypens
				var verificationCode = Code
					.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
					.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
				var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
					user,
					userManager.Options.Tokens.AuthenticatorTokenProvider,
					verificationCode);

				if (is2faTokenValid is not true)
				{
					ModelState.AddModelError("Input.Code", "Verification code is invalid.");
					await LoadSharedKeyAndQrCodeUriAsync(user);
					return Page();
				}

				await userManager.SetTwoFactorEnabledAsync(user, true);
				var userId = await userManager.GetUserIdAsync(user);
				logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);
				StatusMessage = "Your authenticator app has been verified.";

				if (await userManager.CountRecoveryCodesAsync(user) == 0)
				{
					RecoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
					return RedirectToPage("./ShowRecoveryCodes");
				}

				else
				{
					return RedirectToPage("./TwoFactorAuthentication");
				}
			}

			await LoadSharedKeyAndQrCodeUriAsync(user);
			return Page();
		}

		private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
		{
			// Load the authenticator key & QR code URI to display on the form
			var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);

			if (string.IsNullOrEmpty(unformattedKey))
			{
				await userManager.ResetAuthenticatorKeyAsync(user);
				unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
			}

			SharedKey = FormatKey(unformattedKey);
			var email = await userManager.GetEmailAsync(user);
			AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
		}

		private static string FormatKey(string unformattedKey)
		{
			var result = new StringBuilder();
			var currentPosition = 0;

			while (currentPosition + 4 < unformattedKey.Length)
			{
				result.Append(unformattedKey.Substring(currentPosition, 4)).Append(' ');
				currentPosition += 4;
			}

			if (currentPosition < unformattedKey.Length)
			{
				result.Append(unformattedKey[currentPosition..]);
			}

			return result.ToString().ToUpperInvariant();
		}

		private string GenerateQrCodeUri(string email, string unformattedKey)
		{
			return string.Format(CultureInfo.InvariantCulture,
				authenticatorUriFormat,
				urlEncoder.Encode("CRM.Server"),
				urlEncoder.Encode(email),
				unformattedKey);
		}
	}
}