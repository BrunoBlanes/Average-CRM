using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MailKit.Security;

//using Microsoft.AspNetCore.Mvc;

namespace CRM.Core.Models
{
	public class EmailSettings
	{
		[JsonIgnore]
		//[HiddenInput]
		public int Id { get; set; }

		[Display(Prompt = "Port")]
		public ushort? Port { get; set; }

		[Display(Prompt = "Sender Name")]
		public string? Name { get; set; }

		[EmailAddress]
		[Display(Prompt = "Login")]
		public string Login { get; set; }

		[Display(Prompt = "Server Address")]
		public string Server { get; set; }

		[EmailAddress]
		[Display(Prompt = "Sender Address")]
		public string? Address { get; set; }

		[Display(Prompt = "Password")]
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