using System;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Web.Mvc.Ajax;

public static class HtmlExtensions
{
    /// <summary>
    /// ActionLinkUI.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="action">The action.</param>
    /// <returns>ActionLink string</returns>
    public static MvcHtmlString ActionLinkUI<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string action)
    {
        return ActionLinkUI(htmlHelper, expression, action, null, null, null);
    }

    public static MvcHtmlString ActionLinkUI<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string action, string controller)
    {
        return ActionLinkUI(htmlHelper, expression, action, controller, null, null);
    }

    public static MvcHtmlString ActionLinkUI<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string action, string controller, string area)
    {
        return ActionLinkUI(htmlHelper, expression, action, controller, area, null);
    }

    /// <summary>
    /// ActionLinkUI.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="action">The action.</param>
    /// <param name="icon">The icon.</param>
    /// <returns>ActionLink string</returns>
    public static MvcHtmlString ActionLinkUI<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string action, string controller, string area, string icon)
    {
        return ActionLinkUI(htmlHelper, expression, action, controller, area, icon, null);
    }

    /// <summary>
    /// ActionLinkUI.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="action">The action.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns>ActionLink string</returns>
    public static MvcHtmlString ActionLinkUI<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string action, string controller, string area, string icon, object htmlAttributes)
    {
        string controllerName = string.IsNullOrEmpty(controller) ? htmlHelper.ViewContext.RouteData.Values["controller"].ToString() : controller;
        string areaName = string.IsNullOrEmpty(area) ? "" : area + "/";
        ModelMetadata metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

        TagBuilder a = new TagBuilder("a");
        switch (action)
        {
            case "Index":
                a.Attributes.Add("href", String.Format("/{0}{1}", areaName, controllerName));
                a.Attributes.Add("title", "Back to list");
                break;

            case "Create":
                a.Attributes.Add("href", String.Format("/{0}{1}/{2}", areaName, controllerName, action));
                a.Attributes.Add("title", "Create new");
                break;
            default:
                a.Attributes.Add("href", String.Format("/{0}{1}/{2}/{3}", areaName, controllerName, action, metaData.Model));
                a.Attributes.Add("title", action);
                break;
        }


        a.AddCssClass("actionLinkUI");
        a.AddCssClass("ui-widget");
        a.AddCssClass("ui-state-default");
        a.AddCssClass("ui-corner-all");

        TagBuilder span = new TagBuilder("span");
        span.AddCssClass("ui-icon");
        span.AddCssClass(icon);
        span.InnerHtml = action;
        a.InnerHtml = span.ToString(TagRenderMode.Normal);

        if (htmlAttributes != null)
        {
            a.MergeAttributes(new RouteValueDictionary(htmlAttributes));
        }

        return MvcHtmlString.Create(a.ToString(TagRenderMode.Normal));
    }

    //public static MvcHtmlString LabelForUI(this HtmlHelper helper, string)
    //{

    //}

    //public static MvcHtmlString LabelFor<TModel, TValue>(
    //    this HtmlHelper<TModel> html,
    //    Expression<Func<TModel, TValue>> expression,
    //    object htmlAttributes
    //)
    //{
    //    return LabelHelper(
    //        html,
    //        ModelMetadata.FromLambdaExpression(expression, html.ViewData),
    //        ExpressionHelper.GetExpressionText(expression),
    //        htmlAttributes
    //    );
    //}

    //private static MvcHtmlString LabelHelper(
    //    HtmlHelper html,
    //    ModelMetadata metadata,
    //    string htmlFieldName,
    //    object htmlAttributes
    //)
    //{
    //    string resolvedLabelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
    //    if (string.IsNullOrEmpty(resolvedLabelText))
    //    {
    //        return MvcHtmlString.Empty;
    //    }

    //    TagBuilder tag = new TagBuilder("label");
    //    tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
    //    tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
    //    tag.SetInnerText(resolvedLabelText);
    //    return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
    //}
}
