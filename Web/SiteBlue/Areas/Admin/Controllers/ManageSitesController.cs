using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,TrainingManager")]
    public class ManageSitesController : Controller
    {
        private EightHundredEntities db = new EightHundredEntities();

        //
        // GET: /Admin/ManageSites/

        public ViewResult Index()
        {
            return View(db.Sites.ToList());
        }

        //
        // GET: /Admin/ManageSites/Details/5

        public ViewResult Details(int id)
        {
            Site site = db.Sites.Single(s => s.SiteId == id);
            return View(site);
        }

        //
        // GET: /Admin/ManageSites/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/ManageSites/Create

        [HttpPost]
        public ActionResult Create(Site site)
        {
            if (ModelState.IsValid)
            {
                db.Sites.AddObject(site);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(site);
        }
        
        //
        // GET: /Admin/ManageSites/Edit/5
 
        public ActionResult Edit(int id)
        {
            Site site = db.Sites.Single(s => s.SiteId == id);
            return View(site);
        }

        //
        // POST: /Admin/ManageSites/Edit/5

        [HttpPost]
        public ActionResult Edit(Site site)
        {
            if (ModelState.IsValid)
            {
                db.Sites.Attach(site);
                db.ObjectStateManager.ChangeObjectState(site, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(site);
        }

        //
        // GET: /Admin/ManageSites/Delete/5
 
        public ActionResult Delete(int id)
        {
            Site site = db.Sites.Single(s => s.SiteId == id);
            return View(site);
        }

        //
        // POST: /Admin/ManageSites/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Site site = db.Sites.Single(s => s.SiteId == id);
            db.Sites.DeleteObject(site);
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