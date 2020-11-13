using System;
using System.Collections.Generic;

using CRM.TagHelpers.Models;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fast
{
	/// <summary>
	/// <see cref="CheckBoxFastElement"/> implementation targeting &lt;fast-checkbox&gt;
	/// elements with an <c>asp-for</c> attribute.
	/// </summary>
	[HtmlTargetElement("fast-checkbox", Attributes = ForAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FastCheckBoxTagHelper : CheckBoxFastElement
	{
		/// <summary>
		/// Creates a new instance of <see cref="FastCheckBoxTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public FastCheckBoxTagHelper(IHtmlGenerator generator) : base(generator) { }

		/// <inheritdoc />
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			ModelExplorer modelExplorer = For.ModelExplorer;
			IDictionary<string, object>? htmlAttributes = null;

			// Pass through attributes that are also well-known HTML attributes.
			// Must be done prior to any copying from a TagBuilder.
			if (InputTypeName is not null)
			{
				output.CopyHtmlAttribute("type", context);
			}

			if (Name is not null)
			{
				output.CopyHtmlAttribute(nameof(Name), context);
			}

			if (Value is not null)
			{
				output.CopyHtmlAttribute(nameof(Value), context);
			}

			var inputType = string.IsNullOrEmpty(InputTypeName)
				? InputType.CheckBox.ToString().ToLowerInvariant()
				: InputTypeName.ToLowerInvariant();

			// inputType may be more specific than default the generator chooses below.
			if (output.Attributes.ContainsName("type") != true)
			{
				output.Attributes.SetAttribute("type", inputType);
			}

			// Ensure Generator does not throw due to empty "fullName" if user provided a name attribute.
			if (string.IsNullOrEmpty(For.Name) && string.IsNullOrEmpty(ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix)
				&& string.IsNullOrEmpty(Name) != true)
			{
				htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{ "name", Name },
				};
			}

			TagBuilder tagBuilder = GenerateCheckBox(modelExplorer, htmlAttributes);
			output.MergeAttributes(tagBuilder);
		}

		private TagBuilder GenerateCheckBox(ModelExplorer modelExplorer, IDictionary<string, object>? htmlAttributes)
		{
			return Generator.GenerateCheckBox(
				ViewContext,
				modelExplorer,
				For.Name,
				null,
				htmlAttributes,
				DesignSystem);
		}
	}
}