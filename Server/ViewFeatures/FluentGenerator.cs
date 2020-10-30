using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Encodings.Web;

using CRM.Server.ModelBinding;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

namespace CRM.Server.ViewFeatures
{
	public class FluentGenerator : DefaultHtmlGenerator, IHtmlGenerator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FluentGenerator"/> class.
		/// </summary>
		/// <inheritdoc />
		public FluentGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder, ValidationHtmlAttributeProvider validationAttributeProvider) : base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider) { }

		/// <summary>
		/// Generates a &lt;fluent-text-field type="text"&gt; element
		/// </summary>
		/// <inheritdoc />
		public override TagBuilder GenerateTextBox(ViewContext viewContext, ModelExplorer modelExplorer, string expression, object value, string format, object htmlAttributes)
		{
			if (viewContext is null)
			{
				throw new ArgumentNullException(nameof(viewContext));
			}

			IDictionary<string, object>? htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);
			return GenerateInput(
				viewContext,
				InputType.Text,
				modelExplorer,
				expression,
				value,
				useViewData: modelExplorer is null && value is null,
				isChecked: false,
				setId: true,
				isExplicitValue: true,
				format: format,
				htmlAttributes: htmlAttributeDictionary);
		}

		/// <summary>
		/// Generate a <c>fluent-text-field</c> tag.
		/// </summary>
		/// <param name="viewContext">The <see cref="ViewContext"/>.</param>
		/// <param name="inputType">The <see cref="InputType"/>.</param>
		/// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
		/// <param name="expression">The expression.</param>
		/// <param name="value">The value.</param>
		/// <param name="useViewData">Whether to use view data.</param>
		/// <param name="isChecked">If the input is checked.</param>
		/// <param name="setId">Whether this should set id.</param>
		/// <param name="isExplicitValue">Whether this is an explicit value.</param>
		/// <param name="format">The format.</param>
		/// <param name="htmlAttributes">The html attributes.</param>
		/// <returns></returns>
		protected override TagBuilder GenerateInput(ViewContext viewContext, InputType inputType, ModelExplorer modelExplorer, string expression, object value, bool useViewData, bool isChecked, bool setId, bool isExplicitValue, string format, IDictionary<string, object>? htmlAttributes)
		{
			if (viewContext is null)
			{
				throw new ArgumentNullException(nameof(viewContext));
			}

			// Not valid to use TextBoxForModel() and so on in a top-level view; would end up with an unnamed input
			// elements. But we support the *ForModel() methods in any lower-level template, once HtmlFieldPrefix is
			// non-empty.
			string fullName = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);

			if (IsFullNameValid(fullName, htmlAttributes) != true)
			{
				throw new ArgumentException(
					FormatHtmlGenerator_FieldNameCannotBeNullOrEmpty(
						typeof(IHtmlHelper).FullName,
						nameof(IHtmlHelper.Editor),
						typeof(IHtmlHelper<>).FullName,
						nameof(IHtmlHelper<object>.EditorFor),
						"htmlFieldName"),
					nameof(expression));
			}

			string inputTypeString = GetInputTypeString(inputType);
			var tagBuilder = new TagBuilder("fluent-text-field")
			{
				TagRenderMode = TagRenderMode.SelfClosing,
			};

			tagBuilder.MergeAttributes(htmlAttributes);
			tagBuilder.MergeAttribute("type", inputTypeString);

			if (string.IsNullOrEmpty(fullName) != true)
			{
				tagBuilder.MergeAttribute("name", fullName, replaceExisting: true);
			}

			string suppliedTypeString = tagBuilder.Attributes["type"];
			AddPlaceholderAttribute(viewContext.ViewData, tagBuilder, modelExplorer, expression);
			AddMaxLengthAttribute(viewContext.ViewData, tagBuilder, modelExplorer, expression);
			string valueParameter = FormatValue(value, format);

			switch (inputType)
			{
				case InputType.Password:
					if (value is not null)
					{
						tagBuilder.MergeAttribute("value", valueParameter, isExplicitValue);
					}

					break;

				case InputType.Text:
				default:
					string? attributeValue = (string?)GetModelStateValue(viewContext, fullName, typeof(string));

					if (attributeValue is null)
					{
						attributeValue = useViewData ? EvalString(viewContext, expression, format) : valueParameter;
					}

					bool addValue = true;

					if (htmlAttributes is not null && htmlAttributes.TryGetValue("type", out object? typeAttributeValue))
					{
						string? typeAttributeString = typeAttributeValue.ToString();

						if (string.Equals(typeAttributeString, "file", StringComparison.OrdinalIgnoreCase)
							|| string.Equals(typeAttributeString, "image", StringComparison.OrdinalIgnoreCase))
						{
							// 'value' attribute is not needed for 'file' and 'image' input types.
							addValue = false;
						}
					}

					if (addValue)
					{
						tagBuilder.MergeAttribute("value", attributeValue, replaceExisting: isExplicitValue);
					}

					break;
			}

			if (setId)
			{
				NameAndIdProvider.GenerateId(viewContext, tagBuilder, fullName, IdAttributeDotReplacement);
			}

			// If there are any errors for a named field, we add the CSS attribute.
			if (viewContext.ViewData.ModelState.TryGetValue(fullName, out ModelStateEntry? entry) && entry.Errors.Count > 0)
			{
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			}

			AddValidationAttributes(viewContext, tagBuilder, modelExplorer, expression);
			return tagBuilder;
		}

		private static string EvalString(ViewContext viewContext, string key, string format)
		{
			return Convert.ToString(viewContext.ViewData.Eval(key, format), CultureInfo.CurrentCulture);
		}

		private static bool IsFullNameValid(string fullName, IDictionary<string, object>? htmlAttributeDictionary)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				// fullName==null is normally an error because name="" is not valid in HTML 5.
				if (htmlAttributeDictionary is null)
				{
					return false;
				}

				// Check if user has provided an explicit name attribute.
				// Generalized a bit because other attributes e.g. data-valmsg-for refer to element names.
				htmlAttributeDictionary.TryGetValue("name", out object? attributeObject);
				string? attributeString = Convert.ToString(attributeObject, CultureInfo.InvariantCulture);

				if (string.IsNullOrEmpty(attributeString))
				{
					return false;
				}
			}

			return true;
		}

		// Only need a dictionary if htmlAttributes is non-null. TagBuilder.MergeAttributes() is fine with null.
		private static IDictionary<string, object>? GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
		{
			IDictionary<string, object>? htmlAttributeDictionary = null;

			if (htmlAttributes is not null)
			{
				htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;

				if (htmlAttributeDictionary is null)
				{
					htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
				}
			}

			return htmlAttributeDictionary;
		}

		private static object? GetModelStateValue(ViewContext viewContext, string key, Type destinationType)
		{
			return viewContext.ViewData.ModelState.TryGetValue(key, out ModelStateEntry? entry)
				&& entry.RawValue != null
				? ModelBindingHelper.ConvertTo(entry.RawValue, destinationType, culture: null)
				: null;
		}

		private static string GetInputTypeString(InputType inputType)
		{
			return inputType switch
			{
				InputType.Hidden => "hidden",
				InputType.Password => "password",
				InputType.Text => "text",
				_ => "text",
			};
		}

		/// <summary>
		/// The name of an HTML field cannot be null or empty. Instead use methods {0}.{1} or {2}.{3} with a non-empty {4} argument value.
		/// </summary>
		private static string FormatHtmlGenerator_FieldNameCannotBeNullOrEmpty(object? p0, object p1, object? p2, object p3, object p4)
		{
			return string.Format(CultureInfo.CurrentCulture, "The name of an HTML field cannot be null or empty. Instead use methods {0}.{1} or {2}.{3} with a non-empty {4} argument value.", p0, p1, p2, p3, p4);
		}
	}
}