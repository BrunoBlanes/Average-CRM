using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace CRM.Core.Models
{
	public class ApplicationUser : IdentityUser
	{
		[NotMapped]
		[Display(Prompt = "Password")]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		public string Password { get; set; }

		[NotMapped]
		[DataType(DataType.Password)]
		[Display(Prompt = "Confirm password")]
		[Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
		public string? ConfirmPassword { get; set; }

		[EmailAddress]
		[Display(Prompt = "Email")]
		public override string Email
		{
			get => base.Email;
			set => base.Email = value;
		}

		public ICollection<Budget>? Budgets { get; set; }

		public ApplicationUser()
		{
			Email = string.Empty;
			Password = string.Empty;
		}
	}

	public class UserToken
	{
		public string Token { get; set; }
		public DateTime Expiration { get; set; }

		public UserToken()
		{
			Token = string.Empty;
		}
	}

	public readonly struct Roles
	{
		/// <summary>
		/// The <c>Administrator</c> role.
		/// </summary>
		public const string Administrator = nameof(Administrator);
	}
}