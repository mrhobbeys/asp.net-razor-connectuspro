using System.Web.Mvc;

namespace SiteBlue.Areas.LandingPage
{
    public class LandingPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LandingPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LandingPage_default",
                "LandingPage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
