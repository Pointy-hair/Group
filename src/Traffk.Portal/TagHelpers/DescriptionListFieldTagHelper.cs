/*
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace TraffkPortal.TagHelpers
{
    [HtmlTargetElement("dt", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("dd", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class DescriptionListFieldTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        public DescriptionListFieldTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        /// <inheritdoc />
        public override int Order
        {
            get
            {
                return -1000;
            }
        }

        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rendering.ViewContext"/> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <inheritdoc />
        /// <remarks>Does nothing if user provides an <c>href</c> attribute.</remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <c>href</c> attribute is provided and <see cref="Action"/>, <see cref="Controller"/>,
        /// <see cref="Fragment"/>, <see cref="Host"/>, <see cref="Protocol"/>, or <see cref="Route"/> are
        /// non-<c>null</c> or if the user provided <c>asp-route-*</c> attributes. Also thrown if <see cref="Route"/>
        /// and one or both of <see cref="Action"/> and <see cref="Controller"/> are non-<c>null</c>.
        /// </exception>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            Generator.
            For.ModelExplorer.GetSimpleDisplayText
            output.h
            output.Content.
            context.
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            // If "href" is already set, it means the user is attempting to use a normal anchor.
            if (output.Attributes.ContainsName(Href))
            {
                if (Action != null ||
                    Controller != null ||
                    Area != null ||
                    Route != null ||
                    Protocol != null ||
                    Host != null ||
                    Fragment != null ||
                    RouteValues.Count != 0)
                {
                    // User specified an href and one of the bound attributes; can't determine the href attribute.
                    throw new InvalidOperationException("onclick was already specified");
                }
            }
            else
            {
                RouteValueDictionary routeValues = null;
                if (_routeValues != null && _routeValues.Count > 0)
                {
                    routeValues = new RouteValueDictionary(_routeValues);
                }

                if (Area != null)
                {
                    if (routeValues == null)
                    {
                        routeValues = new RouteValueDictionary();
                    }

                    // Unconditionally replace any value from asp-route-area. 
                    routeValues["area"] = Area;
                }

                TagBuilder tagBuilder;
                if (Route == null)
                {
                    tagBuilder = Generator.GenerateActionLink(
                        ViewContext,
                        linkText: string.Empty,
                        actionName: Action,
                        controllerName: Controller,
                        protocol: Protocol,
                        hostname: Host,
                        fragment: Fragment,
                        routeValues: routeValues,
                        htmlAttributes: null);
                }
                else if (Action != null || Controller != null)
                {
                    // Route and Action or Controller were specified. Can't determine the href attribute.
                    throw new InvalidOperationException("FormatAnchorTagHelper_CannotDetermineHrefRouteActionOrControllerSpecified");
                }
                else
                {
                    tagBuilder = Generator.GenerateRouteLink(
                        ViewContext,
                        linkText: string.Empty,
                        routeName: Route,
                        protocol: Protocol,
                        hostName: Host,
                        fragment: Fragment,
                        routeValues: routeValues,
                        htmlAttributes: null);
                }

                if (tagBuilder != null)
                {
                    var buttonBuilder = new TagBuilder("button");
                    buttonBuilder.Attributes["onclick"] = $"location='{tagBuilder.Attributes["href"]}'";
                    output.MergeAttributes(buttonBuilder);
                }
            }
        }
    }
}
*/