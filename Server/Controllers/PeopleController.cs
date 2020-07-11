using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Server.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("[controller]")]
	public class PeopleController : Controller
	{
		private readonly ApplicationDbContext context;
		public PeopleController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<List<Person>>> OnGetAsync()
		{
			return await context.People.Include(x => x.Address).ToListAsync() is List<Person> people && people.Any()
				? people
				: (ActionResult<List<Person>>)NoContent();
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<Person>> OnGetAsync(int id)
		{
			return await context.People.Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == id) is Person individual
				? individual
				: (ActionResult<Person>)NoContent();
		}

		[HttpGet("Email={email}")]
		public async Task<ActionResult<Person>> OnGetAsync(string email)
		{
			return await context.People.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email) is Person individual
				? individual
				: (ActionResult<Person>)NoContent();
		}

		[HttpGet("Cpf={cpf}")]
		public async Task<ActionResult<Person>> OnGetByCPFAsync(string cpf)
		{
			return await context.People.Include(x => x.Address).FirstOrDefaultAsync(x => x.CPF == cpf) is Person person
				? person
				: (ActionResult<Person>)NoContent();
		}

		[HttpPost]
		public async Task<ActionResult<Person>> OnPostAsync([FromBody] Person person)
		{
			if (!ModelState.IsValid || person is null) return BadRequest(ModelState);
			if (await context.People.FirstOrDefaultAsync(x => x.CPF == person.CPF) is null)
			{
				//TODO: Check for issues with inputing the user address
				context.People.Add(person);
				await context.SaveChangesAsync();
				return Created(new Uri(@$"{Request.Scheme}://{Request.Host}{Request.Path}/{person.Id}"), person);
			}

			return Conflict();
		}
	}
}