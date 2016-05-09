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
    public class FranchiseServiceController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/FranchiseService/

        public ViewResult Index(int? id)
        {
            if (id.HasValue)
            {
                ViewBag.FranchiseID = id;
                ViewBag.SelectedFranchise = db.Franchise.Find(id.Value).FranchiseNumber;
                var franchiseservice = db.FranchiseService.Include(f => f.ServiceCategory).Where(fs => fs.FranchiseID == id.Value);
                return View(franchiseservice.ToList());
            }
            return View("Error");
        }

        //
        // GET: /Admin/FranchiseService/Create

        public ActionResult Create(int id)
        {
            ViewBag.FranchiseID = id;
            ViewBag.SelectedFranchise = db.Franchise.Find(id).FranchiseNumber;
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName");
            return View();
        } 

        //
        // POST: /Admin/FranchiseService/Create

        [HttpPost]
        public ActionResult Create(int id, FranchiseService franchiseservice)
        {
            if (ModelState.IsValid)
            {
                franchiseservice.FranchiseID = id;
                db.FranchiseService.Add(franchiseservice);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });  
            }

            ViewBag.FranchiseID = id;
            ViewBag.SelectedFranchise = db.Franchise.Find(id).FranchiseNumber;
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", franchiseservice.ServiceID);
            return View(franchiseservice);
        }
        
        ////
        //// GET: /Admin/FranchiseService/Edit/5
 
        //public ActionResult Edit(int id)
        //{
        //    FranchiseService franchiseservice = db.FranchiseService.Find(id);
        //    ViewBag.FranchiseID = new SelectList(db.Franchise, "FranchiseID", "FranchiseNumber", franchiseservice.FranchiseID);
        //    ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", franchiseservice.ServiceID);
        //    return View(franchiseservice);
        //}

        ////
        //// POST: /Admin/FranchiseService/Edit/5

        //[HttpPost]
        //public ActionResult Edit(FranchiseService franchiseservice)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(franchiseservice).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.FranchiseID = new SelectList(db.Franchise, "FranchiseID", "FranchiseNumber", franchiseservice.FranchiseID);
        //    ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", franchiseservice.ServiceID);
        //    return View(franchiseservice);
        //}

        //
        // GET: /Admin/FranchiseService/Delete/5
 
        public ActionResult Delete(int id)
        {
            FranchiseService franchiseservice = db.FranchiseService.Find(id);
            return View(franchiseservice);
        }

        //
        // POST: /Admin/FranchiseService/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            FranchiseService franchiseservice = db.FranchiseService.Find(id);
            db.FranchiseService.Remove(franchiseservice);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = franchiseservice.FranchiseID });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}