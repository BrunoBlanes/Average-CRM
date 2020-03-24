using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class ProfitMargin
	{
		[DataMember]
		[Column("ProfitMarginId")]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public double Profit { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		public ProfitMargin()
		{
			CreatedOn = DateTime.UtcNow;
		}
	}
}