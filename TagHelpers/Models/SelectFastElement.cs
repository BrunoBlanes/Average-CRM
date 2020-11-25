using CRM.TagHelpers.Enums;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CRM.TagHelpers.Models
{
	/// <summary>
	/// <see cref="SelectTagHelper"/> implementation targeting &lt;fast-select&gt; elements with <c>asp-for</c> and/or
	/// <c>asp-items</c> attribute(s).
	/// </summary>
	public class SelectFastElement : SelectTagHelper
	{
		protected const string ForAttributeName = "asp-for";
		protected const string ItemsAttributeName = "asp-items";

		/// <summary>
		/// Gets the <see cref="FastGenerator"/> used to generate the <see cref="TagHelpers.Fast.FastSelectTagHelper"/>'s output.
		/// </summary>
		protected new FastGenerator Generator { get; }

		/// <summary>
		/// The <see cref="Enums.DesignSystem"/> implementation.
		/// </summary>
		public DesignSystem DesignSystem { get; set; }

		/// <inheritdoc />
		public override int Order => -1000;

		/// <summary>
		/// Creates a new instance of <see cref="SelectFastElement"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public SelectFastElement(IHtmlGenerator generator) : base(generator)
		{
			Generator = (FastGenerator)generator;
		}
	}
}