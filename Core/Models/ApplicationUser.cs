using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(CPF), IsUnique = true)]
	public class ApplicationUser : IdentityUser
	{
		[Required]
		[DataMember]
		public string CPF { get; set; }

		[Required]
		[NotMapped]
		public string Password { get; set; }

		[DataMember]
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