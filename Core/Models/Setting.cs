using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Models
{
	public class Setting
	{
		public int Id { get; set; }
		public bool FirstRun { get; set; }

		[Required]
		public EmailSetting EmailSettings { get; set; }

		public Setting()
		{
			EmailSettings = new EmailSetting();
		}
	}
}