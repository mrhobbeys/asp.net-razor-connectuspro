using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Models;
using System.Web.Helpers;
using System.IO;
using SiteBlue.Questionnaire.Data;
using MVCCentral.Framework;
using SecurityGuard.Core;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using SecurityGuard.ViewModels;
using System.Web.Security;
using TugberkUg.MVC.Helpers;

namespace SiteBlue.Controllers
{
    [Authorize]
    public class QuestionnaireController : Controller
    {
        private QuestionnaireContext db = new QuestionnaireContext();

        //
        // GET: /Questionnaire/

        public ActionResult Index(long? qid)
        {
            if (qid.HasValue)
            {
                var questionnaire = db.Questionnaire.Find(qid.Value);
                return RedirectToAction("OwnerInformation", new { qid = qid.Value });
            }
            else
            {
                IMembershipService membershipService;
                membershipService = new MembershipService(Membership.Provider);
                MembershipUser user = membershipService.GetUser(User.Identity.Name);
                var questionnaires = db.Questionnaire.Where(q => q.UserId == (Guid)user.ProviderUserKey).ToList();
                if ((questionnaires != null) && (questionnaires.Count > 0))
                    return RedirectToAction("OwnerInformation", new { qid = questionnaires[0].QuestionnaireId });
                else
                    return View();
            }
        }

        [HttpPost]
        public ActionResult Index(Questionnaire.Data.Questionnaire questionnaireModel)
        {
            IMembershipService membershipService;
            membershipService = new MembershipService(Membership.Provider);
            MembershipUser user = membershipService.GetUser(User.Identity.Name);

            try
            {
                var questionnaire = new Questionnaire.Data.Questionnaire();
                // Initialize questionnaire default values
                questionnaire.UserId = (Guid)user.ProviderUserKey;
                var date = DateTime.Now;
                questionnaire.CreationDate = date;
                questionnaire.LastModificationDate = date;
                questionnaire.StatusId = null;
                db.Questionnaire.Add(questionnaire);
                // Apply changes to the database
                db.SaveChanges();
                // Initialize child tables with default fields
                InitializeQuestionnaire(questionnaire);
                // return the index view with the new added questionnaire
                return RedirectToAction("OwnerInformation", new { qid = questionnaire.QuestionnaireId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult OwnerInformation(long? qid)
        {
            if (qid.HasValue)
            {
                var ownerInformations = db.OwnerInformation.Where(oi => oi.QuestionnaireId == qid.Value).ToList();
                ViewBag.States = new SelectList(db.State.ToList(), "StateId", "StateName");
                if ((ownerInformations != null) && (ownerInformations.Count > 0))
                    return View(ownerInformations[0]);
                else
                    return View(QuestionnaireModel.InitializeOwnerInformation(qid.Value));
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult OwnerInformation(long qid, OwnerInformation model)
        {
            if (ModelState.IsValid)
            {
                var ownerInformations = db.OwnerInformation.Where(oi => oi.QuestionnaireId == qid).ToList();
                if ((ownerInformations != null) && (ownerInformations.Count > 0))
                    // Edit mode
                    UpdateModel(ownerInformations[0]);
                else
                {
                    // Creation mode
                    model.QuestionnaireId = qid;
                    db.OwnerInformation.Add(model);
                }
                // Commit transaction
                db.SaveChanges();
                return RedirectToAction("BusinessInformation", new { qid = qid });
            }
            return View(model);
        }

        public JsonResult GetOwnerInformations(long id)
        {
            try
            {
                //var businessInformation = db.BusinessInformation.Find(id);
                //var ownerInformation = db.OwnerInformation.Where(oi => oi.QuestionnaireId == businessInformation.QuestionnaireId).ToList();

                var ownerInformation = (from o in db.OwnerInformation
                                         join b in db.BusinessInformation on o.QuestionnaireId equals b.QuestionnaireId
                                         where b.BusinessInformationId == id
                                         select o)
                                         .ToList();

                if ((ownerInformation != null) && (ownerInformation.Count > 0))
                {
                    return Json(new
                    {
                        cellPhone = ownerInformation[0].CellPhone,
                        homePhone = ownerInformation[0].HomePhone,
                        address = ownerInformation[0].HomeAddress,
                        city = ownerInformation[0].City,
                        state = ownerInformation[0].StateId,
                        zipCode = ownerInformation[0].ZipCode
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        cellPhone = "",
                        homePhone = "",
                        address = "",
                        city = "",
                        state = "",
                        zipCode = ""
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            return Json(new { Success = false, Error = "" });
        }

        public JsonResult GetBusinessInformations(long id)
        {
            try
            {
                var businessInformation = db.BusinessInformation.Find(id);
                return Json(new
                {
                    cellPhone = businessInformation.CellPhone,
                    officePhone = businessInformation.OfficePhone,
                    address = businessInformation.BusinessAddress,
                    city = businessInformation.BusinessCity,
                    state = businessInformation.StateId,
                    zipCode = businessInformation.BusinessZipCode
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public JsonResult GetAdditionalDetails(long id)
        {
            try
            {
                var businessInformation = db.BusinessInformation.Find(id);
                return Json(new
                {
                    additionalDetails = businessInformation.AdditionalDetails
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public JsonResult GetServiceTripFees(long id)
        {
            try
            {
                var businessInformation = db.BusinessInformation.Find(id);
                return Json(new { serviceTripFeesDescription = businessInformation.ServiceTripFeesDescription }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public JsonResult GetGPSSystem(long id)
        {
            try
            {
                var businessInformation = db.BusinessInformation.Find(id);
                return Json(new { gpsSystemId = businessInformation.GPSSystemId, otherGpsSystem = businessInformation.OtherGPSSystem }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult BusinessInformation(long? qid)
        {
            List<string> list = new List<string>();
            for (int i = 0; i <= 75; i++)
            {
                list.Add(i.ToString());
            }
            ViewBag.ItemList = list;
            ViewBag.HoursList = getHours();
            ViewBag.States = new SelectList(db.State.ToList(), "StateId", "StateName");
            ViewBag.GPSSystems = new SelectList(db.GPSSystem.ToList(), "GPSSystemId", "GPSSystemName");
            if (qid.HasValue)
            {
                var businessInformations = db.BusinessInformation.Where(bi => bi.QuestionnaireId == qid.Value).ToList();
                if ((businessInformations != null) && (businessInformations.Count > 0))
                    return View(businessInformations[0]);
                else
                    return View(QuestionnaireModel.InitializeBusinessInformation(qid.Value));
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult BusinessInformation(long qid, BusinessInformation model)
        {
            if (ModelState.IsValid)
            {
                var businessInformations = db.BusinessInformation.Where(bi => bi.QuestionnaireId == qid).ToList();
                if ((businessInformations != null) && (businessInformations.Count > 0))
                {
                    // Edit mode
                    //if (string.IsNullOrEmpty(businessInformations[0].ServiceCenterAgentsAnswer)) businessInformations[0].ServiceCenterAgentsAnswer = "Plumbing Service Department, how may we help you?";
                    UpdateModel(businessInformations[0]);
                }
                else
                {
                    // Creation mode
                    //if (string.IsNullOrEmpty(model.ServiceCenterAgentsAnswer)) model.ServiceCenterAgentsAnswer = "Plumbing Service Department, how may we help you?";
                    db.BusinessInformation.Add(model);
                }
                // Commit changes
                db.SaveChanges();
                //return View(model);
                return RedirectToAction("TechnicianInformation", new { qid = qid });
            }
            List<string> list = new List<string>();
            for (int i = 0; i <= 75; i++)
            {
                list.Add(i.ToString());
            }
            // Initialise Business information view
            ViewBag.ItemList = list;
            model = db.BusinessInformation.Where(bi => bi.QuestionnaireId == qid).Single();
            ViewBag.HoursList = getHours();
            ViewBag.States = new SelectList(db.State.ToList(), "StateId", "StateName");
            ViewBag.GPSSystems = new SelectList(db.GPSSystem.ToList(), "GPSSystemId", "GPSSystemName");
            return View(model);
        }

        public ActionResult AccountingInformation(long? qid)
        {
            if (qid.HasValue)
            {
                List<string> list = new List<string>();
                list.Add("Select an option");
                list.Add("1 - I can write a check and use my ATM card");
                list.Add("2");
                list.Add("3");
                list.Add("4");
                list.Add("5 - I’m comfortable discussing accounting issues that affect my business");
                list.Add("6");
                list.Add("7");
                list.Add("8");
                list.Add("9");
                list.Add("10 - I process accounting transactions and prepare my own financial statements");
                ViewBag.AccountingKnowledgeRates = list;
                List<string> state = new List<string>();
                state.Add("Select an option");
                state.Add("Yes");
                state.Add("No");
                ViewBag.State = state;
                List<string> calendarType = new List<string>();
                calendarType.Add("Select an option");
                calendarType.Add("Calendar");
                calendarType.Add("4/4/5");
                calendarType.Add("Other");
                ViewBag.CalendarTypes = calendarType;

                List<string> financialTransactions = new List<string>();
                financialTransactions.Add("Select an option");
                financialTransactions.Add("Accounting Staff");
                financialTransactions.Add("Outside Accountant");
                financialTransactions.Add("Other");
                ViewBag.FinancialTransactions = financialTransactions;

                var accountingInformations = db.AccountingInformation.Where(ai => ai.QuestionnaireId == qid.Value).ToList();
                if ((accountingInformations != null) && (accountingInformations.Count > 0))
                    return View(accountingInformations[0]);
                else
                    return View(QuestionnaireModel.InitializeAccountingInformation(qid.Value));
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AccountingInformation(long? qid, AccountingInformation model)
        {
            List<string> list = new List<string>();
            list.Add("Select an option");
            list.Add("1 - I can write a check and use my ATM card");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            list.Add("5 - I’m comfortable discussing accounting issues that affect my business");
            list.Add("6");
            list.Add("7");
            list.Add("8");
            list.Add("9");
            list.Add("10 - I process accounting transactions and prepare my own financial statements");
            ViewBag.AccountingKnowledgeRates = list;
            List<string> state = new List<string>();
            state.Add("Select an option");
            state.Add("Yes");
            state.Add("No");
            ViewBag.State = state;
            List<string> calendarType = new List<string>();
            calendarType.Add("Select an option");
            calendarType.Add("Calendar");
            calendarType.Add("4/4/5");
            calendarType.Add("Other");
            ViewBag.CalendarTypes = calendarType;

            List<string> financialTransactions = new List<string>();
            financialTransactions.Add("Select an option");
            financialTransactions.Add("Accounting Staff");
            financialTransactions.Add("Outside Accountant");
            financialTransactions.Add("Other");
            ViewBag.FinancialTransactions = financialTransactions;
            if (ModelState.IsValid)
            {
                try
                {
                    var accountingInformations = db.AccountingInformation.Where(ai => ai.QuestionnaireId == qid.Value).ToList();
                    if ((accountingInformations != null) && (accountingInformations.Count > 0))
                        UpdateModel(accountingInformations[0]);
                    else
                        // Creation mode
                        db.AccountingInformation.Add(model);
                    db.SaveChanges();
                    return View(model);
                }
                catch (Exception)
                {
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult TechnicianInformation(long? qid)
        {
            if (qid.HasValue)
            {
                var questionnaire = db.Questionnaire.Find(qid.Value);
                return View(questionnaire);
            }
            else return RedirectToAction("Index");
        }

        public ActionResult Technicians(long qid)
        {
            var technicianInformations = db.TechnicianInformation.Include("State").Where(ti => ti.QuestionnaireId == qid).ToList();
            if ((technicianInformations != null) && (technicianInformations.Count > 0))
                return PartialView(technicianInformations);
            else return PartialView(new List<TechnicianInformation>());
        }

        public ActionResult Technician(long? qid, long? tid)
        {
            List<string> list = new List<string>();
            for (int i = 0; i <= 75; i++)
            {
                list.Add(i.ToString());
            }
            ViewBag.States = new SelectList(db.State.ToList(), "StateId", "StateName");
            ViewBag.Levels = new SelectList(db.Level.ToList(), "LevelId", "LevelName");
            ViewBag.PlumbingHVACWorkTimes = getYearsOfExperience();
            if (tid.HasValue)
            {
                // Edit mode
                return View(db.TechnicianInformation.Find(tid));
            }
            else
            {
                if (qid.HasValue)
                {
                    // Creation mode
                    var technician = new TechnicianInformation();
                    technician.QuestionnaireId = qid.Value;
                    return View(technician);
                }
            }
            return View(new TechnicianInformation());
        }

        [HttpPost]
        public ActionResult Technician(long qid, TechnicianInformation model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.TechnicianInformationId != 0)
                    {
                        var technician = db.TechnicianInformation.Find(model.TechnicianInformationId);
                        UpdateModel(technician);
                    }
                    else
                    {
                        model.QuestionnaireId = qid;
                        db.TechnicianInformation.Add(model);
                    }

                    db.SaveChanges();
                    return RedirectToAction("TechnicianInformation", new { qid = qid });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Technician", new { tid = model.TechnicianInformationId });
            }
            return RedirectToAction("Technician", new { tid = model.TechnicianInformationId });
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteTechnician")]
        public ActionResult AjaxDeleteTechnician(long tid)
        {
            var technician = db.TechnicianInformation.Find(tid);
            var technicians = db.TechnicianInformation.Where(t => t.QuestionnaireId == technician.QuestionnaireId).ToList();
            try
            {
                // Remove dependencies - Note: We can also activate Cascade delete
                // Plumbing
                var plumbings = db.Plumbing.Where(p => p.TechnicianInformationId == tid).ToList();
                if (plumbings.Count > 0)
                {
                    foreach (Plumbing p in plumbings)
                        db.Plumbing.Remove(p);
                }
                // HVAC
                var hvacs = db.Hvac.Where(h => h.TechnicianInformationId == tid).ToList();
                if (hvacs.Count > 0)
                {
                    foreach (Hvac h in hvacs)
                        db.Hvac.Remove(h);
                }
                // LicenseType
                var licenceTypes = db.LicenseType.Where(lt => lt.TechnicianInformationId == tid).ToList();
                if (licenceTypes.Count > 0)
                {
                    foreach (LicenseType lt in licenceTypes)
                        db.LicenseType.Remove(lt);
                }

                db.TechnicianInformation.Remove(technician);
                db.SaveChanges();
                //return PartialView("Technicians", technicians);
                return RedirectToAction("Technicians", new { qid = technician.QuestionnaireId });
            }
            catch (Exception)
            {
                //return PartialView("Technicians", technicians);
                return RedirectToAction("Technicians", new { qid = technician.QuestionnaireId });
            }
        }

        public bool InitializeQuestionnaire(Questionnaire.Data.Questionnaire questionnaire)
        {
            try
            {
                var ownerInformation = QuestionnaireModel.InitializeOwnerInformation(questionnaire.QuestionnaireId);
                ownerInformation.Questionnaire = questionnaire;
                var businessInformation = QuestionnaireModel.InitializeBusinessInformation(questionnaire.QuestionnaireId);
                businessInformation.QuestionnaireId = questionnaire.QuestionnaireId;
                businessInformation.Questionnaire = questionnaire;
                var accountingInformation = QuestionnaireModel.InitializeAccountingInformation(questionnaire.QuestionnaireId);
                accountingInformation.QuestionnaireId = questionnaire.QuestionnaireId;
                accountingInformation.Questionnaire = questionnaire;
                // Add 
                db.OwnerInformation.Add(ownerInformation);
                db.BusinessInformation.Add(businessInformation);
                db.AccountingInformation.Add(accountingInformation);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }
        }

        public bool SaveOwnerInformation(long questionnaireId, OwnerInformation model)
        {
            bool returnedValue = false;
            var ownerInformation = db.OwnerInformation.Where(oi => oi.QuestionnaireId == questionnaireId).ToList();

            if ((ownerInformation != null) && (ownerInformation.Count > 0))
            {
                // Edit mode
                UpdateModel(ownerInformation[0]);
                db.SaveChanges();
                returnedValue = true;
            }
            else
            {
                // Creation mode
                db.OwnerInformation.Add(model);
                db.SaveChanges();
                returnedValue = true;
            }

            return returnedValue;
        }

        #region License Number

        public JsonResult GetLicenseNumber(long id)
        {
            try
            {
                var licenseNumber = db.LicenseNumber.Find(id);
                return Json(new { licenseNumberId = licenseNumber.LicenseNumberId, licenseNumberName = licenseNumber.LicenseNumberName, licenseNumberComment = licenseNumber.Comment, invoicesRequired = licenseNumber.InvoicesRequired }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult LicenseNumbersTable(long? id)
        {
            var licenseNumbers = db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList();
            if ((licenseNumbers != null) && (licenseNumbers.Count > 0))
                return PartialView("LicenseNumbersTable", licenseNumbers);
            else
                return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteLicenseNumber")]
        public ActionResult AjaxDeleteLicenseNumber(long id)
        {
            var licenseNumber = db.LicenseNumber.Find(id);
            long bid = licenseNumber.BusinnessInformationId;
            db.LicenseNumber.Remove(licenseNumber);
            db.SaveChanges();
            var licenseNumbers = db.LicenseNumber.Where(ln => ln.BusinnessInformationId == bid).ToList();
            return PartialView("Partial/LicenseNumbersTable", licenseNumbers);
        }

        [HttpPost]
        public ActionResult SaveLicenseNumber(long? bid, long? id, string name, string comment, bool invoicesRequired)
        {
            try
            {
                LicenseNumber licenseNumber = null;
                if (id == null)
                    // Creation mode
                    db.LicenseNumber.Add(new LicenseNumber { BusinnessInformationId = bid.Value, LicenseNumberName = name, Comment = comment, InvoicesRequired = invoicesRequired });
                else
                {
                    // Edit mode
                    licenseNumber = db.LicenseNumber.Find(id);
                    licenseNumber.LicenseNumberName = name;
                    licenseNumber.Comment = comment;
                    licenseNumber.InvoicesRequired = invoicesRequired;
                    UpdateModel(licenseNumber);
                }
                db.SaveChanges();
                var licenseNumbers = db.LicenseNumber.Where(ln => ln.BusinnessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/LicenseNumbersTable", licenseNumbers) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region Marketing Type

        public JsonResult GetMarketingType(long id)
        {
            try
            {
                var marketingType = db.MarketingType.Find(id);
                return Json(new { marketingTypeId = marketingType.MarketingTypeId, marketingTypeName = marketingType.MarketingTypeName, marketingTypeReferralCode = marketingType.ReferralCode, marketingTypeComment = marketingType.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult MarketingTypesTable(long? id)
        {
            //return View(db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList());
            var marketingTypes = db.MarketingType.Where(mt => mt.BusinessInformationId == id).ToList();
            if ((marketingTypes != null) && (marketingTypes.Count > 0))
                return View(marketingTypes);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteMarketingType")]
        public ActionResult AjaxDeleteMarketingType(long id)
        {
            var marketingType = db.MarketingType.Find(id);
            long bid = marketingType.BusinessInformationId;
            db.MarketingType.Remove(marketingType);
            db.SaveChanges();
            var marketingTypes = db.MarketingType.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/MarketingTypesTable", marketingTypes);
        }

        [HttpPost]
        public ActionResult SaveMarketingType(long? bid, long? id, string name, string referralCode, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.MarketingType.Add(new MarketingType { BusinessInformationId = bid.Value, MarketingTypeName = name, ReferralCode = referralCode, Comment = comment });
                else
                {
                    // Edit mode
                    var marketingType = db.MarketingType.Find(id);
                    marketingType.MarketingTypeName = name;
                    marketingType.ReferralCode = referralCode;
                    marketingType.Comment = comment;
                    UpdateModel(marketingType);
                }
                db.SaveChanges();
                var marketingTypes = db.MarketingType.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/MarketingTypesTable", marketingTypes) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion

        #region AdvertisedPhone

        public JsonResult GetAdvertisedPhoneNumber(long id)
        {
            try
            {
                var AdvertisedPhoneNumber = db.AdvertisedPhoneNumber.Find(id);
                return Json(new { advertisedPhoneNumberId = AdvertisedPhoneNumber.AdvertisedPhoneNumberId, advertisedPhone = AdvertisedPhoneNumber.AdvertisedPhone, advertisedPhoneComment = AdvertisedPhoneNumber.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult AdvertisedPhoneNumbersTable(long? id)
        {
            var advertisedPhoneNumbers = db.AdvertisedPhoneNumber.Where(ap => ap.BusinessInformationId == id).ToList();
            if ((advertisedPhoneNumbers != null) && (advertisedPhoneNumbers.Count > 0))
                return View(advertisedPhoneNumbers);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteAdvertisedPhoneNumber")]
        public ActionResult AjaxDeleteAdvertisedPhoneNumber(long id)
        {
            var advertisedPhoneNumber = db.AdvertisedPhoneNumber.Find(id);
            long bid = advertisedPhoneNumber.BusinessInformationId;
            db.AdvertisedPhoneNumber.Remove(advertisedPhoneNumber);
            db.SaveChanges();
            var advertisedPhoneNumbers = db.AdvertisedPhoneNumber.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/AdvertisedPhoneNumbersTable", advertisedPhoneNumbers);

        }

        [HttpPost]
        public ActionResult SaveAdvertisedPhoneNumber(long? bid, long? id, string name, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.AdvertisedPhoneNumber.Add(new AdvertisedPhoneNumber { BusinessInformationId = bid.Value, AdvertisedPhone = name, Comment = comment });
                else
                {
                    // Edit mode
                    var advertisedPhoneNumber = db.AdvertisedPhoneNumber.Find(id);
                    advertisedPhoneNumber.AdvertisedPhone = name;
                    advertisedPhoneNumber.Comment = comment;
                    UpdateModel(advertisedPhoneNumber);
                }
                db.SaveChanges();
                var advertisedPhoneNumbers = db.AdvertisedPhoneNumber.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/AdvertisedPhoneNumbersTable", advertisedPhoneNumbers) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion

        #region DBA

        public JsonResult GetDba(long id)
        {
            try
            {
                var dba = db.Dba.Find(id);
                return Json(new { dbaId = dba.DbaId, dbaName = dba.DbaName, dbaAdvertisedPhoneNumber = dba.AdvertisedPhoneNumber, dbaComment = dba.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult DbasTable(long? id)
        {
            var dbas = db.Dba.Where(d => d.BusinessInformationId == id).ToList();
            if ((dbas != null) && (dbas.Count > 0))
                return View(dbas);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteDba")]
        public ActionResult AjaxDeleteDba(long id)
        {
            var dba = db.Dba.Find(id);
            long bid = dba.BusinessInformationId;
            db.Dba.Remove(dba);
            db.SaveChanges();
            var dbas = db.Dba.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/dbasTable", dbas);
        }
        
        [HttpPost]
        public ActionResult SaveDba(long? bid, long? id, string name, string advertisedPhoneNumber, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.Dba.Add(new Dba { BusinessInformationId = bid.Value, DbaName = name, Comment = comment });
                else
                {
                    // Edit mode
                    var dba = db.Dba.Find(id);
                    dba.DbaName = name;
                    dba.Comment = comment;
                    UpdateModel(dba);
                }
                db.SaveChanges();
                var dbas = db.Dba.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                ViewBag.BusinessName = db.BusinessInformation.Find(bid).Questionnaire.OwnerInformations[0].BusinessName;
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/DbasTable", dbas) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return Json(new { Success = false });
            }
        }

        #endregion

        #region Discount

        public JsonResult GetDiscount(long id)
        {
            try
            {
                var discount = db.Discount.Find(id);
                return Json(new { discountId = discount.DiscountId, discountName = discount.DiscountName, discountExpirationDate = discount.ExpirationDate.HasValue ? discount.ExpirationDate.Value.ToShortDateString() : "", discountComment = discount.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult DiscountsTable(long? id)
        {
            //return View(db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList());
            var discounts = db.Discount.Where(d => d.BusinessInformationId == id).ToList();
            if ((discounts != null) && (discounts.Count > 0))
                return View(discounts);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteDiscount")]
        public ActionResult AjaxDeleteDiscount(long id)
        {
            var discount = db.Discount.Find(id);
            long bid = discount.BusinessInformationId;
            db.Discount.Remove(discount);
            db.SaveChanges();
            var discounts = db.Discount.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/DiscountsTable", discounts);
        }

        [HttpPost]
        public ActionResult SaveDiscount(long? bid, long? id, string name, DateTime expirationDate, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.Discount.Add(new Discount { BusinessInformationId = bid.Value, DiscountName = name, ExpirationDate = expirationDate, Comment = comment });
                else
                {
                    // Edit mode
                    var discount = db.Discount.Find(id);
                    discount.DiscountName = name;
                    discount.ExpirationDate = expirationDate;
                    discount.Comment = comment;
                    UpdateModel(discount);
                }
                db.SaveChanges();
                var discounts = db.Discount.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/DiscountsTable", discounts) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion

        #region Referral code

        public JsonResult GetReferralCode(long id)
        {
            try
            {
                var referralCode = db.ReferralCode.Find(id);
                return Json(new { referralCodeId = referralCode.ReferralCodeId, referralCodeName = referralCode.ReferralCodeName, referralCodeComment = referralCode.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult ReferralCodesTable(long? id)
        {
            //return View(db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList());
            var referralCodes = db.ReferralCode.Where(rc => rc.BusinessInformationId == id).ToList();
            if ((referralCodes != null) && (referralCodes.Count > 0))
                return View(referralCodes);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteReferralCode")]
        public ActionResult AjaxDeleteReferralCode(long id)
        {
            var referralCode = db.ReferralCode.Find(id);
            long bid = referralCode.BusinessInformationId;
            db.ReferralCode.Remove(referralCode);
            db.SaveChanges();
            var referralCodes = db.ReferralCode.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/ReferralCodesTable", referralCodes);
        }

        [HttpPost]
        public ActionResult SaveReferralCode(long? bid, long? id, string name, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.ReferralCode.Add(new ReferralCode { BusinessInformationId = bid.Value, ReferralCodeName = name, Comment = comment });
                else
                {
                    // Edit mode
                    var referralCode = db.ReferralCode.Find(id);
                    referralCode.ReferralCodeName = name;
                    referralCode.Comment = comment;
                    UpdateModel(referralCode);
                }
                db.SaveChanges();
                var referralCodes = db.ReferralCode.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/ReferralCodesTable", referralCodes) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion

        #region Service plan

        public JsonResult GetServicePlan(long id)
        {
            try
            {
                var servicePlan = db.ServicePlan.Find(id);
                return Json(new { servicePlanId = servicePlan.ServicePlanId, servicePlanName = servicePlan.ServicePlanName, servicePlanComment = servicePlan.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult ServicePlansTable(long? id)
        {
            //return View(db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList());
            var servicePlans = db.ServicePlan.Where(rc => rc.BusinessInformationId == id).ToList();
            if ((servicePlans != null) && (servicePlans.Count > 0))
                return View(servicePlans);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteServicePlan")]
        public ActionResult AjaxDeleteServicePlan(long id)
        {
            var servicePlan = db.ServicePlan.Find(id);
            long bid = servicePlan.BusinessInformationId;
            db.ServicePlan.Remove(servicePlan);
            db.SaveChanges();
            var servicePlans = db.ServicePlan.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/ServicePlansTable", servicePlans);
        }

        [HttpPost]
        public ActionResult SaveServicePlan(long? bid, long? id, string name, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.ServicePlan.Add(new ServicePlan { BusinessInformationId = bid.Value, ServicePlanName = name, Comment = comment });
                else
                {
                    // Edit mode
                    var servicePlan = db.ServicePlan.Find(id);
                    servicePlan.ServicePlanName = name;
                    servicePlan.Comment = comment;
                    UpdateModel(servicePlan);
                }
                db.SaveChanges();
                var servicePlans = db.ServicePlan.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/ServicePlansTable", servicePlans) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion

        #region Warranty work

        public JsonResult GetWarrantyWork(long id)
        {
            try
            {
                var warrantyWork = db.WarrantyWork.Find(id);
                return Json(new { warrantyWorkId = warrantyWork.WarrantyWorkId, warrantyWorkName = warrantyWork.WarrantyWorkName, warrantyWorkComment = warrantyWork.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult WarrantyWorksTable(long? id)
        {
            //return View(db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList());
            var warrantyWorks = db.WarrantyWork.Where(ww => ww.BusinessInformationId == id).ToList();
            if ((warrantyWorks != null) && (warrantyWorks.Count > 0))
                return View(warrantyWorks);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteWarrantyWork")]
        public ActionResult AjaxDeleteWarrantyWork(long id)
        {
            var warrantyWork = db.WarrantyWork.Find(id);
            long bid = warrantyWork.BusinessInformationId;
            db.WarrantyWork.Remove(warrantyWork);
            db.SaveChanges();
            var warrantyWorks = db.WarrantyWork.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/WarrantyWorksTable", warrantyWorks);
        }

        [HttpPost]
        public ActionResult SaveWarrantyWork(long? bid, long? id, string name, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.WarrantyWork.Add(new WarrantyWork { BusinessInformationId = bid.Value, WarrantyWorkName = name, Comment = comment });
                else
                {
                    // Edit mode
                    var warrantyWork = db.WarrantyWork.Find(id);
                    warrantyWork.WarrantyWorkName = name;
                    warrantyWork.Comment = comment;
                    UpdateModel(warrantyWork);
                }
                db.SaveChanges();
                var warrantyWorks = db.WarrantyWork.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/WarrantyWorksTable", warrantyWorks) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion

        #region ZipCode

        public JsonResult GetLastTaxRate(long bid)
        {
            try
            {
                var zipCode = db.ZipCode.Where(zc => zc.BusinessInformationId == bid).OrderByDescending(zc => zc.ZipCodeId).ToList();
                return Json(new { TaxRate = ((zipCode != null) && (zipCode.Count > 0)) ? zipCode[0].TaxRate : 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public JsonResult GetZipCode(long id)
        {
            try
            {
                var zipCode = db.ZipCode.Find(id);
                return Json(new { zipCodeId = zipCode.ZipCodeId, zipCodeNumber = zipCode.ZipCodeNumber, zipCodeTaxRate = zipCode.TaxRate.HasValue ? zipCode.TaxRate : 0, zipCodeComment = zipCode.Comment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult ZipCodesTable(long? id)
        {
            //return View(db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList());
            var zipCodes = db.ZipCode.Where(ww => ww.BusinessInformationId == id).ToList();
            if ((zipCodes != null) && (zipCodes.Count > 0))
                return View(zipCodes);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteZipCode")]
        public ActionResult AjaxDeleteZipCode(long id)
        {
            var zipCode = db.ZipCode.Find(id);
            long bid = zipCode.BusinessInformationId;
            db.ZipCode.Remove(zipCode);
            db.SaveChanges();
            var zipCodes = db.ZipCode.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/ZipCodesTable", zipCodes);
        }

        [HttpPost]
        public ActionResult SaveZipCode(long? bid, long? id, string name, double? taxRateZIP, string comment)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.ZipCode.Add(new ZipCode { BusinessInformationId = bid.Value, ZipCodeNumber = name, TaxRate = taxRateZIP, Comment = comment });
                else
                {
                    // Edit mode
                    var zipCode = db.ZipCode.Find(id);
                    zipCode.ZipCodeNumber = name;
                    zipCode.TaxRate = taxRateZIP;
                    zipCode.Comment = comment;
                    UpdateModel(zipCode);
                }
                db.SaveChanges();
                var zipCodes = db.ZipCode.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/ZipCodesTable", zipCodes) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion

        #region HVAC

        public ActionResult Hvac(long? tid)
        {
            var hvac = db.Hvac.Where(h => h.TechnicianInformationId == tid.Value).ToList();
            if (hvac.Count > 0)
                return View(hvac[0]);
            else
            {
                var technician = db.TechnicianInformation.Find(tid.Value);
                Hvac newHvac = new Hvac();
                newHvac.Technician = technician;
                return View(newHvac);
            }
        }

        [HttpPost]
        public ActionResult Hvac(long? tid, Hvac model)
        {
            if (ModelState.IsValid)
            {
                var hvac = db.Hvac.Where(h => h.TechnicianInformationId == tid.Value).ToList();
                if (hvac.Count > 0)
                {
                    // Edit mode
                    UpdateModel(hvac[0]);
                }
                else
                {
                    // Creation mode
                    model.TechnicianInformationId = tid.Value;
                    db.Hvac.Add(model);
                }
                // Update plumbing passed status
                var technician = db.TechnicianInformation.Find(tid);
                //technician.HvacPassed = true;
                db.SaveChanges();
                return RedirectToAction("TechnicianInformation", new { qid = db.TechnicianInformation.Find(tid.Value).QuestionnaireId });
            }
            return View(model);
        }

        #endregion

        #region Plumbing

        public ActionResult Plumbing(long? tid)
        {
            var plumbing = db.Plumbing.Where(p => p.TechnicianInformationId == tid.Value).ToList();
            if (plumbing.Count > 0)
                return View(plumbing[0]);
            else
            {
                var technician = db.TechnicianInformation.Find(tid.Value);
                Plumbing newPlumbing = new Plumbing();
                newPlumbing.Technician = technician;
                return View(newPlumbing);
            }
        }

        [HttpPost]
        public ActionResult Plumbing(long? tid, Plumbing model)
        {
            if (ModelState.IsValid)
            {
                var plumbing = db.Plumbing.Where(p => p.TechnicianInformationId == tid.Value).ToList();
                if (plumbing.Count > 0)
                {
                    // Edit mode
                    UpdateModel(plumbing[0]);
                }
                else
                {
                    // Creation mode
                    model.TechnicianInformationId = tid.Value;
                    db.Plumbing.Add(model);
                }
                // Update plumbing passed status
                var technician = db.TechnicianInformation.Find(tid);
                //technician.HvacPassed = true;
                db.SaveChanges();
                return RedirectToAction("TechnicianInformation", new { qid = db.TechnicianInformation.Find(tid.Value).QuestionnaireId });
            }
            return View(model);
        }

        #endregion

        #region Address

        public JsonResult GetAddress(int id)
        {
            try
            {
                var address = db.Address.Find(id);

                return Json(new
                {
                    addressId = address.AddressId,
                    businessInformationId = address.BusinessInformationId,
                    addressTypeId = address.AddressTypeId,
                    addressOne = address.Address1,
                    addressTwo = address.Address2,
                    city = address.City,
                    stateId = address.StateId,
                    zipCode = address.ZipCode
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            return Json(new { Success = false, Error = "" });
        }

        public ActionResult AddressesTable(long? id)
        {
            var addresses = db.Address.Where(d => d.BusinessInformationId == id).ToList();

            if ((addresses != null) && (addresses.Count > 0))
                return View(addresses);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteAddress")]
        public ActionResult AjaxDeleteAddress(long id)
        {
            var address = db.Address.Find(id);
            long bid = address.BusinessInformationId;
            db.Address.Remove(address);
            db.SaveChanges();
            var addresses = db.Address.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/AddressesTable", addresses);
        }

        [HttpPost]
        public ActionResult SaveAddress(long? bid, long? id, int addressTypeId, string addressOne, string addressTwo, string city, int stateId, string zipCode)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.Address.Add(new Address { BusinessInformationId = bid.Value, AddressTypeId = addressTypeId, Address1 = addressOne, Address2 = addressTwo, City = city, StateId = stateId, ZipCode = zipCode });
                else
                {
                    // Edit mode
                    var address = db.Address.Find(id);
                    address.AddressTypeId = addressTypeId;
                    address.Address1 = addressOne;
                    address.Address2 = addressTwo;
                    address.City = city;
                    address.StateId = stateId;
                    address.ZipCode = zipCode;
                    UpdateModel(address);
                }
                db.SaveChanges();
                var addresses = db.Address.Include("AddressType").Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/AddressesTable", addresses) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return Json(new { Success = false, message = message });
            }
        }

        #endregion

        #region OfficePersonnel

        public JsonResult GetOfficePersonnel(long id)
        {
            try
            {
                var officePersonnel = db.OfficePersonnel.Find(id);
                return Json(new { officePersonnelId = officePersonnel.OfficePersonnelId, businessInformationId = officePersonnel.BusinessInformationId, firstName = officePersonnel.FirstName, lastName = officePersonnel.LastName, title = officePersonnel.Title, cellPhone = officePersonnel.CellPhone, officePhone = officePersonnel.OfficePhone }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        public ActionResult OfficePersonnelsTable(long? id)
        {
            //return View(db.LicenseNumber.Where(ln => ln.BusinnessInformationId == id).ToList());
            var officePersonnels = db.OfficePersonnel.Where(d => d.BusinessInformationId == id).ToList();
            if ((officePersonnels != null) && (officePersonnels.Count > 0))
                return View(officePersonnels);
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [ActionName("DeleteOfficePersonnel")]
        public ActionResult AjaxDeleteOfficePersonnel(long id)
        {
            var officePersonnel = db.OfficePersonnel.Find(id);
            long bid = officePersonnel.BusinessInformationId;
            db.OfficePersonnel.Remove(officePersonnel);
            db.SaveChanges();
            var officePersonnels = db.OfficePersonnel.Where(ln => ln.BusinessInformationId == bid).ToList();
            return PartialView("Partial/OfficePersonnelsTable", officePersonnels);
        }

        [HttpPost]
        public ActionResult SaveOfficePersonnel(long? bid, long? id, string firstName, string lastName, string title, string cellPhone, string officePhone)
        {
            try
            {
                if (id == null)
                    // Creation mode
                    db.OfficePersonnel.Add(new OfficePersonnel { BusinessInformationId = bid.Value, FirstName = firstName, LastName = lastName, Title = title, CellPhone = cellPhone, OfficePhone = officePhone });
                else
                {
                    // Edit mode
                    var officePersonnel = db.OfficePersonnel.Find(id);
                    officePersonnel.FirstName = firstName;
                    officePersonnel.LastName = lastName;
                    officePersonnel.Title = title;
                    officePersonnel.CellPhone = cellPhone;
                    officePersonnel.OfficePhone = officePhone;
                    UpdateModel(officePersonnel);
                }
                db.SaveChanges();
                var officePersonnels = db.OfficePersonnel.Where(ln => ln.BusinessInformationId == bid.Value).ToList();
                return Json(new { Success = true, data = this.RenderPartialViewToString("Partial/OfficePersonnelsTable", officePersonnels) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return Json(new { Success = false, message = message });
            }
        }

        #endregion


        private List<string> getHours()
        {
            List<string> list = new List<string>();
            //list.Add("");
            list.Add("No hours on a given day");
            list.Add("12:00 am");
            list.Add("1:00 am");
            list.Add("2:00 am");
            list.Add("3:00 am");
            list.Add("4:00 am");
            list.Add("5:00 am");
            list.Add("6:00 am");
            list.Add("7:00 am");
            list.Add("8:00 am");
            list.Add("9:00 am");
            list.Add("10:00 am");
            list.Add("11:00 am");
            list.Add("12:00 pm");
            list.Add("1:00 pm");
            list.Add("2:00 pm");
            list.Add("3:00 pm");
            list.Add("4:00 pm");
            list.Add("5:00 pm");
            list.Add("6:00 pm");
            list.Add("7:00 pm");
            list.Add("8:00 pm");
            list.Add("9:00 pm");
            list.Add("10:00 pm");
            list.Add("11:00 pm");
            return list;
        }
        private List<string> getYearsOfExperience()
        {
            List<string> list = new List<string>();
            //list.Add("");
            //list.Add("0");
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            list.Add("5");
            list.Add("6");
            list.Add("7");
            list.Add("8");
            list.Add("9");
            list.Add("10");
            list.Add("11");
            list.Add("12");
            list.Add("13");
            list.Add("14");
            list.Add("15");
            list.Add("16");
            list.Add("17");
            list.Add("18");
            list.Add("19");
            list.Add("20+");
            return list;
        }
    }
}
