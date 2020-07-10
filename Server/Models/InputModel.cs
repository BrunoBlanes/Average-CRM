using System.ComponentModel.DataAnnotations;
using CRM.Shared.Attributes;

namespace CRM.Server.Models
{
	public class InputModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[CpfValidation]
		[StringLength(14, ErrorMessage = "The {0} must be exactly 11 characters long.", MinimumLength = 14)]
		public string CPF { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string? Code { get; set; }

		public InputModel()
		{
			CPF = string.Empty;
			Email = string.Empty;
			Password = string.Empty;
			ConfirmPassword = string.Empty;
		}
	}
}