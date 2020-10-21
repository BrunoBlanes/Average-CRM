using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	public class Address
	{
		[DataMember]
		public int Id { get; set; }

		[Required]
		[DataMember]
		[Column(TypeName = "DECIMAL(10,7)")]
		public decimal Latitude { get; set; }

		[Required]
		[DataMember]
		[Column(TypeName = "DECIMAL(10,7)")]
		public decimal Longitude { get; set; }

		[Required]
		[DataMember]
		public string Street { get; set; }

		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public string Neighbourhood { get; set; }

		[DataMember]
		public string CEP { get; set; }

		[DataMember]
		public string City { get; set; }

		[DataMember]
		public string State { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public ICollection<Contact>? Contacts { get; set; }

		public Address()
		{
			CEP = string.Empty;
			City = string.Empty;
			State = string.Empty;
			Street = string.Empty;
			CreatedOn = DateTime.UtcNow;
			Neighbourhood = string.Empty;
		}
	}
}