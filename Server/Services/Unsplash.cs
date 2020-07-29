using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using CRM.Server.Models;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Services
{
	public class Unsplash : IHostedService, IDisposable
	{
		private readonly IHttpClientFactory clientFactory;
		private readonly ILogger<Unsplash> logger;
		private bool isDisposed;
		private Timer? timer;

		public Unsplash(IHttpClientFactory clientFactory, ILogger<Unsplash> logger)
		{
			this.logger = logger;
			this.clientFactory = clientFactory;
		}

		// Starts the periodic task (from IHostedService)
		public Task StartAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Unsplash service is running.");
			timer = new Timer(GetNewRandomImage, null, TimeSpan.Zero, TimeSpan.FromMinutes(1.5));
			return Task.CompletedTask;
		}


		// Gets a random image from Unsplash
		private async void GetNewRandomImage(object? state)
		{
			logger.LogInformation("Unsplash is working.");

			// Gets the http client for the Unsplash API
			using HttpClient? httpClient = clientFactory.CreateClient("Unsplash");
			using var request = new HttpRequestMessage(HttpMethod.Get, $"/photos/random");
			HttpResponseMessage? response = await httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				// Deserialize json as UnsplashModel and assign to static property
				using Stream? responseStream = await response.Content.ReadAsStreamAsync();
				UnsplashModel.Unsplash = await JsonSerializer.DeserializeAsync<UnsplashModel>(responseStream);
				logger.LogInformation("Got a new random picture from Unsplash.");
			}

			else
			{
				// Log error
				logger.LogError("Could not retrieve an image from Unsplash", response.StatusCode);
			}
		}

		// Stops the periodic task (from IHostedService)
		public Task StopAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Unsplash service is stopping.");
			timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		// Dispose() calls Dispose(true)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose(bool disposing)
		{
			if (isDisposed) return;
			if (disposing) timer?.Dispose();
			isDisposed = true;
		}
	}
}
