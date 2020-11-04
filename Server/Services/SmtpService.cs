using System;
using System.Threading;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;
using CRM.Server.Interfaces;

using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

namespace CRM.Server.Services
{
	public class SmtpService : ISmtpService, IDisposable
	{
		private readonly IDisposable optionsListener;
		private MailboxAddress? mailboxAddress;
		private readonly SmtpClient client;
		private readonly ILogger logger;
		private Task? configureTask;
		private SmtpOptions options;
		private bool disposedValue;

		/// <summary>
		/// <c>true</c> if the configuration was successful; otherwise <c>false</c>.
		/// </summary>
		public bool IsConfigured { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="SmtpService"/>.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/>.</param>
		/// <param name="options">The <see cref="IOptionsMonitor{TOptions}"/>.</param>
		public SmtpService(ILogger<SmtpService> logger, IOptionsMonitor<SmtpOptions> options)
		{
			this.logger = logger;
			client = new SmtpClient();
			this.options = new SmtpOptions();
			Task.Run(async () => await ConfigureAsync(options.CurrentValue));
			
			// Adds an event listener for when 'appsettings.json' changes
			optionsListener = options.OnChange(async (change) =>
			{
				// Wait for an update to finish
				// before invoking another one
				if (configureTask is Task)
				{
					await configureTask;
				}

				configureTask = ConfigureAsync(change);
			});
		}

		/// <summary>
		/// Attempts to configure the SMTP client with the provided <paramref name="smtpOptions"/>.
		/// </summary>
		/// <param name="smtpOptions">The <see cref="SmtpOptions"/>.</param>
		/// <returns>A <see cref="Task"/> that represents the configuration process.</returns>
		/// <exception cref="AuthenticationException">Thrown when authentication using the supplied credentials has failed.</exception>
		/// <exception cref="NotSupportedException">Thrown when options was set to <see cref="SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.</exception>
		private async Task ConfigureAsync(SmtpOptions smtpOptions)
		{
			SmtpOptions options = smtpOptions.Clone();

			// smtpOptions has default value
			if (options.Equals(new()) is false)
			{
				options.Password = string.Empty;
				using var passwordHasher = new PasswordHasher();
				options.Password = passwordHasher.Decrypt(smtpOptions.Password, options);

				// Only run if new settings are different
				if (this.options.Equals(options) is false)
				{
					if (await ConnectAsync(options))
					{
						IsConfigured = true;
						this.options = options;
						mailboxAddress = new MailboxAddress(options.Name, options.Address);
						logger.LogInformation("Email service was configured successfully.");
					}
				}
			}

			else
			{
				IsConfigured = false;
				logger.LogCritical("Could not retrieve SMTP options from configuration file.");
			}
		}

		/// <summary>
		/// Attempts to open a connection to the SMTP server using the provided <paramref name="options"/>.
		/// </summary>
		/// <param name="options">The <see cref="SmtpOptions"/>.</param>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <returns><c>true</c> if the connection was successful; otherwise <c>false</c>.</returns>
		/// <exception cref="AuthenticationException">Thrown when authentication using the supplied credentials has failed.</exception>
		/// <exception cref="NotSupportedException">Thrown when options was set to <see cref="SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.</exception>
		private async Task<bool> ConnectAsync(SmtpOptions options, CancellationToken token = default)
		{
			// Disconnect to avoid thorwing InvalidOperationException
			// by the client when connected to the same host
			await client.DisconnectAsync(true, token);

			for (int attempts = 0; attempts < 3; attempts++)
			{
				try
				{
					logger.LogInformation(@$"Attempting to connect to the SMTP server at ""{options.Server}"".");
					await client.ConnectAsync(options.Server, options.Port ?? 0, (SecureSocketOptions)options.SecureSocket, token);
					logger.LogInformation(@$"Attempting to authenticate to the SMTP server as ""{options.Login}"".");
					await client.AuthenticateAsync(options.Login, options.Password, token);
					return true;
				}

				// Retrow AuthenticationException
				// to be handled by the view model
				catch (AuthenticationException e)
				{
					logger.LogError(e.Message);
					throw;
				}

				catch (OperationCanceledException e)
				{
					logger.LogWarning(e.Message);
					return false;
				}

				// Retrow NotSupportedException to be handled by the view model
				catch (Exception e) when (e is not NotSupportedException)
				{
					await LogAndTryAgainAsync(e, attempts, token);
				}
			}

			return false;
		}

		/// <inheritdoc/>>
		public async Task SendEmailAsync(MimeMessage message, CancellationToken token = default)
		{
			// Set the default message author
			message.From.Add(mailboxAddress);

			if (client.IsAuthenticated)
			{
				for (int attempts = 0; attempts < 3; attempts++)
				{
					try
					{
						await client.SendAsync(message, token);
						logger.LogInformation($"New email sent to {message.To}.");
					}

					catch (CommandException e)
					{
						logger.LogError(e.Message);
					}

					catch (OperationCanceledException e)
					{
						logger.LogWarning(e.Message);
					}

					// Retrow InvalidOperationException to be handled by the view
					catch (Exception e) when (e is not InvalidOperationException)
					{
						await LogAndTryAgainAsync(e, attempts, token);
					}
				}
			}

			else
			{
				// Try to connect before sending the message
				// TODO: Improve this by avoiding disconnection
				if (await ConnectAsync(options, token))
				{
					await SendEmailAsync(message, token);
				}
			}
		}

		/// <summary>
		/// Log the <see cref="Exception.Message"/> and wait for a random delay of up to 3 seconds.
		/// </summary>
		/// <param name="exception">The <see cref="Exception"/>.</param>
		/// <param name="attempts">The number of <paramref name="attempts"/> already been made.</param>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <remarks>When 3 <paramref name="attempts"/> have been made, this method will log the <paramref name="exception"/> and return without delay.</remarks>
		/// <returns>A <see cref="Task"/> that represents the logging and the time delay.</returns>
		private async Task LogAndTryAgainAsync(Exception exception, int attempts, CancellationToken token = default)
		{
			logger.LogError(exception, exception.Message);

			if (attempts + 2 <= 3)
			{
				// Wait for a delay as to not spam the server
				await Task.Delay(new Random().Next(3000), token);
				logger.LogInformation($"Trying again for attempt number {attempts + 2}.");
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
					configureTask?.Dispose();
					optionsListener.Dispose();
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