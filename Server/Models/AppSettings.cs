using System.Text.Json.Serialization;

using CRM.Core.Models;

namespace CRM.Server.Models
{
	public class AppSettings
	{
		public ConnectionStrings? ConnectionStrings { get; set; }
		public Logging? Logging { get; set; }
		public IdentityServer? IdentityServer { get; set; }
		public string? AllowedHosts { get; set; }
		public SmtpOptions? SmtpOptions { get; set; }
	}

	public class ConnectionStrings
	{
		[JsonPropertyName("DefaultConnection")]
		public string? DefaultConnection { get; set; }
	}

	public class LogLevel
	{
		[JsonPropertyName("Default")]
		public string? Default { get; set; }

		[JsonPropertyName("Microsoft")]
		public string? Microsoft { get; set; }

		[JsonPropertyName("Microsoft.Hosting.Lifetime")]
		public string? MicrosoftHostingLifetime { get; set; }

		[JsonPropertyName("Microsoft.EntityFrameworkCore")]
		public string? MicrosoftEntityFrameworkCore { get; set; }
	}

	public class Logging
	{
		public LogLevel? LogLevel { get; set; }
	}

	public class CRMClient
	{
		public string? Profile { get; set; }
	}

	public class Clients
	{
		[JsonPropertyName("CRM.Client")]
		public CRMClient? CRMClient { get; set; }
	}

	public class IdentityServer
	{
		public Clients? Clients { get; set; }
	}
}