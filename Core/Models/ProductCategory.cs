using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(Category), IsUnique = true)]
	public class ProductCategory
	{
		[DataMember]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Category { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public ICollection<Product>? Products { get; set; }

		public ProductCategory()
		{
			Category = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}