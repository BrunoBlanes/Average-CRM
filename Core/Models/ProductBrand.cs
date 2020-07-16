using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	[DataContract(IsReference = true)]
	public class ProductBrand
	{
		[DataMember]
		[Column("ProductBrandId")]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Brand { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public IList<Product>? Products { get; set; }

		public ProductBrand()
		{
			Brand = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}