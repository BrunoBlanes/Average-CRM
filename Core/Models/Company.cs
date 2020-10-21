using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(IE), nameof(Email), nameof(CNPJ), IsUnique = true)]
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
		public IList<Person>? Contacts { get; set; }

		public Company() : base()
		{
			CNPJ = string.Empty;
			LegalName = string.Empty;
			FantasyName = string.Empty;
		}
	}
}