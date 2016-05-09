using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Areas.companydocuments.Controllers
{
    [Authorize]
    public class LandingPageController : Controller
    {
        //
        // GET: /companydocuments/LandingPage/

        public ActionResult Index()
        {
            return View();
        }

    }
}
