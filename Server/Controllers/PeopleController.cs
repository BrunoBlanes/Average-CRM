using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Server.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("[controller]")]
	[Produces("application/json")]
	public class PeopleController : Controller
	{
		private readonly ApplicationDbContext context;
		public PeopleController(ApplicationDbContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Lists all the people registered.
		/// </summary>
		/// <response code="200">Returns a list of people.</response>
		/// <response code="204">If the list is empty.</response> 
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<List<Person>>> OnListAsync()
		{
			return await context.People.Include(x => x.Address).ToListAsync() is List<Person> people && people.Any()
				? people
				: (ActionResult<List<Person>>)NoContent();
		}


		/// <summary>
		/// Gets a person by its <paramref name="id"/>.
		/// </summary>
		/// <param name="id">The user <paramref name="id"/>.</param>
		[HttpGet("{id:int}")]
		public async Task<ActionResult<Person>> OnGetAsync(int id)
		{
			return await context.People.Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == id) is Person individual
				? individual
				: (ActionResult<Person>)NoContent();
		}


		/// <summary>
		/// Gets a person by its <paramref name="email"/>.
		/// </summary>
		/// <param name="email">The user <paramref name="email"/>.</param>
		[HttpGet("Email={email}")]
		public async Task<ActionResult<Person>> OnGetAsync([EmailAddress] string email)
		{
			return await context.People.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email) is Person individual
				? individual
				: (ActionResult<Person>)NoContent();
		}


		/// <summary>
		/// Gets a person by its <paramref name="cpf"/>.
		/// </summary>
		/// <param name="cpf">The user <paramref name="cpf"/>.</param>
		[HttpGet("Cpf={cpf}")]
		public async Task<ActionResult<Person>> OnGetByCpfAsync(string cpf)
		{
			return await context.People.Include(x => x.Address).FirstOrDefaultAsync(x => x.CPF == cpf) is Person person
				? person
				: (ActionResult<Person>)NoContent();
		}

		[HttpPost]
		public async Task<ActionResult<Person>> OnPostAsync([FromBody] Person person)
		{
			if (!ModelState.IsValid || person is null)
			{
				return BadRequest(ModelState);
			}

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