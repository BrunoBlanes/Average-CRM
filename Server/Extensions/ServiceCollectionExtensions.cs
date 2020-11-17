using CRM.Server.Interfaces;
using CRM.Server.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Server.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void ConfigureWritable<T>(
			this IServiceCollection services,
			IConfigurationSection section,
			string file = "appsettings.json") where T : class, new()
		{
			services.Configure<T>(section);
			services.AddTransient<IWritableOptions<T>>(provider =>
			{
				IWebHostEnvironment environment = provider.GetService<IWebHostEnvironment>()!;
				IOptionsMonitor<T> options = provider.GetService<IOptionsMonitor<T>>()!;
				ILogger<T> logger = provider.GetService<ILogger<T>>()!;
				return new WritableOptions<T>(environment, options, logger, section.Key, file);
			});
		}
	}
}