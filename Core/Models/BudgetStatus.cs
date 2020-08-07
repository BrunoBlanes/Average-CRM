using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	[DataContract(IsReference = true)]
	public class BudgetStatus
	{
		[DataMember]
		[Column("BudgetStatusId")]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public IList<Budget>? Budgets { get; set; }

		public BudgetStatus()
		{
			Status = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}