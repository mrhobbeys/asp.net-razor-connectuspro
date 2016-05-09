using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class ElectricalController : Controller
    {
        //
        // GET: /Electrical/

        public ActionResult Index()
        {
            ViewBag.Title = "Electrical Services Overview";
            return View();
        }

    }
}
