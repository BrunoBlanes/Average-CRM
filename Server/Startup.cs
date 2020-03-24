using CRM.Shared.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRM.Server
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

			services.AddDefaultIdentity<ApplicationUser>(options =>
			{
				options.Password.RequiredLength = 8;
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedAccount = true;
				options.Password.RequireNonAlphanumeric = false;
				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
			}).AddEntityFrameworkStores<AppDbContext>();

			services.AddIdentityServer().AddApiAuthorization<ApplicationUser, AppDbContext>();
			services.AddAuthentication().AddIdentityServerJwt();

			services.AddMvc().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.WriteIndented = true;
				options.JsonSerializerOptions.IgnoreNullValues = true;
				options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				options.JsonSerializerOptions.ReferenceHandling = ReferenceHandling.Preserve;
				options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			});

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

			services.AddHttpsRedirection(options =>
			{
				options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
				options.HttpsPort = 5001;
			});

			//services.Configure<AuthMessageSenderOptions>(configuration);
			//services.AddTransient<EmailSender>();
			services.AddControllersWithViews();
			services.AddRazorPages();
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
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
				app.UseHttpsRedirection();

				// TODO: The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseStaticFiles();
			app.UseBlazorFrameworkFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseIdentityServer();
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
