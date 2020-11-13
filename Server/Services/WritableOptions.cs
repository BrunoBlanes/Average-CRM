using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

using CRM.Server.Interfaces;
using CRM.Server.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Server.Services
{
	public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
	{
		private readonly IWebHostEnvironment environment;
		private readonly IOptionsMonitor<T> options;
		private readonly string propertyName;
		private readonly ILogger<T> logger;
		private readonly string file;

		public WritableOptions(IWebHostEnvironment environment, IOptionsMonitor<T> options, ILogger<T> logger, string propertyName, string file)
		{
			this.file = file;
			this.logger = logger;
			this.options = options;
			this.environment = environment;
			this.propertyName = propertyName;
		}

		public T Value => options.CurrentValue;

		public T Get(string name)
		{
			return options.Get(name);
		}

		/// <inheritdoc/>
		public Task UpdateAsync(object obj)
		{
			List<Action<T>> actions = new();

			foreach (PropertyInfo property in obj.GetType().GetProperties())
			{
				actions.Add(x => x.GetType().GetProperty(property.Name)?.SetValue(x, property.GetValue(obj)));
			}

			return UpdateAsync((Action<T>)Delegate.Combine(actions.ToArray())!);
		}

		/// <inheritdoc/>
		public async Task UpdateAsync(Action<T> updateAction)
		{
			IFileProvider fileProvider = environment.ContentRootFileProvider;
			IFileInfo fileInfo = fileProvider.GetFileInfo(file);
			var physicalPath = fileInfo.PhysicalPath;
			updateAction(Value);
			AppSettings appSettings = await ReadAppSettingsAsync(physicalPath, true);

			foreach (PropertyInfo property in appSettings.GetType().GetProperties())
			{
				if (property.Name == propertyName)
				{
					property.SetValue(appSettings, Value);
					break;
				}
			}

			await File.WriteAllTextAsync(physicalPath, JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true }));
		}

		/// <summary>
		/// Deserialize and create a backup of a JSON file. 
		/// </summary>
		/// <remarks>
		/// When <paramref name="fallback"/> is <c>true</c> and the requested file is not found or is corrupted,
		/// a new instance of this method will be called with "<paramref name="physicalPath"/>.bak" as the file path.
		/// </remarks>
		/// <param name="physicalPath">The local path to the file.</param>
		/// <param name="fallback">Whether to fall back to the backup file in case the requested file is not found or is corrupted.</param>
		/// <returns>An instance of <see cref="AppSettings"/> populated with the contents of the requested file.</returns>
		private async Task<AppSettings> ReadAppSettingsAsync(string physicalPath, bool fallback = false)
		{
			try
			{
				// Try reading and deserializing the contents of the file
				var appOptions = await File.ReadAllTextAsync(physicalPath);

				if (JsonSerializer.Deserialize<AppSettings>(appOptions) is AppSettings appSettings)
				{
					// Save a backup of the deserialized JSON file and return its content
					await File.WriteAllTextAsync($"{physicalPath}.bak", appOptions);
					return appSettings;
				}

				return new();
			}

			catch (JsonException)
			{
				logger.LogError(@$"File""{physicalPath}"" is not in a valid JSON format.");
				return fallback ? await ReadAppSettingsAsync($"{physicalPath}.bak") : new();
			}

			catch (Exception)
			{
				return fallback ? await ReadAppSettingsAsync($"{physicalPath}.bak") : new();
			}
		}
	}
}