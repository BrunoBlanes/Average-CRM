using CRM.TagHelpers.Enums;
using CRM.TagHelpers.TagHelpers.Fast;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fluent
{
	/// <summary>
	/// <see cref="FastSelectTagHelper"/> implementation targeting &lt;fluent-select&gt; elements with <c>asp-for</c> and/or <c>asp-items</c> attribute(s).
	/// </summary>
	[HtmlTargetElement("fluent-select", Attributes = ForAttributeName)]
	[HtmlTargetElement("fluent-select", Attributes = ItemsAttributeName)]
	public class FluentSelectTagHelper : FastSelectTagHelper
	{
		/// <summary>
		/// Creates a new instance of <see cref="FluentTextFieldTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public FluentSelectTagHelper(IHtmlGenerator generator) : base(generator)
		{
			DesignSystem = DesignSystem.Fluent;
		}
	}
}