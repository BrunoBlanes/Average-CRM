using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(Header), IsUnique = true)]
	public class ProductGroup
	{
		public int Id { get; set; }

		[DataMember]
		public string Header { get; set; }

		[DataMember]
		public DateTime CreatedOn { get; set; }

		[DataMember]
		public ICollection<Product>? Products { get; set; }

		public ProductGroup()
		{
			Header = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}
	}
}