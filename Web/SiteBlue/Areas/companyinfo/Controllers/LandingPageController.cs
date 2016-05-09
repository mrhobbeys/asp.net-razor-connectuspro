using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Areas.companyinfo.Controllers
{
    [Authorize]
    public class LandingPageController : Controller
    {
        //
        // GET: /companyinfo/LandingPage/

        public ActionResult Index()
        {
            return View();
        }

    }
}
