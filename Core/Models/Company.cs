using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Table("Companies")]
	[Index(nameof(IE), IsUnique = true)]
	[Index(nameof(IM), IsUnique = true)]
	[Index(nameof(CNPJ), IsUnique = true)]
	public class Company : Contact
	{
		[Required]
		[DataMember]
		public string LegalName { get; set; }

		[Required]
		[DataMember]
		public string FantasyName { get; set; }

		[Required]
		[DataMember]
		public string CNPJ { get; set; }

		[DataMember]
		public string? IE { get; set; }

		[DataMember]
		public string? IM { get; set; }

		[DataMember]
		public ICollection<Person>? Contacts { get; set; }

		public Company() : base()
		{
			CNPJ = string.Empty;
			LegalName = string.Empty;
			FantasyName = string.Empty;
		}
	}
}