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
    public class ServiceController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Service/

        public ViewResult Index()
        {
            return View(db.Service.ToList());
        }

        //
        // GET: /Admin/Service/Details/5

        public ViewResult Details(int id)
        {
            Service service = db.Service.Find(id);
            return View(service);
        }

        //
        // GET: /Admin/Service/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Service/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                db.Service.Add(service);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(service);
        }
        
        //
        // GET: /Admin/Service/Edit/5
 
        public ActionResult Edit(int id)
        {
            Service service = db.Service.Find(id);
            return View(service);
        }

        //
        // POST: /Admin/Service/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(service);
        }

        //
        // GET: /Admin/Service/Delete/5
 
        public ActionResult Delete(int id)
        {
            Service service = db.Service.Find(id);
            return View(service);
        }

        //
        // POST: /Admin/Service/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Service service = db.Service.Find(id);
            db.Service.Remove(service);
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