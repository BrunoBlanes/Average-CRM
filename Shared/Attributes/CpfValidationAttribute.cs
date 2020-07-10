using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace CRM.Shared.Attributes
{
	public class CpfValidationAttribute : ValidationAttribute
	{
		private readonly Regex regex = new Regex(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$");

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var cpf = value?.ToString();
			if (string.IsNullOrEmpty(cpf))
				return new ValidationResult("The field CPF cannot be empty.");

			if (regex.IsMatch(cpf))
			{
				// Remove dots and dash from string
				cpf = cpf.Replace(".", "", StringComparison.OrdinalIgnoreCase)
					.Replace("-", "", StringComparison.OrdinalIgnoreCase);
				double firstDigit = 0;
				double secondDigit = 0;

				// Something went wrong
				if (cpf.Length != 11)
					throw new FormatException($"CPF {cpf} is not in a valid format.");

				// Repeated digits passes the algorithm, but are still invalid
				for (int i = 1; i < cpf.Length; i++)
				{
					if (cpf[i] != cpf[0])
						break;
					else if (i == cpf.Length - 1)
						return InvalidCPF();
				}

				// First digit verification
				for (int i = 10; i >= 2; i--)
					firstDigit += char.GetNumericValue(cpf[10 - i]) * i;
				DigitCheck(ref firstDigit);
				if (firstDigit != char.GetNumericValue(cpf[9]))
					return InvalidCPF();

				// Second digit verification
				for (int i = 11; i >= 2; i--)
					secondDigit += char.GetNumericValue(cpf[11 - i]) * i;
				DigitCheck(ref secondDigit);
				if (secondDigit != char.GetNumericValue(cpf[10]))
					return InvalidCPF();

				// If we've made this far, then we've passed validation
				return ValidationResult.Success;

				// Common final multiplication
				static void DigitCheck(ref double product)
				{
					product = product * 10 % 11;
					if (product == 10) product = 0;
				}

				static ValidationResult InvalidCPF()
				{
					return new ValidationResult("Invalid CPF.");
				}
			}

			return new ValidationResult("Please insert oly digits.");
		}
	}

	// TODO: Add CNPJ validation
	public class CnpjValidationAttribute : ValidationAttribute
	{
		private readonly Regex regex = new Regex(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$");

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (string.IsNullOrEmpty(value?.ToString())) return new ValidationResult("O campo CPF não pode estar vazio.");

			if (regex.IsMatch(value.ToString()))
			{
				return ValidationResult.Success;
			}

			return new ValidationResult("Apenas são permitidos números.");
		}
	}
}