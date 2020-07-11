using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Server.Migrations
{
	public partial class Initial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.EnsureSchema(
				name: "dbo");

			migrationBuilder.CreateSequence<int>(
				name: "BudgetSequence",
				schema: "dbo",
				startValue: 10000L);

			migrationBuilder.CreateTable(
				name: "Address",
				columns: table => new
				{
					AddressId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Latitude = table.Column<decimal>(type: "DECIMAL(10,7)", nullable: false),
					Longitude = table.Column<decimal>(type: "DECIMAL(10,7)", nullable: false),
					Destination = table.Column<string>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Address", x => x.AddressId);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<string>(nullable: false),
					Name = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<string>(nullable: false),
					UserName = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
					Email = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(nullable: false),
					PasswordHash = table.Column<string>(nullable: true),
					SecurityStamp = table.Column<string>(nullable: true),
					ConcurrencyStamp = table.Column<string>(nullable: true),
					PhoneNumber = table.Column<string>(nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(nullable: false),
					TwoFactorEnabled = table.Column<bool>(nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
					LockoutEnabled = table.Column<bool>(nullable: false),
					AccessFailedCount = table.Column<int>(nullable: false),
					CPF = table.Column<string>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "BudgetStatus",
				columns: table => new
				{
					BudgetStatusId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Status = table.Column<string>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_BudgetStatus", x => x.BudgetStatusId);
				});

			migrationBuilder.CreateTable(
				name: "ClientType",
				columns: table => new
				{
					ClientTypeId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Type = table.Column<string>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ClientType", x => x.ClientTypeId);
				});

			migrationBuilder.CreateTable(
				name: "ProductBrand",
				columns: table => new
				{
					ProductBrandId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Brand = table.Column<string>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductBrand", x => x.ProductBrandId);
				});

			migrationBuilder.CreateTable(
				name: "ProductCategory",
				columns: table => new
				{
					ProductCategoryId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Category = table.Column<string>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductCategory", x => x.ProductCategoryId);
				});

			migrationBuilder.CreateTable(
				name: "ProductUnit",
				columns: table => new
				{
					ProductUnitId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Unit = table.Column<string>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductUnit", x => x.ProductUnitId);
				});

			migrationBuilder.CreateTable(
				name: "Contact",
				columns: table => new
				{
					ContactId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Email = table.Column<string>(nullable: false),
					Phone = table.Column<string>(nullable: true),
					CreatedOn = table.Column<DateTime>(nullable: false),
					AddressId = table.Column<int>(nullable: true),
					Discriminator = table.Column<string>(nullable: false),
					LegalName = table.Column<string>(nullable: true),
					FantasyName = table.Column<string>(nullable: true),
					CNPJ = table.Column<string>(nullable: true),
					IE = table.Column<string>(nullable: true),
					IM = table.Column<string>(nullable: true),
					FirstName = table.Column<string>(nullable: true),
					LastName = table.Column<string>(nullable: true),
					CPF = table.Column<string>(nullable: true),
					RG = table.Column<string>(nullable: true),
					CompanyId = table.Column<int>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Contact", x => x.ContactId);
					table.ForeignKey(
						name: "FK_Contact_Address_AddressId",
						column: x => x.AddressId,
						principalTable: "Address",
						principalColumn: "AddressId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Contact_Contact_CompanyId",
						column: x => x.CompanyId,
						principalTable: "Contact",
						principalColumn: "ContactId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					RoleId = table.Column<string>(nullable: false),
					ClaimType = table.Column<string>(nullable: true),
					ClaimValue = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<string>(nullable: false),
					ClaimType = table.Column<string>(nullable: true),
					ClaimValue = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
					ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
					ProviderDisplayName = table.Column<string>(nullable: true),
					UserId = table.Column<string>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<string>(nullable: false),
					RoleId = table.Column<string>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<string>(nullable: false),
					LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
					Name = table.Column<string>(maxLength: 128, nullable: false),
					Value = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ProfitMargins",
				columns: table => new
				{
					ProfitMarginId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Profit = table.Column<double>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false),
					ClientTypeId = table.Column<int>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProfitMargins", x => x.ProfitMarginId);
					table.ForeignKey(
						name: "FK_ProfitMargins_ClientType_ClientTypeId",
						column: x => x.ClientTypeId,
						principalTable: "ClientType",
						principalColumn: "ClientTypeId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Product",
				columns: table => new
				{
					ProductId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					NCM = table.Column<string>(nullable: true),
					Model = table.Column<string>(nullable: false),
					Description = table.Column<string>(nullable: false),
					BrandId = table.Column<int>(nullable: false),
					UnitId = table.Column<int>(nullable: false),
					CategoryId = table.Column<int>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Product", x => x.ProductId);
					table.ForeignKey(
						name: "FK_Product_ProductBrand_BrandId",
						column: x => x.BrandId,
						principalTable: "ProductBrand",
						principalColumn: "ProductBrandId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Product_ProductCategory_CategoryId",
						column: x => x.CategoryId,
						principalTable: "ProductCategory",
						principalColumn: "ProductCategoryId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Product_ProductUnit_UnitId",
						column: x => x.UnitId,
						principalTable: "ProductUnit",
						principalColumn: "ProductUnitId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Budget",
				columns: table => new
				{
					BudgetId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Number = table.Column<int>(nullable: false, defaultValueSql: "NEXT VALUE FOR [dbo].[BudgetSequence]"),
					Name = table.Column<string>(nullable: false),
					DueBy = table.Column<DateTime>(nullable: true),
					CreatedOn = table.Column<DateTime>(nullable: false),
					StatusId = table.Column<int>(nullable: false),
					ClientId = table.Column<int>(nullable: false),
					SalesRepId = table.Column<string>(nullable: false),
					PricingRepId = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Budget", x => x.BudgetId);
					table.ForeignKey(
						name: "FK_Budget_Contact_ClientId",
						column: x => x.ClientId,
						principalTable: "Contact",
						principalColumn: "ContactId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Budget_AspNetUsers_PricingRepId",
						column: x => x.PricingRepId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Budget_AspNetUsers_SalesRepId",
						column: x => x.SalesRepId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Budget_BudgetStatus_StatusId",
						column: x => x.StatusId,
						principalTable: "BudgetStatus",
						principalColumn: "BudgetStatusId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ProductSupplierDetail",
				columns: table => new
				{
					ProductId = table.Column<int>(nullable: false),
					SupplierId = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductSupplierDetail", x => new { x.ProductId, x.SupplierId });
					table.ForeignKey(
						name: "FK_ProductSupplierDetail_Contact_ProductId",
						column: x => x.ProductId,
						principalTable: "Contact",
						principalColumn: "ContactId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ProductSupplierDetail_Product_SupplierId",
						column: x => x.SupplierId,
						principalTable: "Product",
						principalColumn: "ProductId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ProductGroup",
				columns: table => new
				{
					ProductGroupId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Header = table.Column<string>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false),
					BudgetId = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductGroup", x => x.ProductGroupId);
					table.ForeignKey(
						name: "FK_ProductGroup_Budget_BudgetId",
						column: x => x.BudgetId,
						principalTable: "Budget",
						principalColumn: "BudgetId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ProductGroupDetail",
				columns: table => new
				{
					ProductId = table.Column<int>(nullable: false),
					ProductGroupId = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductGroupDetail", x => new { x.ProductId, x.ProductGroupId });
					table.ForeignKey(
						name: "FK_ProductGroupDetail_ProductGroup_ProductGroupId",
						column: x => x.ProductGroupId,
						principalTable: "ProductGroup",
						principalColumn: "ProductGroupId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ProductGroupDetail_Product_ProductId",
						column: x => x.ProductId,
						principalTable: "Product",
						principalColumn: "ProductId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Address_Destination",
				table: "Address",
				column: "Destination",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true,
				filter: "[NormalizedName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_CPF",
				table: "AspNetUsers",
				column: "CPF",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true,
				filter: "[NormalizedUserName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Budget_ClientId",
				table: "Budget",
				column: "ClientId");

			migrationBuilder.CreateIndex(
				name: "IX_Budget_PricingRepId",
				table: "Budget",
				column: "PricingRepId");

			migrationBuilder.CreateIndex(
				name: "IX_Budget_SalesRepId",
				table: "Budget",
				column: "SalesRepId");

			migrationBuilder.CreateIndex(
				name: "IX_Budget_StatusId",
				table: "Budget",
				column: "StatusId");

			migrationBuilder.CreateIndex(
				name: "IX_BudgetStatus_Status",
				table: "BudgetStatus",
				column: "Status",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_ClientType_Type",
				table: "ClientType",
				column: "Type",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Contact_CNPJ",
				table: "Contact",
				column: "CNPJ",
				unique: true,
				filter: "[CNPJ] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Contact_IE",
				table: "Contact",
				column: "IE",
				unique: true,
				filter: "[IE] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Contact_IM",
				table: "Contact",
				column: "IM",
				unique: true,
				filter: "[IM] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Contact_AddressId",
				table: "Contact",
				column: "AddressId");

			migrationBuilder.CreateIndex(
				name: "IX_Contact_Email",
				table: "Contact",
				column: "Email",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Contact_CPF",
				table: "Contact",
				column: "CPF",
				unique: true,
				filter: "[CPF] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Contact_CompanyId",
				table: "Contact",
				column: "CompanyId");

			migrationBuilder.CreateIndex(
				name: "IX_Contact_RG",
				table: "Contact",
				column: "RG",
				unique: true,
				filter: "[RG] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Product_BrandId",
				table: "Product",
				column: "BrandId");

			migrationBuilder.CreateIndex(
				name: "IX_Product_CategoryId",
				table: "Product",
				column: "CategoryId");

			migrationBuilder.CreateIndex(
				name: "IX_Product_Model",
				table: "Product",
				column: "Model",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Product_UnitId",
				table: "Product",
				column: "UnitId");

			migrationBuilder.CreateIndex(
				name: "IX_ProductBrand_Brand",
				table: "ProductBrand",
				column: "Brand",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_ProductCategory_Category",
				table: "ProductCategory",
				column: "Category",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_ProductGroup_BudgetId",
				table: "ProductGroup",
				column: "BudgetId");

			migrationBuilder.CreateIndex(
				name: "IX_ProductGroup_Header",
				table: "ProductGroup",
				column: "Header",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_ProductGroupDetail_ProductGroupId",
				table: "ProductGroupDetail",
				column: "ProductGroupId");

			migrationBuilder.CreateIndex(
				name: "IX_ProductSupplierDetail_SupplierId",
				table: "ProductSupplierDetail",
				column: "SupplierId");

			migrationBuilder.CreateIndex(
				name: "IX_ProductUnit_Unit",
				table: "ProductUnit",
				column: "Unit",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_ProfitMargins_ClientTypeId",
				table: "ProfitMargins",
				column: "ClientTypeId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			migrationBuilder.DropTable(
				name: "ProductGroupDetail");

			migrationBuilder.DropTable(
				name: "ProductSupplierDetail");

			migrationBuilder.DropTable(
				name: "ProfitMargins");

			migrationBuilder.DropTable(
				name: "AspNetRoles");

			migrationBuilder.DropTable(
				name: "ProductGroup");

			migrationBuilder.DropTable(
				name: "Product");

			migrationBuilder.DropTable(
				name: "ClientType");

			migrationBuilder.DropTable(
				name: "Budget");

			migrationBuilder.DropTable(
				name: "ProductBrand");

			migrationBuilder.DropTable(
				name: "ProductCategory");

			migrationBuilder.DropTable(
				name: "ProductUnit");

			migrationBuilder.DropTable(
				name: "Contact");

			migrationBuilder.DropTable(
				name: "AspNetUsers");

			migrationBuilder.DropTable(
				name: "BudgetStatus");

			migrationBuilder.DropTable(
				name: "Address");

			migrationBuilder.DropSequence(
				name: "BudgetSequence",
				schema: "dbo");
		}
	}
}
