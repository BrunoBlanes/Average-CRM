using CRM.TagHelpers.Models;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fast
{
	/// <summary>
	/// <see cref="InputFastElement"/> implementation targeting &lt;fast-button&gt; elements.
	/// </summary>
	[HtmlTargetElement("fast-button", TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FastButtonTagHelper : TagHelper
	{
		/// <summary>
		/// The appearance of the FAST button element.
		/// </summary>
		/// <remarks>
		/// Passed through to the generated HTML in all cases. Defaults to <c>neutral</c>.
		/// </remarks>
		public AppearanceAttribute Appearance { get; set; }

		/// <inheritdoc />
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.Attributes.SetAttribute(nameof(Appearance), Appearance.ToString().ToLowerInvariant());
		}

		public enum AppearanceAttribute
		{
			Neutral,
			Accent,
			Lightweight,
			Outline,
			Stealth
		}
	}
}