using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Models;

namespace SiteBlue.Controllers
{
    public class TestimonialsController : Controller
    {
        //
        // GET: /Testimonials/

        public ActionResult Index()
        {
            ViewBag.Title = "Testimonials";
            ViewBag.ActivePage = WebPage.AboutUs;
            return View();
        }

    }
}
