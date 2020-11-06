using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public UsersController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<ICollection<ApplicationUser>>> OnGetAsync()
		{
			return await context.Users.ToListAsync() is List<ApplicationUser> addresses && addresses.Any()
				? addresses
				: (ActionResult<ICollection<ApplicationUser>>)NoContent();
		}
	}
}