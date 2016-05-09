using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Areas.customers.Controllers
{
    [Authorize]
    public class LandingPageController : Controller
    {
        //
        // GET: /customers/LandingPage/

        public ActionResult Index()
        {
            return View();
        }

    }
}
