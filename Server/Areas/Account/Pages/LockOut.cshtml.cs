using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages
{
	[AllowAnonymous]
	public class LockOutModel : PageModel
	{
		public static void OnGet()
		{

		}
	}
}