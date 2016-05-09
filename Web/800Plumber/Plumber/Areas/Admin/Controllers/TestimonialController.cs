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
    public class TestimonialController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Testimonial/

        public ViewResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return View(db.Testimonial.OrderByDescending(t => t.PublicationDate).ToList());
            else
            {
                if (id == "Archived")
                    return View(db.Testimonial.Where(t => t.IsDeleted == true).OrderByDescending(t => t.PublicationDate).ToList());
                else if (id == "Active")
                    return View(db.Testimonial.Where(t => t.IsDeleted == false).OrderByDescending(t => t.PublicationDate).ToList());
                else
                    return View(db.Testimonial.OrderByDescending(t => t.PublicationDate).ToList());
            }
        }

        //
        // GET: /Admin/Testimonial/Details/5

        public ViewResult Details(int id)
        {
            Testimonial testimonial = db.Testimonial.Find(id);
            return View(testimonial);
        }

        //
        // GET: /Admin/Testimonial/Create

        public ActionResult Create()
        {
            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName");
            return View();
        } 

        //
        // POST: /Admin/Testimonial/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Testimonial testimonial, HttpPostedFileBase file)
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
                        testimonial.MediaPath = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                testimonial.IsArchived = false;
                testimonial.IsDeleted = false;
                db.Testimonial.Add(testimonial);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName", testimonial.MediaExtensionId);
            return View(testimonial);
        }
        
        //
        // GET: /Admin/Testimonial/Edit/5
 
        public ActionResult Edit(int id)
        {
            Testimonial testimonial = db.Testimonial.Find(id);
            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName", testimonial.MediaExtensionId);
            return View(testimonial);
        }

        //
        // POST: /Admin/Testimonial/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Testimonial testimonial, HttpPostedFileBase file)
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
                        testimonial.MediaPath = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                //testimonial.IsArchived = false;
                //testimonial.IsDeleted = false;
                db.Entry(testimonial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName", testimonial.MediaExtensionId);
            return View(testimonial);
        }

        //
        // GET: /Admin/Testimonial/Delete/5
 
        public ActionResult Delete(int id)
        {
            Testimonial testimonial = db.Testimonial.Find(id);
            return View(testimonial);
        }

        //
        // POST: /Admin/Testimonial/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Testimonial testimonial = db.Testimonial.Find(id);
            testimonial.IsDeleted = true;
            db.Entry(testimonial).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { });
        }

        public ActionResult ChangeStatus(int id, bool status)
        {
            Testimonial testimonial = db.Testimonial.Find(id);
            testimonial.IsArchived = status;
            db.Entry(testimonial).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ReAdd(int id)
        {
            Testimonial testimonial = db.Testimonial.Find(id);
            testimonial.IsDeleted = false;
            db.Entry(testimonial).State = EntityState.Modified;
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