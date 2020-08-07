using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ForgotPasswordConfirmation : PageModel
	{
		public static void OnGet()
		{

		}
	}
}
