using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class SetPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		[TempData]
		public string? StatusMessage { get; set; }

		[Required]
		[BindProperty]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		public string NewPassword { get; set; }

		[BindProperty]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public SetPasswordModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			ConfirmPassword = string.Empty;
			NewPassword = string.Empty;
		}
		public async Task<IActionResult> OnGetAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			var hasPassword = await userManager.HasPasswordAsync(user);
			return hasPassword ? RedirectToPage("./ChangePassword") : (IActionResult)Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				ApplicationUser? user = await userManager.GetUserAsync(User);
				
				if (user is null)
				{
					return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
				}

				IdentityResult? addPasswordResult = await userManager.AddPasswordAsync(user, NewPassword);

				if (addPasswordResult.Succeeded is not true)
				{
					foreach (IdentityError? error in addPasswordResult.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}

					return Page();
				}

				await signInManager.RefreshSignInAsync(user);
				StatusMessage = "Your password has been set.";
				return RedirectToPage();
			}

			return Page();
		}
	}
}
