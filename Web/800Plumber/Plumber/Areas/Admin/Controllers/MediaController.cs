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
    public class MediaController : Controller
    {
        private PlumberContext db = new PlumberContext();

        //
        // GET: /Admin/Media/

        public ViewResult Index()
        {
            return View(db.Media.ToList());
        }

        //
        // GET: /Admin/Media/Details/5

        public ViewResult Details(int id)
        {
            Media media = db.Media.Find(id);
            return View(media);
        }

        //
        // GET: /Admin/Media/Create

        public ActionResult Create()
        {
            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName");
            return View();
        }

        //
        // POST: /Admin/Media/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Media media, HttpPostedFileBase file)
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
                        media.MediaPath = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Media.Add(media);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName", media.MediaExtensionId);
            return View(media);
        }

        //
        // GET: /Admin/Media/Edit/5

        public ActionResult Edit(int id)
        {
            Media media = db.Media.Find(id);
            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName", media.MediaExtensionId);
            return View(media);
        }

        //
        // POST: /Admin/Media/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Media media, HttpPostedFileBase file)
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
                        media.MediaPath = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Entry(media).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MediaExtensionId = new SelectList(db.MediaExtension, "MediaExtensionId", "MediaExtensionName", media.MediaExtensionId);
            return View(media);
        }

        //
        // GET: /Admin/Media/Delete/5

        public ActionResult Delete(int id)
        {
            Media media = db.Media.Find(id);
            return View(media);
        }

        //
        // POST: /Admin/Media/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Media media = db.Media.Find(id);
            db.Media.Remove(media);
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