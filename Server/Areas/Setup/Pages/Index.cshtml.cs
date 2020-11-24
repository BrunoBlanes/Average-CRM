using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;
using CRM.Server.Interfaces;
using CRM.Server.Models;

using MailKit.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Setup.Pages
{
	[BindProperties]
	public class IndexModel : PageModel
	{

		private readonly IWritableOptions<Application> writableApplication;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly IWritableOptions<Smtp> writableSmtp;
		private readonly ILogger<IndexModel> logger;
		private readonly ISmtpService smtpService;
		private string? password;

		public Smtp Smtp { get; set; }
		public bool Start { get; set; } = true;
		public bool IsSmtpConfigured { get; set; }
		public bool IsIdentityConfigured { get; set; }

		public IndexModel(
			IWritableOptions<Application> writableApplication,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IWritableOptions<Smtp> writableSmtp,
			ILogger<IndexModel> logger,
			ISmtpService smtpService)
		{
			Smtp = new();
			this.logger = logger;
			this.smtpService = smtpService;
			this.userManager = userManager;
			this.roleManager = roleManager;
			this.writableSmtp = writableSmtp;
			this.writableApplication = writableApplication;
		}

		public IActionResult OnGet()
		{
			return Program.FirstRun ? Page() : Unauthorized();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (Start)
			{
				Start = false;
			}

			else if (IsSmtpConfigured is false)
			{
				await ConfigureSmtpAsync();
			}

			else if (IsIdentityConfigured is false)
			{
				await ConfigureIdentityAsync();
			}

			else
			{
				// TODO: Add a timer
				Program.Shutdown();
			}

			return Page();
		}

		public async Task ConfigureSmtpAsync()
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
				IsSmtpConfigured = true;
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
		}

		public async Task ConfigureIdentityAsync()
		{

			await CreateRootUserAsync();
		}

		public async Task CreateRootUserAsync()
		{
			var user = new ApplicationUser { UserName = "admin" };
			await userManager.CreateAsync(user, "root");
			logger.LogInformation("Root user account created successfully.");
			await roleManager.CreateAsync(new IdentityRole(Roles.Administrator));
			await userManager.AddToRoleAsync(user, Roles.Administrator);
		}
	}
}