using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Server
{
	public class Program
	{
		private static IHost? host;

		public static async Task Main(string[] args)
		{
			host = CreateHostBuilder(args).Build();
			using IServiceScope scope = host.Services.CreateScope();
			IServiceProvider services = scope.ServiceProvider;
			ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
			ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
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
			if (host is null)
			{
				throw new InvalidOperationException();
			}

			await host.StopAsync();
		}
	}
}