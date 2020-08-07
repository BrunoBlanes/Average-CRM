using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	[DataContract(IsReference = true)]
	public class Address
	{
		[DataMember]
		[Column("AddressId")]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public decimal Latitude { get; set; }

		[Required]
		[DataMember]
		public decimal Longitude { get; set; }

		[Required]
		[DataMember]
		public string Destination { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public List<Contact>? Contacts { get; set; }

		public Address()
		{
			Destination = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}