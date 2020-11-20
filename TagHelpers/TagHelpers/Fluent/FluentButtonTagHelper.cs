using CRM.TagHelpers.TagHelpers.Fast;

using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fluent
{
	/// <summary>
	/// <see cref="FluentButtonTagHelper"/> implementation targeting &lt;fluent-button&gt; elements.
	/// </summary>
	[HtmlTargetElement("fluent-button", TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FluentButtonTagHelper : FastButtonTagHelper
	{
		/// <summary>
		/// Creates a new <see cref="FluentButtonTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public FluentButtonTagHelper(IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory)
		{
			Appearance = AppearanceAttribute.Accent;
		}
	}
}