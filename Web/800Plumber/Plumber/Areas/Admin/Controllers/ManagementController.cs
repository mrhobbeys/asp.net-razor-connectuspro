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
    public class ManagementController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Management/

        public ViewResult Index()
        {
            return View(db.Management.ToList());
        }

        //
        // GET: /Admin/Management/Details/5

        public ViewResult Details(int id)
        {
            Management management = db.Management.Find(id);
            return View(management);
        }

        //
        // GET: /Admin/Management/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Management/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Management management, HttpPostedFileBase file)
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
                        management.ImageUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Management.Add(management);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(management);
        }
        
        //
        // GET: /Admin/Management/Edit/5
 
        public ActionResult Edit(int id)
        {
            Management management = db.Management.Find(id);
            return View(management);
        }

        //
        // POST: /Admin/Management/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Management management, HttpPostedFileBase file)
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
                        management.ImageUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Entry(management).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(management);
        }

        //
        // GET: /Admin/Management/Delete/5
 
        public ActionResult Delete(int id)
        {
            Management management = db.Management.Find(id);
            return View(management);
        }

        //
        // POST: /Admin/Management/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Management management = db.Management.Find(id);
            db.Management.Remove(management);
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