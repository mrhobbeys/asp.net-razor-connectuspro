using System.Web.Mvc;

namespace SiteBlue.Areas.companyinfo
{
    public class companyinfoAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "companyinfo";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "companyinfo_default",
                "companyinfo/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
