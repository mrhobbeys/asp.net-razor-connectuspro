using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class HowController : Controller
    {
        //
        // GET: /How/

        public ActionResult Index()
        {
            ViewBag.Current = "How";
            return View();
        }

    }
}
