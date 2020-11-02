using System;
using System.Collections.Generic;

using CRM.TagHelpers.Models;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fast
{
	/// <summary>
	/// <see cref="InputFastElement"/> implementation targeting &lt;fast-text-field&gt;
	/// elements with an <c>asp-for</c> attribute.
	/// </summary>
	[HtmlTargetElement("fast-text-field", Attributes = ForAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FastTextFieldTagHelper : InputFastElement
	{
		/// <summary>
		/// The appearance of the FAST text-field element.
		/// </summary>
		/// <remarks>
		/// Passed through to the generated HTML in all cases. Defaults to <c>outline</c>.
		/// </remarks>
		public AppearanceAttribute Appearance { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="FastTextFieldTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public FastTextFieldTagHelper(IHtmlGenerator generator) : base(generator) { }

		/// <inheritdoc />
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			string inputType;
			string? inputTypeHint;
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

			output.Attributes.SetAttribute(nameof(Appearance), Appearance.ToString().ToLowerInvariant());

			if (string.IsNullOrEmpty(InputTypeName))
			{
				// Note GetInputType never returns null.
				inputType = GetInputType(modelExplorer, out inputTypeHint);
			}

			else
			{
				inputType = InputTypeName.ToLowerInvariant();
				inputTypeHint = null;
			}

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

			TagBuilder tagBuilder = inputType switch
			{
				"hidden" => GenerateHiddenField(modelExplorer, htmlAttributes),
				"password" => GeneratePasswordField(modelExplorer, inputType, htmlAttributes),
				_ => GenerateTextField(modelExplorer, inputTypeHint, inputType, htmlAttributes),
			};

			output.MergeAttributes(tagBuilder);
		}

		// Imitate Generator.GenerateHidden() using Generator.GenerateTextBox(). This adds support
		// for asp-format that is not available in Generator.GenerateHidden().
		private TagBuilder GenerateHiddenField(ModelExplorer modelExplorer, IDictionary<string, object>? htmlAttributes)
		{
			object value = For.Model;

			if (value is byte[] byteArrayValue)
			{
				value = Convert.ToBase64String(byteArrayValue);
			}

			if (htmlAttributes is null)
			{
				htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			}

			// In DefaultHtmlGenerator(), GenerateTextBox() calls GenerateInput() _almost_ identically to how
			// GenerateHidden() does and the main switch inside GenerateInput() handles InputType.Text and
			// InputType.Hidden identically. No behavior differences at all when a type HTML attribute already exists.
			htmlAttributes["type"] = "hidden";
			return Generator.GenerateTextBox(ViewContext, modelExplorer, For.Name, value, Format, htmlAttributes);
		}

		private TagBuilder GeneratePasswordField(ModelExplorer modelExplorer, string inputType, IDictionary<string, object>? htmlAttributes)
		{
			return Generator.GenerateTextField(ViewContext, modelExplorer, inputType, For.Name, modelExplorer.Model, null, htmlAttributes, DesignSystem);
		}

		private TagBuilder GenerateTextField(ModelExplorer modelExplorer, string? inputTypeHint, string inputType, IDictionary<string, object>? htmlAttributes)
		{
			string format = Format;

			if (string.IsNullOrEmpty(format))
			{
				if (modelExplorer.Metadata.HasNonDefaultEditFormat != true
					&& string.Equals("week", inputType, StringComparison.OrdinalIgnoreCase)
					&& (modelExplorer.Model is DateTime || modelExplorer.Model is DateTimeOffset))
				{
					modelExplorer = modelExplorer.GetExplorerForModel(FormatWeekHelper.GetFormattedWeek(modelExplorer));
				}

				else
				{
					format = GetFormat(modelExplorer, inputTypeHint, inputType);
				}
			}

			if (htmlAttributes is null)
			{
				htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			}

			htmlAttributes["type"] = inputType;
			return Generator.GenerateTextField(ViewContext, modelExplorer, inputType, For.Name, modelExplorer.Model, format, htmlAttributes, DesignSystem);
		}

		/// <summary>
		/// Gets a &lt;fast-text-field&gt; element's "type" attribute value based on the given <paramref name="modelExplorer"/>
		/// or <see cref="InputType"/>.
		/// </summary>
		/// <param name="modelExplorer">The <see cref="ModelExplorer"/> to use.</param>
		/// <param name="inputTypeHint">When this method returns, contains the string, often the name of a
		/// <see cref="ModelMetadata.ModelType"/> base class, used to determine this method's return value.</param>
		/// <returns>A &lt;fast-text-field&gt; element's "type" attribute value.</returns>
		private new static string GetInputType(ModelExplorer modelExplorer, out string inputTypeHint)
		{
			foreach (string hint in TemplateRenderer.GetInputTypeHints(modelExplorer))
			{
				if (defaultInputTypes.TryGetValue(hint, out string? inputType))
				{
					inputTypeHint = hint;
					return inputType;
				}
			}

			inputTypeHint = InputType.Text.ToString().ToLowerInvariant();
			return inputTypeHint;
		}

		// Get a fall-back format based on the metadata.
		private string GetFormat(ModelExplorer modelExplorer, string? inputTypeHint, string inputType)
		{
			string format;

			if (string.Equals("month", inputType, StringComparison.OrdinalIgnoreCase))
			{
				// "month" is a new HTML5 input type that only will be rendered in Rfc3339 mode
				format = "{0:yyyy-MM}";
			}

			else if (string.Equals("decimal", inputTypeHint, StringComparison.OrdinalIgnoreCase)
				&& string.Equals("text", inputType, StringComparison.Ordinal)
				&& string.IsNullOrEmpty(modelExplorer.Metadata.EditFormatString))
			{
				// Decimal data is edited using an <fluent-text-field type="text"/> element, with a reasonable format.
				// EditFormatString has precedence over this fall-back format.
				format = "{0:0.00}";
			}

			else if (ViewContext.Html5DateRenderingMode == Html5DateRenderingMode.Rfc3339
				&& modelExplorer.Metadata.HasNonDefaultEditFormat != true
				&& (typeof(DateTime) == modelExplorer.Metadata.UnderlyingOrModelType
				|| typeof(DateTimeOffset) == modelExplorer.Metadata.UnderlyingOrModelType))
			{
				// Rfc3339 mode _may_ override EditFormatString in a limited number of cases. Happens only when
				// EditFormatString has a default format i.e. came from a [DataType] attribute.
				if (string.Equals("text", inputType)
					&& string.Equals(nameof(DateTimeOffset), inputTypeHint, StringComparison.OrdinalIgnoreCase))
				{
					// Auto-select a format that round-trips Offset and sub-Second values in a DateTimeOffset. Not
					// done if user chose the "text" type in .cshtml file or with data annotations i.e. when
					// inputTypeHint==null or "text".
					format = dateTimeFormats["datetime"];
				}

				else if (dateTimeFormats.TryGetValue(inputType, out string? rfc3339Format))
				{
					format = rfc3339Format;
				}

				else
				{
					// Otherwise use default EditFormatString.
					format = modelExplorer.Metadata.EditFormatString;
				}
			}

			else
			{
				// Otherwise use EditFormatString, if any.
				format = modelExplorer.Metadata.EditFormatString;
			}

			return format;
		}

		public enum AppearanceAttribute
		{
			Outline,
			Filled
		}
	}
}