namespace CRM.Shared.Models
{
	public class ProductGroupDetail
	{
		public Product Product { get; set; }
		public int ProductId { get; set; }
		public ProductGroup ProductGroup { get; set; }
		public int ProductGroupId { get; set; }

		public ProductGroupDetail()
		{
			Product = new Product();
			ProductGroup = new ProductGroup();
		}
	}
}
