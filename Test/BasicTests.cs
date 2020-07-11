using System.Net.Http;
using System.Threading.Tasks;
using CRM.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CRM.Test
{
	public class BasicTests : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly WebApplicationFactory<Startup> factory;

		public BasicTests(WebApplicationFactory<Startup> factory)
		{
			this.factory = factory;
		}

		[Theory]
		[InlineData("/")]
		[InlineData("/Index")]
		[InlineData("/Counter")]
		[InlineData("/Authentication")]
		[InlineData("/Authenticationn")]
		public async Task GetEndpointsReturnSuccessAndCorrectContentType(string url)
		{
			// Arrange
			var client = factory.CreateClient();

			// Act
			var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

			// Assert
			response.EnsureSuccessStatusCode(); // Status Code 200-299
			Assert.Equal("text/html", response.Content.Headers.ContentType.ToString());
		}
	}
}
