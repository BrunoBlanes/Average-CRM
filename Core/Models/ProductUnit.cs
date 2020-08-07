using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	[DataContract(IsReference = true)]
	public class ProductUnit
	{
		[DataMember]
		[Column("ProductUnitId")]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Unit { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public IList<Product>? Products { get; set; }

		public ProductUnit()
		{
			Unit = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}