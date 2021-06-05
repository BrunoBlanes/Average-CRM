using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class PersonalDataModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<PersonalDataModel> logger;

		public PersonalDataModel(
			UserManager<ApplicationUser> userManager,
			ILogger<PersonalDataModel> logger)
		{
			this.userManager = userManager;
			this.logger = logger;
		}

		public async Task<IActionResult> OnGet()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			return Page();
		}
	}
}