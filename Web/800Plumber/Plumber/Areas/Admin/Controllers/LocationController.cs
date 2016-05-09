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
    public class LocationController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Location/

        public ViewResult Index()
        {
            return View(db.Location.ToList());
        }

        //
        // GET: /Admin/Location/Details/5

        public ViewResult Details(int id)
        {
            Location location = db.Location.Find(id);
            return View(location);
        }

        //
        // GET: /Admin/Location/Create

        public ActionResult Create()
        {
            ViewBag.FranchiseID = new SelectList(db.Franchise, "FranchiseId", "ShipCompany");
            ViewBag.TemplateId = new SelectList(db.Template, "TemplateId", "TemplateName");
            return View();
        } 

        //
        // POST: /Admin/Location/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Location location)
        {
            if (ModelState.IsValid)
            {
                location.IsDeleted = false;
                db.Location.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.FranchiseID = new SelectList(db.Franchise, "FranchiseId", "ShipCompany");
            ViewBag.TemplateId = new SelectList(db.Template, "TemplateId", "TemplateName");
            return View(location);
        }
        
        //
        // GET: /Admin/Location/Edit/5
 
        public ActionResult Edit(int id)
        {
            Location location = db.Location.Find(id);
            ViewBag.FranchiseID = new SelectList(db.Franchise, "FranchiseID", "ShipCompany", location.FranchiseID);
            ViewBag.TemplateId = new SelectList(db.Template, "TemplateId", "TemplateName", location.TemplateId);
            return View(location);
        }

        //
        // POST: /Admin/Location/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                location.IsDeleted = false;
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FranchiseID = new SelectList(db.Franchise, "FranchiseID", "ShipCompany", location.FranchiseID);
            ViewBag.TemplateId = new SelectList(db.Template, "TemplateId", "TemplateName", location.TemplateId);
            return View(location);
        }

        //
        // GET: /Admin/Location/Delete/5
 
        public ActionResult Delete(int id)
        {
            Location location = db.Location.Find(id);
            return View(location);
        }

        //
        // POST: /Admin/Location/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Location location = db.Location.Find(id);
            db.Location.Remove(location);
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