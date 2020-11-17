using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Controllers
{
	[Produces("application/json")]
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

		/// <summary>
		/// Get the configuration for the specified <paramref name="clientId"/>.
		/// </summary>
		/// <param name="clientId">The client id registered with this server.</param>
		/// <response code="200">Returns the configuration for the requested <paramref name="clientId"/>.</response>
		/// <response code="404">If no configuration for the specified <paramref name="clientId"/> is found.</response> 
		/// <returns></returns>
		[HttpGet("_configuration/{clientId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetClientRequestParameters([FromRoute] string clientId)
		{
			try
			{
				IDictionary<string, string>? parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
				logger.LogInformation($"OIDC Configuration requested for {clientId}.");
				return Ok(parameters);
			}

			catch (InvalidOperationException)
			{
				logger.LogWarning($"OIDC Configuration requested for {clientId} was not found.");
				return NotFound();
			}
		}
	}
}