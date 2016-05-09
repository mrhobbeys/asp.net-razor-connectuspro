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
    public class CitiesController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Cities/

        public ViewResult Index(int? id)
        {
            if (id.HasValue)
            {
                var location = db.Location.Find(id);
                ViewBag.SelectedLocation = location.LocationName;
                ViewBag.SelectedLocationId = location.LocationId;
                var cityserved = db.CityServed.Include(c => c.Location).Where(c => c.LocationId == id);
                return View(cityserved.ToList());
            }
            return View("Error");
        }

        ////
        //// GET: /Admin/Cities/Details/5

        //public ViewResult Details(int id)
        //{
        //    CityServed cityserved = db.CityServed.Find(id);
        //    return View(cityserved);
        //}

        //
        // GET: /Admin/Cities/Create

        public ActionResult Create(int id)
        {
            ViewBag.LocationId = id;
            return View();
        } 

        //
        // POST: /Admin/Cities/Create

        [HttpPost]
        public ActionResult Create(int id, CityServed cityserved)
        {
            if (ModelState.IsValid)
            {
                cityserved.LocationId = id;
                db.CityServed.Add(cityserved);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });  
            }

            ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", cityserved.LocationId);
            return View(cityserved);
        }
        
        ////
        //// GET: /Admin/Cities/Edit/5
 
        //public ActionResult Edit(int id)
        //{
        //    CityServed cityserved = db.CityServed.Find(id);
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", cityserved.LocationId);
        //    return View(cityserved);
        //}

        ////
        //// POST: /Admin/Cities/Edit/5

        //[HttpPost]
        //public ActionResult Edit(CityServed cityserved)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(cityserved).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", cityserved.LocationId);
        //    return View(cityserved);
        //}

        //
        // GET: /Admin/Cities/Delete/5
 
        public ActionResult Delete(int id)
        {
            CityServed cityserved = db.CityServed.Find(id);
            return View(cityserved);
        }

        //
        // POST: /Admin/Cities/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            CityServed cityserved = db.CityServed.Find(id);
            db.CityServed.Remove(cityserved);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = cityserved.LocationId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}