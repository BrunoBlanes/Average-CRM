using System.Collections.Generic;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Controllers
{
	public class OidcConfigurationController : Controller
	{
		private readonly ILogger<OidcConfigurationController> logger;

		public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

		public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider,
			ILogger<OidcConfigurationController> logger)
		{
			ClientRequestParametersProvider = clientRequestParametersProvider;
			this.logger = logger;
		}

		[HttpGet("_configuration/{clientId}")]
		public IActionResult GetClientRequestParameters([FromRoute] string clientId)
		{
			IDictionary<string, string>? parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
			logger.LogInformation($"OIDC Configuration requested by client {clientId}.");
			return Ok(parameters);
		}
	}
}