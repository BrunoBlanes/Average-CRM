using CRM.Core.Models;

using IdentityServer4.EntityFramework.Options;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRM.Server
{
	public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
	{
		public DbSet<Address> Addresses { get; set; } = null!;
		public DbSet<Budget> Budgets { get; set; } = null!;
		public DbSet<BudgetStatus> BudgetStatuses { get; set; } = null!;
		public DbSet<Company> Companies { get; set; } = null!;
		public DbSet<Person> People { get; set; } = null!;
		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<ProductBrand> ProductBrands { get; set; } = null!;
		public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
		public DbSet<ProductGroup> ProductGroups { get; set; } = null!;
		public DbSet<ProductUnit> ProductUnits { get; set; } = null!;
		public DbSet<Supplier> Suppliers { get; set; } = null!;
		public DbSet<EmailSettings> EmailSettings { get; set; } = null!;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
			IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.HasSequence<int>("BudgetSequence", schema: "dbo").StartsAt(10000).IncrementsBy(1);
			modelBuilder.Entity<Budget>(budget => budget.Property(x => x.Number).HasDefaultValueSql("NEXT VALUE FOR [dbo].[BudgetSequence]"));
		}
	}
}