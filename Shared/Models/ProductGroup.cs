using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class ProductGroup
	{
		[Column("ProductGroupId")]
		public int Id { get; set; }

		[DataMember]
		public string Header { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public Budget Budget { get; set; }
		public int BudgetId { get; set; }

		[DataMember]
		public IList<ProductGroupDetail>? Products { get; set; }

		public ProductGroup()
		{
			Header = string.Empty;
			Budget = new Budget();
			CreatedOn = DateTime.UtcNow;
		}
	}
}