using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterConfirmationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IEmailSender sender;

		public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender)
		{
			this.userManager = userManager;
			this.sender = sender;
		}

		public string? Email { get; set; }

		public bool DisplayConfirmAccountLink { get; set; }

		public Uri? EmailConfirmationUrl { get; set; }

		public async Task<IActionResult> OnGetAsync(string email, Uri? returnUrl = null)
		{
			if (email == null)
			{
				return RedirectToPage("/Index");
			}

			ApplicationUser? user = await userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return NotFound($"Unable to load user with email '{email}'.");
			}

			Email = email;
			var userId = await userManager.GetUserIdAsync(user);
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			EmailConfirmationUrl = new Uri(Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { area = "Identity", userId, code, returnUrl },
				protocol: Request.Scheme));
			await sender.SendEmailAsync(email, "Confirme sua conta.", EmailConfirmationUrl.AbsoluteUri);
			return Page();
		}
	}
}