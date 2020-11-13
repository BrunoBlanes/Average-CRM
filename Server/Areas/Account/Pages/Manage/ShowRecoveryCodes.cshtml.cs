using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class ShowRecoveryCodesModel : PageModel
	{
		[TempData]
		public IList<string>? RecoveryCodes { get; private set; }

		[TempData]
		public string? StatusMessage { get; set; }

		public IActionResult OnGet()
		{
			return RecoveryCodes is null || RecoveryCodes.Any()
				? RedirectToPage("./TwoFactorAuthentication")
				: (IActionResult)Page();
		}
	}
}
