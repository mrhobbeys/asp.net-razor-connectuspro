using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Controllers
{
    public class MediaAndPressController : Controller
    {

        private PlumberContext db = new PlumberContext();

        public ActionResult Index()
        {
            ViewBag.Title = "Plumbers and Plumbing Repair Services | 1-800-PLUMBER";
            return View(db.Media.OrderByDescending(m => m.PublicationDate).ToList());
        }

        public ActionResult Release(int id)
        {
            ViewBag.Title = "Plumbers and Plumbing Repair Services | 1-800-PLUMBER";
            var media = db.Media.Find(id);
            return View(media);
        }

    }
}
