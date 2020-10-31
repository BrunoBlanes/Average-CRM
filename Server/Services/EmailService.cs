using System;
using System.Threading;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;

using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MimeKit;

namespace CRM.Server.Services
{
	public class EmailService : IEmailSender, IDisposable
	{
		private readonly ApplicationDbContext context;
		private readonly SmtpClient client;
		private readonly ILogger logger;
		private bool disposedValue;

		public EmailSettings? Settings { get; set; }
		public MailboxAddress? MailboxAddress { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="EmailService"/>.
		/// </summary>
		/// <param name="context">The <see cref="ApplicationDbContext"/>.</param>
		/// <param name="logger">The <see cref="ILogger"/>.</param>
		public EmailService(ApplicationDbContext context, ILogger<EmailService> logger)
		{
			this.logger = logger;
			this.context = context;
			client = new SmtpClient();
			Task.Run(() => TryConnectAsync());
		}

		/// <summary>
		/// Attempts to open a connection to the SMTP server using a <see cref="SmtpClient"/> instance.
		/// </summary>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <returns><c>true</c> if connection was stabilished; otherwise <c>false</c>.</returns>
		/// <exception cref="NotSupportedException">Thrown when options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP server does not support the STARTTLS extension.</exception>
		/// <exception cref="InvalidOperationException">Thrown when the email configuration could not be found.</exception>
		/// <exception cref="OperationCanceledException">Thrown only when the <paramref name="token"/> is provided and <see cref="CancellationTokenSource.Cancel"/> is requested.</exception>
		public async Task<bool> TryConnectAsync(CancellationToken token = default)
		{
			if (client.IsConnected)
			{
				return true;
			}

			if (Settings is null)
			{
				if (await context.EmailSettings.SingleOrDefaultAsync(token) is EmailSettings settings)
				{
					Settings = settings;
					using var passwordHasher = new PasswordHasher();
					Settings.Password = passwordHasher.Decrypt(settings.Password, settings);
					MailboxAddress = new MailboxAddress(Settings.Name, Settings.Address);
				}

				else
				{
					throw new InvalidOperationException("Could not retrieve email configuration from the database.");
				}
			}

			int attempts = 1;
			while (attempts <= 3)
			{
				try
				{
					logger.LogInformation("Attempting to connect to the SMTP server.");
					await client.ConnectAsync(Settings.Server, Settings.Port ?? 0, Settings.SecureSocketOptions, token);
					logger.LogInformation("Connection successful.");
					return true;
				}

				catch (NotSupportedException)
				{
					throw;
				}

				catch (Exception e)
				{
					attempts++;
					logger.LogError(e, e.Message);
					await Task.Delay(new Random().Next(1000), token);

					if (attempts <= 3)
					{
						logger.LogInformation($"Trying again for attempt number {attempts}.");
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Attempts to authenticate to the SMTP server.
		/// </summary>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <returns><c>true</c> if authentication was successful; otherwise <c>false</c>.</returns>
		/// <exception cref="AuthenticationException">Thrown when authentication using the supplied credentials has failed.</exception>
		/// <exception cref="InvalidOperationException">Thrown when the SMTP client is not connected.</exception>
		/// <exception cref="OperationCanceledException">Thrown only when the <paramref name="token"/> is provided and <see cref="CancellationTokenSource.Cancel"/> is requested.</exception>
		public async Task<bool> TryAuthenticateAsync(CancellationToken token = default)
		{
		Auth:
			if (client.IsAuthenticated)
			{
				return true;
			}

			if (client.IsConnected)
			{
				int attempts = 1;
				while (attempts <= 3)
				{
					try
					{
						await client.AuthenticateAsync(Settings?.Login, Settings?.Password, token);
					}

					catch (AuthenticationException e)
					{
						logger.LogWarning(e.Message);
						throw;
					}

					catch (Exception e)
					{
						attempts++;
						logger.LogError(e, e.Message);
						await Task.Delay(new Random().Next(1000), token);

						if (attempts <= 3)
						{
							logger.LogInformation($"Trying again for attempt number {attempts}.");
						}
					}
				}
			}

			else if (await TryConnectAsync(token))
			{
				goto Auth;
			}

			return false;
		}

		/// <inheritdoc/>
		/// <param name="email">The receiver's address.</param>
		/// <param name="subject">The email suject.</param>
		/// <param name="body">The email body.</param>
		public async Task SendEmailAsync(string email, string subject, string body)
		{
			// Create the email message
			var message = new MimeMessage
			{
				Subject = subject,
				Body = new TextPart("plain") { Text = $@"{body}" }
			};
			message.To.Add(MailboxAddress.Parse(email));
			await SendEmailAsync(message);
		}

		/// <summary>
		/// Attempts to send a message using the <see cref="SmtpClient"/>.
		/// </summary>
		/// <param name="message">The <see cref="MimeMessage"/> to be sent.</param>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <exception cref="InvalidOperationException">Thrown when no recipients have been specified.</exception>
		/// <exception cref="OperationCanceledException">Thrown only when the <paramref name="token"/> is provided and <see cref="CancellationTokenSource.Cancel"/> is requested.</exception>
		public async Task SendEmailAsync(MimeMessage message, CancellationToken token = default)
		{
		Send:
			if (client.IsAuthenticated)
			{
				message.From.Add(MailboxAddress);

				int attempts = 1;
				while (attempts <= 3)
				{
					try
					{
						await client.SendAsync(message);
					}

					catch (InvalidOperationException)
					{
						throw;
					}

					catch (Exception e)
					{
						attempts++;
						await TryAgainAsync(e, attempts, token);
					}
				}
			}

			else if (client.IsConnected)
			{
				if (await TryAuthenticateAsync(token))
				{
					goto Send;
				}
			}
		}

		private async Task TryAgainAsync(Exception e, int attempts, CancellationToken token)
		{
			logger.LogError(e, e.Message);
			await Task.Delay(new Random().Next(1000), token);

			if (attempts <= 3)
			{
				logger.LogInformation($"Trying again for attempt number {attempts}.");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue != true)
			{
				if (disposing)
				{
					// Close SMTP connection
					client.Disconnect(true);

					client.Dispose();
					context.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}