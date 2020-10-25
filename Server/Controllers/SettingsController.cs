using System;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Server.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	public class SettingsController : Controller
	{
		private readonly ApplicationDbContext context;

		public SettingsController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<Settings>> OnGetAsync()
		{
			return await context.Settings
				.Include(x => x.EmailSettings)
				.FirstOrDefaultAsync() is Settings settings
				? settings
				: (ActionResult<Settings>)NoContent();
		}

		[HttpPost]
		public async Task<ActionResult> OnPostAsync([FromBody] Settings settings)
		{
			if (ModelState.IsValid is false || settings is null)
			{
				return BadRequest(ModelState);
			}

			if (await context.Settings.FirstOrDefaultAsync() is Settings oldSettings)
			{
				oldSettings.FirstRun = settings.FirstRun;
				oldSettings.EmailSettings = settings.EmailSettings;
				context.Settings.Update(oldSettings);
				await context.SaveChangesAsync();
				return Ok(oldSettings);
			}

			context.Settings.Add(settings);
			await context.SaveChangesAsync();
			return Created(new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}/{settings.Id}"), settings);
		}
	}
}