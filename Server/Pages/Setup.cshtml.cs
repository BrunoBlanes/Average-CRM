using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Pages
{
	public class SetupModel : PageModel
	{
		private readonly IHttpClientFactory clientFactory;

		[BindProperty]
		public EmailSettings Settings { get; set; }

		public SetupModel(IHttpClientFactory clientFactory)
		{
			Settings = new EmailSettings();
			this.clientFactory = clientFactory;
		}

		public async Task OnGetAsync()
		{
			HttpClient client = clientFactory.CreateClient("ServerAPI");
			HttpResponseMessage? response = await client.GetAsync("EmailSettings");
			
			if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
			{
				Settings = await response.Content.ReadFromJsonAsync<EmailSettings>() ?? new();
				Settings.Password = string.Empty;
			}
		}
	}
}