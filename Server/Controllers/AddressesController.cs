using CRM.Shared.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AddressesController : Controller
	{
		private readonly AppDbContext context;

		public AddressesController(AppDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<List<Address>>> OnGetAsync()
		{
			return await context.Addresses.ToListAsync() is List<Address> addresses && addresses.Any()
				? addresses
				: (ActionResult<List<Address>>)NoContent();
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<Address>> OnGetAsync(int id)
		{
			return await context.Addresses.FindAsync(id) is Address address
				? address
				: (ActionResult<Address>)NoContent();
		}

		[HttpPost]
		public async Task<ActionResult> OnPostAsync([FromBody] Address address)
		{
			if (!ModelState.IsValid || address is null) return BadRequest(ModelState);
			context.Addresses.Add(address);
			await context.SaveChangesAsync();
			return Created(new Uri(@$"{Request.Scheme}://{Request.Host}{Request.Path}/{address.Id}"), address);
		}
	}
}