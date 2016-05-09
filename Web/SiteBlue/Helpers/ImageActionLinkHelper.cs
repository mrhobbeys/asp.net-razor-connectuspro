using System.Web.Mvc;
using System.Web.Mvc.Ajax;

public static class ImageActionLinkHelper
{
    public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttribute)
    {
        var builder = new TagBuilder("img");
        builder.MergeAttribute("src", imageUrl);
        builder.MergeAttribute("alt", "");
        var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions, htmlAttribute);
        var html = link.ToHtmlString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        return new MvcHtmlString(html);
    }
}
