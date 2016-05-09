using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Controllers
{
    public class TerritoryOwnersController : Controller
    {

        private PlumberContext db = new PlumberContext();
        //
        // GET: /TerritoryOwners/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var territoryOwner = db.TerritoryOwners.Find(id);
            if (territoryOwner != null)
                return View(territoryOwner);
            else
                throw new HttpException(404, "Not found");
        }

        // Franchise-02
        public ActionResult Owners()
        {
            var territoryOwners = db.TerritoryOwners.ToList();
            return View(territoryOwners);
        }

        // Franchise Vid
        public ActionResult Media()
        {
            return View();
        }

        // Application
        public ActionResult Application()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Application(object model)
        {
            return View();
        }

        public ActionResult TTab()
        {
            return View();
        }

    }
}
