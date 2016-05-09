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
    public class TerritoryOwnerController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/TerritoryOwner/

        public ViewResult Index()
        {
            return View(db.TerritoryOwners.ToList());
        }

        //
        // GET: /Admin/TerritoryOwner/Details/5

        public ViewResult Details(int id)
        {
            TerritoryOwner territoryowner = db.TerritoryOwners.Find(id);
            return View(territoryowner);
        }

        //
        // GET: /Admin/TerritoryOwner/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/TerritoryOwner/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TerritoryOwner territoryowner, HttpPostedFileBase file)
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
                        territoryowner.ImageUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.TerritoryOwners.Add(territoryowner);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(territoryowner);
        }
        
        //
        // GET: /Admin/TerritoryOwner/Edit/5
 
        public ActionResult Edit(int id)
        {
            TerritoryOwner territoryowner = db.TerritoryOwners.Find(id);
            return View(territoryowner);
        }

        //
        // POST: /Admin/TerritoryOwner/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TerritoryOwner territoryowner, HttpPostedFileBase file)
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
                        territoryowner.ImageUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Entry(territoryowner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(territoryowner);
        }

        //
        // GET: /Admin/TerritoryOwner/Delete/5
 
        public ActionResult Delete(int id)
        {
            TerritoryOwner territoryowner = db.TerritoryOwners.Find(id);
            return View(territoryowner);
        }

        //
        // POST: /Admin/TerritoryOwner/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TerritoryOwner territoryowner = db.TerritoryOwners.Find(id);
            db.TerritoryOwners.Remove(territoryowner);
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