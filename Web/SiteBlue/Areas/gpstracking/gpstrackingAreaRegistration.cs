using System.Web.Mvc;

namespace SiteBlue.Areas.gpstracking
{
    public class gpstrackingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "gpstracking";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "gpstracking_default",
                "gpstracking/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
