using System;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SettingsController : Controller
	{
		private readonly ApplicationDbContext context;

		public SettingsController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<Setting>> OnGetAsync()
		{
			return await context.Settings.FindAsync(1) is Setting settings
				? settings
				: (ActionResult<Setting>)NoContent();
		}

		[HttpPost]
		public async Task<ActionResult> OnPostAsync([FromBody] Setting settings)
		{
			if (ModelState.IsValid is false || settings is null)
			{
				return BadRequest(ModelState);
			}

			if (await context.Settings.FindAsync(1) is Setting oldSettings)
			{
				oldSettings.FirstRun = settings.FirstRun;
				oldSettings.EmailSettings.Name = settings.EmailSettings.Name;
				oldSettings.EmailSettings.Port = settings.EmailSettings.Port;
				oldSettings.EmailSettings.Login = settings.EmailSettings.Login;
				oldSettings.EmailSettings.Server = settings.EmailSettings.Server;
				oldSettings.EmailSettings.Address = settings.EmailSettings.Address;
				oldSettings.EmailSettings.Password = settings.EmailSettings.Password;
				context.Settings.Update(oldSettings);
				await context.SaveChangesAsync();
			}

			else
			{
				settings.Id = 1;
				context.Settings.Add(settings);
				await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Settings ON");
				await context.SaveChangesAsync();
				await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Settings OFF");
			}

			return Created(new Uri(@$"{Request.Scheme}://{Request.Host}{Request.Path}/1"), settings);
		}
	}
}