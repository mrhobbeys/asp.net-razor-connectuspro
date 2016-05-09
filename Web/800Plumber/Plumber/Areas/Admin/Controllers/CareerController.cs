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
    public class CareerController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Careers/

        public ViewResult Index()
        {
            return View(db.Career.OrderByDescending(c => c.PublicationDate).ToList());
        }

        //
        // GET: /Admin/Careers/Details/5

        public ViewResult Details(int id)
        {
            Career career = db.Career.Find(id);
            return View(career);
        }

        //
        // GET: /Admin/Careers/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Careers/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Career career)
        {
            career.IsArchived = false;
            career.IsDeleted = false;
            if (ModelState.IsValid)
            {
                db.Career.Add(career);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(career);
        }
        
        //
        // GET: /Admin/Careers/Edit/5
 
        public ActionResult Edit(int id)
        {
            Career career = db.Career.Find(id);
            return View(career);
        }

        //
        // POST: /Admin/Careers/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Career career)
        {
            if (ModelState.IsValid)
            {
                db.Entry(career).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(career);
        }

        //
        // GET: /Admin/Careers/Delete/5
 
        public ActionResult Delete(int id)
        {
            Career career = db.Career.Find(id);
            return View(career);
        }

        //
        // POST: /Admin/Careers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Career career = db.Career.Find(id);
            career.IsDeleted = true;
            db.Entry(career).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { });
        }

        public ActionResult ChangeStatus(int id, bool status)
        {
            Career career = db.Career.Find(id);
            career.IsArchived = status;
            db.Entry(career).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ReAdd(int id)
        {
            Career career = db.Career.Find(id);
            career.IsDeleted = false;
            db.Entry(career).State = EntityState.Modified;
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