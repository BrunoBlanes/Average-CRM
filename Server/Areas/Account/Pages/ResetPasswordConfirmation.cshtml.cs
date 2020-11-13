using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages
{
	[AllowAnonymous]
	public class ResetPasswordConfirmationModel : PageModel
	{
		public static void OnGet()
		{

		}
	}
}