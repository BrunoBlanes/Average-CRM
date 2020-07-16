using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

using CRM.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ResetPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		[Required]
		[BindProperty]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		public string Password { get; set; }

		[BindProperty]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string? Code { get; set; }

		public ResetPasswordModel(UserManager<ApplicationUser> userManager)
		{
			Email = string.Empty;
			Password = string.Empty;
			ConfirmPassword = string.Empty;
			this.userManager = userManager;
		}

		public IActionResult OnGet(string? code = null)
		{
			if (code is null) return BadRequest("A code must be supplied for password reset.");

			else
			{
				Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				ApplicationUser? user = await userManager.FindByEmailAsync(Email);

				// Don't reveal that the user does not exist
				if (user is null) return RedirectToPage("./ResetPasswordConfirmation");

				// Resets the user's password
				IdentityResult? result = await userManager.ResetPasswordAsync(user, Code, Password);
				if (result.Succeeded) return RedirectToPage("./ResetPasswordConfirmation");

				// Log errors
				foreach (IdentityError? error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
				return Page();
			}

			return Page();
		}
	}
}