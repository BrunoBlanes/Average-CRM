using System;
using CRM.Server.Areas.Identity;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace CRM.Server.Areas.Identity
{
	public class IdentityHostingStartup : IHostingStartup
	{
		public void Configure(IWebHostBuilder builder)
		{
			if (builder is null) throw new ArgumentNullException(nameof(builder));
			builder.ConfigureServices((context, services) => { });
		}
	}
}