using System.Text.Json.Serialization;

namespace CRM.Server.Models
{
	public class Settings
	{
		public ConnectionStrings? ConnectionStrings { get; set; }
		public Logging? Logging { get; set; }
		public IdentityServer? IdentityServer { get; set; }
		public Application Application { get; set; } = new();
		public Smtp? Smtp { get; set; }
	}

	public class ConnectionStrings
	{
		public string? SqlConnection { get; set; }
	}

	public class Application
	{
		public const string Section = "Application";

		public bool FirstRun { get; set; } = true;
		public string? AllowedHosts { get; set; } = "*";
		public string BaseUrl { get; set; } = "https://127.0.0.1:5001/";
	}

	public class Logging
	{
		public LogLevel LogLevel { get; set; } = new();
	}

	public class LogLevel
	{
		public string Microsoft { get; set; } = "Warning";
		public string Default { get; set; } = "Information";

		[JsonPropertyName("Microsoft.Hosting.Lifetime")]
		public string MicrosoftHostingLifetime { get; set; } = "Information";

		[JsonPropertyName("Microsoft.EntityFrameworkCore")]
		public string MicrosoftEntityFrameworkCore { get; set; } = "Information";
	}

	public class IdentityServer
	{
		public Key Key { get; set; } = new();
	}

	public class Key
	{
		public string Type { get; set; } = "Development";
	}
}