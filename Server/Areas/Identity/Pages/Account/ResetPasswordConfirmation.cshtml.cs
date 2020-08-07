using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ResetPasswordConfirmationModel : PageModel
	{
		public static void OnGet()
		{

		}
	}
}