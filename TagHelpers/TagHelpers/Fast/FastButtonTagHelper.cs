using System;
using System.Collections.Generic;
using System.Globalization;

using CRM.TagHelpers.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace CRM.TagHelpers.TagHelpers.Fast
{
	/// <summary>
	/// <see cref="InputFastElement"/> implementation targeting &lt;fast-button&gt; elements.
	/// </summary>
	[HtmlTargetElement("fast-button", TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FastButtonTagHelper : TagHelper
	{
		private const string routeValuesDictionaryName = "asp-all-route-data";
		private const string pageHandlerAttributeName = "asp-page-handler";
		private const string controllerAttributeName = "asp-controller";
		private const string fragmentAttributeName = "asp-fragment";
		private const string actionAttributeName = "asp-action";
		private const string routeValuesPrefix = "asp-route-";
		private const string routeAttributeName = "asp-route";
		private readonly IUrlHelperFactory urlHelperFactory;
		private const string pageAttributeName = "asp-page";
		private const string areaAttributeName = "asp-area";
		private IDictionary<string, string>? routeValues;
		private const string formAction = "formaction";

		/// <summary>
		/// The appearance of the FAST button element.
		/// </summary>
		/// <remarks>
		/// Passed through to the generated HTML in all cases. Defaults to <c>neutral</c>.
		/// </remarks>
		public AppearanceAttribute Appearance { get; set; }

		/// <inheritdoc />
		public override int Order => -1000;

		/// <summary>
		/// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext"/> for the current request.
		/// </summary>
		[ViewContext]
		[HtmlAttributeNotBound]
		public ViewContext ViewContext { get; set; } = null!;

		/// <summary>
		/// The name of the action method.
		/// </summary>
		[HtmlAttributeName(actionAttributeName)]
		public string? Action { get; set; }

		/// <summary>
		/// The name of the controller.
		/// </summary>
		[HtmlAttributeName(controllerAttributeName)]
		public string? Controller { get; set; }

		/// <summary>
		/// The name of the area.
		/// </summary>
		[HtmlAttributeName(areaAttributeName)]
		public string? Area { get; set; }

		/// <summary>
		/// The name of the page.
		/// </summary>
		[HtmlAttributeName(pageAttributeName)]
		public string? Page { get; set; }

		/// <summary>
		/// The name of the page handler.
		/// </summary>
		[HtmlAttributeName(pageHandlerAttributeName)]
		public string? PageHandler { get; set; }

		/// <summary>
		/// Gets or sets the URL fragment.
		/// </summary>
		[HtmlAttributeName(fragmentAttributeName)]
		public string? Fragment { get; set; }

		/// <summary>
		/// Name of the route.
		/// </summary>
		/// <remarks>
		/// Must be <c>null</c> if <see cref="Action"/> or <see cref="Controller"/> is non-<c>null</c>.
		/// </remarks>
		[HtmlAttributeName(routeAttributeName)]
		public string? Route { get; set; }

		/// <summary>
		/// Additional parameters for the route.
		/// </summary>
		[HtmlAttributeName(routeValuesDictionaryName, DictionaryAttributePrefix = routeValuesPrefix)]
		public IDictionary<string, string> RouteValues
		{
			get
			{
				if (routeValues is null)
				{
					routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				}

				return routeValues;
			}
			set => routeValues = value;
		}

		/// <summary>
		/// Creates a new <see cref="FastButtonTagHelper"/>.
		/// </summary>
		/// <param name="urlHelperFactory">The <see cref="IUrlHelperFactory"/>.</param>
		public FastButtonTagHelper(IUrlHelperFactory urlHelperFactory)
		{
			this.urlHelperFactory = urlHelperFactory;
		}

		/// <inheritdoc />
		/// <remarks>Does nothing if user provides an <c>formAction</c> attribute.</remarks>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <c>formAction</c> attribute is provided and <see cref="Action"/>,
		/// <see cref="Controller"/> or <see cref="Fragment"/>  are non-<c>null</c>
		/// or if the user provided <c>asp-route-*</c> attributes.
		/// </exception>
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			// If "FormAction" is already set, it means the user is attempting to use a normal button or input element.
			if (output.Attributes.ContainsName(formAction))
			{
				if (Action is not null || Controller is not null || Area is not null
					|| Page is not null || PageHandler is not null || Fragment is not null
					|| Route is not null
					|| (this.routeValues is not null && this.routeValues.Count > 0))
				{
					// User specified a FormAction and one of the bound attributes
					// Can't override that FormAction attribute.
					throw new InvalidOperationException(
						CannotOverrideFormAction(
							formAction,
							output.TagName,
							routeValuesPrefix,
							actionAttributeName,
							controllerAttributeName,
							areaAttributeName,
							fragmentAttributeName,
							routeAttributeName,
							pageAttributeName,
							pageHandlerAttributeName));
				}

				return;
			}

			var routeLink = Route is not null;
			var actionLink = Controller is not null || Action is not null;
			var pageLink = Page is not null || PageHandler is not null;

			if ((routeLink && actionLink) || (routeLink && pageLink) || (actionLink && pageLink))
			{
				var message = string.Join(
					Environment.NewLine,
					FormatCannotDetermineAttributeFor(formAction, '<' + output.TagName + '>'),
					routeAttributeName,
					controllerAttributeName + ", " + actionAttributeName,
					pageAttributeName + ", " + pageHandlerAttributeName);
				throw new InvalidOperationException(message);
			}

			RouteValueDictionary? routeValues = null;

			if (this.routeValues is not null && this.routeValues.Count > 0)
			{
				routeValues = new RouteValueDictionary(this.routeValues);
			}

			if (Area is not null)
			{
				if (routeValues is null)
				{
					routeValues = new RouteValueDictionary();
				}

				// Unconditionally replace any value from asp-route-area.
				routeValues["area"] = Area;
			}

			var url = string.Empty;
			IUrlHelper? urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

			if (pageLink)
			{
				url = urlHelper.Page(Page, PageHandler, routeValues, null, null, Fragment);
			}

			else if (routeLink)
			{
				url = urlHelper.RouteUrl(Route, routeValues, null, null, Fragment);
			}

			else if (actionLink)
			{
				url = urlHelper.Action(Action, Controller, routeValues, null, null, Fragment);
			}

			if (string.IsNullOrEmpty(url) is false)
			{
				output.Attributes.SetAttribute(formAction, url);
			}

			// Set the button appearance
			output.Attributes.SetAttribute(nameof(Appearance), Appearance.ToString().ToLowerInvariant());
			output.Attributes.SetAttribute("type", "submit");
		}

		/// <summary>
		/// Cannot override the '{0}' attribute for &lt;{1}&gt;. &lt;{1}&gt; elements with a specified '{0}' must not have attributes starting with '{2}' or an '{3}', '{4}', '{5}', '{6}', '{7}', '{8}' or '{9}' attribute.
		/// </summary>
		private static string CannotOverrideFormAction(object p0, object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9)
		{
			return string.Format(CultureInfo.CurrentCulture, "Cannot override the '{0}' attribute for &lt;{1}&gt;. &lt;{1}&gt; elements with a specified '{0}' must not have attributes starting with '{2}' or an '{3}','{4}','{5}', '{6}', '{7}', '{8}' or '{9}' attribute.", p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
		}

		/// <summary>
		/// Cannot determine the '{0}' attribute for {1}. The following attributes are mutually exclusive:
		/// </summary>
		private static string FormatCannotDetermineAttributeFor(object p0, object p1)
		{
			return string.Format(CultureInfo.CurrentCulture, "Cannot determine the '{0}' attribute for {1}. The following attributes are mutually exclusive:", p0, p1);
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