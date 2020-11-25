using System;
using System.Globalization;

using CRM.TagHelpers.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace CRM.TagHelpers.TagHelpers.Fast
{
	/// <summary>
	/// <see cref="ButtonFastElement"/> implementation targeting &lt;fast-button&gt; elements.
	/// </summary>
	[HtmlTargetElement("fast-button", TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FastButtonTagHelper : ButtonFastElement
	{
		public FastButtonTagHelper(IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory) { }

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
			if (output.Attributes.ContainsName(FormAction))
			{
				if (Action is not null || Controller is not null || Area is not null
					|| Page is not null || PageHandler is not null || Fragment is not null
					|| Route is not null
					|| (RouteValues is not null && RouteValues.Count > 0))
				{
					// User specified a FormAction and one of the bound attributes
					// Can't override that FormAction attribute.
					throw new InvalidOperationException(
						CannotOverrideFormAction(
							FormAction,
							output.TagName,
							RouteValuesPrefix,
							ActionAttributeName,
							ControllerAttributeName,
							AreaAttributeName,
							FragmentAttributeName,
							RouteAttributeName,
							PageAttributeName,
							PageHandlerAttributeName));
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
					FormatCannotDetermineAttributeFor(FormAction, '<' + output.TagName + '>'),
					RouteAttributeName,
					ControllerAttributeName + ", " + ActionAttributeName,
					PageAttributeName + ", " + PageHandlerAttributeName);
				throw new InvalidOperationException(message);
			}

			RouteValueDictionary? routeValues = null;

			if (RouteValues is not null && RouteValues.Count > 0)
			{
				routeValues = new RouteValueDictionary(RouteValues);
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
			IUrlHelper? urlHelper = UrlHelperFactory.GetUrlHelper(ViewContext);

			if (pageLink)
			{
				url = urlHelper.Page(Page, PageHandler, routeValues, protocol: null, host: null, fragment: Fragment);
			}

			else if (routeLink)
			{
				url = urlHelper.RouteUrl(Route, routeValues, protocol: null, host: null, fragment: Fragment);
			}

			else
			{
				url = urlHelper.Action(Action, Controller, routeValues, null, null, Fragment);
			}

			if (string.IsNullOrEmpty(url) is false)
			{
				output.Attributes.SetAttribute(FormAction, url);
			}

			// Set the button appearance
			output.Attributes.SetAttribute(nameof(Appearance), Appearance.ToString().ToLowerInvariant());
			output.Attributes.SetAttribute("type", "submit");
			output.Attributes.SetAttribute(formAction, url);
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
	}
}