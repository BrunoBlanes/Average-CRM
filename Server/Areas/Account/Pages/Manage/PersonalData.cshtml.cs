using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class PersonalDataModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;

		public PersonalDataModel(
			UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<IActionResult> OnGet()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			return user is null
				? NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.")
				: (IActionResult)Page();
		}
	}
}