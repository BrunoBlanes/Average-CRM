using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class Supplier : Contact
	{
		[DataMember]
		public IList<ProductSupplierDetail>? Products { get; set; }
	}
}
