using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class Contact
	{
		[DataMember]
		[Column("ContactId")]
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
		public int? AddressId { get; set; }

		[DataMember]
		public IList<Budget>? Budgets { get; set; }

		public Contact()
		{
			Email = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}