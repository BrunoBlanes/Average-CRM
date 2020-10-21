using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(RG), nameof(CPF), IsUnique = true)]
	public class Person : Contact
	{
		[Required]
		[DataMember]
		public string FirstName { get; set; }

		[Required]
		[DataMember]
		public string LastName { get; set; }

		[Required]
		[DataMember]
		public string CPF { get; set; }

		[DataMember]
		public string? RG { get; set; }

		public Person() : base()
		{
			CPF = string.Empty;
			LastName = string.Empty;
			FirstName = string.Empty;
		}
	}
}