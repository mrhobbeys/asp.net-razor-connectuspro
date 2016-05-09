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
    public class TemplateController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Template/

        public ViewResult Index(int? id)
        {
            if (id.HasValue)
            {
                return View(db.Template.ToList());
            }
            return View(db.Template.Where(t => t.IsDeleted == false).ToList());
        }

        //
        // GET: /Admin/Template/Details/5

        public ViewResult Details(int id)
        {
            Template template = db.Template.Find(id);
            return View(template);
        }

        //
        // GET: /Admin/Template/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Template/Create

        [HttpPost]
        public ActionResult Create(Template template)
        {
            if (ModelState.IsValid)
            {
                db.Template.Add(template);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(template);
        }
        
        //
        // GET: /Admin/Template/Edit/5
 
        public ActionResult Edit(int id)
        {
            Template template = db.Template.Find(id);
            return View(template);
        }

        //
        // POST: /Admin/Template/Edit/5

        [HttpPost]
        public ActionResult Edit(Template template)
        {
            if (ModelState.IsValid)
            {
                db.Entry(template).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(template);
        }

        //
        // GET: /Admin/Template/Delete/5
 
        public ActionResult Delete(int id)
        {
            Template template = db.Template.Find(id);
            return View(template);
        }

        //
        // POST: /Admin/Template/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Template template = db.Template.Find(id);
            //db.Template.Remove(template);
            //db.SaveChanges();
            template.IsDeleted = true;
            db.Entry(template).State = EntityState.Modified;
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