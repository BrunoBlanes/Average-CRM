using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	public class Budget
	{
		[DataMember]
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
		public ICollection<ApplicationUser> Users { get; set; }

		[DataMember]
		public ICollection<ProductGroup>? Groups { get; set; }

		public Budget()
		{
			Name = string.Empty;
			Client = new Contact();
			Status = new BudgetStatus();
			CreatedOn = DateTime.UtcNow;
			Users = new Collection<ApplicationUser>();
		}
	}
}