using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class Budget
	{
		[DataMember]
		[Column("BudgetId")]
		public int Id { get; set; }

		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DateTime? DueBy { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public BudgetStatus Status { get; set; }

		[DataMember]
		public Contact Client { get; set; }

		[DataMember]
		public User SalesRep { get; set; }

		[DataMember]
		public User? PricingRep { get; set; }

		[DataMember]
		public IList<ProductGroup>? Groups { get; set; }

		public Budget()
		{
			Name = string.Empty;
			Client = new Contact();
			CreatedOn = DateTime.UtcNow;
			Status = new BudgetStatus();
			SalesRep = new User();
		}
	}
}