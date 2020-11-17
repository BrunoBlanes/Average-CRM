using System;
using System.Threading.Tasks;

using CRM.Server.Data;
using CRM.Server.Interfaces;
using CRM.Server.Models;

using MailKit.Security;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Setup.Pages
{
	public class SmtpModel : PageModel
	{
		private readonly IWritableOptions<Application> writableApplication;
		private readonly IWritableOptions<Smtp> writableSmtp;
		private readonly ISmtpService smtpService;
		private readonly Application application;
		private string password;

		[BindProperty]
		public Smtp Smtp { get; set; }

		public SmtpModel(
			IWritableOptions<Application> writableApplication,
			IWritableOptions<Smtp> writableSmtp,
			ISmtpService smtpService)
		{
			this.smtpService = smtpService;
			this.writableSmtp = writableSmtp;
			this.writableApplication = writableApplication;
			application = writableApplication.Value ?? new();
			Smtp = writableSmtp.Value ??  new();
			password = Smtp.Password;
			Smtp.Password = string.Empty;
		}

		public IActionResult OnGet()
		{
			if (application.FirstRun)
			{
				return Page();
			}

			return new NoContentResult();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (Smtp.Address is null)
			{
				Smtp.Address = Smtp.Login;
			}

			if (Smtp.Name is null)
			{
				Smtp.Name = "CRM Server";
			}

			password = Smtp.Password;
			Smtp.Password = string.Empty;
			using var passwordHasher = new PasswordHasher();
			Smtp.Password = passwordHasher.Encrypt(password, Smtp);

			try
			{
				await smtpService.ConfigureAsync(Smtp);
				await writableSmtp.UpdateAsync(Smtp);

				// Disables setup
				application.FirstRun = false;
				await writableApplication.UpdateAsync(application);
				return LocalRedirect("~/");
			}

			catch (AuthenticationException)
			{
				return Page();
			}

			catch (NotSupportedException)
			{
				return Page();
			}
		}
	}
}