using System.Web.Mvc;

namespace SiteBlue.Areas.CallCenter
{
    public class CallCenterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CallCenter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "CallCenter_default",
                "CallCenter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
             );

            /*context.MapRoute(
                "CallCenter_default1",
                "CallCenter/{controller}/{action}/{id}/{id2}",
                new { action = "Index", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
            );*/
 
        }
    }
}
