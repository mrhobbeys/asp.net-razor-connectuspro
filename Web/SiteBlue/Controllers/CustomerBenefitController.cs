using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class CustomerBenefitController : Controller
    {
        //
        // GET: /CustomerBenefit/

        public ActionResult Index()
        {
            ViewBag.Title = "Customer Focused";
            return View();
        }

    }
}
