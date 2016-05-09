using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class TechBenefitController : Controller
    {
        //
        // GET: /TechBenefit/

        public ActionResult Index()
        {
            ViewBag.Title = "Technician Benefit";
            return View();
        }

    }
}
