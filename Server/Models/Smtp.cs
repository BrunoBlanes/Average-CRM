using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Server.Models
{
	public class Smtp : IEquatable<Smtp?>
	{
		public const string Section = "Smtp";

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

		public Smtp()
		{
			Login = string.Empty;
			Server = string.Empty;
			Password = string.Empty;
			SecureSocket = SecureSocket.Auto;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as Smtp);
		}

		public bool Equals(Smtp? other)
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

		public Smtp Clone()
		{
			return (Smtp)MemberwiseClone();
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