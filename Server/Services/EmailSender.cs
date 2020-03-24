using Microsoft.Extensions.Options;

using SendGrid;
using SendGrid.Helpers.Mail;

using System;
using System.Threading.Tasks;

namespace CRM.Server.Services
{
	public class EmailSender
	{
		private readonly AuthMessageSenderOptions options;

		public EmailSender(IOptions<AuthMessageSenderOptions> options)
		{
			if (options is null) throw new ArgumentNullException(nameof(options));
			this.options = options.Value;
		}

		public Task SendEmailAsync(string email, string subject, string message)
		{
			return ExecuteAsync(options.SendGridKey, subject, message, email);
		}

		private Task ExecuteAsync(string apiKey, string subject, string message, string email)
		{
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("crm.server@brunoblanes.eng.br", options.SendGridUser),
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
