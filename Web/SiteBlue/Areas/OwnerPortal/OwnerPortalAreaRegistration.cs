using System.Web.Mvc;

namespace SiteBlue.Areas.OwnerPortal
{
    public class OwnerPortalAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OwnerPortal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OwnerPortal_default",
                "OwnerPortal/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
