using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CRM.Core.Attributes;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Table("People")]
	[Index(nameof(RG), IsUnique = true)]
	[Index(nameof(CPF), IsUnique = true)]
	public class Person : Contact
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }

		[CpfValidation]
		[Display(Prompt = "CPF")]
		[StringLength(14, MinimumLength = 14)]
		public string CPF { get; set; }
		public string? RG { get; set; }

		public Person() : base()
		{
			CPF = string.Empty;
			LastName = string.Empty;
			FirstName = string.Empty;
		}
	}
}