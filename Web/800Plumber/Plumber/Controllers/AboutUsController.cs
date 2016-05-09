using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Controllers
{
    public class AboutUsController : Controller
    {
        private PlumberContext db = new PlumberContext();
        //
        // GET: /AboutUs/

        public ActionResult Index()
        {
            //var managements = new ManagementModel().GetManagements();
            var managements = db.Management.ToList();
            return View(managements);
        }

        public ActionResult Details(int id)
        {
            var management = db.Management.Find(id);
            if (management != null)
                return View(management);
            else
                throw new HttpException(404, "Not found");
        }

    }
}
