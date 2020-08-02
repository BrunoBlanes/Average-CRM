using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using CRM.Server.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Services
{
	public class Unsplash : IHostedService, IDisposable
	{
		private readonly IHttpClientFactory clientFactory;
		private readonly IWebHostEnvironment environment;
		private readonly ILogger<Unsplash> logger;
		private bool disposedValue;
		private Timer? timer;

		public Unsplash(IHttpClientFactory clientFactory,
			IWebHostEnvironment environment,
			ILogger<Unsplash> logger)
		{
			this.logger = logger;
			this.environment = environment;
			this.clientFactory = clientFactory;
		}

		// Starts the periodic task (from IHostedService)
		public Task StartAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Unsplash service is running.");
			//timer = new Timer(GetNewRandomImage, null, TimeSpan.Zero, TimeSpan.FromMinutes(1.5));
			return Task.CompletedTask;
		}

		// Gets a random image from Unsplash
		private async void GetNewRandomImage(object? state)
		{
			logger.LogInformation("Unsplash service is working.");
			string searchQuery = "/photos/random" +
				"?query=nature" +
				"&content_filter=high" +
				"&orientation=landscape";
			HttpResponseMessage? response = await SendHttpGetRequestAsync(searchQuery);

			if (response.IsSuccessStatusCode)
			{
				// Deserialize json as UnsplashModel
				UnsplashModel? unsplash = await response.Content.ReadFromJsonAsync<UnsplashModel>();
				response = await SendHttpGetRequestAsync(unsplash.Links.DownloadLocation);

				if (response.IsSuccessStatusCode)
				{
					// Get the download link and format it as needed
					using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

					if (document.RootElement.TryGetProperty("url", out JsonElement download))
					{
						string url = download.GetString() ?? string.Empty;
						var uri = new Uri(url.Substring(0, url.IndexOf('&', StringComparison.OrdinalIgnoreCase)) +
							"&q=0" +
							"&w=1920" +
							"&fit=clip" +
							"&auto=compress");
						response = await SendHttpGetRequestAsync(uri);

						if (response.IsSuccessStatusCode)
						{
							using Stream? responseStream = await response.Content.ReadAsStreamAsync();
							using var bitmap = new Bitmap(responseStream);

							if (bitmap is not null)
							{
								bitmap.Save(@$"{environment.WebRootPath}\img\login-bg-image.jpg", ImageFormat.Jpeg);
								logger.LogInformation("Got a new random picture from Unsplash.");
								return;
							}
						}
					}
				}
			}

			// If we've made this far, than there were errors
			using var errorDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
			JsonElement errors = errorDocument.RootElement.GetProperty("errors");

			// Logs every error to the console
			foreach (JsonElement error in errors.EnumerateArray())
			{
				logger.LogError(error.GetString());
			}
		}

		private async Task<HttpResponseMessage> SendHttpGetRequestAsync(string uri)
		{
			// Gets the http client for the Unsplash API
			using HttpClient? httpClient = clientFactory.CreateClient("Unsplash");
			using var request = new HttpRequestMessage(HttpMethod.Get, uri);
			return await httpClient.SendAsync(request);
		}

		private Task<HttpResponseMessage> SendHttpGetRequestAsync(Uri uri)
		{
			return SendHttpGetRequestAsync(uri.AbsoluteUri);
		}

		// Stops the periodic task (from IHostedService)
		public Task StopAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Unsplash service is stopping.");
			timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue is false)
			{
				if (disposing)
				{
					// dispose managed state (managed objects)
					timer?.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
