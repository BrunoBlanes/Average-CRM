using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using CRM.Core.Models;

using MailKit.Security;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Pages
{
	[BindProperties]
	public class SetupModel : PageModel
	{
		[Required]
		public int Port { get; set; }
		public string? Name { get; set; }

		[Required]
		[EmailAddress]
		public string Login { get; set; }

		[Required]
		public string Server { get; set; }

		[EmailAddress]
		public string? Address { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		public SecureSocket SecureSocket { get; set; }

		public SetupModel()
		{
			Login = string.Empty;
			Server = string.Empty;
			Password = string.Empty;
			SecureSocket = SecureSocket.Auto;
		}

		public async Task OnGetAsync()
		{
			using var client = new HttpClient { BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}/api/") };
			HttpResponseMessage? response = await client.GetAsync("Settings");

			if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
			{
				Settings settings = await response.Content.ReadFromJsonAsync<Settings>() ?? new Settings();
				Port = settings.EmailSettings.Port;
				Name = settings.EmailSettings.Name;
				Login = settings.EmailSettings.Login;
				Server = settings.EmailSettings.Server;
				Address = settings.EmailSettings.Address;
				Password = settings.EmailSettings.Password;
				SecureSocket = (SecureSocket)settings.EmailSettings.SecureSocketOptions;
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var settings = new Settings
				{
					FirstRun = false,
					EmailSettings = new EmailSettings
					{
						Port = Port,
						Login = Login,
						Server = Server,
						Password = Password,
						Address = Address ?? Login,
						Name = Name ?? "CRM Server",
						SecureSocketOptions = (SecureSocketOptions)SecureSocket
					}
				};

				using var client = new HttpClient { BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}/api/") };
				HttpResponseMessage? response = await client.PostAsJsonAsync("Settings", settings);

				if (response.IsSuccessStatusCode)
				{
					return LocalRedirect("~/");
				}
			}

			return Page();
		}
	}

	public enum SecureSocket
	{
		[Display(Name = "None")]
		None,
		[Display(Name = "Auto")]
		Auto,
		[Display(Name = "SSL/TLS")]
		SslOnConnect,
		[Display(Name = "STARTTLS")]
		StartTls
	}
}