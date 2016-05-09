using System.Web.Mvc;

namespace SiteBlue.Areas.connectuspoints
{
    public class connectuspointsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "connectuspoints";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "connectuspoints_default",
                "connectuspoints/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
