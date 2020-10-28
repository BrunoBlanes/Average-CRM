using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity.UI.Services;

using MimeKit;

namespace CRM.Server.Services
{
	public class EmailService : IEmailSender
	{
		private readonly IHttpClientFactory clientFactory;

		public EmailService(IHttpClientFactory clientFactory)
		{
			this.clientFactory = clientFactory;
		}

		public async Task SendEmailAsync(string email, string subject, string body)
		{
			// Gets the settings from the database
			HttpClient httpClient = clientFactory.CreateClient("ServerAPI");
			HttpResponseMessage? response = await httpClient.GetAsync("EmailSettings");

			if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
			{
				EmailSettings settings = await response.Content.ReadFromJsonAsync<EmailSettings>() ?? new();
				// Decrypts the password
				string password = settings.Password;
				using var passwordHasher = new PasswordHasher();
				password = passwordHasher.Decrypt(password, settings);

				// Create the email message
				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(settings.Name, settings.Address));
				message.To.Add(MailboxAddress.Parse(email));
				message.Subject = subject;
				message.Body = new TextPart("plain") { Text = $@"{body}" };

				// Connect to the server and send the message
				using var smtpClient = new SmtpClient();
				await smtpClient.ConnectAsync(settings.Server, settings.Port, settings.SecureSocketOptions);
				await smtpClient.AuthenticateAsync(settings.Login, password);
				await smtpClient.SendAsync(message);
				await smtpClient.DisconnectAsync(true);
			}
		}
	}
}