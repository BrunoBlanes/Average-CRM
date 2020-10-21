namespace CRM.Core.Models
{
	public class EmailSetting
	{
		public int Id { get; set; }
		public int Port { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }
		public string Server { get; set; }
		public string Address { get; set; }
		public string Password { get; set; }

		public EmailSetting()
		{
			Name = string.Empty;
			Login = string.Empty;
			Server = string.Empty;
			Address = string.Empty;
			Password = string.Empty;
		}
	}
}