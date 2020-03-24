using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class ClientType
	{
		[DataMember]
		[Column("ClientTypeId")]
		public int Id { get; set; }

		[Required]
		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public IList<ProfitMargin>? ProfitMargins { get; }

		public ClientType()
		{
			Type = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}