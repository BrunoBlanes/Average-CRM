using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using CRM.Core.Models;
using CRM.Server.Extensions;
using CRM.Server.Interfaces;
using CRM.Server.Services;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CRM.Server
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
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
				options.Conventions.AuthorizeAreaPage("account", "/logout");
				options.Conventions.AuthorizeAreaFolder("account", "/manage");
			});

			// Configure identity path for cookies
			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/account/login";
				options.LogoutPath = "/account/logout";
				options.AccessDeniedPath = "/account/accessdenied";
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

			// TODO: Fix httpclient to get current domain
			services.AddHttpClient("ServerAPI", client =>
			{
				client.BaseAddress = new Uri($"{Configuration["BaseUrl"]}api/");
			});

			// Sets the email service
			services.AddSingleton<ISmtpService, SmtpService>();

			// See https://github.com/aspnet/Announcements/issues/432
			services.AddDatabaseDeveloperPageExceptionFilter();

			// Configure options
			services.ConfigureWritable<SmtpOptions>(Configuration.GetSection(SmtpOptions.Section));

			// Adds the FAST based Fluent HTML generator
			services.AddScoped<IHtmlGenerator, FastGenerator>();

			// Sets the view
			services.AddControllersWithViews();
			services.AddServerSideBlazor();
			services.AddRazorPages();
			services.AddHttpClient();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseWebAssemblyDebugging();
				app.UseMigrationsEndPoint();
			}

			else
			{
				app.UseExceptionHandler("/error");

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
				endpoints.MapBlazorHub();
				endpoints.MapRazorPages();
				endpoints.MapFallbackToFile("index.html");
				endpoints.MapControllerRoute("default", "api/{controller=name}/{action=Index}");
			});
		}
	}
}
