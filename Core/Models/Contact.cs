using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Table("Contacts")]
	[Index(nameof(Email), IsUnique = true)]
	public class Contact
	{
		[DataMember]
		public int Id { get; set; }

		[Required]
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
			Email = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}