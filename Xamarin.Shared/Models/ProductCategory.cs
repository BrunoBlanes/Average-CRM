using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class ProductCategory
	{
		[DataMember]
		[Column("ProductCategoryId")]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Category { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public IList<Product>? Products { get; set; }

		public ProductCategory()
		{
			Category = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}