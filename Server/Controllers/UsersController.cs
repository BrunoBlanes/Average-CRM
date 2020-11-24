using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = Roles.Administrator)]
	public class UsersController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<ApplicationUser> logger;
		private readonly ApplicationDbContext context;
		private readonly ISmtpService smtpService;

		public UsersController(ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			ILogger<ApplicationUser> logger,
			ISmtpService smtpService)
		{
			this.logger = logger;
			this.context = context;
			this.smtpService = smtpService;
			this.userManager = userManager;
		}

		[HttpGet]
		public async Task<ActionResult<ICollection<ApplicationUser>>> OnGetAsync()
		{
			return await context.Users.ToListAsync() is List<ApplicationUser> addresses && addresses.Any()
				? addresses
				: (ActionResult<ICollection<ApplicationUser>>)NoContent();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Consumes("application/x-www-form-urlencoded")]
		public async Task<IActionResult> OnPostFromFormAsync([FromForm] ApplicationUser user)
		{
			if (ModelState.IsValid is false || user is null)
			{
				return BadRequest(ModelState);
			}

			return await OnPostAsync(user);
		}

		[HttpPost]
		[Consumes("application/json")]
		public async Task<IActionResult> OnPostAsync([FromBody] ApplicationUser user)
		{
			if (ModelState.IsValid is false || user is null)
			{
				return BadRequest(ModelState);
			}

			user.UserName = user.Email;
			var returnUrl = Url.Content("~/");
			IdentityResult result = await userManager.CreateAsync(user, user.Password);

			if (result.Succeeded)
			{
				// Generates the user account confirmation code
				logger.LogInformation($"User {user.Email} created a new account with password.");
				var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page("/account/emailconfirmation", null, new
				{
					area = "identity",
					userId = user.Id,
					code,
					returnUrl
				}, Request.Scheme);

				// Sends a confirmation email to the user
				await smtpService.SendAccountConfirmationEmailAsync(callbackUrl, user);

				// Redirect to the account confirmation page
				return Created(new Uri(@$"{Request.Scheme}://{Request.Host}{Request.Path}/{user.Id}"), user);
			}

			foreach (IdentityError? error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return BadRequest(ModelState);
		}
	}
}