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
    public class ManageTrainingTypesController : Controller
    {
        private EightHundredEntities db = new EightHundredEntities();

        //
        // GET: /Admin/ManageTrainingTypes/

        public ViewResult Index()
        {
            return View(db.TrainingTypes.ToList());
        }

        //
        // GET: /Admin/ManageTrainingTypes/Details/5

        public ViewResult Details(int id)
        {
            TrainingType trainingtype = db.TrainingTypes.Single(t => t.TrainingTypeId == id);
            return View(trainingtype);
        }

        //
        // GET: /Admin/ManageTrainingTypes/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/ManageTrainingTypes/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TrainingType trainingtype, HttpPostedFileBase file)
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
                        trainingtype.Icon = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.TrainingTypes.AddObject(trainingtype);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(trainingtype);
        }
        
        //
        // GET: /Admin/ManageTrainingTypes/Edit/5
 
        public ActionResult Edit(int id)
        {
            TrainingType trainingtype = db.TrainingTypes.Single(t => t.TrainingTypeId == id);
            return View(trainingtype);
        }

        //
        // POST: /Admin/ManageTrainingTypes/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TrainingType trainingtype, HttpPostedFileBase file)
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
                        trainingtype.Icon = string.Format("~/Content/Uploads/{0}", fileName);
                    }
                }
                db.TrainingTypes.Attach(trainingtype);
                db.ObjectStateManager.ChangeObjectState(trainingtype, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainingtype);
        }

        //
        // GET: /Admin/ManageTrainingTypes/Delete/5
 
        public ActionResult Delete(int id)
        {
            TrainingType trainingtype = db.TrainingTypes.Single(t => t.TrainingTypeId == id);
            return View(trainingtype);
        }

        //
        // POST: /Admin/ManageTrainingTypes/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TrainingType trainingtype = db.TrainingTypes.Single(t => t.TrainingTypeId == id);
            db.TrainingTypes.DeleteObject(trainingtype);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetTrainingTypeInfo(int id)
        {
            try
            {
                var trainingType = db.TrainingTypes.First(tt => tt.TrainingTypeId == id);
                return Json(new { isFile = trainingType.IsFile }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}