using CRM.Shared.Interfaces;

using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CRM.Shared.Models
{
	[DataContract(IsReference = true)]
	public class User : IdentityUser, ISqlObject
	{
		[Required]
		[DataMember]
		public string CPF { get; set; }

		[Required]
		[NotMapped]
		public string Password { get; set; }

		[DataMember]
		public IList<Budget>? SalesRepBudgets { get; set; }

		[DataMember]
		public IList<Budget>? PricingRepBudgets { get; set; }

		public User()
		{
			CPF = string.Empty;
			Password = string.Empty;
		}

		public User(string email, string password, string cpf)
		{
			CPF = cpf;
			Email = email;
			UserName = email;
			Password = password;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public async Task RegisterAsync()
		{
			await App.WebApi.PostAsync(this, "Create").ConfigureAwait(false);
		}
	}
}