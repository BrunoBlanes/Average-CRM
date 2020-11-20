using System;
using System.Net.Sockets;
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
			Smtp = writableSmtp.Value ??  new();
			password = Smtp.Password;
			Smtp.Password = string.Empty;
		}

		public IActionResult OnGet()
		{
			return Program.FirstRun ? Page() : Unauthorized();
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
				Application application = writableApplication.Value ?? new();
				application.FirstRun = false;
				await writableApplication.UpdateAsync(application);
				return LocalRedirect("~/");
			}

			catch (SocketException)
			{
				Smtp.Password = password;
				ModelState.AddModelError("Smtp.Server", " ");
				ModelState.AddModelError("Smtp", "Could not connect to the specified SMTP server.");
			}

			catch (AuthenticationException)
			{
				Smtp.Password = password;
				ModelState.AddModelError("Smtp.Login", " ");
				ModelState.AddModelError("Smtp.Password", " ");
				ModelState.AddModelError("Smtp", "Failed to authenticate with the SMTP server.");
			}

			catch (NotSupportedException)
			{
				Smtp.Password = password;
				ModelState.AddModelError("Smtp.SecureSocket", " ");
				ModelState.AddModelError("Smtp", "The specified SMTP server does not support selected Secure Socket option.");
			}

			catch (SslHandshakeException)
			{
				Smtp.Password = password;
				ModelState.AddModelError("Smtp.SecureSocket", " ");
				ModelState.AddModelError("Smtp", "An error occurred while attempting to establish an SSL or TLS connection.");
			}

			catch (Exception)
			{
				Smtp.Password = password;
				ModelState.AddModelError("Smtp", $"An error occurred while attempting to establish a connection.");
			}

			return Page();
		}
	}
}