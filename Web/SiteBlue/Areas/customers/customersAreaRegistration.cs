using System.Web.Mvc;

namespace SiteBlue.Areas.customers
{
    public class customersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "customers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "customers_default",
                "customers/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
