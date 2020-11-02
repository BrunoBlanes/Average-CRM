using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;
using CRM.Server.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Pages
{
	public class SetupModel : PageModel
	{
		private readonly IWritableOptions<SmtpOptions> options;
		private string? password;

		[BindProperty]
		public SmtpOptions SmtpOptions { get; set; }

		public SetupModel(IWritableOptions<SmtpOptions> options)
		{
			this.options = options;
			SmtpOptions = options.Value ?? new();
			password = SmtpOptions.Password;
			SmtpOptions.Password = string.Empty;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (SmtpOptions.Address is null)
			{
				SmtpOptions.Address = SmtpOptions.Login;
			}

			if (SmtpOptions.Name is null)
			{
				SmtpOptions.Name = "CRM Server";
			}

			password = SmtpOptions.Password;
			SmtpOptions.Password = string.Empty;
			using var passwordHasher = new PasswordHasher();
			SmtpOptions.Password = passwordHasher.Encrypt(password, SmtpOptions);
			await options.UpdateAsync(SmtpOptions);
			return LocalRedirect("~/");
		}
	}
}