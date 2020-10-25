using System.Threading.Tasks;

using CRM.Core.Models;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

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
			Settings settings = await context.Settings.Include(x => x.EmailSettings).FirstOrDefaultAsync();
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(settings.EmailSettings.Name, settings.EmailSettings.Address));
			message.To.Add(MailboxAddress.Parse(email));
			message.Subject = subject;
			message.Body = new TextPart("plain") { Text = $@"{body}" };
			using var client = new SmtpClient();
			await client.ConnectAsync(settings.EmailSettings.Server, settings.EmailSettings.Port, settings.EmailSettings.SecureSocketOptions);
			await client.AuthenticateAsync(settings.EmailSettings.Login, settings.EmailSettings.Password);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}
	}
}