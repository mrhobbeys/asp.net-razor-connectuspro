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
    public class ManagedCouponsController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/ManagedCoupons/

        public ViewResult Index(int? id)
        {
            if (id.HasValue)
            {
                var location = db.Location.Find(id);
                ViewBag.SelectedLocation = location.LocationName;
                ViewBag.SelectedLocationId = location.LocationId;
                var locationcoupons = db.LocationCoupons.Include(l => l.Location).Include(l => l.Offer).Where(ls => ls.LocationId == id);
                return View(locationcoupons.ToList());
            }
            return View("Error");
        }

        //
        // GET: /Admin/ManagedCoupons/Details/5

        //public ViewResult Details(int id)
        //{
        //    LocationCoupon locationcoupon = db.LocationCoupons.Find(id);
        //    return View(locationcoupon);
        //}

        //
        // GET: /Admin/ManagedCoupons/Create

        public ActionResult Create(int id)
        {
            ViewBag.LocationId = id;
            var offers = from s in db.Service
                           where !(from fs in db.LocationServices
                                   where (fs.LocationId == id)
                                   select fs.ServiceId).Contains(s.ServiceId)
                           select s;

            ViewBag.OffersCount = offers.Count();
            ViewBag.OfferId = new SelectList(offers, "OfferId", "Title");
            return View();
        } 

        //
        // POST: /Admin/ManagedCoupons/Create

        [HttpPost]
        public ActionResult Create(int id, LocationCoupon locationcoupon)
        {
            if (ModelState.IsValid)
            {
                locationcoupon.LocationId = id;
                db.LocationCoupons.Add(locationcoupon);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });  
            }

            ViewBag.LocationId = id;
            var offers = from s in db.Service
                         where !(from fs in db.LocationServices
                                 where (fs.LocationId == id)
                                 select fs.ServiceId).Contains(s.ServiceId)
                         select s;

            ViewBag.OffersCount = offers.Count();
            ViewBag.OfferId = new SelectList(offers, "OfferId", "Title", locationcoupon.OfferId);
            return View(locationcoupon);
        }
        
        ////
        //// GET: /Admin/ManagedCoupons/Edit/5
 
        //public ActionResult Edit(int id)
        //{
        //    LocationCoupon locationcoupon = db.LocationCoupons.Find(id);
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", locationcoupon.LocationId);
        //    ViewBag.OfferId = new SelectList(db.Offer, "OfferId", "Title", locationcoupon.OfferId);
        //    return View(locationcoupon);
        //}

        ////
        //// POST: /Admin/ManagedCoupons/Edit/5

        //[HttpPost]
        //public ActionResult Edit(LocationCoupon locationcoupon)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(locationcoupon).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.LocationId = new SelectList(db.Location, "LocationId", "LocationName", locationcoupon.LocationId);
        //    ViewBag.OfferId = new SelectList(db.Offer, "OfferId", "Title", locationcoupon.OfferId);
        //    return View(locationcoupon);
        //}

        //
        // GET: /Admin/ManagedCoupons/Delete/5
 
        public ActionResult Delete(int id)
        {
            LocationCoupon locationcoupon = db.LocationCoupons.Find(id);
            return View(locationcoupon);
        }

        //
        // POST: /Admin/ManagedCoupons/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            LocationCoupon locationcoupon = db.LocationCoupons.Find(id);
            db.LocationCoupons.Remove(locationcoupon);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = locationcoupon.LocationId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}