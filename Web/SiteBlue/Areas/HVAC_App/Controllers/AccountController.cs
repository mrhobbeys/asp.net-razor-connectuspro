using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SiteBlue;
using SiteBlue.Areas.HVAC_App.Controllers;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;

namespace HVACapp.Areas.HVAC_App.Controllers
{
    public class AccountController : HVACController
    {
        [HttpGet]
        public ActionResult Login(string input_login, string input_password, int input_rem)
        {
            if (authenticationService.LogOn(input_login, input_password, input_rem == 1))
            {
                var user = membershipService.GetUser(input_login);
                SetCookies((Guid)user.ProviderUserKey); 
                //Response.Cookies.Add(new HttpCookie("franchise_id", "51"));
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
           return Json(new { result = "fail" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IsLogged()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = membershipService.GetUser(User.Identity.Name);
                SetCookies((Guid)user.ProviderUserKey);
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }

        [HttpPost]
        public ActionResult Logout()
        {
            ConfigId.ClearStoreItem();
            //var cookieConfig = Request.Cookies["id_config"];
            //if (cookieConfig != null) cookieConfig.Expires = DateTime.Now;

            var cookieJob = Request.Cookies["id_job"];
            if (cookieJob != null) cookieJob.Expires = DateTime.Now;

            authenticationService.LogOff();
            SessionContainer.Remove();
            //var cookieFranchise = Request.Cookies["franchise_id"];
            //if (cookieFranchise != null) cookieFranchise.Expires = DateTime.Now.AddDays(7);

            return Json(new {result = "success"});
        }


    }
}
