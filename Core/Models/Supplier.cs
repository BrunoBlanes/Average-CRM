using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Core.Models
{
	[Table("Suppliers")]
	public class Supplier : Contact
	{
		public ICollection<Product>? Products { get; set; }
	}
}