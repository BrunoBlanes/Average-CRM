using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace CRM.Client
{
	[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "<Pending>")]
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");
			ConfigureServices(builder.Services);
			await builder.Build().RunAsync().ConfigureAwait(false);
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.AddApiAuthorization();
			services.AddAuthorizationCore();
			services.AddBaseAddressHttpClient();
		}
	}
}
