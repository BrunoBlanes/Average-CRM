using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class DownloadPersonalDataModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<DownloadPersonalDataModel> logger;

		public DownloadPersonalDataModel(
			UserManager<ApplicationUser> userManager,
			ILogger<DownloadPersonalDataModel> logger)
		{
			this.userManager = userManager;
			this.logger = logger;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userManager.GetUserId(User));

			// Only include personal data for download
			var personalData = new Dictionary<string, string>();
			IEnumerable<System.Reflection.PropertyInfo>? personalDataProps = typeof(ApplicationUser).GetProperties().Where(
							prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
			foreach (System.Reflection.PropertyInfo? p in personalDataProps)
			{
				personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
			}

			IList<UserLoginInfo>? logins = await userManager.GetLoginsAsync(user);
			foreach (UserLoginInfo? l in logins)
			{
				personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
			}

			Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
			return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
		}
	}
}
