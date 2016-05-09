using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using SiteBlue.Questionnaire.Data;
using System.IO;
using DHTMLX.Export.Excel;

namespace SiteBlue.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Administrator,ElectronicOnboardingManager")]
    [Authorize]
    public class ManageQuestionnaireController : Controller
    {
        private QuestionnaireContext db = new QuestionnaireContext();

        //
        // GET: /Admin/ManageQuestionnaire/

        public ViewResult Index()
        {
            return View(db.QuestionnaireInformation.ToList());
        }

        //
        // GET: /Admin/ManageQuestionnaire/Details/5

        public ViewResult Details(long id)
        {
            var questionnaire = db.QuestionnaireInformation.Find(id);
            return View(questionnaire);
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult ExportToExcel()
        {
            //var questionnaires = db.VW_Questionnaire.ToList();
            
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "Questionnaire.xlsx"); 
        }

        public ActionResult OwnerInformation(long? id, long? questionnaire)
        {
            OwnerInformation ownerInformation = null;
            if (id.HasValue)
                ownerInformation = db.OwnerInformation.Find(id.Value);
            if (questionnaire.HasValue)
                ownerInformation = db.OwnerInformation.Single(oi => oi.QuestionnaireId == questionnaire.Value);

            if (ownerInformation != null)
                return View(ownerInformation);

            return RedirectToAction("Index");
        }

        public ActionResult BusinessInformation(long? id, long? questionnaire)
        {
            BusinessInformation businessInformation = null;
            if (id.HasValue)
                businessInformation = db.BusinessInformation.Find(id.Value);
            if (questionnaire.HasValue)
                businessInformation = db.BusinessInformation.Single(oi => oi.QuestionnaireId == questionnaire.Value);

            if (businessInformation != null)
                return View(businessInformation);

            return RedirectToAction("Index");
        }

        public ActionResult AccountingInformation(long? id, long? questionnaire)
        {
            AccountingInformation accountingInformation = null;
            if (id.HasValue)
                accountingInformation = db.AccountingInformation.Find(id.Value);
            if (questionnaire.HasValue)
                accountingInformation = db.AccountingInformation.Single(oi => oi.QuestionnaireId == questionnaire.Value);

            if (accountingInformation != null)
                return View(accountingInformation);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}