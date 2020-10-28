using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MailKit.Security;

namespace CRM.Core.Models
{
	public class EmailSettings
	{
		[JsonIgnore]
		public int Id { get; set; }
		public int Port { get; set; }
		public string? Name { get; set; }

		[EmailAddress]
		public string Login { get; set; }

		[Display(Name = "Server Address")]
		public string Server { get; set; }

		[EmailAddress]
		[Display(Name = "Email Address")]
		public string? Address { get; set; }

		[DataType(DataType.Password)]
		public string Password { get; set; }

		[NotMapped]
		[JsonIgnore]
		public SecureSocket SecureSocket
		{
			get => (SecureSocket)SecureSocketOptions;
			set => SecureSocketOptions = (SecureSocketOptions)value;
		}

		public SecureSocketOptions SecureSocketOptions { get; set; }

		public EmailSettings()
		{
			Login = string.Empty;
			Server = string.Empty;
			Password = string.Empty;
			SecureSocket = SecureSocket.Auto;
		}
	}

	public enum SecureSocket
	{
		[Display(Name = "None")]
		None,
		[Display(Name = "Auto")]
		Auto,
		[Display(Name = "SSL/TLS")]
		SslOnConnect,
		[Display(Name = "STARTTLS")]
		StartTls
	}
}