﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace TraffkPortal
{
    public static class AspHelpers
    {
        public const string PleaseSelectOneDropdownItemText = "-- Please Select --";
        public const string NoneDropdownItemText = "-- None --";
        public const string NoneDropdownItemValue = "vv--None--^^";

        public enum MainNavigationPageKeys
        {
            Main,
            Messaging,
            Reporting,
            CRM,
            Setup,
            Manage,
            NotSpecified,
        }

        public static class ViewDataKeys
        {
            public const string Heading = "Heading";
            public const string SubHeading = "SubHeading";
            public const string MainNavPageKey = "MainNavPageKey";
            public const string ParentEntityId = "ParentEntityId";
            public const string EntityId = "EntityId";
            public const string EntityTitle = "EntityTitle";
            public const string EntityType = "EntityType";
            public const string PageKey = "PageKey";
            public const string TenantName = "TenantName";
        }

        public static SelectListItem CreateNoneSelectedSelectListItem(bool preSelected=true) 
            => new SelectListItem { Disabled = false, Text = NoneDropdownItemText, Selected = preSelected, Value = NoneDropdownItemValue };

        public static SelectListItem CreatePleaseSelectListItem(bool preSelected=true)
            => new SelectListItem { Disabled = true, Text = PleaseSelectOneDropdownItemText, Selected = preSelected };

        public const string UntouchedPassword = "_UNTOUCHED_AaBbCc1122##44%%66_19740409_JBT_password";

        public static string UntouchedPasswordOrEmpty(string existingPassword)
        {
            return string.IsNullOrWhiteSpace(existingPassword) ? "" : UntouchedPassword;
        }

        public static string GetDisplayName(Enum e) => e.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? e.ToString();

        public const string SortDirAscending = "asc";
        public const string SortDirDescending = "desc";

        public static bool IsSortDirAscending(string sortDir)
        {
            return !StringHelpers.IsSameIgnoreCase(sortDir, SortDirDescending);
        }

        public static Task<IHtmlContent> PartialAsync<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, string partialViewName, object model, Expression<Func<TModel, TResult>> containerExpression)
        {
            var containerName = containerExpression.GetName();
            return htmlHelper.PartialAsync(partialViewName, model, containerName);
        }

        public static Task<IHtmlContent> PartialAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, string partialViewName, object model, string containerName)
        {
            var vdd = new ViewDataDictionary(htmlHelper.ViewData);
            vdd.TemplateInfo.HtmlFieldPrefix = containerName;
            return htmlHelper.PartialAsync(partialViewName, model, vdd);
        }

        /*
        public static ViewDataDictionary<TModel> CreateContainedModel<TModel>(this HttpContext httpContext, TModel model)
        {
            ViewDataDictionary<TModel> vdd;
            vdd.
            var vdd = new ViewDataDictionary<TModel>(httpContext.Response.sou)
            ViewDataDictionary vdd
            vdd.ModelExplorer.Container.
            new ViewDataDictionary<int>() {   }
            throw new NotImplementedException();
        }
        */

        private class RawHtmlContent : IHtmlContent
        {
            private readonly string Html;

            public RawHtmlContent(string html)
            {
                Html = html;
            }
            void IHtmlContent.WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                writer.Write(encoder.Encode(Html));
            }
        }

        private class JoinHtmlContent : IHtmlContent
        {
            private readonly IHtmlContent[] Contents;

            public JoinHtmlContent(params IHtmlContent [] contents)
            {
                Contents = contents;
            }

            void IHtmlContent.WriteTo(TextWriter writer, HtmlEncoder encoder) => Contents.ForEach(c => c.WriteTo(writer, encoder));
        }


        public static string CreateActionUrl(this IHtmlHelper hh, string actionName, string controllerName = null, object routeValues = null)
        {
            var atag = (TagBuilder)hh.ActionLink("aaa", actionName, controllerName, routeValues, null);
            return atag.Attributes["href"];
        }

        public static IHtmlContent DescriptionListElement<TModelItem, TResult>(this IHtmlHelper<TModelItem> hh, Expression<Func<TModelItem, TResult>> columnExpression)
        {
            var name = hh.FriendlyNameFor(columnExpression);
            var dt = new TagBuilder("dt");
            dt.InnerHtml.Append(name);

            var val = hh.DisplayFor(columnExpression);
            var dd = new TagBuilder("dd");
            dd.InnerHtml.AppendHtml(val);

            return new JoinHtmlContent(dt, dd);
        }

        public static string FriendlyNameFor<TModelItem, TResult>(this IHtmlHelper<TModelItem> hh, Expression<Func<TModelItem, TResult>> columnExpression)
        {
            var fieldMemberInfo = columnExpression.GetMembers().Last();
            if (fieldMemberInfo != null)
            {
                var dna = fieldMemberInfo.GetCustomAttribute<DisplayNameAttribute>();
                if (dna != null) return dna.DisplayName;
            }
            return fieldMemberInfo.Name;
        }

        public static string FriendlyNameFor<TModelItem, TResult>(this IHtmlHelper<IEnumerable<TModelItem>> hh, Expression<Func<TModelItem, TResult>> columnExpression)
        {
            try
            {
                var fieldMemberInfo = columnExpression.GetMembers().Last();
                if (fieldMemberInfo != null)
                {
                    var dna = fieldMemberInfo.GetCustomAttribute<DisplayNameAttribute>();
                    if (dna != null) return dna.DisplayName;
                }
            }
            catch { }
            return hh.DisplayNameFor(columnExpression);
        }

        public static IHtmlContent SortableHeaderFor<TModelItem, TResult>(this IHtmlHelper<IEnumerable<TModelItem>> hh, Expression<Func<TModelItem, TResult>> columnExpression, string currentSortColName = null, string currentSortDir = null, string actionName = null)
        {
            currentSortColName = currentSortColName ?? hh.ViewBag.SortCol as string;
            currentSortDir = currentSortDir ?? hh.ViewBag.SortDir as string;

            actionName = actionName ?? hh.ViewContext.RouteData.Values["action"] as string;
            var colName = columnExpression.GetName();
            var displayName = hh.FriendlyNameFor(columnExpression);
            if (colName == currentSortColName)
            {
                var tb = hh.ActionLink(
                    displayName,
                    actionName,
                    new { sortCol = colName, sortDir = IsSortDirAscending(currentSortDir) ? SortDirDescending : SortDirAscending }) as TagBuilder;
                tb.InnerHtml.AppendHtml(currentSortDir == SortDirAscending ? "<span class='caret-up'></span>" : "<span class='caret-down'></span>");
                return tb;
            }
            else
            {
                return hh.ActionLink(displayName, actionName, new { sortCol = colName, sortDir = SortDirAscending });
            }
        }

        public static IHtmlContent ReplayablePasswordFor<TModel, TResult>(this IHtmlHelper<TModel> hh, Expression<Func<TModel, TResult>> passwordExpression, Expression<Func<TModel, TResult>> confirmExpression, object htmlAttributes)
        {
            var password = passwordExpression.Compile()(hh.ViewData.Model) as string;
            if (!string.IsNullOrEmpty(password))
            {
                var confirm = confirmExpression.Compile()(hh.ViewData.Model) as string;
                if (password == confirm)
                {
                    var tb = hh.TextBoxFor(passwordExpression, htmlAttributes) as TagBuilder;
                    if (tb != null)
                    {
                        tb.Attributes["type"] = "password";
                        tb.Attributes["value"] = UntouchedPassword;
                        return tb;
                    }
                }
            }
            return hh.PasswordFor(passwordExpression, htmlAttributes);
        }

        public static void SetIfMissing(this ViewDataDictionary v, string key, object val)
        {
            if (!v.ContainsKey(key))
            {
                v[key] = val;
            }
        }

        public static void SetTitleAndHeading(this ViewDataDictionary v, string fallbackTitle, string subHeading = null)
        {
            v.SetIfMissing("Title", fallbackTitle);
            v["Heading"] = v["Title"];
            if (subHeading != null)
            {
                v["SubHeading"] = subHeading;
            }
        }

        public static object GetRouteValue(this ViewContext c, string key=null)
        {
            return c.ModelState[key??"id"].RawValue;
        }

        public static Uri GetRequestUrl(this HttpContext c)
        {
            var req = c.Request;
            var ub = new UriBuilder(req.Scheme, req.Host.Host);
            if (req.Host.Port != null)
            {
                ub.Port = req.Host.Port.Value;
            }
            ub.Path = req.Path;
            ub.Query = req.QueryString.Value;
            return ub.Uri;
        }

        public static string GetRequestRootUrl(this HttpContext c)
        {
            return c.GetRequestUrl().GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
        }

        /// <remarks>https://rburnham.wordpress.com/2015/03/13/asp-net-mvc-defining-scripts-in-partial-views/</remarks>
        public static HtmlString Script(this IHtmlHelper htmlHelper, Func<object, Microsoft.AspNetCore.Mvc.Razor.HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return HtmlString.Empty;
        }

        /// <remarks>https://rburnham.wordpress.com/2015/03/13/asp-net-mvc-defining-scripts-in-partial-views/</remarks>
        public static HtmlString RenderPartialViewScripts(this IHtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, Microsoft.AspNetCore.Mvc.Razor.HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return HtmlString.Empty;
        }
    }
}