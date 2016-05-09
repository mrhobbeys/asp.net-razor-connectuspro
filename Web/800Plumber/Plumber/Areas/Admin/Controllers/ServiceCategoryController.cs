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
    public class ServiceCategoryController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/ServiceCategory/

        public ViewResult Index(int? id)
        {
            if (id.HasValue)
            {
                var location = db.Location.Find(id);
                ViewBag.SelectedLocation = location.LocationName;
                ViewBag.SelectedLocationId = location.LocationId;
                var locationservicecategories = db.LocationServiceCategory.Include(l => l.Location).Include(l => l.ServiceCategory).Where(l => l.LocationId == id);
                return View(locationservicecategories.ToList());
            }
            else return View("Error");
        }

        ////
        //// GET: /Admin/ServiceCategory/Details/5

        //public ViewResult Details(int id)
        //{
        //    LocationServiceCategory locationservicecategory = db.LocationServiceCategories.Find(id);
        //    return View(locationservicecategory);
        //}

        //
        // GET: /Admin/ServiceCategory/Create

        public ActionResult Create(int id)
        {
            ViewBag.LocationId = id;
            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategory, "ServiceCategoryId", "ServiceCategoryName");
            return View();
        } 

        //
        // POST: /Admin/ServiceCategory/Create

        [HttpPost]
        public ActionResult Create(int id, LocationServiceCategory locationservicecategory)
        {
            if (ModelState.IsValid)
            {
                locationservicecategory.LocationId = id;
                db.LocationServiceCategory.Add(locationservicecategory);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });  
            }

            ViewBag.LocationId = id;
            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategory, "ServiceCategoryId", "ServiceCategoryName", locationservicecategory.ServiceCategoryId);
            return View(locationservicecategory);
        }
        
        ////
        //// GET: /Admin/ServiceCategory/Edit/5
 
        //public ActionResult Edit(int id)
        //{
        //    LocationServiceCategory locationservicecategory = db.LocationServiceCategories.Find(id);
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", locationservicecategory.LocationId);
        //    ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategory, "ServiceCategoryId", "ServiceCategoryName", locationservicecategory.ServiceCategoryId);
        //    return View(locationservicecategory);
        //}

        ////
        //// POST: /Admin/ServiceCategory/Edit/5

        //[HttpPost]
        //public ActionResult Edit(LocationServiceCategory locationservicecategory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(locationservicecategory).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", locationservicecategory.LocationId);
        //    ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategory, "ServiceCategoryId", "ServiceCategoryName", locationservicecategory.ServiceCategoryId);
        //    return View(locationservicecategory);
        //}

        //
        // GET: /Admin/ServiceCategory/Delete/5
 
        public ActionResult Delete(int id)
        {
            LocationServiceCategory locationservicecategory = db.LocationServiceCategory.Find(id);
            return View(locationservicecategory);
        }

        //
        // POST: /Admin/ServiceCategory/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            LocationServiceCategory locationservicecategory = db.LocationServiceCategory.Find(id);
            db.LocationServiceCategory.Remove(locationservicecategory);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = locationservicecategory.LocationId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}