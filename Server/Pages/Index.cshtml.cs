using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Pages
{
	public class IndexModel : PageModel
	{
		public IActionResult OnGet()
		{
			if (Program.FirstRun)
			{
				return LocalRedirect("~/setup");
			}

			return LocalRedirect("~/home");
		}
	}
}