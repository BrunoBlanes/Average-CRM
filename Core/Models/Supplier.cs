using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CRM.Core.Models
{
	[DataContract(IsReference = true)]
	public class Supplier : Contact
	{
		[DataMember]
		public IList<ProductSupplierDetail>? Products { get; set; }
	}
}