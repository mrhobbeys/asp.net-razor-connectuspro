using System.Web.Mvc;

namespace SiteBlue.Areas.MyFinances
{
    public class MyFinancesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MyFinances";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MyFinances_default",
                "MyFinances/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
