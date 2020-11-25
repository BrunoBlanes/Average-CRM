using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Table("Contacts")]
	[Index(nameof(Email), IsUnique = true)]
	public class Contact
	{
		public int Id { get; set; }

		[EmailAddress]
		public string Email { get; set; }
		public string? Phone { get; set; }
		public DateTime CreatedOn { get; set; }
		public Address? Address { get; set; }
		public ICollection<Budget>? Budgets { get; set; }

		public Contact()
		{
			Email = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}