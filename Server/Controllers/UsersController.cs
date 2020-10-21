using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using CRM.Core.Models;
using CRM.Server.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly ILogger logger;
		private readonly EmailSender emailSender;
		private readonly ApplicationDbContext context;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		public UsersController(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger<UsersController> logger,
			ApplicationDbContext context,
			EmailSender emailSender)
		{
			this.logger = logger;
			this.context = context;
			this.emailSender = emailSender;
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		[HttpGet]
		public async Task<ActionResult<IList<ApplicationUser>>> OnGetAsync()
		{
			return await context.Users.ToListAsync() is List<ApplicationUser> addresses && addresses.Any()
				? addresses
				: (ActionResult<IList<ApplicationUser>>)NoContent();
		}


		//[Authorize]
		[HttpPost("Create")]
		public async Task<ActionResult<UserToken>> CreateUserAsync([FromBody] ApplicationUser user)
		{
			if (!ModelState.IsValid || user is null)
				return BadRequest(ModelState);
			IdentityResult result = await userManager.CreateAsync(user, user.Password);

			if (result.Succeeded)
			{
				logger.LogInformation($"User {user.NormalizedUserName} account was created.");
				string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
				token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
				SendConfirmationMail(token);
				return Created(new Uri(@$"{Request.Scheme}://{Request.Host}{Request.Path}/{user.Id}"), user);
				//return RedirectToPage("RegisterConfirmation", new { email = user.Email });
			}

			async void SendConfirmationMail(string token)
			{
				string? callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new
				{
					area = "Identity",
					userId = user.Id,
					token
				}, protocol: Request.Scheme);

				await emailSender.SendEmailAsync(user.Email,
					"Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
			}

			foreach (IdentityError? error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return BadRequest("Username or password invalid");
		}

		[HttpPost("Login")]
		public async Task<ActionResult<UserToken>> LoginAsync([FromBody] ApplicationUser user)
		{
			if (!ModelState.IsValid || user is null)
				return BadRequest(ModelState);
			Microsoft.AspNetCore.Identity.SignInResult? result = await signInManager.PasswordSignInAsync(user.Email, user.Password, true, false);

			if (result.Succeeded)
			{
				logger.LogInformation($"User {user.NormalizedUserName} logged in.");
			}

			if (result.RequiresTwoFactor)
			{
				return RedirectToPage("./LoginWith2fa", new
				{
					//ReturnUrl = returnUrl,
					RememberMe = true
				});
			}

			if (result.IsLockedOut)
			{
				logger.LogWarning($"User account {user.NormalizedUserName} is locked out.");
				return RedirectToPage("./Lockout");
			}

			else
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return BadRequest(ModelState);
			}
		}
	}
}
