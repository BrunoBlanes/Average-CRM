using System.Threading;
using System.Threading.Tasks;

using MimeKit;

namespace CRM.Server.Interfaces
{
	public interface ISmtpService
	{
		/// <summary>
		/// Attempts to send a message using the <see cref="MailKit.Net.Smtp.SmtpClient"/>.
		/// </summary>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <param name="message">The <see cref="MimeMessage"/> to be sent.</param>
		/// <returns>A <see cref="Task"/> that represents the message being sent.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown when no recipients have been specified.</exception>
		/// <exception cref="MailKit.Security.AuthenticationException">Thrown when authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="System.NotSupportedException">Thrown when options was set to <see cref="MailKit.Security.SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.</exception>
		Task SendEmailAsync(MimeMessage message, CancellationToken token = default);
	}
}