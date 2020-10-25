using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Models
{
	public class Settings
	{
		public int Id { get; set; }
		public bool FirstRun { get; set; }

		[Required]
		public EmailSettings EmailSettings { get; set; } = null!;
	}
}