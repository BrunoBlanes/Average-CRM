using CRM.TagHelpers.Enums;
using CRM.TagHelpers.TagHelpers.Fast;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fluent
{
	/// <summary>
	/// <see cref="FastCheckBoxTagHelper"/> implementation targeting &lt;fluent-checkbox&gt;
	/// elements with an <c>asp-for</c> attribute.
	/// </summary>
	[HtmlTargetElement("fluent-checkbox", Attributes = ForAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FluentCheckBoxTagHelper : FastCheckBoxTagHelper
	{
		/// <summary>
		/// Creates a new instance of <see cref="FluentTextFieldTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public FluentCheckBoxTagHelper(IHtmlGenerator generator) : base(generator)
		{
			DesignSystem = DesignSystem.Fluent;
		}
	}
}