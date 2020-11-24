using System;
using System.Threading.Tasks;

using CRM.Server.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CRM.Server
{
	public class Program
	{
		private static IHost? host;

		public static bool FirstRun { get; private set; }

		public static async Task Main(string[] args)
		{
			host = CreateHostBuilder(args).Build();
			using IServiceScope scope = host.Services.CreateScope();
			IServiceProvider services = scope.ServiceProvider;
			ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
			IOptionsMonitor<Application> application = services.GetRequiredService<IOptionsMonitor<Application>>();

			FirstRun = application.CurrentValue.FirstRun;
			application.OnChange(change =>
			{
				FirstRun = change.FirstRun;
			});

			await context.Database.MigrateAsync();
			await host.RunAsync();
		}

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});
		}

		/// <summary>
		/// Attempts to gracefully stop the program.
		/// </summary>
		public static async void Shutdown()
		{
			if (host is not null)
			{
				await host.StopAsync();
			}
		}
	}
}