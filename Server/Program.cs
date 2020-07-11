using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IHost? host = CreateHostBuilder(args).Build();

			using (IServiceScope? scope = host.Services.CreateScope())
			{
				IServiceProvider? services = scope.ServiceProvider;

				try
				{
					ApplicationDbContext? context = services.GetRequiredService<ApplicationDbContext>();
					context.Database.EnsureCreated();
				}

				catch (Exception ex)
				{
					ILogger<Program>? logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, $"An error occured while creating the database: {ex.Message}.");
				}
			}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
		}
	}
}