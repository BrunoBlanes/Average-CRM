using System;
using System.Threading;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;

using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

namespace CRM.Server.Services
{
	public class SmtpService : IEmailSender, IDisposable
	{
		private MailboxAddress? mailboxAddress;
		private readonly SmtpClient client;
		private readonly ILogger logger;
		private SmtpOptions? settings;
		private bool disposedValue;

		/// <summary>
		/// <c>true</c> if the configuration was successful; otherwise <c>false</c>.
		/// </summary>
		public bool IsConfigured { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="SmtpService"/>.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/>.</param>
		public SmtpService(ILogger<SmtpService> logger, IOptionsMonitor<SmtpOptions> options)
		{
			this.logger = logger;
			client = new SmtpClient();
			options.OnChange(change => Configure(options));
			Configure(options);
		}

		private async void Configure(IOptionsMonitor<SmtpOptions> options)
		{
			if (options.CurrentValue.Equals(new()) is false)
			{
				settings = options.CurrentValue;
				string password = settings.Password;
				settings.Password = string.Empty;
				using var passwordHasher = new PasswordHasher();
				settings.Password = passwordHasher.Decrypt(password, settings);
				mailboxAddress = new MailboxAddress(settings.Name, settings.Address);
				await TryAuthenticateAsync();
				logger.LogInformation("Email service configured successfully.");
				IsConfigured = true;
			}

			else
			{
				logger.LogError("Could not retrieve SMTP options from configuration file.");
			}
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

			if (settings is null)
			{
				throw new InvalidOperationException("Could not retrieve email configuration from the database.");
			}

			int attempts = 1;
			while (attempts <= 3)
			{
				try
				{
					logger.LogInformation("Attempting to connect to the SMTP server.");
					await client.ConnectAsync(settings.Server, settings.Port ?? 0, (SecureSocketOptions)settings.SecureSocket, token);
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
					await TryAgainAsync(e, attempts, token);
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
						logger.LogInformation("Attempting to authenticate to the SMTP server.");
						await client.AuthenticateAsync(settings?.Login, settings?.Password, token);
						logger.LogInformation("Authentication successful.");
						return true;
					}

					catch (AuthenticationException e)
					{
						logger.LogWarning(e.Message);
						throw;
					}

					catch (Exception e)
					{
						attempts++;
						await TryAgainAsync(e, attempts, token);
					}
				}
			}

			else if (await TryConnectAsync(token))
			{
				return await TryAuthenticateAsync(token);
			}

			return false;
		}

		/// <inheritdoc/>
		/// <param name="email">The receiver's address.</param>
		/// <param name="subject">The email suject.</param>
		/// <param name="body">The email body.</param>
		/// <exception cref="InvalidOperationException">Thrown when no recipients have been specified.</exception>
		public async Task SendEmailAsync(string email, string subject, string body)
		{
			// Create the email message
			var message = new MimeMessage
			{
				Subject = subject,
				Body = new TextPart("plain") { Text = $@"{body}" }
			};
			message.To.Add(MailboxAddress.Parse(email));
			message.From.Add(mailboxAddress);
			await TrySendEmailAsync(message);
		}

		/// <summary>
		/// Attempts to send a message using the <see cref="SmtpClient"/>.
		/// </summary>
		/// <param name="message">The <see cref="MimeMessage"/> to be sent.</param>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <exception cref="InvalidOperationException">Thrown when no recipients have been specified.</exception>
		/// <exception cref="OperationCanceledException">Thrown only when the <paramref name="token"/> is provided and <see cref="CancellationTokenSource.Cancel"/> is requested.</exception>
		public async Task<bool> TrySendEmailAsync(MimeMessage message, CancellationToken token = default)
		{
			if (client.IsAuthenticated)
			{
				int attempts = 1;
				while (attempts <= 3)
				{
					try
					{
						await client.SendAsync(message);
						return true;
					}

					catch (InvalidOperationException)
					{
						throw;
					}

					catch (CommandException e)
					{
						logger.LogWarning(e.Message);
						return false;
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
					return await TrySendEmailAsync(message, token);
				}
			}

			return false;
		}

		private async Task TryAgainAsync(Exception e, int attempts, CancellationToken token)
		{
			if (attempts <= 3)
			{
				logger.LogError(e, e.Message);
				await Task.Delay(new Random().Next(1000), token);
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