using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages
{
	[AllowAnonymous]
	public class LockoutModel : PageModel
	{
		public void OnGet()
		{

		}
	}
}
