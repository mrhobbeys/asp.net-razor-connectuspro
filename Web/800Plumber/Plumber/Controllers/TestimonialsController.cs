using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Controllers
{
    public class TestimonialsController : Controller
    {
        private PlumberContext db = new PlumberContext();

        public ActionResult Index()
        {
            return View(db.Testimonial.Where(t => t.IsDeleted == false).Where(t => t.IsArchived == false).OrderByDescending(t => t.PublicationDate).ToList());
        }

    }
}
