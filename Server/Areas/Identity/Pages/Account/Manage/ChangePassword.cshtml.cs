using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using CRM.Shared.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
namespace CRM.Server.Areas.Identity.Pages.Account.Manage
{
	public class ChangePasswordModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<ChangePasswordModel> logger;

		[Required]
		[BindProperty]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

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

		[TempData]
		public string? StatusMessage { get; set; }

		public ChangePasswordModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ILogger<ChangePasswordModel> logger)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
			OldPassword = string.Empty;
			NewPassword = string.Empty;
			ConfirmPassword = string.Empty;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user is null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			if (await userManager.HasPasswordAsync(user) is not true) return RedirectToPage("./SetPassword");
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				ApplicationUser? user = await userManager.GetUserAsync(User);
				if (user is null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
				IdentityResult? changePasswordResult = await userManager.ChangePasswordAsync(user, OldPassword, NewPassword);

				if (changePasswordResult.Succeeded is not true)
				{
					foreach (IdentityError? error in changePasswordResult.Errors)
						ModelState.AddModelError(string.Empty, error.Description);
					return Page();
				}

				await signInManager.RefreshSignInAsync(user);
				logger.LogInformation("User changed their password successfully.");
				StatusMessage = "Your password has been changed.";
				return RedirectToPage();
			}

			return Page();
		}
	}
}