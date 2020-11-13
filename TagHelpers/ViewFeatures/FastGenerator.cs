using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.Encodings.Web;

using CRM.TagHelpers.Enums;
using CRM.TagHelpers.ModelBinding;

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
		public FastGenerator(
			IAntiforgery antiforgery,
			IOptions<MvcViewOptions> optionsAccessor,
			IModelMetadataProvider metadataProvider,
			IUrlHelperFactory urlHelperFactory,
			HtmlEncoder htmlEncoder,
			ValidationHtmlAttributeProvider validationAttributeProvider)
			: base(
				antiforgery,
				optionsAccessor,
				metadataProvider,
				urlHelperFactory,
				htmlEncoder,
				validationAttributeProvider)
		{

		}

		/// <summary>
		/// Generate a <c>text-field</c> element.
		/// </summary>
		/// <param name="viewContext">The <see cref="ViewContext"/>.</param>
		/// <param name="inputType">The <see cref="InputType"/>.</param>
		/// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
		/// <param name="expression">The expression.</param>
		/// <param name="value">The value.</param>
		/// <param name="format">The format.</param>
		/// <param name="htmlAttributes">The html attributes.</param>
		/// <param name="designSystem">The <see cref="DesignSystem"/> to apply to the component.</param>
		public TagBuilder GenerateTextField(
			ViewContext viewContext,
			ModelExplorer modelExplorer,
			InputType inputType,
			string expression,
			object value,
			string? format,
			object? htmlAttributes,
			DesignSystem designSystem)
		{
			IDictionary<string, object>? htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);
			return GenerateInputElement(
				viewContext,
				inputType,
				modelExplorer,
				expression,
				value,
				modelExplorer is null && value is null,
				false,
				true,
				format,
				designSystem,
				htmlAttributeDictionary);
		}

		/// <summary>
		/// Generate <c>checkbox</c> element.
		/// </summary>
		/// <param name="viewContext">The <see cref="ViewContext"/>.</param>
		/// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
		/// <param name="expression">The expression.</param>
		/// <param name="isChecked">If the input is checked.</param>
		/// <param name="htmlAttributes">The html attributes.</param>
		/// <param name="designSystem">The <see cref="DesignSystem"/> to apply to the component.</param>
		public TagBuilder GenerateCheckBox(
			ViewContext viewContext,
			ModelExplorer modelExplorer,
			string expression,
			bool? isChecked,
			object? htmlAttributes,
			DesignSystem designSystem)
		{
			// CheckBoxFor() case. That API does not support passing isChecked directly.
			Debug.Assert(isChecked.HasValue is false);
			IDictionary<string, object>? htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);

			if (bool.TryParse(modelExplorer.Model.ToString(), out var modelChecked))
			{
				isChecked = modelChecked;
			}

			if (isChecked.HasValue && htmlAttributeDictionary is not null)
			{
				// Explicit isChecked value must override "checked" in dictionary.
				htmlAttributeDictionary.Remove("checked");
			}

			// Use ViewData only in CheckBox case (metadata null) and when the user didn't pass an isChecked value.
			return GenerateInputElement(
				viewContext,
				InputType.CheckBox,
				modelExplorer,
				expression,
				"true",
				modelExplorer is null && isChecked.HasValue is false,
				isChecked ?? false,
				false,
				null,
				designSystem,
				htmlAttributeDictionary);
		}

		/// <summary>
		/// Generate an input element.
		/// </summary>
		/// <param name="viewContext">The <see cref="ViewContext"/>.</param>
		/// <param name="inputType">The <see cref="InputType"/>.</param>
		/// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
		/// <param name="expression">The expression.</param>
		/// <param name="value">The value.</param>
		/// <param name="useViewData">Whether to use view data.</param>
		/// <param name="isChecked">If the input is checked.</param>
		/// <param name="isExplicitValue">Whether this is an explicit value.</param>
		/// <param name="format">The format.</param>
		/// <param name="htmlAttributeDictionary">The html attributes.</param>
		private TagBuilder GenerateInputElement(
			ViewContext viewContext,
			InputType inputType,
			ModelExplorer modelExplorer,
			string expression,
			object value,
			bool useViewData,
			bool isChecked,
			bool isExplicitValue,
			string? format,
			DesignSystem designSystem,
			IDictionary<string, object>? htmlAttributeDictionary)
		{
			var fullName = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);

			if (IsFullNameValid(fullName, htmlAttributeDictionary) is not true)
			{
				throw new ArgumentException(
					FieldNameCannotBeNullOrEmpty(typeof(IHtmlHelper).FullName,
					nameof(IHtmlHelper.Editor),
					typeof(IHtmlHelper<>).FullName,
					nameof(IHtmlHelper<object>.EditorFor), "htmlFieldName"),
					nameof(expression));
			}

			TagBuilder tagBuilder;
			var valueParameter = FormatValue(value, format);
			var usedModelState = false;

			// Element tag name prefix is its design system name
			var prefix = designSystem.ToString().ToLowerInvariant();

			if (inputType is InputType.CheckBox)
			{
				var modelStateWasChecked = GetModelStateValue(viewContext, fullName, typeof(bool)) as bool?;
				tagBuilder = new TagBuilder($"{prefix}-checkbox") { TagRenderMode = TagRenderMode.EndTag };

				if (modelStateWasChecked.HasValue)
				{
					isChecked = modelStateWasChecked.Value;
					usedModelState = true;
				}

				if (usedModelState != true)
				{
					if (GetModelStateValue(viewContext, fullName, typeof(string)) is string modelStateValue)
					{
						isChecked = string.Equals(modelStateValue, valueParameter, StringComparison.Ordinal);
						usedModelState = true;
					}
				}

				if (usedModelState != true && useViewData)
				{
					isChecked = EvalBoolean(viewContext, expression);
				}

				if (isChecked)
				{
					tagBuilder.MergeAttribute("checked", "checked");
				}

				tagBuilder.MergeAttribute("value", valueParameter, isExplicitValue);
			}

			else
			{
				tagBuilder = new TagBuilder($"{prefix}-text-field") { TagRenderMode = TagRenderMode.SelfClosing };
				AddPlaceholderAttribute(viewContext.ViewData, tagBuilder, modelExplorer, expression);
				AddMaxLengthAttribute(viewContext.ViewData, tagBuilder, modelExplorer, expression);

				if (inputType is InputType.Password)
				{
					if (value is not null)
					{
						tagBuilder.MergeAttribute("value", valueParameter, true);
					}
				}

				else
				{
					var attributeValue = (string?)GetModelStateValue(viewContext, fullName, typeof(string));

					if (attributeValue is null)
					{
						attributeValue = modelExplorer is null && value is null
							? EvalString(viewContext, expression, format)
							: valueParameter;
					}

					tagBuilder.MergeAttribute("value", attributeValue, true);
				}
			}

			if (string.IsNullOrEmpty(fullName) is not true)
			{
				tagBuilder.MergeAttribute("name", fullName, true);
			}

			tagBuilder.MergeAttributes(htmlAttributeDictionary);
			var inputTypeString = GetInputTypeString(inputType);
			tagBuilder.MergeAttribute("type", inputTypeString);
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

		private static bool EvalBoolean(ViewContext viewContext, string key)
		{
			return Convert.ToBoolean(viewContext.ViewData.Eval(key), CultureInfo.InvariantCulture);
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
				htmlAttributeDictionary.TryGetValue("name", out var attributeObject);

				if (string.IsNullOrEmpty(Convert.ToString(attributeObject, CultureInfo.InvariantCulture)))
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
			return viewContext.ViewData.ModelState.TryGetValue(key, out ModelStateEntry? entry) && entry.RawValue is not null
				? ModelBindingHelper.ConvertTo(entry.RawValue, destinationType, null)
				: null;
		}

		// Convert type to string
		private static string GetInputTypeString(InputType inputType)
		{
			return inputType switch
			{
				InputType.CheckBox => "checkbox",
				InputType.Password => "password",
				InputType.Hidden => "hidden",
				InputType.Radio => "radio",
				InputType.Text => "text",
				_ => "text",
			};
		}

		/// <summary>
		/// The name of an HTML field cannot be null or empty. Instead use methods {0}.{1} or {2}.{3} with a non-empty {4} argument value.
		/// </summary>
		private static string FieldNameCannotBeNullOrEmpty(object? p0, object p1, object? p2, object p3, object p4)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				"The name of an HTML field cannot be null or empty. Instead use methods {0}.{1} or {2}.{3} with a non-empty {4} argument value.",
				p0, p1, p2, p3, p4);
		}
	}
}