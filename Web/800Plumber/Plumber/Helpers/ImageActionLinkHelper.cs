using System.Web.Mvc;
using System.Web.Mvc.Ajax;

public static class ImageActionLinkHelper
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="imageUrl">Image URL</param>
    /// <param name="actionName">Action Name</param>
    /// <returns></returns>
    public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string actionName)
    {
        return ImageActionLink(helper, imageUrl, actionName, null, null, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="imageUrl">Image URL</param>
    /// <param name="actionName">Action Name</param>
    /// <param name="ajaxOptions">Ajax Options</param>
    /// <returns></returns>
    public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string actionName, AjaxOptions ajaxOptions)
    {
        return ImageActionLink(helper, imageUrl, actionName, null, null, ajaxOptions, null);
    }

    public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttribute)
    {
        return ImageActionLink(helper, imageUrl, actionName, null, routeValues, ajaxOptions, htmlAttribute);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="imageUrl">Image URL</param>
    /// <param name="actionName">Action Name</param>
    /// <param name="routeValues">Route Values</param>
    /// <param name="ajaxOptions">Ajax Options</param>
    /// <param name="htmlAttribute">HTML Attributes</param>
    /// <returns></returns>
    public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttribute)
    {
        var builder = new TagBuilder("img");
        builder.MergeAttribute("src", imageUrl);
        builder.MergeAttribute("alt", "");
        var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, htmlAttribute);
        var html = link.ToHtmlString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        return new MvcHtmlString(html);
    }
}
