using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class HvacDescriptionController : Controller
    {
        //
        // GET: /HVAC/

        public ActionResult Index()
        {
            ViewBag.Title = "HVAC Services Overview";
            return View();
        }

    }
}
