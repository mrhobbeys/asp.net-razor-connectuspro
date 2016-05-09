using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class WhoController : Controller
    {
        //
        // GET: /Who/

        public ActionResult Index()
        {
            ViewBag.Current = "Who";
            return View();
        }

    }
}
