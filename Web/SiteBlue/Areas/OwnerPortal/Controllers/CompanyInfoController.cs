using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Services;
using SiteBlue.Areas.OwnerPortal.Models;
using SiteBlue.Areas.PriceBook.Models;
using SiteBlue.Controllers;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class CompanyInfoController : SiteBlueBaseController
    {
        readonly EightHundredEntities DB = new EightHundredEntities();
        private readonly MembershipConnection memberShipContext = new MembershipConnection();

        public ActionResult Index()
        {
            return RedirectToAction("MyCompanyInfo");
        }

        public ActionResult HeaderData()
        {
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                ViewBag.FunctionName = "doOnLoad()";
                var franchiseeList = DB.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, FranchiseNumber = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToList();

                ViewBag.frenchise = franchiseeList;
                var objFranchisee = (from e in DB.tbl_Franchise
                                     join o in DB.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
                                     where e.FranchiseID == 56
                                     select e).FirstOrDefault();
                ViewBag.FranchiseeId = objFranchisee.FranchiseID;
                ViewBag.FranchiseeNumber = objFranchisee.FranchiseNUmber + "-" + objFranchisee.LegalName;


            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return PartialView("Header");
        }


        public ActionResult MyCompanyInfo(FormCollection collection)
        {
            //var phoneType = (from p in DB.tbl_PhoneType select p);
            //ViewBag.PhoneType = phoneType;
            //var ddlcountry = (from e in DB.tbl_Franchise_Country select e).ToList();
            //ViewBag.ddlCountry = ddlcountry;
            //var ddlconcept = (from c in DB.tbl_Concept select c).ToList();
            //ViewBag.ddlConcept = ddlconcept;
            //var ddlstatus = (from s in DB.tbl_Franchise_Status select s).ToList();
            //ViewBag.ddlStatus = ddlstatus;
            //var ddlfranchiseetype = (from c in DB.tbl_Franchise_Type select c).ToList();
            //ViewBag.ddlFranchiseeType = ddlfranchiseetype;
            //var ddldivision = (from d in DB.tbl_Franchise_Division select d).ToList();
            //ViewBag.ddlRegion = ddldivision;

            // Trying to get current franchise id from the UserInfo object.
            var franchiseID = UserInfo.CurrentFranchise.FranchiseID;

            // Trying to get company information when this view gets called.

            var companyInfo = (from f in DB.tbl_Franchise
                               join fo in DB.tbl_Franchise_Owner on f.OwnerID equals fo.OwnerID
                               where f.FranchiseID == franchiseID
                               select new { Franchise = f, FranchiseOwner = fo }).FirstOrDefault();

            var phoneTypeList = (from p in DB.tbl_PhoneType select p);
            var countryList = (from c in DB.tbl_Franchise_Country select c).ToList();
            countryList.Insert(0, new tbl_Franchise_Country { CountryID = 0, Country = "Select Country" });

            var model = new CompanyInfoViewModel
            {
                PhoneType = new SelectList(phoneTypeList, "PhoneTypeID", "PhoneType"),
                CountryList = new SelectList(countryList, "CountryID", "Country")
            };

            return View(model);
        }

        public ActionResult MyCompanyInfoData(int FranchiseeId)
        {
            Session["franchiseId"] = FranchiseeId;
            var companydata = (from e in DB.tbl_Franchise
                               join o in DB.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
                               where e.FranchiseID == FranchiseeId
                               select new
                               {
                                   OwnerName = o.OwnerName,
                                   LegalName = e.LegalName,
                                   LegalAddress = e.LegalAddress,
                                   LegalCity = e.LegalCity,
                                   LegalState = e.LegalState,
                                   LegalPostal = e.LegalPostal,
                                   LegalCountryID = e.LegalCountryID,
                                   ShipName = e.ShipName,
                                   ShipCompany = e.ShipCompany,
                                   ShipAddress = e.ShipAddress,
                                   ShipCity = e.ShipCity,
                                   ShipState = e.ShipState,
                                   ShipPostal = e.ShipPostal,
                                   ShipCountryID = e.ShipCountryID,
                                   MailName = e.MailName,
                                   MailCompany = e.MailCompany,
                                   MailAddress = e.MailAddress,
                                   MailCity = e.MailCity,
                                   MailState = e.MailState,
                                   MailPostal = e.MailPostal,
                                   MailCountryID = e.MailCountryID,
                                   OfficeName = e.OfficeName,
                                   OfficeCompany = e.OfficeCompany,
                                   OfficeAddress = e.OfficeAddress,
                                   OfficeCity = e.OfficeCity,
                                   OfficeState = e.OfficeState,
                                   OfficePostal = e.OfficePostal,
                                   OfficeCountryID = e.OfficeCountryID,
                               }).FirstOrDefault();
            return Json(companydata);
        }

        public PartialViewResult PartialMyCompanyInfo(FormCollection collection, string code)
        {
            if (code.LastIndexOf("-") > 0)
            {
                code = code.Substring(code.LastIndexOf("-") + 1).Trim();
            }

            var frid = (from g in memberShipContext.MembershipFranchise
                        where g.FranchiseNumber == code
                        select g.FranchiseID).FirstOrDefault();

            var phoneType = (from p in DB.tbl_PhoneType select p);
            ViewBag.PhoneType = phoneType;

            var objFranchisee = (from e in DB.tbl_Franchise
                                 join o in DB.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
                                 where e.FranchiseID == frid
                                 select e).FirstOrDefault();
            ViewBag.FranchiseeId = objFranchisee.FranchiseID;
            ViewBag.FranchiseeNumber = objFranchisee.FranchiseNUmber + "-" + objFranchisee.LegalName;

            var ddlcountry = (from e in DB.tbl_Franchise_Country select e).ToList();
            ViewBag.ddlCountry = ddlcountry;
            var ddlconcept = (from c in DB.tbl_Concept select c).ToList();
            ViewBag.ddlConcept = ddlconcept;
            var ddlstatus = (from s in DB.tbl_Franchise_Status select s).ToList();
            ViewBag.ddlStatus = ddlstatus;
            var ddlfranchiseetype = (from c in DB.tbl_Franchise_Type select c).ToList();
            ViewBag.ddlFranchiseeType = ddlfranchiseetype;
            var ddldivision = (from d in DB.tbl_Franchise_Division select d).ToList();
            ViewBag.ddlRegion = ddldivision;

            var owner =
                (from e in DB.tbl_Franchise_Owner where e.OwnerID == objFranchisee.FranchiseID select e).FirstOrDefault();

            var objFranchisee1 = (from e in DB.tbl_Franchise
                                  join o in DB.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
                                  where e.FranchiseID == frid
                                  select e).FirstOrDefault();

            ViewBag.OwnerName = owner.OwnerName;
            ViewBag.LegalName = objFranchisee1.LegalName;
            ViewBag.Address = objFranchisee1.LegalAddress;
            ViewBag.City = objFranchisee1.LegalCity;
            ViewBag.State = objFranchisee1.LegalState;
            ViewBag.Postal = objFranchisee1.LegalPostal;
            ViewBag.Country = objFranchisee1.LegalCountryID;
            ViewBag.TimeZone = "";

            ViewBag.SName = objFranchisee1.ShipName;
            ViewBag.SCompany = objFranchisee1.ShipCompany;
            ViewBag.SAddress = objFranchisee1.ShipAddress;
            ViewBag.SCity = objFranchisee1.ShipCity;
            ViewBag.SState = objFranchisee1.ShipState;
            ViewBag.SPostal = objFranchisee1.ShipPostal;
            ViewBag.SCountry = objFranchisee1.ShipCountryID;

            ViewBag.MName = objFranchisee1.MailName;
            ViewBag.MCompany = objFranchisee1.MailCompany;
            ViewBag.MAddress = objFranchisee1.MailAddress;
            ViewBag.MCity = objFranchisee1.MailCity;
            ViewBag.MState = objFranchisee1.MailState;
            ViewBag.MPostal = objFranchisee1.MailPostal;
            ViewBag.MCountry = objFranchisee1.MailCountryID;

            ViewBag.OName = objFranchisee1.OfficeName;
            ViewBag.OCompany = objFranchisee1.OfficeCompany;
            ViewBag.OAddress = objFranchisee1.OfficeAddress;
            ViewBag.OCity = objFranchisee1.OfficeCity;
            ViewBag.OState = objFranchisee1.OfficeState;
            ViewBag.OPostal = objFranchisee1.OfficePostal;
            ViewBag.OCountry = objFranchisee1.MailCountryID;

            ViewBag.Concept = objFranchisee1.ConceptID;
            ViewBag.Status = objFranchisee1.FranchiseStatusID;
            ViewBag.FranchiseeType = objFranchisee1.FranchiseTypeID;
            ViewBag.StartDate = objFranchisee1.StartDate;
            ViewBag.Renewal = objFranchisee1.RenewalDate;
            ViewBag.SalesRep = "";
            ViewBag.FBC = "";
            ViewBag.WebSite = objFranchisee1.WebSite;
            ViewBag.Email = objFranchisee1.EMail;
            ViewBag.Region = objFranchisee1.DivisionID;

            var budget = (from p in DB.tbl_DailyBudget where p.FranchiseID == frid select p).FirstOrDefault();
            if (budget != null)
            {
                ViewBag.SalesAnnual = budget.AnualSales;
                ViewBag.SalesMonthly = budget.MonthlySales;
                ViewBag.SalesDaily = budget.DailySales;
                ViewBag.AvgTicketAnnual = budget.AnnualAvgTicket;
                ViewBag.AvgTicketMonthly = budget.MonthlyAvgTicket;
                ViewBag.AvgTicketDaily = budget.DailyAvgTicket;
                ViewBag.HomeGuardAnnual = budget.AnnualHomeGuard;
                ViewBag.HomeGuardMonthly = budget.MonthlyHomeGuard;
                ViewBag.HomeGuardDaily = budget.DailyHomeGuard;
                ViewBag.BioAnnual = budget.AnnualBio;
                ViewBag.BioMonthly = budget.MonthlyBio;
                ViewBag.BioDaily = budget.DailyBio;
                ViewBag.PayrollSalesAnnual = budget.AnnualPayrollPercentOfJobs;
                ViewBag.RecallJobsAnnual = budget.AnnualRecallPercentOfJobs;
            }


            return PartialView("PartialMyCompanyInfo");
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveMyCompanyInfo(CompanyInfoViewModel model, FormCollection collection)
        {
            //var franchiseID = Convert.ToInt32(collection["FranchiseID"]);
            var franchiseID = model.FranchiseID;

            var companyInfo = (from e in DB.tbl_Franchise
                               join o in DB.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
                               where e.FranchiseID == franchiseID
                               select e).FirstOrDefault();

            var ownerInfo = (from e in DB.tbl_Franchise_Owner where e.OwnerID == companyInfo.OwnerID select e).FirstOrDefault();

            ownerInfo.OwnerName = model.OwnerName;
            companyInfo.LegalName = model.LegalName;
            companyInfo.LegalAddress = model.LegalAddress;
            companyInfo.LegalCity = model.LegalCity;
            companyInfo.LegalState = model.LegalState;
            companyInfo.LegalPostal = model.LegalPostal;
            companyInfo.LegalCountryID = model.LegalCountryID;

            companyInfo.ShipName = model.ShipName;
            companyInfo.ShipCompany = model.ShipCompany;
            companyInfo.ShipAddress = model.ShipAddress;
            companyInfo.ShipCity = model.ShipCity;
            companyInfo.ShipState = model.ShipState;
            companyInfo.ShipPostal = model.ShipPostal;
            companyInfo.ShipCountryID = model.ShipCountryID;

            companyInfo.MailName = model.MailName;
            companyInfo.MailCompany = model.MailCompany;
            companyInfo.MailAddress = model.MailAddress;
            companyInfo.MailCity = model.MailCity;
            companyInfo.MailState = model.MailState;
            companyInfo.MailPostal = model.MailPostal;
            companyInfo.MailCountryID = model.MailCountryID;

            companyInfo.OfficeName = model.OfficeName;
            companyInfo.OfficeCompany = model.OfficeCompany;
            companyInfo.OfficeAddress = model.OfficeAddress;
            companyInfo.OfficeCity = model.OfficeCity;
            companyInfo.OfficeState = model.OfficeState;
            companyInfo.OfficePostal = model.OfficePostal;
            companyInfo.OfficeCountryID = model.OfficeCountryID;

            DB.SaveChanges();

            return RedirectToAction("MyCompanyInfo");
        }
        public ActionResult DBAList(string frId)
        {
            int frenchiseid = Convert.ToInt32(frId);
            List<tbl_Dispatch_DBA> objDba = (from dba in DB.tbl_Dispatch_DBA where dba.FranchiseID == frenchiseid select dba).ToList();
            return Json(objDba);
        }
        public ActionResult Save_DBA_Data(String DBA, int frId)
        {
            //int frenchiseId = Convert.ToInt32(FrenchiseID);
            var test = "Already exists in the list..";
            try
            {
                var objDba = new tbl_Dispatch_DBA { FranchiseID = frId, DBAName = DBA };
                DB.tbl_Dispatch_DBA.AddObject(objDba);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                test = e.Message;
            }

            return Json(test);
        }

        public ActionResult DeleteDBA(string DBA)
        {

            var test = "Already exists in the list..";
            try
            {
                int dbaid = Convert.ToInt32(DBA);
                tbl_Dispatch_DBA objDBA = (from dba in DB.tbl_Dispatch_DBA where dba.DBAID == dbaid select dba).FirstOrDefault();
                DB.tbl_Dispatch_DBA.DeleteObject(objDBA);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                test = e.Message;
            }
            return Json(test);
        }

        public ActionResult DeleteContact(string Contactid)
        {

            var test = "Already exists in the list..";
            try
            {
                int contactId = Convert.ToInt32(Contactid);
                tbl_Franchise_Contacts objFranchiseContact = (from franchisecontact in DB.tbl_Franchise_Contacts where franchisecontact.FranchiseContactID == contactId select franchisecontact).FirstOrDefault();
                DB.tbl_Franchise_Contacts.DeleteObject(objFranchiseContact);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                test = e.Message;
            }
            return Json(test);
        }

        public ActionResult EmployeeContactAdd(string Phonetype, string Phonenumber, string Contactname, int FranchiseeId)
        {
            var objFranchiseContact = new tbl_Franchise_Contacts
                                          {
                                              FranchiseID = FranchiseeId,
                                              ContactName = Contactname,
                                              PhoneTypeID = Convert.ToInt32(Phonetype),
                                              PhoneNumber = Phonenumber
                                          };
            DB.tbl_Franchise_Contacts.AddObject(objFranchiseContact);
            DB.SaveChanges();
            return Json(string.Empty);
        }

        public ActionResult MyCompanyContact(int FranchiseeId)
        {
            var listEClass = new List<Franchisee>();
            var eclass = new Franchisee();

            var cDetails = (from p in DB.tbl_Franchise_Contacts where p.FranchiseID == FranchiseeId select p);


            foreach (var details in cDetails)
            {
                var det = details;
                var phone = (from d in DB.tbl_PhoneType where d.PhoneTypeID == det.PhoneTypeID select d).FirstOrDefault();
                if (phone != null)
                {
                    eclass.FranchiseID = details.FranchiseID;
                    eclass.PhoneType = phone.PhoneType;
                    eclass.PhoneNumber = details.PhoneNumber;
                    eclass.PhoneTypeID = details.PhoneTypeID;
                    eclass.ContactName = details.ContactName;
                    eclass.FranchiseContactID = details.FranchiseContactID;

                    var summary = new Franchisee
                                             {
                                                 FranchiseContactID = eclass.FranchiseContactID,
                                                 PhoneTypeID = eclass.PhoneTypeID,
                                                 FranchiseID = eclass.FranchiseID,
                                                 PhoneNumber = eclass.PhoneNumber,
                                                 PhoneType = eclass.PhoneType,
                                                 ContactName = eclass.ContactName

                                             };
                    listEClass.Add(summary);
                }
            }

            return Json(listEClass);
        }

        public ActionResult GetBudgetInfo(int franchiseId)
        {
            var budget = (from p in DB.tbl_DailyBudget
                          where p.FranchiseID == franchiseId
                          select new
                          {
                              p.AnualSales,
                              p.MonthlySales,
                              p.DailySales,
                              p.AnnualAvgTicket,
                              p.AnnualClosingRate,
                              p.AnnualHomeGuard,
                              p.MonthlyHomeGuard,
                              p.DailyHomeGuard,
                              p.AnnualBio,
                              p.MonthlyBio,
                              p.DailyBio,
                              p.AnnualPayrollPercentOfJobs,
                              p.AnnualRecallPercentOfJobs,
                              p.AnnualJobs
                          }).FirstOrDefault();

            return Json(budget);
        }

        public ActionResult MyBudgetInfo()
        {
            return View(new MyBudgetInfo());
        }

        public ActionResult SaveBudgetInfo(FormCollection collection)
        {
            var mybudgetinfo = new MyBudgetInfo();

            if (collection.AllKeys.Count() <= 1) return View("MyBudgetInfo", mybudgetinfo);

            var franchiseId = Convert.ToInt32(collection["franchiseId"]);
            var isNew = false;
            var dailybudget = DB.tbl_DailyBudget.SingleOrDefault(p => p.FranchiseID == franchiseId);

            if (dailybudget == null)
            {
                dailybudget = new tbl_DailyBudget();
                isNew = true;
            }

            try
            {
                dailybudget.AnualSales = Convert.ToDecimal(collection["SalesAnnual"].Replace(",", ""));
                dailybudget.MonthlySales = dailybudget.AnualSales / Models.MyBudgetInfo.MonthsPerYear;
                dailybudget.DailySales = dailybudget.AnualSales / Models.MyBudgetInfo.DaysPerYear;
                dailybudget.AnnualAvgTicket = Convert.ToDecimal(collection["AvgTicketAnnual"].Replace(",", ""));

                dailybudget.AnnualHomeGuard = Convert.ToInt32(collection["HomeGuardAnnual"].Replace(",", ""));
                dailybudget.MonthlyHomeGuard = dailybudget.AnnualHomeGuard / Models.MyBudgetInfo.MonthsPerYear;
                dailybudget.DailyHomeGuard = dailybudget.AnnualHomeGuard / Models.MyBudgetInfo.DaysPerYear;

                dailybudget.AnnualBio = Convert.ToInt32(collection["BioAnnual"].Replace(",", ""));
                dailybudget.MonthlyBio = dailybudget.AnnualBio / Models.MyBudgetInfo.MonthsPerYear;
                dailybudget.DailyBio = dailybudget.AnnualBio / Models.MyBudgetInfo.DaysPerYear;

                dailybudget.AnnualClosingRate = Convert.ToDecimal(collection["ClosingRateAnnual"]);
                dailybudget.AnnualRecallPercentOfJobs = Convert.ToDecimal(collection["RecallJobsAnnual"]);
                dailybudget.AnnualPayrollPercentOfJobs = Convert.ToDecimal(collection["PayrollSalesAnnual"]);

                dailybudget.AnnualJobs = Convert.ToInt32(Convert.ToDecimal(collection["txtAnnualjobs"].Replace(",", "")));


                if (isNew)
                {
                    dailybudget.FranchiseID = franchiseId;
                    DB.tbl_DailyBudget.AddObject(dailybudget);
                    ViewBag.lblmessage = "Record created successfully.";
                }
                else
                    ViewBag.lblmessage = "Record updated successfully.";

                DB.SaveChanges();

            }
            catch (Exception)
            {
                ViewBag.lblmessage = "Error saving budget values.  Please be sure all values you have entered result in valid numbers.";
            }

            var budget = DB.tbl_DailyBudget.SingleOrDefault(p => p.FranchiseID == franchiseId);
            if (budget != null)
            {
                mybudgetinfo.SalesAnnual = budget.AnualSales.GetValueOrDefault();
                mybudgetinfo.AvgTicketAnnual = budget.AnnualAvgTicket.GetValueOrDefault();
                mybudgetinfo.ClosingRateAnnual = budget.AnnualClosingRate.GetValueOrDefault();
                mybudgetinfo.HomeGuardAnnual = budget.AnnualHomeGuard.GetValueOrDefault();
                mybudgetinfo.BioAnnual = budget.AnnualBio.GetValueOrDefault();
                mybudgetinfo.PayrollSalesAnnual = budget.AnnualPayrollPercentOfJobs.GetValueOrDefault();
                mybudgetinfo.RecallJobsAnnual = budget.AnnualRecallPercentOfJobs.GetValueOrDefault();
            }

            return View("MyBudgetInfo", mybudgetinfo);
        }
    }
}
