using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,ContentManager")]
    public class ManagedServicesController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Services/

        public ViewResult Index(int? id)
        {
            if (id.HasValue)
            {
                var location = db.Location.Find(id);
                ViewBag.SelectedLocation = location.LocationName;
                ViewBag.SelectedLocationId = location.LocationId;
                var locationservices = db.LocationServices.Include(l => l.Location).Include(l => l.Service).Where(ls => ls.LocationId == id);
                return View(locationservices.ToList());
            }
            else return View("Error");
        }

        ////
        //// GET: /Admin/Services/Details/5

        //public ViewResult Details(int id)
        //{
        //    LocationService locationservice = db.LocationServices.Find(id);
        //    return View(locationservice);
        //}

        //
        // GET: /Admin/Services/Create

        public ActionResult Create(int id)
        {
            ViewBag.LocationId = id;
            var services = from s in db.Service
                                where !(from fs in db.LocationServices
                                        where (fs.LocationId == id)
                                        select fs.ServiceId).Contains(s.ServiceId)
                                select s;

            ViewBag.ServicesCount = services.Count();
            ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
            return View();
        } 

        //
        // POST: /Admin/Services/Create

        [HttpPost]
        public ActionResult Create(int id, LocationService locationservice)
        {
            if (ModelState.IsValid)
            {
                locationservice.LocationId = id;
                db.LocationServices.Add(locationservice);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });  
            }

            ViewBag.LocationId = id;
            var services = from s in db.Service
                           where !(from fs in db.LocationServices
                                   where (fs.LocationId == id)
                                   select fs.ServiceId).Contains(s.ServiceId)
                           select s;
            ViewBag.ServicesCount = services.Count();
            ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName", locationservice.ServiceId);
            return View(locationservice);
        }
        
        ////
        //// GET: /Admin/Services/Edit/5
 
        //public ActionResult Edit(int id)
        //{
        //    LocationService locationservice = db.LocationServices.Find(id);
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", locationservice.LocationId);
        //    ViewBag.ServiceId = new SelectList(db.Service, "ServiceId", "ServiceName", locationservice.ServiceId);
        //    return View(locationservice);
        //}

        ////
        //// POST: /Admin/Services/Edit/5

        //[HttpPost]
        //public ActionResult Edit(LocationService locationservice)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(locationservice).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", locationservice.LocationId);
        //    ViewBag.ServiceId = new SelectList(db.Service, "ServiceId", "ServiceName", locationservice.ServiceId);
        //    return View(locationservice);
        //}

        //
        // GET: /Admin/Services/Delete/5
 
        public ActionResult Delete(int id)
        {
            LocationService locationservice = db.LocationServices.Find(id);
            return View(locationservice);
        }

        //
        // POST: /Admin/Services/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            LocationService locationservice = db.LocationServices.Find(id);
            db.LocationServices.Remove(locationservice);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = locationservice.LocationId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}