using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CRM.Server.Services
{
	public class EmailSender : IEmailSender
	{
		public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

		public EmailSender(IOptions<AuthMessageSenderOptions> options)
		{
			if (options is null) throw new ArgumentNullException(nameof(options));
			Options = options.Value;
		}

		public Task SendEmailAsync(string email, string subject, string message)
		{
			return ExecuteAsync(Options.SendGridKey, subject, message, email);
		}

		private static Task ExecuteAsync(string apiKey, string subject, string message, string email)
		{
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("server@crm.brunoblanes.eng.br", "Average CRM Server"),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};
			msg.AddTo(new EmailAddress(email));

			// Disable click tracking.
			// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
			msg.SetClickTracking(false, false);
			return client.SendEmailAsync(msg);
		}
	}

	public class AuthMessageSenderOptions
	{
		public string SendGridUser { get; set; }
		public string SendGridKey { get; set; }

		public AuthMessageSenderOptions()
		{
			SendGridKey = string.Empty;
			SendGridUser = string.Empty;
		}
	}
}