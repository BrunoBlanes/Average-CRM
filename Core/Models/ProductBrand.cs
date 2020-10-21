using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(Brand), IsUnique = true)]
	public class ProductBrand
	{
		[DataMember]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Brand { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public ICollection<Product>? Products { get; set; }

		public ProductBrand()
		{
			Brand = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}