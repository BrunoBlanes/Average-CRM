using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(Model), IsUnique = true)]
	public class Product
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string? NCM { get; set; }

		[Required]
		[DataMember]
		public string Model { get; set; }

		[Required]
		[DataMember]
		public string Description { get; set; }

		[Required]
		[DataMember]
		public ProductBrand Brand { get; set; }

		[Required]
		[DataMember]
		public ProductUnit Unit { get; set; }

		[Required]
		[DataMember]
		public ProductCategory Category { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public ICollection<ProductGroup>? ProductGroups { get; set; }

		[DataMember]
		public ICollection<Supplier>? Suppliers { get; set; }

		public Product()
		{
			Model = string.Empty;
			Unit = new ProductUnit();
			Brand = new ProductBrand();
			Description = string.Empty;
			CreatedOn = DateTime.UtcNow;
			Category = new ProductCategory();
		}
	}
}