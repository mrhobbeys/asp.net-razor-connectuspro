using System.Web.Mvc;

namespace HVACapp.Areas.HVAC_App
{
    public class HVAC_AppAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HVAC_App";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HVAC_App_default",
                "HVAC_App/{controller}/{action}/{id}",
                new { controller="Wizard", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
