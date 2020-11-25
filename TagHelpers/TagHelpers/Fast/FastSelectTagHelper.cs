using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using CRM.TagHelpers.Models;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fast
{
	/// <summary>
	/// <see cref="SelectFastElement"/> implementation targeting &lt;fast-select&gt; elements with <c>asp-for</c> and/or <c>asp-items</c> attribute(s).
	/// </summary>
	[HtmlTargetElement("fast-select", Attributes = ForAttributeName)]
	[HtmlTargetElement("fast-select", Attributes = ItemsAttributeName)]
	public class FastSelectTagHelper : SelectFastElement
	{
		private ICollection<string>? currentValues;
		private bool allowMultiple;

		/// <summary>
		/// Creates a new instance of <see cref="FastSelectTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public FastSelectTagHelper(IHtmlGenerator generator) : base(generator) { }

		/// <inheritdoc />
		public override void Init(TagHelperContext context)
		{
			if (For is null)
			{
				// Informs contained elements that they're running within a targeted <select/> element.
				context.Items[typeof(SelectTagHelper)] = null;
				return;
			}

			// Note null or empty For.Name is allowed because TemplateInfo.HtmlFieldPrefix may be sufficient.
			// IHtmlGenerator will enforce name requirements.
			if (For.Metadata is null)
			{
				throw new InvalidOperationException(NoProvidedMetadata(
					$"<{DesignSystem.ToString().ToLowerInvariant()}-select>",
					ForAttributeName,
					nameof(IModelMetadataProvider),
					For.Name));
			}

			// Base allowMultiple on the instance or declared type of the expression to avoid a
			// "SelectExpressionNotEnumerable" InvalidOperationException during generation.
			// Metadata.IsEnumerableType is similar but does not take runtime type into account.
			Type realModelType = For.ModelExplorer.ModelType;
			allowMultiple = typeof(string) != realModelType && typeof(IEnumerable).IsAssignableFrom(realModelType);
			this.currentValues = Generator.GetCurrentValues(ViewContext, For.ModelExplorer, For.Name, allowMultiple);

			// Whether or not (not being highly unlikely) we generate anything, could update contained <option/>
			// elements. Provide selected values for <option/> tag helpers.
			CurrentValues? currentValues = this.currentValues is null ? null : new CurrentValues(this.currentValues);
			context.Items[typeof(SelectTagHelper)] = currentValues;
		}

		/// <inheritdoc />
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			// Pass through attribute that is also a well-known HTML attribute. Must be done prior to any copying
			// from a TagBuilder.
			if (Name is not null)
			{
				output.CopyHtmlAttribute(nameof(Name), context);
			}

			// Ensure GenerateSelect() _never_ looks anything up in ViewData.
			IEnumerable<SelectListItem>? items = Items ?? Enumerable.Empty<SelectListItem>();

			if (For is null)
			{
				IHtmlContent options = Generator.GenerateGroupsAndOptions(null, items);
				output.PostContent.AppendHtml(options);
				return;
			}

			// Ensure Generator does not throw due to empty "fullName" if user provided a name attribute.
			IDictionary<string, object>? htmlAttributes = null;

			if (string.IsNullOrEmpty(For.Name)
				&& string.IsNullOrEmpty(ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix)
				&& string.IsNullOrEmpty(Name) is false)
			{
				htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{ "name", Name ?? string.Empty },
				};
			}

			TagBuilder tagBuilder = Generator.GenerateSelect(
				ViewContext,
				For.ModelExplorer,
				null,
				For.Name,
				items,
				currentValues,
				allowMultiple,
				htmlAttributes,
				DesignSystem);

			if (tagBuilder is not null)
			{
				output.MergeAttributes(tagBuilder);

				if (tagBuilder.HasInnerHtml)
				{
					output.PostContent.AppendHtml(tagBuilder.InnerHtml);
				}
			}
		}

		/// <summary>
		/// The {2} was unable to provide metadata about '{1}' expression value '{3}' for {0}.
		/// </summary>
		private static string NoProvidedMetadata(object p0, object p1, object p2, object p3)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				"The {2} was unable to provide metadata about '{1}' expression value '{3}' for {0}.",
				p0, p1, p2, p3);
		}
	}

	class CurrentValues
	{
		public ICollection<string>? Values { get; }
		public ICollection<string>? ValuesAndEncodedValues { get; set; }

		public CurrentValues(ICollection<string> values)
		{
			Debug.Assert(values is not null);
			Values = values;
		}
	}
}