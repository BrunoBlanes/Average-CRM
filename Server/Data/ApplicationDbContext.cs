using CRM.Shared.Models;

using IdentityServer4.EntityFramework.Options;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System;

namespace CRM.Server
{
#pragma warning disable CS8603 // Possible null reference return.
	public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
	{
		public DbSet<Address> Addresses { get; set; } = null!;
		public DbSet<Budget> Budgets { get; set; } = null!;
		public DbSet<BudgetStatus> BudgetStatuses { get; set; } = null!;
		public DbSet<ClientType> ClientTypes { get; set; } = null!;
		public DbSet<Company> Companies { get; set; } = null!;
		public DbSet<Person> People { get; set; } = null!;
		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<ProductBrand> ProductBrands { get; set; } = null!;
		public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
		public DbSet<ProductGroup> ProductGroups { get; set; } = null!;
		public DbSet<ProductUnit> ProductUnits { get; set; } = null!;
		public DbSet<ProfitMargin> ProfitMargins { get; set; } = null!;
		public DbSet<Supplier> Suppliers { get; set; } = null!;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
			IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));
			base.OnModelCreating(modelBuilder);
			modelBuilder.HasSequence<int>("BudgetSequence", schema: "dbo").StartsAt(10000).IncrementsBy(1);

			modelBuilder.Entity<ApplicationUser>(user =>
			{
				user.HasIndex(x => x.CPF).IsUnique();
				user.HasMany(x => x.SalesRepBudgets).WithOne(x => x.SalesRep).HasForeignKey(x => x.SalesRepId);
				user.HasMany(x => x.PricingRepBudgets).WithOne(x => x.PricingRep).HasForeignKey(x => x.PricingRepId);
			});

			modelBuilder.Entity<Contact>(contact =>
			{
				contact.ToTable("Contact");
				contact.HasIndex(x => x.Email).IsUnique();
				contact.Property(x => x.Id).UseIdentityColumn();
				contact.HasMany(x => x.Budgets).WithOne(x => x.Client).HasForeignKey(x => x.ClientId);
			});

			modelBuilder.Entity<Person>(person =>
			{
				person.HasBaseType<Contact>();
				person.HasIndex(x => x.RG).IsUnique();
				person.HasIndex(x => x.CPF).IsUnique();
			});

			modelBuilder.Entity<Company>(company =>
			{
				company.HasBaseType<Contact>();
				company.HasIndex(x => x.IE).IsUnique();
				company.HasIndex(x => x.IM).IsUnique();
				company.HasIndex(x => x.CNPJ).IsUnique();
			});

			modelBuilder.Entity<Supplier>(supplier => supplier.HasBaseType<Contact>());

			modelBuilder.Entity<ClientType>(clientType =>
			{
				clientType.ToTable("ClientType");
				clientType.HasIndex(x => x.Type).IsUnique();
			});

			modelBuilder.Entity<Address>(address =>
			{
				address.ToTable("Address");
				address.HasIndex(x => x.Destination).IsUnique();
				address.Property(x => x.Latitude).HasColumnType("DECIMAL(10,7)");
				address.Property(x => x.Longitude).HasColumnType("DECIMAL(10,7)");
				address.HasMany(x => x.Contacts).WithOne(x => x.Address).HasForeignKey(x => x.AddressId);
			});

			modelBuilder.Entity<Product>(product =>
			{
				product.ToTable("Product");
				product.HasIndex(x => x.Model).IsUnique();
			});

			modelBuilder.Entity<ProductUnit>(productUnit =>
			{
				productUnit.ToTable("ProductUnit");
				productUnit.HasIndex(x => x.Unit).IsUnique();
			});

			modelBuilder.Entity<ProductBrand>(productBrand =>
			{
				productBrand.ToTable("ProductBrand");
				productBrand.HasIndex(x => x.Brand).IsUnique();
			});

			modelBuilder.Entity<ProductCategory>(productCategory =>
			{
				productCategory.ToTable("ProductCategory");
				productCategory.HasIndex(x => x.Category).IsUnique();
			});

			modelBuilder.Entity<ProductGroup>(productGroup =>
			{
				productGroup.ToTable("ProductGroup");
				productGroup.HasIndex(x => x.Header).IsUnique();
				productGroup.HasOne(x => x.Budget).WithMany(x => x.Groups).HasForeignKey(x => x.BudgetId);
			});

			modelBuilder.Entity<ProductGroupDetail>(productGroupDetail =>
			{
				productGroupDetail.HasKey(x => new { x.ProductId, x.ProductGroupId });
				productGroupDetail.HasOne(x => x.Product).WithMany(x => x.Groups).HasForeignKey(x => x.ProductId);
				productGroupDetail.HasOne(x => x.ProductGroup).WithMany(x => x.Products).HasForeignKey(x => x.ProductGroupId);
			});

			modelBuilder.Entity<ProductSupplierDetail>(productSupplierDetail =>
			{
				productSupplierDetail.HasKey(x => new { x.ProductId, x.SupplierId });
				productSupplierDetail.HasOne(x => x.Supplier).WithMany(x => x.Products).HasForeignKey(x => x.ProductId);
				productSupplierDetail.HasOne(x => x.Product).WithMany(x => x.Suppliers).HasForeignKey(x => x.SupplierId);
			});

			modelBuilder.Entity<Budget>(budget =>
			{
				budget.ToTable("Budget");
				budget.Property(x => x.Number).HasDefaultValueSql("NEXT VALUE FOR [dbo].[BudgetSequence]");
			});

			modelBuilder.Entity<BudgetStatus>(budgetStatus =>
			{
				budgetStatus.ToTable("BudgetStatus");
				budgetStatus.HasIndex(x => x.Status).IsUnique();
			});
		}
	}
}
