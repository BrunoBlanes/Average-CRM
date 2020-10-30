using System;
using System.Collections.Generic;

using CRM.Server.ViewFeatures;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.Server.TagHelpers
{
	/// <summary>
	/// <see cref="InputTagHelper"/> implementation targeting &lt;fluent-text-field&gt;
	/// elements with an <c>asp-for</c> attribute.
	/// </summary>
	[HtmlTargetElement("fluent-text-field", Attributes = ForAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class TextFieldTagHelper : InputTagHelper
	{
		private const string ForAttributeName = "asp-for";

		// Mapping from datatype names and data annotation hints to values for the <fluent-text-field/> element's "type" attribute.
		private static readonly Dictionary<string, string> defaultInputTypes = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "HiddenInput", InputType.Hidden.ToString().ToLowerInvariant() },
			{ "Password", InputType.Password.ToString().ToLowerInvariant() },
			{ "Text", InputType.Text.ToString().ToLowerInvariant() },
			{ "PhoneNumber", "tel" },
			{ "Url", "url" },
			{ "EmailAddress", "email" },
			{ "Date", "date" },
			{ "DateTime", "datetime-local" },
			{ "DateTime-local", "datetime-local" },
			{ nameof(DateTimeOffset), "text" },
			{ "Time", "time" },
			{ "Week", "week" },
			{ "Month", "month" },
			{ nameof(Byte), "number" },
			{ nameof(SByte), "number" },
			{ nameof(Int16), "number" },
			{ nameof(UInt16), "number" },
			{ nameof(Int32), "number" },
			{ nameof(UInt32), "number" },
			{ nameof(Int64), "number" },
			{ nameof(UInt64), "number" },
			{ nameof(Single), InputType.Text.ToString().ToLowerInvariant() },
			{ nameof(Double), InputType.Text.ToString().ToLowerInvariant() },
			{ nameof(Decimal), InputType.Text.ToString().ToLowerInvariant() },
			{ nameof(String), InputType.Text.ToString().ToLowerInvariant() },
		};

		// Mapping from <fluent-text-field/> element's type to RFC 3339 date and time formats.
		private static readonly Dictionary<string, string> dateTimeFormats = new(StringComparer.Ordinal)
		{
			{ "date", "{0:yyyy-MM-dd}" },
			{ "datetime", @"{0:yyyy-MM-ddTHH\:mm\:ss.fffK}" },
			{ "datetime-local", @"{0:yyyy-MM-ddTHH\:mm\:ss.fff}" },
			{ "time", @"{0:HH\:mm\:ss.fff}" },
		};

		/// <summary>
		/// Gets the <see cref="FluentGenerator"/> used to generate the <see cref="TextFieldTagHelper"/>'s output.
		/// </summary>
		protected new FluentGenerator Generator { get; }

		/// <summary>
		/// The appearance of the &lt;fluent-text-field&gt; element.
		/// </summary>
		/// <remarks>
		/// Passed through to the generated HTML in all cases. Defaults to <c>filled</c>.
		/// </remarks>
		public string Appearance { get; set; }

		/// <summary>
		/// Creates a new <see cref="TextFieldTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FluentGenerator"/>.</param>
		public TextFieldTagHelper(IHtmlGenerator generator) : base(generator)
		{
			Generator = (FluentGenerator)generator;
			Appearance = "filled";
		}

		/// <inheritdoc />
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
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

			string inputType;
			string? inputTypeHint;
			ModelExplorer modelExplorer = For.ModelExplorer;
			output.Attributes.SetAttribute(nameof(Appearance), Appearance);

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
			IDictionary<string, object>? htmlAttributes = null;
			if (string.IsNullOrEmpty(For.Name)
				&& string.IsNullOrEmpty(ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix)
				&& string.IsNullOrEmpty(Name) != true)
			{
				htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{ "name", Name },
				};
			}

			TagBuilder tagBuilder = inputType switch
			{
				//"hidden" => GenerateHidden(modelExplorer, htmlAttributes),

				// TODO: Override the method below to implement fluent design
				"password" => Generator.GeneratePassword(ViewContext, modelExplorer, For.Name, value: null, htmlAttributes: htmlAttributes),
				_ => GenerateTextBox(modelExplorer, inputTypeHint, inputType, htmlAttributes),
			};

			if (tagBuilder is not null)
			{
				// This TagBuilder contains the one <fluent-text-field/> element of interest.
				output.MergeAttributes(tagBuilder);

				if (tagBuilder.HasInnerHtml)
				{
					// Since this is not the "checkbox" special-case, no guarantee that output is a self-closing
					// element. A later tag helper targeting this element may change output.TagMode.
					output.Content.AppendHtml(tagBuilder.InnerHtml);
				}
			}
		}

		/// <summary>
		/// Gets a &lt;fluent-text-field&gt; element's "type" attribute value based on the given <paramref name="modelExplorer"/>
		/// or <see cref="InputType"/>.
		/// </summary>
		/// <param name="modelExplorer">The <see cref="ModelExplorer"/> to use.</param>
		/// <param name="inputTypeHint">When this method returns, contains the string, often the name of a
		/// <see cref="ModelMetadata.ModelType"/> base class, used to determine this method's return value.</param>
		/// <returns>A &lt;fluent-text-field&gt; element's "type" attribute value.</returns>
		private new static string GetInputType(ModelExplorer modelExplorer, out string inputTypeHint)
		{
			foreach (string hint in GetInputTypeHints(modelExplorer))
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

		private TagBuilder GenerateTextBox(ModelExplorer modelExplorer, string? inputTypeHint, string inputType, IDictionary<string, object>? htmlAttributes)
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
			return Generator.GenerateTextBox(
				ViewContext,
				modelExplorer,
				For.Name,
				modelExplorer.Model,
				format,
				htmlAttributes);
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

		// A variant of TemplateRenderer.GetViewNames().
		private static IEnumerable<string> GetInputTypeHints(ModelExplorer modelExplorer)
		{
			if (string.IsNullOrEmpty(modelExplorer.Metadata.TemplateHint) != true)
			{
				yield return modelExplorer.Metadata.TemplateHint;
			}

			if (string.IsNullOrEmpty(modelExplorer.Metadata.DataTypeName) != true)
			{
				yield return modelExplorer.Metadata.DataTypeName;
			}

			// We don't want to search for Nullable<T>, we want to search for T
			// (which should handle both T and Nullable<T>).
			Type? fieldType = modelExplorer.Metadata.UnderlyingOrModelType;

			foreach (string typeName in TemplateRenderer.GetTypeNames(modelExplorer.Metadata, fieldType))
			{
				yield return typeName;
			}
		}
	}
}