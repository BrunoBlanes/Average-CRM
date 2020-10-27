using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Server.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	public class EmailSettingsController : Controller
	{
		private readonly ApplicationDbContext context;

		public EmailSettingsController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<EmailSettings>> OnGetAsync()
		{
			return await context.EmailSettings.SingleOrDefaultAsync() is EmailSettings settings
				? settings
				: (ActionResult<EmailSettings>)NoContent();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync([FromForm] EmailSettings settings)
		{
			if (settings.Address is null)
			{
				settings.Address = settings.Login;
			}

			if (settings.Name is null)
			{
				settings.Name = "CRM Server";
			}

			using var passwordHasher = new PasswordHasher();
			settings.Password = passwordHasher.Encrypt(settings.Password, settings);

			if (await context.EmailSettings.SingleOrDefaultAsync() is EmailSettings oldSettings)
			{
				oldSettings.Name = settings.Name;
				context.EmailSettings.Update(oldSettings);
			}

			else
			{
				context.EmailSettings.Add(settings);
			}

			await context.SaveChangesAsync();
			return RedirectToPage("~/");
		}
	}
}