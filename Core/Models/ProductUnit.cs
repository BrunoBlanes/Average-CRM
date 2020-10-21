using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(Unit), IsUnique = true)]
	public class ProductUnit
	{
		[DataMember]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Unit { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public ICollection<Product>? Products { get; set; }

		public ProductUnit()
		{
			Unit = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}