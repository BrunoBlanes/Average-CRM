using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using CRM.Server.Models;

namespace CRM.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
	public class ResetPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;

		public ResetPasswordModel(UserManager<ApplicationUser> userManager)
		{
			Input = new InputModel();
			this.userManager = userManager;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public IActionResult OnGet(string? code = null)
		{
			if (code == null)
			{
				return BadRequest("A code must be supplied for password reset.");
			}

			else
			{
				Input = new InputModel
				{
					Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
				};

				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

            ApplicationUser? user = await userManager.FindByEmailAsync(Input.Email);

			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToPage("./ResetPasswordConfirmation");
			}

            IdentityResult? result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

			if (result.Succeeded)
			{
				return RedirectToPage("./ResetPasswordConfirmation");
			}

			foreach (IdentityError? error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return Page();
		}
	}
}
