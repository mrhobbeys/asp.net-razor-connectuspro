using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Models;

namespace SiteBlue.Controllers
{
    public class TechnologyController : Controller
    {
        //
        // GET: /Technology/

        public ActionResult Index()
        {
            ViewBag.PageType = PageType.Technology;
            return View();
        }

    }
}
