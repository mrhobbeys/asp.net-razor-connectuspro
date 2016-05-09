using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Data.EightHundred;
using System.IO;

namespace SiteBlue.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,TrainingManager")]
    public class ManageTrainingsController : Controller
    {
        private EightHundredEntities db = new EightHundredEntities();

        //
        // GET: /Admin/ManageTrainings/

        public ViewResult Index()
        {
            var trainings = db.Trainings.Include("Site").Include("TrainingType");
            return View(trainings.ToList());
        }

        //
        // GET: /Admin/ManageTrainings/Details/5

        public ViewResult Details(int id)
        {
            Training training = db.Trainings.Single(t => t.TrainingId == id);
            return View(training);
        }

        //
        // GET: /Admin/ManageTrainings/Create

        public ActionResult Create()
        {
            ViewBag.SiteId = new SelectList(db.Sites, "SiteId", "SiteName");
            ViewBag.TrainingTypeId = new SelectList(db.TrainingTypes, "TrainingTypeId", "TrainingTypeName");
            return View();
        } 

        //
        // POST: /Admin/ManageTrainings/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Training training, HttpPostedFileBase file)
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
                        training.NavigateUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Trainings.AddObject(training);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.SiteId = new SelectList(db.Sites, "SiteId", "SiteName", training.SiteId);
            ViewBag.TrainingTypeId = new SelectList(db.TrainingTypes, "TrainingTypeId", "TrainingTypeName", training.TrainingTypeId);
            return View(training);
        }
        
        //
        // GET: /Admin/ManageTrainings/Edit/5
 
        public ActionResult Edit(int id)
        {
            Training training = db.Trainings.Single(t => t.TrainingId == id);
            ViewBag.SiteId = new SelectList(db.Sites, "SiteId", "SiteName", training.SiteId);
            ViewBag.TrainingTypeId = new SelectList(db.TrainingTypes, "TrainingTypeId", "TrainingTypeName", training.TrainingTypeId);
            return View(training);
        }

        //
        // POST: /Admin/ManageTrainings/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Training training, HttpPostedFileBase file)
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
                        training.NavigateUrl = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.Trainings.Attach(training);
                db.ObjectStateManager.ChangeObjectState(training, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SiteId = new SelectList(db.Sites, "SiteId", "SiteName", training.SiteId);
            ViewBag.TrainingTypeId = new SelectList(db.TrainingTypes, "TrainingTypeId", "TrainingTypeName", training.TrainingTypeId);
            return View(training);
        }

        //
        // GET: /Admin/ManageTrainings/Delete/5
 
        public ActionResult Delete(int id)
        {
            Training training = db.Trainings.Single(t => t.TrainingId == id);
            return View(training);
        }

        //
        // POST: /Admin/ManageTrainings/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Training training = db.Trainings.Single(t => t.TrainingId == id);
            db.Trainings.DeleteObject(training);
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