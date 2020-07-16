namespace CRM.Core.Models
{
	public class ProductSupplierDetail
	{
		public Product Product { get; set; }
		public int ProductId { get; set; }
		public Supplier Supplier { get; set; }
		public int SupplierId { get; set; }

		public ProductSupplierDetail()
		{
			Product = new Product();
			Supplier = new Supplier();
		}
	}
}
