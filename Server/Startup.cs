using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using CRM.Server.Services;
using CRM.Shared.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CRM.Server
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// Sets the database connection string
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			// Configure identity
			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				// Password strength settings
				options.Password.RequiredLength = 8;
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedAccount = true;
				options.Password.RequireNonAlphanumeric = false;

				// Default Lockout settings
				options.Lockout.AllowedForNewUsers = false;
				options.Lockout.MaxFailedAccessAttempts = 3;
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
			}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

			// Sets identity to use JWT tokens
			services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
			services.AddAuthentication().AddIdentityServerJwt();

			// Configure json and razor pages
			services.AddMvc().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.WriteIndented = true;
				options.JsonSerializerOptions.IgnoreNullValues = true;
				options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
				options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			}).AddRazorPagesOptions(options =>
			{
				options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
				options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
			});

			// Configure identity path for cookies
			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = $"/Identity/Account/Login";
				options.LogoutPath = $"/Identity/Account/Logout";
				options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
			});

			// Configure api versioning
			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
			});

			services.AddHsts(options =>
			{
				options.Preload = true;
				options.IncludeSubDomains = true;
				options.MaxAge = TimeSpan.FromDays(60);
			});

			// Sets redirection to https
			services.AddHttpsRedirection(options =>
			{
				options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
				options.HttpsPort = 5001;
			});

			// Sets the email service
			services.Configure<AuthMessageSenderOptions>(Configuration);
			services.AddTransient<IEmailSender, EmailSender>();

			// Sets the view
			services.AddControllersWithViews();
			services.AddRazorPages();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseWebAssemblyDebugging();
			}

			else
			{
				app.UseExceptionHandler("/Error");

				// TODO: The default HSTS value is 30 days.
				// You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();

			app.UseRouting();

			app.UseIdentityServer();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToFile("index.html");
			});
		}
	}
}
