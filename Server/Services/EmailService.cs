using System.Threading.Tasks;

using CRM.Core.Models;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.AspNetCore.Identity.UI.Services;

using MimeKit;

namespace CRM.Server.Services
{
	public class EmailService : IEmailSender
	{
		private readonly ApplicationDbContext context;

		public EmailService(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task SendEmailAsync(string email, string subject, string body)
		{
			EmailSetting settings = (await context.Settings.FindAsync(1)).EmailSettings;
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(settings.Name, settings.Address));
			message.To.Add(MailboxAddress.Parse(email));
			message.Subject = subject;
			message.Body = new TextPart("plain") { Text = $@"{body}" };
			using var client = new SmtpClient();
			await client.ConnectAsync(settings.Server, settings.Port, SecureSocketOptions.StartTls);
			await client.AuthenticateAsync(settings.Login, settings.Password);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}
	}
}