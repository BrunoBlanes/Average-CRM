using CRM.TagHelpers.Enums;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CRM.TagHelpers.Models
{
	public class CheckBoxFastElement : InputTagHelper
	{
		protected const string ForAttributeName = "asp-for";

		/// <summary>
		/// Gets the <see cref="FastGenerator"/> used to generate the <see cref="FastTextFieldTagHelper"/>'s output.
		/// </summary>
		protected new FastGenerator Generator { get; }

		/// <summary>
		/// The <see cref="Enums.DesignSystem"/> implementation.
		/// </summary>
		public DesignSystem DesignSystem { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="CheckBoxFastElement"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public CheckBoxFastElement(IHtmlGenerator generator) : base(generator)
		{
			Generator = (FastGenerator)generator;
		}
	}
}