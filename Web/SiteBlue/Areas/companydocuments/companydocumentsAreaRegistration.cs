using System.Web.Mvc;

namespace SiteBlue.Areas.companydocuments
{
    public class companydocumentsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "companydocuments";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "companydocuments_default",
                "companydocuments/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
