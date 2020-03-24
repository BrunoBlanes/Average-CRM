using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class Product
	{
		[DataMember]
		[Column("ProductId")]
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
		public int BrandId { get; set; }

		[Required]
		[DataMember]
		public ProductUnit Unit { get; set; }
		public int UnitId { get; set; }

		[Required]
		[DataMember]
		public ProductCategory Category { get; set; }
		public int CategoryId { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public IList<ProductGroupDetail>? Groups { get; set; }

		[DataMember]
		public IList<ProductSupplierDetail>? Suppliers { get; set; }

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