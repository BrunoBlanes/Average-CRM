using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using CRM.Core.Attributes;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Table("Contacts")]
	[Index(nameof(CPF), IsUnique = true)]
	[Index(nameof(Email), IsUnique = true)]
	public class Contact
	{
		[DataMember]
		public int Id { get; set; }

		[CpfValidation]
		[Display(Prompt = "CPF")]
		[StringLength(14, MinimumLength = 14)]
		public string CPF { get; set; }

		[DataMember]
		[EmailAddress]
		public string Email { get; set; }

		[DataMember]
		public string? Phone { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public Address? Address { get; set; }

		[DataMember]
		public ICollection<Budget>? Budgets { get; set; }

		public Contact()
		{
			CPF = string.Empty;
			Email = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}