using System.Web.Mvc;

namespace SiteBlue.Areas.shopping
{
    public class shoppingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "shopping";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "shopping_default",
                "shopping/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
