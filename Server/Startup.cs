using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using CRM.Core.Models;
using CRM.Server.Extensions;
using CRM.Server.Interfaces;
using CRM.Server.Models;
using CRM.Server.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace CRM.Server
{
	public class Startup
	{
		private readonly IConfiguration configuration;
		private readonly string baseUrl;
		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
			baseUrl = this.configuration["Application:BaseUrl"];
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// Sets the database connection string
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
			});

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

			// Adds identity server and the client's settings
			services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
			{
				options.Clients.AddSPA("CRM.Client", client =>
				{
					client.WithRedirectUri($"{baseUrl}account/login-callback");
					client.WithLogoutRedirectUri($"{baseUrl}account/logout-callback");
				});
			});

			services.AddAuthentication().AddIdentityServerJwt();

			// Configure json and razor pages
			services.AddMvc().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.WriteIndented = true;
				options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
				options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
				options.AccessDeniedPath = "/accessdenied";
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
				client.BaseAddress = new Uri($"{baseUrl}api/");
			});

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "CRM API",
					Description = "A simple example ASP.NET Core Web API",
					TermsOfService = new Uri($"{baseUrl}terms"),
					Contact = new OpenApiContact
					{
						Name = "Bruno Blanes",
						Email = "bruno.blanes@outlook.com",
						Url = new Uri($"{baseUrl}contact"),
					},
					License = new OpenApiLicense
					{
						Name = "Use under LICX",
						Url = new Uri($"{baseUrl}license"),
					}
				});

				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				options.IncludeXmlComments(xmlPath);
			});

			// Sets the email service
			services.AddSingleton<ISmtpService, SmtpService>();

			// See https://github.com/aspnet/Announcements/issues/432
			services.AddDatabaseDeveloperPageExceptionFilter();

			// Configure writable appsettings
			services.ConfigureWritable<Smtp>(configuration.GetSection(Smtp.Section));
			services.ConfigureWritable<Application>(configuration.GetSection(Application.Section));

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

				// Enable middleware to serve generated Swagger as a JSON endpoint.
				app.UseSwagger();

				// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
				// specifying the Swagger JSON endpoint.
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/api/v1/swagger.json", "CRM API V1");
					options.DefaultModelRendering(ModelRendering.Model);
					options.DisplayRequestDuration();
					options.RoutePrefix = "api";
					options.EnableDeepLinking();
					options.EnableValidator();
					options.EnableFilter();
				});
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
				endpoints.MapSwagger("api/{documentName}/swagger.json");
				endpoints.MapControllerRoute("default", "api/{controller=name}/{action=Index}");
			});
		}
	}
}
