using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Encodings.Web;

using CRM.TagHelpers.ModelBinding;
using CRM.TagHelpers.Models;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

namespace CRM.TagHelpers.ViewFeatures
{
	public class FastGenerator : DefaultHtmlGenerator, IHtmlGenerator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FastGenerator"/> class.
		/// </summary>
		/// <inheritdoc />
		public FastGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder, ValidationHtmlAttributeProvider validationAttributeProvider) : base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider) { }

		/// <summary>
		/// Generate a <c>text-field</c> element.
		/// </summary>
		/// <param name="viewContext">The <see cref="ViewContext"/>.</param>
		/// <param name="inputType">The input type.</param>
		/// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
		/// <param name="expression">The expression.</param>
		/// <param name="value">The value.</param>
		/// <param name="format">The format.</param>
		/// <param name="htmlAttributes">The html attributes.</param>
		/// <param name="designSystem">The <see cref="DesignSystemLanguage"/> to apply to the component.</param>
		/// <returns></returns>
		public TagBuilder GenerateTextField(ViewContext viewContext, ModelExplorer modelExplorer, string inputType, string expression, object value, string? format, object? htmlAttributes, DesignSystemLanguage designSystem)
		{
			string fullName = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
			IDictionary<string, object>? htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);

			if (IsFullNameValid(fullName, htmlAttributeDictionary) != true)
			{
				throw new ArgumentException(FieldNameCannotBeNullOrEmpty(typeof(IHtmlHelper).FullName, nameof(IHtmlHelper.Editor), typeof(IHtmlHelper<>).FullName, nameof(IHtmlHelper<object>.EditorFor), "htmlFieldName"), nameof(expression));
			}

			var tagBuilder = new TagBuilder($"{designSystem.ToString().ToLowerInvariant()}-text-field")
			{
				TagRenderMode = TagRenderMode.SelfClosing,
			};

			tagBuilder.MergeAttributes(htmlAttributeDictionary);
			tagBuilder.MergeAttribute("type", inputType);

			if (string.IsNullOrEmpty(fullName) != true)
			{
				tagBuilder.MergeAttribute("name", fullName, true);
			}

			AddPlaceholderAttribute(viewContext.ViewData, tagBuilder, modelExplorer, expression);
			AddMaxLengthAttribute(viewContext.ViewData, tagBuilder, modelExplorer, expression);
			string valueParameter = FormatValue(value, format);

			if (inputType == InputType.Password.ToString().ToLowerInvariant())
			{
				if (value is not null)
				{
					tagBuilder.MergeAttribute("value", valueParameter, true);
				}
			}

			else
			{
				string? attributeValue = (string?)GetModelStateValue(viewContext, fullName, typeof(string));

				if (attributeValue is null)
				{
					attributeValue = modelExplorer is null && value is null ? EvalString(viewContext, expression, format) : valueParameter;
				}

				tagBuilder.MergeAttribute("value", attributeValue, true);
			}

			NameAndIdProvider.GenerateId(viewContext, tagBuilder, fullName, IdAttributeDotReplacement);

			// If there are any errors for a named field, we add the CSS attribute.
			if (viewContext.ViewData.ModelState.TryGetValue(fullName, out ModelStateEntry? entry) && entry.Errors.Count > 0)
			{
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			}

			AddValidationAttributes(viewContext, tagBuilder, modelExplorer, expression);
			return tagBuilder;
		}

		private static string EvalString(ViewContext viewContext, string key, string? format)
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
		private static IDictionary<string, object>? GetHtmlAttributeDictionaryOrNull(object? htmlAttributes)
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

		/// <summary>
		/// The name of an HTML field cannot be null or empty. Instead use methods {0}.{1} or {2}.{3} with a non-empty {4} argument value.
		/// </summary>
		private static string FieldNameCannotBeNullOrEmpty(object? p0, object p1, object? p2, object p3, object p4)
		{
			return string.Format(CultureInfo.CurrentCulture, "The name of an HTML field cannot be null or empty. Instead use methods {0}.{1} or {2}.{3} with a non-empty {4} argument value.", p0, p1, p2, p3, p4);
		}
	}
}