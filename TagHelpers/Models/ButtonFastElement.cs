using System;
using System.Collections.Generic;

using CRM.TagHelpers.Enums;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.Models
{
	/// <summary>
	/// <see cref="TagHelper"/> implementation targeting &lt;fast-button&gt; elements.
	/// </summary>
	public class ButtonFastElement : TagHelper
	{
		protected const string RouteValuesDictionaryName = "asp-all-route-data";
		protected const string PageHandlerAttributeName = "asp-page-handler";
		protected const string ControllerAttributeName = "asp-controller";
		protected const string FragmentAttributeName = "asp-fragment";
		protected const string ActionAttributeName = "asp-action";
		protected const string RouteValuesPrefix = "asp-route-";
		protected const string RouteAttributeName = "asp-route";
		protected readonly IUrlHelperFactory UrlHelperFactory;
		protected const string PageAttributeName = "asp-page";
		protected const string AreaAttributeName = "asp-area";
		protected const string FormAction = "formaction";
		private IDictionary<string, string>? routeValues;

		/// <summary>
		/// The appearance of the FAST button element.
		/// </summary>
		/// <remarks>
		/// Passed through to the generated HTML in all cases. Defaults to <c>neutral</c>.
		/// </remarks>
		public ButtonAppearance Appearance { get; set; }

		/// <summary>
		/// The <see cref="Enums.DesignSystem"/> implementation.
		/// </summary>
		public DesignSystem DesignSystem { get; set; }

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
		[HtmlAttributeName(ActionAttributeName)]
		public string? Action { get; set; }

		/// <summary>
		/// The name of the controller.
		/// </summary>
		[HtmlAttributeName(ControllerAttributeName)]
		public string? Controller { get; set; }

		/// <summary>
		/// The name of the area.
		/// </summary>
		[HtmlAttributeName(AreaAttributeName)]
		public string? Area { get; set; }

		/// <summary>
		/// The name of the page.
		/// </summary>
		[HtmlAttributeName(PageAttributeName)]
		public string? Page { get; set; }

		/// <summary>
		/// The name of the page handler.
		/// </summary>
		[HtmlAttributeName(PageHandlerAttributeName)]
		public string? PageHandler { get; set; }

		/// <summary>
		/// Gets or sets the URL fragment.
		/// </summary>
		[HtmlAttributeName(FragmentAttributeName)]
		public string? Fragment { get; set; }

		/// <summary>
		/// Name of the route.
		/// </summary>
		/// <remarks>
		/// Must be <c>null</c> if <see cref="Action"/> or <see cref="Controller"/> is non-<c>null</c>.
		/// </remarks>
		[HtmlAttributeName(RouteAttributeName)]
		public string? Route { get; set; }

		/// <summary>
		/// Additional parameters for the route.
		/// </summary>
		[HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
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
		/// Creates a new <see cref="ButtonFastElement"/>.
		/// </summary>
		/// <param name="urlHelperFactory">The <see cref="IUrlHelperFactory"/>.</param>
		public ButtonFastElement(IUrlHelperFactory urlHelperFactory)
		{
			UrlHelperFactory = urlHelperFactory;
		}
	}
}