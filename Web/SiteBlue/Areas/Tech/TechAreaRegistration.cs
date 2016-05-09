using System.Web.Mvc;

namespace SiteBlue.Areas.Tech
{
    public class TechAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Tech";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Tech_default",
                "Tech/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
