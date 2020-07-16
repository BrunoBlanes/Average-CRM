using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Core.Models
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
		public int StatusId { get; set; }

		[DataMember]
		public Contact Client { get; set; }
		public int ClientId { get; set; }

		[DataMember]
		public ApplicationUser SalesRep { get; set; }
		public string SalesRepId { get; set; }

		[DataMember]
		public ApplicationUser? PricingRep { get; set; }
		public string? PricingRepId { get; set; }

		[DataMember]
		public IList<ProductGroup>? Groups { get; set; }

		public Budget()
		{
			Name = string.Empty;
			SalesRepId = string.Empty;
			CreatedOn = DateTime.UtcNow;
			Client = new Contact();
			Status = new BudgetStatus();
			SalesRep = new ApplicationUser();
		}
	}
}