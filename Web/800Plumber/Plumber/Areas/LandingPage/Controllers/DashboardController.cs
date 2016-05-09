using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Areas.LandingPage.Controllers
{
    public class DashboardController : Controller
    {
        private PlumberContext db = new PlumberContext();
        //
        // GET: /LandingPage/Dashboard/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Training()
        {
            var trainings = db.Training.Where(t => t.SiteId == 2).Where(t => t.IsActive == true).OrderBy(t => t.Title).ToList();
            return View(trainings);
        }

    }
}
