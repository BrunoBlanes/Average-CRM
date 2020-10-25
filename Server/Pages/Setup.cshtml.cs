using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using CRM.Core.Models;

using MailKit.Security;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Pages
{
	public class SetupModel : PageModel
	{
		[Required]
		[BindProperty]
		public int Port { get; set; }

		[BindProperty]
		public string? Name { get; set; }

		[Required]
		[BindProperty]
		[EmailAddress]
		public string Login { get; set; }

		[Required]
		[BindProperty]
		public string Server { get; set; }

		[BindProperty]
		[EmailAddress]
		public string? Address { get; set; }

		[Required]
		[BindProperty]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[BindProperty]
		public SecureSocketOptions SecureSocketOptions { get; set; }

		public SetupModel()
		{
			Login = string.Empty;
			Server = string.Empty;
			Password = string.Empty;
			SecureSocketOptions = SecureSocketOptions.StartTls;
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
						SecureSocketOptions = SecureSocketOptions
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
}