using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class PlumbingController : Controller
    {
        //
        // GET: /Plumbing/

        public ActionResult Index()
        {
            ViewBag.Title = "Plumbing Services Overview";
            return View();
        }

    }
}
