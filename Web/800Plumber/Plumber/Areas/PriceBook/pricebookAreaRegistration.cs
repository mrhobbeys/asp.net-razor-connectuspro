using System.Web.Mvc;

namespace SiteBlue.Areas.PriceBook
{
    public class PriceBookAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PriceBook";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PriceBook_default",
                //"{controller}/{action}/{id}",
                //new { controller = "PriceBook", action = "Index", id = UrlParameter.Optional }
                "PriceBook/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
