using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;
using System.IO;

namespace Plumber.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,ContentManager")]
    public class CouponController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Coupon/

        public ViewResult Index()
        {
            return View(db.Offer.ToList());
        }

        //
        // GET: /Admin/Coupon/Details/5

        public ViewResult Details(int id)
        {
            Offer offer = db.Offer.Find(id);
            return View(offer);
        }

        //
        // GET: /Admin/Coupon/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Coupon/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Offer offer, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Uploads"), fileName);
                        file.SaveAs(path);
                        offer.ImageUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Offer.Add(offer);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(offer);
        }
        
        //
        // GET: /Admin/Coupon/Edit/5
 
        public ActionResult Edit(int id)
        {
            Offer offer = db.Offer.Find(id);
            return View(offer);
        }

        //
        // POST: /Admin/Coupon/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Offer offer, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Uploads"), fileName);
                        file.SaveAs(path);
                        offer.ImageUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Entry(offer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(offer);
        }

        //
        // GET: /Admin/Coupon/Delete/5
 
        public ActionResult Delete(int id)
        {
            Offer offer = db.Offer.Find(id);
            return View(offer);
        }

        //
        // POST: /Admin/Coupon/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Offer offer = db.Offer.Find(id);
            db.Offer.Remove(offer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}