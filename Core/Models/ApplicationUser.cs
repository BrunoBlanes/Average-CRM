using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using CRM.Core.Attributes;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Core.Models
{
	[Index(nameof(CPF), IsUnique = true)]
	public class ApplicationUser : IdentityUser
	{
		[Required]
		[CpfValidation]
		public string CPF { get; set; }

		[DataMember]
		public ICollection<Budget>? Budgets { get; set; }

		public ApplicationUser()
		{
			CPF = string.Empty;
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