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

			// Add server api httpclient
			builder.Services.AddHttpClient("ServerAPI", client =>
			{
				client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
			});

			// Add server httpclient
			builder.Services.AddHttpClient("Server", client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

			// Custom authentication paths
			builder.Services.AddApiAuthorization(options =>
			{
				options.AuthenticationPaths.LogInPath = "account/login";
				options.AuthenticationPaths.LogInCallbackPath = "account/login-callback";
				options.AuthenticationPaths.LogInFailedPath = "account/login-failed";
				options.AuthenticationPaths.LogOutPath = "account/logout";
				options.AuthenticationPaths.LogOutCallbackPath = "account/logout-callback";
				options.AuthenticationPaths.LogOutFailedPath = "account/logout-failed";
				options.AuthenticationPaths.LogOutSucceededPath = "account/logged-out";
				options.AuthenticationPaths.ProfilePath = "account/profile";
				options.AuthenticationPaths.RegisterPath = "account/register";
			});

			// Supply HttpClient instances that include access tokens when making requests to the server project
			builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Server"));
			builder.Services.AddOptions();
			builder.Services.AddApiAuthorization();
			await builder.Build().RunAsync();
		}
	}
}