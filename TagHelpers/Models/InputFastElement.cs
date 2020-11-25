using System;
using System.Collections.Generic;

using CRM.TagHelpers.Enums;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CRM.TagHelpers.Models
{
	/// <summary>
	/// <see cref="InputTagHelper"/> implementation targeting &lt;fast-text-field&gt; elements with an <c>asp-for</c> attribute.
	/// </summary>
	public class TextFieldFastElement : InputTagHelper
	{
		protected const string ForAttributeName = "asp-for";

		/// <summary>
		/// The appearance of the FAST text-field element.
		/// </summary>
		/// <remarks>
		/// Passed through to the generated HTML in all cases. Defaults to <c>outline</c>.
		/// </remarks>
		public TextFieldAppearance Appearance { get; set; }

		// Mapping from datatype names and data annotation hints to values for the <fast-text-field/> element's "type" attribute.
		protected static readonly Dictionary<string, string> defaultInputTypes = new(StringComparer.OrdinalIgnoreCase)
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

		// Mapping from <fast-text-field/> element's type to RFC 3339 date and time formats.
		protected static readonly Dictionary<string, string> dateTimeFormats = new(StringComparer.Ordinal)
		{
			{ "date", "{0:yyyy-MM-dd}" },
			{ "datetime", @"{0:yyyy-MM-ddTHH\:mm\:ss.fffK}" },
			{ "datetime-local", @"{0:yyyy-MM-ddTHH\:mm\:ss.fff}" },
			{ "time", @"{0:HH\:mm\:ss.fff}" },
		};

		/// <summary>
		/// Gets the <see cref="FastGenerator"/> used to generate the <see cref="TagHelpers.Fast.FastTextFieldTagHelper"/>'s output.
		/// </summary>
		protected new FastGenerator Generator { get; }

		/// <summary>
		/// The <see cref="Enums.DesignSystem"/> implementation.
		/// </summary>
		public DesignSystem DesignSystem { get; set; }

		/// <inheritdoc />
		public override int Order => -1000;

		/// <summary>
		/// Creates a new instance of <see cref="TextFieldFastElement"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public TextFieldFastElement(IHtmlGenerator generator) : base(generator)
		{
			Generator = (FastGenerator)generator;
		}
	}
}