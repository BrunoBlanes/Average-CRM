using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class LogoutModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ILogger<LogoutModel> logger;

		public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
		{
			this.signInManager = signInManager;
			this.logger = logger;
		}

		public static void OnGet()
		{

		}

		public async Task<IActionResult> OnPost(string? returnUrl = null)
		{
			await signInManager.SignOutAsync();
			logger.LogInformation("User logged out.");
			return returnUrl is not null
				? LocalRedirect(returnUrl)
				: (IActionResult)RedirectToPage();
		}
	}
}