using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Models
{
	public class SmtpOptions : IEquatable<SmtpOptions?>
	{
		public const string Section = "SmtpOptions";

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

		[DataType(DataType.Password)]
		[Display(Prompt = "Password")]
		public string Password { get; set; }
		public SecureSocket SecureSocket { get; set; }

		public SmtpOptions()
		{
			Login = string.Empty;
			Server = string.Empty;
			Password = string.Empty;
			SecureSocket = SecureSocket.Auto;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as SmtpOptions);
		}

		public bool Equals(SmtpOptions? other)
		{
			return other is not null
				&& Port == other.Port
				&& Name == other.Name
				&& Login == other.Login
				&& Server == other.Server
				&& Address == other.Address
				&& Password == other.Password
				&& SecureSocket == other.SecureSocket;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Port, Name, Login, Server, Address, Password, SecureSocket);
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