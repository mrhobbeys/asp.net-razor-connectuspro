using System.Web.Mvc;

namespace SiteBlue.Areas.MyCalls
{
    public class MyCallsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MyCalls";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MyCalls_default",
                "MyCalls/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
