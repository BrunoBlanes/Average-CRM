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

		public SetPasswordModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		public class InputModel
		{
			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "New password")]
			public string NewPassword { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm new password")]
			[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			var hasPassword = await userManager.HasPasswordAsync(user);

			if (hasPassword)
			{
				return RedirectToPage("./ChangePassword");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			IdentityResult? addPasswordResult = await userManager.AddPasswordAsync(user, Input.NewPassword);
			if (!addPasswordResult.Succeeded)
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
	}
}
