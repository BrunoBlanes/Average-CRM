using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;

using CRM.TagHelpers.Enums;
using CRM.TagHelpers.ModelBinding;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
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
		private readonly IModelMetadataProvider metadataProvider;

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
			this.metadataProvider = metadataProvider;
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
		/// Generate a &lt;fast-select&gt; element for the <paramref name="expression"/>.
		/// </summary>
		/// <param name="optionLabel">Optional text for a default empty &lt;fast-option&gt; element.</param>
		/// <param name="selectList">
		/// A collection of <see cref="SelectListItem"/> objects used to populate the
		/// &lt;fast-select&gt; element with &lt;optgroup&gt; and &lt;fast-option&gt; elements.
		/// If <c>null</c>, finds this collection at <c>ViewContext.ViewData[expression]</c>.
		/// </param>
		/// <param name="currentValues">
		/// An <see cref="ICollection{String}"/> containing values for &lt;fast-option&gt; elements
		/// to select. If <c>null</c>, selects &lt;fast-option&gt; elements based on <see cref="SelectListItem.Selected"/> values in <paramref name="selectList"/>.
		/// </param>
		/// <param name="allowMultiple">
		/// If <c>true</c>, includes a <c>multiple</c> attribute in the generated HTML.
		/// Otherwise generates a single-selection &lt;fast-select&gt; element.
		/// </param>
		/// <param name="htmlAttributes">
		/// An <see cref="object"/> that contains the HTML attributes for the &lt;fast-select&gt; element. Alternatively, an
		/// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
		/// </param>
		/// <returns>A new <see cref="TagBuilder"/> describing the &lt;fast-select&gt; element.</returns>
		/// <remarks>
		/// <para>
		/// Combines <see cref="TemplateInfo.HtmlFieldPrefix"/> and <paramref name="expression"/> to set
		/// &lt;fast-select&gt; element's "name" attribute. Sanitizes <paramref name="expression"/> to set element's "id"
		/// attribute.
		/// </para>
		/// <para>
		/// See <see cref="GetCurrentValues"/> for information about how the <paramref name="currentValues"/>
		/// collection may be created.
		/// </para>
		/// </remarks>
		///<inheritdoc/>
		public TagBuilder GenerateSelect(
			ViewContext viewContext,
			ModelExplorer modelExplorer,
			string? optionLabel,
			string expression,
			IEnumerable<SelectListItem> selectList,
			ICollection<string>? currentValues,
			bool allowMultiple,
			object? htmlAttributes,
			DesignSystem designSystem
			)
		{

			// Element tag name prefix is its design system name
			var prefix = designSystem.ToString().ToLowerInvariant();
			var fullName = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
			IDictionary<string, object>? htmlAttributeDictionary = GetHtmlAttributeDictionaryOrNull(htmlAttributes);
			if (IsFullNameValid(fullName, htmlAttributeDictionary) is false)
			{
				throw new ArgumentException(
					FieldNameCannotBeNullOrEmpty(
						typeof(IHtmlHelper).FullName,
						nameof(IHtmlHelper.Editor),
						typeof(IHtmlHelper<>).FullName,
						nameof(IHtmlHelper<object>.EditorFor),
						"htmlFieldName"),
					nameof(expression));
			}

			// If we got a null selectList, try to use ViewData to get the list of items.
			if (selectList is null)
			{
				selectList = GetSelectListItems(viewContext, expression);
			}

			modelExplorer ??= ExpressionMetadataProvider.FromStringExpression(expression, viewContext.ViewData, metadataProvider);

			// Convert each ListItem to an <option> tag and wrap them with <optgroup> if requested.
			IHtmlContent listItemBuilder = GenerateGroupsAndOptions(optionLabel, selectList, currentValues, prefix);

			var tagBuilder = new TagBuilder($"{prefix}-select");
			tagBuilder.InnerHtml.SetHtmlContent(listItemBuilder);
			tagBuilder.MergeAttributes(htmlAttributeDictionary);
			NameAndIdProvider.GenerateId(viewContext, tagBuilder, fullName, IdAttributeDotReplacement);

			if (string.IsNullOrEmpty(fullName) is false)
			{
				tagBuilder.MergeAttribute("name", fullName, replaceExisting: true);
			}

			if (allowMultiple)
			{
				tagBuilder.MergeAttribute("multiple", "multiple");
			}

			// If there are any errors for a named field, we add the css attribute.
			if (viewContext.ViewData.ModelState.TryGetValue(fullName, out ModelStateEntry entry))
			{
				if (entry.Errors.Count > 0)
				{
					tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
				}
			}

			AddValidationAttributes(viewContext, tagBuilder, modelExplorer, expression);
			return tagBuilder;
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
		private static IEnumerable<SelectListItem> GetSelectListItems(
			ViewContext viewContext,
			string expression)
		{
			if (viewContext is null)
			{
				throw new ArgumentNullException(nameof(viewContext));
			}

			// Method is called only if user did not pass a select list in.
			// They must provide select list items in the ViewData dictionary and definitely
			// not as the Model. (Even if the Model datatype were correct, a <fast-select>
			// element generated for a collection of SelectListItems would be useless.)
			var value = viewContext.ViewData.Eval(expression);

			// First check whether above evaluation was successful and did not match ViewData.Model.
			if (value is null || value == viewContext.ViewData.Model)
			{
				throw new InvalidOperationException(MissingSelectData(
					$"IEnumerable<{nameof(SelectListItem)}>",
					expression));
			}

			// Second check the Eval() call returned a collection of SelectListItems.
			if (value is not IEnumerable<SelectListItem> selectList)
			{
				throw new InvalidOperationException(WrongSelectDataType(
					expression,
					value.GetType().FullName,
					$"IEnumerable<{nameof(SelectListItem)}>"));
			}

			return selectList;
		}

		private static IHtmlContent GenerateGroupsAndOptions(
			string? optionLabel,
			IEnumerable<SelectListItem> selectList,
			ICollection<string>? currentValues,
			string prefix)
		{
			if (selectList is not IList<SelectListItem> itemsList)
			{
				itemsList = selectList.ToList();
			}

			var count = itemsList.Count;
			if (optionLabel is not null)
			{
				count++;
			}

			// Short-circuit work below if there's nothing to add.
			if (count == 0)
			{
				return HtmlString.Empty;
			}

			var listItemBuilder = new HtmlContentBuilder(count);

			// Make optionLabel the first item that gets rendered.
			if (optionLabel is not null)
			{
				listItemBuilder.AppendLine(GenerateOption(
					new SelectListItem()
					{
						Text = optionLabel,
						Value = string.Empty,
						Selected = false,
					},
					null, prefix));
			}

			// Group items in the SelectList if requested.
			// The worst case complexity of this algorithm is O(number of groups*n).
			// If there aren't any groups, it is O(n) where n is number of items in the list.
			var optionGenerated = new bool[itemsList.Count];

			for (var i = 0; i < itemsList.Count; i++)
			{
				if (optionGenerated[i] is false)
				{
					SelectListItem item = itemsList[i];
					SelectListGroup optGroup = item.Group;

					if (optGroup is not null)
					{
						var groupBuilder = new TagBuilder("optgroup");
						if (optGroup.Name is not null)
						{
							groupBuilder.MergeAttribute("label", optGroup.Name);
						}

						if (optGroup.Disabled)
						{
							groupBuilder.MergeAttribute("disabled", "disabled");
						}

						groupBuilder.InnerHtml.AppendLine();

						for (var j = i; j < itemsList.Count; j++)
						{
							SelectListItem groupItem = itemsList[j];

							if (optionGenerated[j] is false &&
								ReferenceEquals(optGroup, groupItem.Group))
							{
								groupBuilder.InnerHtml.AppendLine(GenerateOption(groupItem, currentValues, prefix));
								optionGenerated[j] = true;
							}
						}

						listItemBuilder.AppendLine(groupBuilder);
					}
					else
					{
						listItemBuilder.AppendLine(GenerateOption(item, currentValues, prefix));
						optionGenerated[i] = true;
					}
				}
			}

			return listItemBuilder;
		}

		private static IHtmlContent GenerateOption(SelectListItem item, ICollection<string>? currentValues, string prefix)
		{
			var selected = item.Selected;

			if (currentValues is not null)
			{
				var value = item.Value ?? item.Text;
				selected = currentValues.Contains(value);
			}

			return GenerateOption(item, item.Text, selected, prefix);
		}

		internal static TagBuilder GenerateOption(SelectListItem item, string text, bool selected, string prefix)
		{
			var tagBuilder = new TagBuilder($"{prefix}-option");
			tagBuilder.InnerHtml.SetContent(text);

			if (item.Value is not null)
			{
				tagBuilder.Attributes["value"] = item.Value;
			}

			if (selected)
			{
				tagBuilder.Attributes["selected"] = "selected";
			}

			if (item.Disabled)
			{
				tagBuilder.Attributes["disabled"] = "disabled";
			}

			return tagBuilder;
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

		/// <summary>
		/// The ViewData item that has the key '{0}' is of type '{1}' but must be of type '{2}'.
		/// </summary>
		private static string WrongSelectDataType(object? p0, object? p1, object? p2)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				"The ViewData item that has the key '{0}' is of type '{1}' but must be of type '{2}'.",
				p0, p1, p2);
		}

		/// <summary>
		/// There is no ViewData item of type '{0}' that has the key '{1}'..
		/// </summary>
		private static string MissingSelectData(object? p0, object? p1)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				"There is no ViewData item of type '{0}' that has the key '{1}'.",
				p0, p1);
		}
	}
}