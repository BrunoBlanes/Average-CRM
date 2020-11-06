using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CRM.Core.Attributes;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(CPF), IsUnique = true)]
	public class ApplicationUser : IdentityUser
	{
		[CpfValidation]
		[Display(Prompt = "CPF")]
		[StringLength(14, ErrorMessage = "The {0} must be exactly 11 characters long.", MinimumLength = 14)]
		public string CPF { get; set; }

		[NotMapped]
		[DataType(DataType.Password)]
		[Display(Prompt = "Password")]
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
			CPF = string.Empty;
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
}