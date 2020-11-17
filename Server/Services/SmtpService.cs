using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;
using CRM.Server.Interfaces;
using CRM.Server.Models;

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
		private bool disposedValue;
		private Smtp smtp;

		/// <summary>
		/// <c>true</c> if the configuration was successful; otherwise <c>false</c>.
		/// </summary>
		public bool IsConfigured { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="SmtpService"/>.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/>.</param>
		/// <param name="smtpSettings">The <see cref="IOptionsMonitor{TOptions}"/>.</param>
		public SmtpService(ILogger<SmtpService> logger, IOptionsMonitor<Smtp> smtpSettings)
		{
			smtp = new Smtp();
			this.logger = logger;
			client = new SmtpClient();
			Task.Run(async () => await ConfigureAsync(smtpSettings.CurrentValue));

			// Adds an event listener for when 'appsettings.json' changes
			optionsListener = smtpSettings.OnChange(async (change) =>
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

		/// <inheritdoc/>
		public async Task ConfigureAsync(Smtp smtp)
		{
			Smtp settings = smtp.Clone();

			// smtpOptions has default value
			if (settings.Equals(new()) is false)
			{
				settings.Password = string.Empty;
				using var passwordHasher = new PasswordHasher();
				settings.Password = passwordHasher.Decrypt(smtp.Password, settings);

				// Only run if new settings are different
				if (this.smtp.Equals(settings) is false)
				{
					if (await ConnectAsync(settings))
					{
						IsConfigured = true;
						this.smtp = settings;
						mailboxAddress = new MailboxAddress(settings.Name, settings.Address);
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
		/// Attempts to open a connection to the SMTP server using the provided <paramref name="smtp"/>.
		/// </summary>
		/// <param name="smtp">The <see cref="Smtp"/>.</param>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <returns><c>true</c> if the connection was successful; otherwise <c>false</c>.</returns>
		/// <exception cref="AuthenticationException">Thrown when authentication using the supplied credentials has failed.</exception>
		/// <exception cref="NotSupportedException">Thrown when options was set to <see cref="SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.</exception>
		private async Task<bool> ConnectAsync(Smtp smtp, CancellationToken token = default)
		{
			// Disconnect to avoid thorwing InvalidOperationException
			// by the client when connected to the same host
			await client.DisconnectAsync(true, token);

			for (var attempts = 0; attempts < 3; attempts++)
			{
				try
				{
					logger.LogInformation(@$"Attempting to connect to the SMTP server at ""{smtp.Server}"".");
					await client.ConnectAsync(smtp.Server, smtp.Port ?? 0, (SecureSocketOptions)smtp.SecureSocket, token);
					logger.LogInformation(@$"Attempting to authenticate to the SMTP server as ""{smtp.Login}"".");
					await client.AuthenticateAsync(smtp.Login, smtp.Password, token);
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
				for (var attempts = 0; attempts < 3; attempts++)
				{
					try
					{
						await client.SendAsync(message, token);
						logger.LogInformation($"New email sent to {message.To}.");
						break;
					}

					catch (OperationCanceledException e)
					{
						logger.LogWarning(e.Message);
						break;
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
				if (await ConnectAsync(smtp, token))
				{
					await SendEmailAsync(message, token);
				}
			}
		}

		/// <inheritdoc/>
		public async Task SendAccountConfirmationEmailAsync(string callbackUrl, ApplicationUser user, CancellationToken token = default)
		{
			var message = new MimeMessage
			{
				Subject = "Confirm your email",
				Body = new TextPart("html")
				{
					Text = @$"Please confirm your account by <a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"">clicking here</a>."
				},
			};
			message.To.Add(MailboxAddress.Parse(user.Email));

			for (var attempts = 0; attempts < 3; attempts++)
			{
				try
				{
					await SendEmailAsync(message, token);
					break;
				}

				catch (AuthenticationException e)
				{
					logger.LogError(e, e.Message);
					break;
				}

				catch (OperationCanceledException e)
				{
					logger.LogWarning(e.Message);
					break;
				}

				catch (Exception e)
				{
					await LogAndTryAgainAsync(e, attempts, token);
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