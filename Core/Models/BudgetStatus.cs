using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	public class BudgetStatus
	{
		[DataMember]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public ICollection<Budget>? Budgets { get; set; }

		public BudgetStatus()
		{
			Status = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}