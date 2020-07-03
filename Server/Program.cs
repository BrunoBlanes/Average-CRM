using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics.CodeAnalysis;

namespace CRM.Server
{
	[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "<Pending>")]
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

#pragma warning disable CA1031 // Do not catch general exception types
				catch (Exception ex)
				{
					ILogger<Program>? logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, $"An error occured while creating the database: {ex.Message}.");
				}
#pragma warning restore CA1031 // Do not catch general exception types
			}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
		}
	}
}