using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");
			builder.Services.AddHttpClient("ServerAPI", client =>
			{
				client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
			});

			builder.Services.AddHttpClient("Server", client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

			// Supply HttpClient instances that include access tokens when making requests to the server project
			builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Server"));
			builder.Services.AddOptions();
			builder.Services.AddApiAuthorization();
			await builder.Build().RunAsync();
		}
	}
}