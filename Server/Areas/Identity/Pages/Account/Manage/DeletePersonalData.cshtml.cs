using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Identity.Pages.Account.Manage
{
	public class DeletePersonalDataModel : PageModel
	{
		private readonly ILogger<DeletePersonalDataModel> logger;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		public bool RequirePassword { get; set; }

		[Required]
		[BindProperty]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public DeletePersonalDataModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ILogger<DeletePersonalDataModel> logger)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
			Password = string.Empty;
		}

		public async Task<IActionResult> OnGet()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user is null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			RequirePassword = await userManager.HasPasswordAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user is null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			RequirePassword = await userManager.HasPasswordAsync(user);

			if (RequirePassword)
			{
				if (await userManager.CheckPasswordAsync(user, Password) is false)
				{
					ModelState.AddModelError(string.Empty, "Incorrect password.");
					return Page();
				}
			}

			IdentityResult? result = await userManager.DeleteAsync(user);
			var userId = await userManager.GetUserIdAsync(user);
			if (result.Succeeded is not true)
				throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
			await signInManager.SignOutAsync();
			logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);
			return Redirect("~/");
		}
	}
}