using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using System.Data.Entity;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Areas.SecurityGuard.Models;
using SiteBlue.Data.EightHundred;
using SiteBlue.Controllers;
using DHTMLX.Export.Excel;
using SiteBlue.Business.Job;
using System.Data.Objects.SqlClient;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class EmployeeController : SiteBlueBaseController
    {
        private MembershipConnection memberShipContext = new MembershipConnection();
        EightHundredEntities db = new EightHundredEntities();

        private readonly MembershipService membership = new MembershipService(Membership.Provider);
        private MembershipUser _currentUser;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _currentUser = _currentUser ?? membership.GetUser(User.Identity.Name);
        }

        public ActionResult Index()
        {
            return RedirectToAction("EmployeeList");

        }
        public ActionResult TimeCard()
        {
            return View();
        }
        public ActionResult HR_Form()
        {

            //var reportTechnicians = new List<Technician>();
            //if (User.Identity.Name != "")
            //{
            //    int fId = 56;

            //    if (Session["selectedFranchiseId"] != null)
            //    {
            //        fId = Convert.ToInt32(Session["selectedFranchiseId"]);

            //        DefaultCompamyName = (from f in db.tbl_Franchise
            //                              where f.FranchiseID == fId
            //                              select string.Concat(f.FranchiseNUmber, " - ", f.LegalName)).First();
            //    }
            //    else
            //    {
            //        var membership = new MembershipService(Membership.Provider);
            //        var user = membership.GetUser(User.Identity.Name);
            //        var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            //        var isCorporate = User.IsInRole("Corporate");
            //        int[] assignedFranchises;

            //        DefaultCompamyName = (from g in memberShipContext.UserFranchise
            //                              where g.FranchiseID == fId && g.UserId == userId
            //                              select g.Franchise.FranchiseNumber).FirstOrDefault();
            //        if (DefaultCompamyName == null)
            //        {
            //            DefaultCompamyName = (from g in memberShipContext.UserFranchise
            //                                  where g.UserId == userId
            //                                  select g.Franchise.FranchiseNumber).FirstOrDefault();
            //        }
            //        using (var ctx = new MembershipConnection())
            //        {
            //            assignedFranchises = ctx.UserFranchise
            //                                    .Where(uf => uf.UserId == userId)
            //                                    .Select(f => f.FranchiseID)
            //                                    .ToArray();
            //        }

            //        var franchises = db.tbl_Franchise
            //                           .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
            //                           .OrderBy(f => f.FranchiseNUmber)
            //                           .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
            //                           .ToArray();


            //        ViewBag.frenchise = franchises;

            //        if (Session["selectedFranchiseCode"] != null)
            //        {
            //            Session["code"] = Session["selectedFranchiseCode"];
            //            ViewBag.code = Session["selectedFranchiseCode"];
            //        }
            //        if (Session["code"] == null)
            //        {
            //            ViewBag.code = DefaultCompamyName;
            //        }
            //    }
            //}
            //else
            //{

            //    return new RedirectResult("/SGAccount/LogOn");

            //}
            return View();
        }

        public PartialViewResult PartialHR_Form(string code)
        {
            try
            {
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                var FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();

                Session["selectedFranchiseId"] = FranchiseID;
                Session["selectedFranchiseCode"] = code;
                Session["Code"] = code;

                return PartialView("PartialHR_Form", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult Load_HR_Form()
        {
            List<tbl_HR_Forms> objHrForms = (from hr_form in db.tbl_HR_Forms orderby hr_form.Form select hr_form).ToList();
            var objlstHrForms = new List<tbl_HR_Forms>();
            foreach (var item in objHrForms)
            {
                var objhrforms = new tbl_HR_Forms();
                objhrforms.Form = item.Form;
                objhrforms.Usage = item.Usage;
                objhrforms.Purpose = item.Purpose;
                objhrforms.DocumentName = item.DocumentName;
                objhrforms.FormID = item.FormID;
                objlstHrForms.Add(objhrforms);
            }
            return Json(objlstHrForms);
        }


        public PartialViewResult PartialHR_ReviewForm(string code)
        {
            try
            {
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                var FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();

                Session["selectedFranchiseId"] = FranchiseID;
                Session["selectedFranchiseCode"] = code;
                Session["Code"] = code;

                //var emptyList = new List<string>();
                //emptyList.Add("");
                ViewBag.emptylist = new SelectList(db.tbl_HR_Forms.ToList(), "FormID", "Form");

                return PartialView("PartialHR_ReviewForm", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult HR_ReviewForm(FormCollection fomrcollection)
        {

            //var reportTechnicians = new List<Technician>();
            //if (User.Identity.Name != "")
            //{
            //    int fId = 56;

            //    if (Session["selectedFranchiseId"] != null)
            //    {
            //        fId = Convert.ToInt32(Session["selectedFranchiseId"]);

            //        DefaultCompamyName = (from f in db.tbl_Franchise
            //                              where f.FranchiseID == fId
            //                              select string.Concat(f.FranchiseNUmber, " - ", f.LegalName)).First();
            //    }
            //    else
            //    {
            //        var membership = new MembershipService(Membership.Provider);
            //        var user = membership.GetUser(User.Identity.Name);
            //        var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            //        var isCorporate = User.IsInRole("Corporate");
            //        int[] assignedFranchises;

            //        DefaultCompamyName = (from g in memberShipContext.UserFranchise
            //                              where g.FranchiseID == fId && g.UserId == userId
            //                              select g.Franchise.FranchiseNumber).FirstOrDefault();
            //        if (DefaultCompamyName == null)
            //        {
            //            DefaultCompamyName = (from g in memberShipContext.UserFranchise
            //                                  where g.UserId == userId
            //                                  select g.Franchise.FranchiseNumber).FirstOrDefault();
            //        }
            //        using (var ctx = new MembershipConnection())
            //        {
            //            assignedFranchises = ctx.UserFranchise
            //                                    .Where(uf => uf.UserId == userId)
            //                                    .Select(f => f.FranchiseID)
            //                                    .ToArray();
            //        }

            //        var franchises = db.tbl_Franchise
            //                           .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
            //                           .OrderBy(f => f.FranchiseNUmber)
            //                           .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
            //                           .ToArray();


            //        ViewBag.frenchise = franchises;

            //        if (Session["selectedFranchiseCode"] != null)
            //        {
            //            Session["code"] = Session["selectedFranchiseCode"];
            //            ViewBag.code = Session["selectedFranchiseCode"];
            //        }
            //        if (Session["code"] == null)
            //        {
            //            ViewBag.code = DefaultCompamyName;
            //        }
            //    }
            //}
            //else
            //{
            //    return new RedirectResult("/SGAccount/LogOn");
            //}
            ViewBag.emptylist = new SelectList(db.tbl_HR_Forms.ToList(), "FormID", "Form");


            return View();
        }

        public ActionResult LoadHRReviewData()
        {
            List<tbl_HR_Forms> objHRForms = (from hrform in db.tbl_HR_Forms orderby hrform.Form select hrform).ToList();

            return Json(objHRForms);
        }

        public ActionResult LoadReviewHisotryData(string franchisid)
        {
            int FranchiseID = Convert.ToInt32(franchisid);

            var ReviewList = (from r in db.tbl_HR_Reviews
                              where r.FranchiseID == FranchiseID
                              join e in db.tbl_Employee on r.EmployeeID equals e.EmployeeID
                              orderby r.ReviewDate
                              select new
                                  {
                                      r.Comments,
                                      r.CompletedYN,
                                      r.EmployeeID,
                                      r.FranchiseID,
                                      r.ReviewID,
                                      r.Subject,
                                      r.timestamp,
                                      r.FormID,
                                      r.ReviewDate,
                                      e.Employee
                                  }).ToList();

            var objEmployeeForms = new List<Employee_Forms>();
            foreach (var item in ReviewList)
            {
                tbl_HR_Forms objhrform = (from hr_form in db.tbl_HR_Forms where hr_form.FormID == item.FormID select hr_form).FirstOrDefault();

                var oEmployeeForm = new Employee_Forms();

                oEmployeeForm.ReviewID = item.ReviewID;
                oEmployeeForm.ReviewDate = item.ReviewDate.Value;
                if (objhrform != null)
                {
                    oEmployeeForm.Form = objhrform.Form;
                }
                else { oEmployeeForm.Form = ""; }
                oEmployeeForm.Subject = item.Subject;
                oEmployeeForm.CompletedYN = item.CompletedYN;
                oEmployeeForm.Employee = item.Employee;
                objEmployeeForms.Add(oEmployeeForm);
            }

            return Json(objEmployeeForms);
        }

        public JsonResult AddReview(tbl_HR_Reviews newdata)
        {
            try
            {
                db.tbl_HR_Reviews.AddObject(newdata);
                db.SaveChanges();

                return Json("Success");
            }
            catch
            {
                return Json("Failed");
            }
        }

        //public ActionResult AddReview(FormCollection formcollection)
        //{
        //    int franchiseId = Convert.ToInt32(Session["FranchiseId"]);
        //    if (formcollection["hdnAction"] == "Add")
        //    {
        //        var objReview = new tbl_HR_Reviews();
        //        objReview.Comments = formcollection["txtComments"];
        //        objReview.Subject = formcollection["txtSubject"];
        //        if (formcollection["chkCompleted"] == "false")
        //            objReview.CompletedYN = false;
        //        else
        //            objReview.CompletedYN = true;
        //        //objReview.CompletedYN =  Convert.ToBoolean(formcollection["chkCompleted"]);
        //        objReview.EmployeeID = Convert.ToInt32(formcollection["hdnEmpID"]);
        //        //objReview.FormID 
        //        if (franchiseId.Equals(null))
        //        {
        //            objReview.FranchiseID = 56;
        //            franchiseId = 56;
        //        }
        //        else
        //        {
        //            objReview.FranchiseID = franchiseId;
        //        }
        //        objReview.ReviewDate = Convert.ToDateTime(formcollection["txtdate"]);
        //        db.tbl_HR_Reviews.AddObject(objReview);
        //        db.SaveChanges();

        //    }
        //    if (formcollection["hdnAction"] == "Edit")
        //    {
        //        int reviewid = Convert.ToInt32(formcollection["hdnReviewID"]);
        //        var Review = (from r in db.tbl_HR_Reviews where r.ReviewID == reviewid select r).FirstOrDefault();
        //        Review.Comments = formcollection["txtComments"];
        //        Review.Subject = formcollection["txtSubject"];
        //        if (formcollection["chkCompleted"] == "false")
        //            Review.CompletedYN = false;
        //        else
        //            Review.CompletedYN = true;
        //        if (franchiseId.Equals(null))
        //        {
        //            Review.FranchiseID = 56;
        //            franchiseId = 56;

        //        }
        //        else
        //        {
        //            Review.FranchiseID = franchiseId;
        //        }
        //        Review.ReviewDate = Convert.ToDateTime(formcollection["txtdate"]);
        //        db.SaveChanges();


        //    }

        //    int employeeid = 68;
        //    ViewBag.empid = employeeid;
        //    ViewBag.FrenchiseID = franchiseId.ToString();
        //    Session["FranchiseId"] = 56;
        //    ViewBag.FrenchiseID = Session["FranchiseId"].ToString();
        //    if ((formcollection["ddlFrenchise"]) != null)
        //    {
        //        Session["FranchiseId"] = (formcollection["ddlFrenchise"]);
        //        ViewBag.FrenchiseID = (formcollection["ddlFrenchise"]).ToString();
        //    }
        //    //var lst = new List<string>() { "" };
        //    ViewBag.emptylist = new SelectList(db.tbl_HR_Forms.ToList(), "FormID", "Form");
        //    List<tbl_Franchise> f = (from frn in db.tbl_Franchise select frn).ToList();
        //    ViewBag.frenchise = f;
        //    ViewBag.Date = DateTime.Now.ToShortDateString();


        //    return View("HR_ReviewForm");

        //}

        public ActionResult EditReview(string reviewID)
        {

            int ReviewID = Convert.ToInt32(reviewID);
            tbl_HR_Reviews objReview = (from review in db.tbl_HR_Reviews where review.ReviewID == ReviewID select review).FirstOrDefault();


            //List<Employee_Forms> objEmployeeForms = new List<Employee_Forms>();
            //HR_Forms objhrform = (from hr_form in db.tbl_HR_Forms where hr_form.FormID == objReview.FormID select hr_form).FirstOrDefault();

            //Employee_Forms oEmployeeForm = new Employee_Forms();

            //oEmployeeForm.ReviewID = objReview.ReviewID;
            //oEmployeeForm.ReviewDate = objReview.ReviewDate;
            //if (objhrform != null)
            //{
            //    oEmployeeForm.Form = objhrform.Form;
            //}
            //else { 
            //    oEmployeeForm.Form = ""; 
            //}
            //oEmployeeForm.Subject = objReview.Subject;
            //oEmployeeForm.CompletedYN = objReview.CompletedYN;

            return Json(objReview);
        }

        public ActionResult EmployeeList(FormCollection formcollection)
        {
            var reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                Session["showInactive"] = true;

            }
            else
            {
                return new RedirectResult("/SGAccount/LogOn");
            }
            return View();

        }

        public PartialViewResult PartialEmployeeList(string code)
        {
            try
            {
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                var FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();


                Session["showInactive"] = true;
                Session["selectedFranchiseId"] = FranchiseID;
                Session["selectedFranchiseCode"] = code;
                Session["Code"] = code;
                return PartialView("PartialEmployeeList", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult EmployeeData(bool checkStatus, string frenchiseeID)
        {

            var collection = new FormCollection();

            var check1 = Request.QueryString["showInactive"];
            var FranchiseID = Convert.ToInt32(frenchiseeID);
            var objemp = new Emplist();
            var lstemp = new List<Emplist>();
            Emplist objsummary;
            if (checkStatus == true)
            {
                ViewBag.showInactive = false;
                var objemployee = new List<tbl_Employee>();
                objemployee = (from e in db.tbl_Employee where e.FranchiseID == FranchiseID && e.ActiveYN == true select e).ToList();

                if (objemployee != null)
                {
                    foreach (var item in objemployee)
                    {
                        tbl_Employee_Contact ObjEc = (from ec in db.tbl_Employee_Contact where ec.PhoneTypeID == 2 && ec.EmployeeID == item.EmployeeID select ec).FirstOrDefault();
                        objemp.EmpId = item.EmployeeID;
                        objemp.EmpAddress = item.Address;
                        objemp.EmpCity = item.City;
                        objemp.EmpName = item.Employee;
                        objemp.EmpPostal = item.Postal;
                        objemp.EmpState = item.State;
                        if (ObjEc != null)
                        {
                            objemp.EmpPrimaryPhone = Convert.ToInt64(ObjEc.PhoneNumber.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "")).ToString("(###) ###-####");
                        }
                        else
                        {
                            objemp.EmpPrimaryPhone = "";
                        }
                        objsummary = new Emplist
                        {
                            EmpId = objemp.EmpId,
                            EmpAddress = objemp.EmpAddress,
                            EmpCity = objemp.EmpCity,
                            EmpName = objemp.EmpName,
                            EmpPostal = objemp.EmpPostal,
                            EmpState = objemp.EmpState,
                            EmpPrimaryPhone = objemp.EmpPrimaryPhone
                        };
                        lstemp.Add(objsummary);
                    }

                }

                return Json(lstemp);
            }
            else if (checkStatus == false)
            {
                ViewBag.showInactive = true;
                var objemployee = new List<tbl_Employee>();
                objemployee = (from e in db.tbl_Employee where e.FranchiseID == FranchiseID && e.ActiveYN == false select e).ToList();

                if (objemployee != null)
                {
                    foreach (var item in objemployee)
                    {
                        tbl_Employee_Contact ObjEc = (from ec in db.tbl_Employee_Contact where ec.PhoneTypeID == 2 && ec.EmployeeID == item.EmployeeID select ec).FirstOrDefault();
                        objemp.EmpId = item.EmployeeID;
                        objemp.EmpAddress = item.Address;
                        objemp.EmpCity = item.City;
                        objemp.EmpName = item.Employee;
                        objemp.EmpPostal = item.Postal;
                        objemp.EmpState = item.State;
                        if (ObjEc != null)
                        {
                            objemp.EmpPrimaryPhone = ObjEc.PhoneNumber;
                        }
                        else
                        {
                            objemp.EmpPrimaryPhone = "";
                        }
                        objsummary = new Emplist
                        {
                            EmpId = objemp.EmpId,
                            EmpAddress = objemp.EmpAddress,
                            EmpCity = objemp.EmpCity,
                            EmpName = objemp.EmpName,
                            EmpPostal = objemp.EmpPostal,
                            EmpState = objemp.EmpState,
                            EmpPrimaryPhone = objemp.EmpPrimaryPhone
                        };
                        lstemp.Add(objsummary);
                    }

                }
                return Json(lstemp);
            }
            else
            {
                ViewBag.showInactive = true;
                var objemployee = new List<tbl_Employee>();
                objemployee = (from e in db.tbl_Employee where e.FranchiseID == FranchiseID && e.ActiveYN == true select e).ToList();

                if (objemployee != null)
                {
                    foreach (var item in objemployee)
                    {
                        tbl_Employee_Contact ObjEc = (from ec in db.tbl_Employee_Contact where ec.PhoneTypeID == 2 && ec.EmployeeID == item.EmployeeID select ec).FirstOrDefault();
                        objemp.EmpId = item.EmployeeID;
                        objemp.EmpAddress = item.Address;
                        objemp.EmpCity = item.City;
                        objemp.EmpName = item.Employee;
                        objemp.EmpPostal = item.Postal;
                        objemp.EmpState = item.State;
                        if (ObjEc != null)
                        {
                            objemp.EmpPrimaryPhone = ObjEc.PhoneNumber;
                        }
                        else
                        {
                            objemp.EmpPrimaryPhone = "";
                        }
                        objsummary = new Emplist
                        {
                            EmpId = objemp.EmpId,
                            EmpAddress = objemp.EmpAddress,
                            EmpCity = objemp.EmpCity,
                            EmpName = objemp.EmpName,
                            EmpPostal = objemp.EmpPostal,
                            EmpState = objemp.EmpState,
                            EmpPrimaryPhone = objemp.EmpPrimaryPhone
                        };
                        lstemp.Add(objsummary);
                    }

                }
                return Json(lstemp);
            }
        }

        public void AddContactNumber(string Employeeid, string PhoneTypeID, string PhoneNumber)
        {

            var objEmpContact = new tbl_Employee_Contact();


            objEmpContact.PhoneNumber = Convert.ToInt64(PhoneNumber).ToString("(###)###-####");
            objEmpContact.EmployeeID = Convert.ToInt32(Employeeid);
            objEmpContact.PhoneTypeID = Convert.ToInt32(PhoneTypeID);


            db.tbl_Employee_Contact.AddObject(objEmpContact);
            db.SaveChanges();

        }

        public ActionResult EmployeeInformation(int id, int frid)
        {
            ViewBag.FranchiseId = frid.ToString();
            //string s = (string)RouteData.Values["frid"];

            var swapBranding = false;
            var isOwner = HttpContext.User.IsInRole("CompanyOwner");
            var user = membership.GetUser(User.Identity.Name);

            var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            if (isOwner)
            {

                userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
                using (var ctx = new MembershipConnection())
                {
                    swapBranding = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .ToArray()
                                            .Select(uf => (from f in db.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).SingleOrDefault())
                                            .Any(f => f.FranchiseTypeID == 6);
                }
            }

            tbl_Employee employee;

            ViewBag.SwapBranding = swapBranding;
            if (id == 0)
            {
                employee = new tbl_Employee();
                employee.FranchiseID = frid;
                //ViewBag.EmpId = "0";
                //ViewBag.EmpName = "";
                //ViewBag.Address = "";
                //ViewBag.City = "";
                //ViewBag.State = "";
                //ViewBag.Postal = "";
                //ViewBag.Country = "";
                //DateTime? dt = null;
                //ViewBag.BirthDate = dt;
                //ViewBag.Anniversary = dt;
                //ViewBag.Spouse = "";
                //ViewBag.Email = "";

                //ViewBag.ECContactName = "";
                //ViewBag.ECAddress = "";
                //ViewBag.ECCity = "";
                //ViewBag.ECState = "";
                //ViewBag.ECPostal = "";
                //ViewBag.ECCountry = "";
                //ViewBag.ECPNum = "";
                //ViewBag.ECPNum = "";
                //ViewBag.ECSNum = "";

                //ViewBag.Active = false;
                //ViewBag.CommLabor = 0.00;
                //ViewBag.Parts = 0.00;
                //ViewBag.HRate = 0.00;
                //ViewBag.WSalary = 0.00;
                //ViewBag.Exempt = false;
                //ViewBag.DLicense = "";
                //ViewBag.DEXDate = dt;
                //ViewBag.SS = "";
                //ViewBag.PLicense = "";
                //ViewBag.PEXDate = dt;
                //ViewBag.Level = "";
                //ViewBag.Hired = dt;
                //ViewBag.Terminated = dt;
                //ViewBag.ServicePro = false;
                ////ViewBag.CallTaker = false;
                ////ViewBag.Dispatcher = objemployee.DispatcherYN;
                //ViewBag.HVACSales = false;
                //ViewBag.UserKey = "";

                //ViewBag.VEDays = 0.00;
                //ViewBag.VUDays = 0.00;
                //ViewBag.VADays = 0.00;
                //ViewBag.SEDays = 0.00;
                //ViewBag.SUDays = 0.00;
                //ViewBag.SADays = 0.00;
                //ViewBag.PEDays = 0.00;
                //ViewBag.PUDays = 0.00;
                //ViewBag.PADays = 0.00;
                //ViewBag.OEDays = 0.00;
                //ViewBag.OUDays = 0.00;
                //ViewBag.OADays = 0.00;


                var franchiseUsers = memberShipContext.UserFranchise.Where(uf => uf.FranchiseID == frid).ToArray();
                var membership1 = new MembershipService(Membership.Provider);
                ViewBag.ddlFranchiseUsers = franchiseUsers.Select(uf => membership1.GetUser(uf.UserId)).ToArray();

                //List<tbl_Employee_PayType> objtemp = new List<tbl_Employee_PayType>();
                //tbl_Employee_PayType objpaytype = new tbl_Employee_PayType();
                //objtemp.Add(objpaytype);
                //ViewBag.PalType = new SelectList(objtemp, "PayTypeID", "PayType", 0);
                ViewBag.AwardsList = "";
                var Phone = (from d in db.tbl_PhoneType where d.PhoneTypeID == 2 select d).ToList();
                ViewBag.ddlPhoneTypeAdd = Phone;
                //       var franchiseUsers = memberShipContext.UserFranchise.Where(uf => uf.FranchiseID == objemployee.FranchiseID).ToArray();
                //var membership = new MembershipService(Membership.Provider);
                //ViewBag.ddlFranchiseUsers = franchiseUsers.Select(uf => membership.GetUser(uf.UserId)).ToArray();

                // ViewBag.ddlFranchiseUsers 
            }
            else
            {
                employee = db.tbl_Employee.FirstOrDefault(e => e.EmployeeID == id);
                //var objemployee = (from e in db.tbl_Employee where e.EmployeeID == id select e).FirstOrDefault();
                //ViewBag.EmpId = objemployee.EmployeeID.ToString();
                //ViewBag.EmpName = objemployee.Employee;
                //ViewBag.Address = objemployee.Address;
                //ViewBag.City = objemployee.City;
                //ViewBag.State = objemployee.State;
                //ViewBag.Postal = objemployee.Postal;
                //ViewBag.Country = objemployee.Country;
                //ViewBag.BirthDate = objemployee.BirthDate;
                //ViewBag.Anniversary = objemployee.Anniversary;
                //ViewBag.Spouse = objemployee.Spouse;
                //ViewBag.Email = objemployee.Email;

                //ViewBag.ECContactName = objemployee.EmerContact;
                //ViewBag.ECAddress = objemployee.EmerAddress;
                //ViewBag.ECCity = objemployee.EmerCity;
                //ViewBag.ECState = objemployee.EmerState;
                //ViewBag.ECPostal = objemployee.EmerPostal;
                //ViewBag.ECCountry = objemployee.EmerCountry;

                //if (objemployee.EmerPrimaryPhone != null && objemployee.EmerPrimaryPhone.Trim() != "")
                //{
                //    if (objemployee.EmerPrimaryPhone.Length > 12)
                //    {
                //        ViewBag.ECPNum =
                //            Convert.ToInt64(
                //                objemployee.EmerPrimaryPhone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(
                //                    ")", "")).ToString("(###) ###-####");
                //    }
                //    else
                //    {
                //        ViewBag.ECPNum =
                //            Convert.ToInt64(
                //                objemployee.EmerPrimaryPhone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(
                //                    ")", "")).ToString("(###) ###-####");
                //    }
                //}
                //else
                //{
                //    ViewBag.ECPNum = "";
                //}
                //if (objemployee.EmerSecondaryPhone != null && objemployee.EmerSecondaryPhone.Trim() != "")
                //{
                //    if (objemployee.EmerSecondaryPhone.Length > 10)
                //    {
                //        ViewBag.ECSNum =
                //            Convert.ToInt64(
                //                objemployee.EmerSecondaryPhone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(
                //                    ")", "").Replace("x", "")).ToString("(###) ###-####");
                //    }
                //    else
                //    {
                //        ViewBag.ECSNum =
                //            Convert.ToInt64(
                //                objemployee.EmerSecondaryPhone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(
                //                    ")", "").Replace("x", "")).ToString("(###) ###-####");
                //    }
                //}
                //else
                //{
                //    ViewBag.ECSNum = "";
                //}

                //ViewBag.Active = objemployee.ActiveYN;
                //ViewBag.CommLabor = objemployee.LaborRate;
                //ViewBag.Parts = objemployee.PartRate;
                //ViewBag.HRate = objemployee.HourlyRate;
                //ViewBag.WSalary = objemployee.WeeklySalary;
                //ViewBag.Exempt = objemployee.ExemptYN;
                //ViewBag.DLicense = objemployee.DriversLic;
                //ViewBag.DEXDate = objemployee.DLExpDate;
                //ViewBag.SS = !string.IsNullOrEmpty(objemployee.SS_)
                //                 ? (Convert.ToInt32(
                //                     objemployee.SS_.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", ""))).
                //                       ToString("###-##-####")
                //                 : string.Empty;
                //ViewBag.PLicense = objemployee.ProLisc;
                //ViewBag.PEXDate = objemployee.ProLiscExpDate;
                //ViewBag.Level = objemployee.ProLevel;
                //ViewBag.Hired = objemployee.HiredDate.GetValueOrDefault();
                //ViewBag.Terminated = objemployee.TerminatedDate;
                //ViewBag.ServicePro = objemployee.ServiceProYN;
                //ViewBag.CallTaker = objemployee.CallTakerYN;
                //ViewBag.Dispatcher = objemployee.DispatcherYN;
                //ViewBag.HVACSales = objemployee.HVACSalesYN;
                //ViewBag.UserKey = objemployee.UserKey;

                //ViewBag.VEDays = objemployee.VacationEarned;
                //ViewBag.VUDays = objemployee.VacationUsed;
                //ViewBag.VADays = objemployee.VacationEarned - objemployee.VacationUsed;
                //ViewBag.SEDays = objemployee.SickEarned;
                //ViewBag.SUDays = objemployee.SickUsed;
                //ViewBag.SADays = objemployee.SickEarned - objemployee.SickUsed;
                //ViewBag.PEDays = objemployee.PersonalEarned;
                //ViewBag.PUDays = objemployee.PersonalUsed;
                //ViewBag.PADays = objemployee.PersonalEarned - objemployee.PersonalUsed;
                //ViewBag.OEDays = objemployee.OtherEarned;
                //ViewBag.OUDays = objemployee.OtherUsed;
                //ViewBag.OADays = objemployee.OtherEarned - objemployee.OtherUsed;

                var objemployee2 = db.tbl_Employee_Awards.Where(e => e.EmployeeID == employee.EmployeeID).ToList();
                //(from e in db.tbl_Employee_Awards where e.EmployeeID == employee.EmployeeID select e).ToList();

                foreach (var award in objemployee2)
                {
                    ViewBag.AwardsList = (ViewBag.AwardsList
                                          + (Convert.ToDateTime(award.DateReceived).Date + (" - "
                                                                                            +
                                                                                            (from e in db.tbl_Award
                                                                                             where
                                                                                                 e.AwardID == award.AwardID
                                                                                             select e.Award).FirstOrDefault() +
                                                                                            "\r\n")));
                }
                var objec = (from ec in db.tbl_Employee_Contact
                             where ec.EmployeeID == employee.EmployeeID && ec.PhoneTypeID == 2 && ec.PhoneTypeID != null
                             select ec);
                if (objec.Count() != 0)
                {
                    var Phone = (from d in db.tbl_PhoneType select d).ToList();
                    ViewBag.ddlPhoneTypeAdd = Phone;
                }
                else
                {
                    var Phone = (from d in db.tbl_PhoneType where d.PhoneTypeID == 2 select d).ToList();
                    ViewBag.ddlPhoneTypeAdd = Phone;
                }

                //var franchiseUsers = memberShipContext.UserFranchise.Where(uf => uf.FranchiseID == employee.FranchiseID).ToArray();
                var franchiseUsers = memberShipContext.UserFranchise.Where(uf => uf.FranchiseID == frid).ToArray();
                var membership1 = new MembershipService(Membership.Provider);
                ViewBag.ddlFranchiseUsers = franchiseUsers.Select(uf => membership1.GetUser(uf.UserId)).ToArray();
            }

            return View(employee);
        }

        public ActionResult popCustomerNumber(int id)
        {
            ViewBag.EmpId = id;
            return View();
        }

        public ActionResult EmployeeContact(int id)
        {
            var ListEClass = new List<EmployeeContactClass>();
            var Eclass = new EmployeeContactClass();

            var CDetails = (from p in db.tbl_Employee_Contact where p.EmployeeID == id select p);
            foreach (var Details in CDetails)
            {
                var Phone = (from d in db.tbl_PhoneType where d.PhoneTypeID == Details.PhoneTypeID select d).FirstOrDefault();

                var Summury = new EmployeeContactClass
                {
                    PhoneTypeID = 0,
                    EmployeeID = 0,
                    PhoneNumber = "",
                    PhoneType = ""
                };

                if (Phone != null)
                {
                    Summury.EmployeeID = Details.EmployeeID;
                    Summury.PhoneType = Phone.PhoneType;
                    if (Details.PhoneNumber != null)
                    {
                        if (Details.PhoneNumber.Length > 12)
                        {
                            Summury.PhoneNumber = Convert.ToInt64(Details.PhoneNumber.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "")).ToString("(###) ###-####");
                        }
                        else
                        {
                            Summury.PhoneNumber = Convert.ToInt64(Details.PhoneNumber.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "")).ToString("(###) ###-####");
                        }
                    }

                    Summury.PhoneTypeID = Details.PhoneTypeID;
                    Summury.EmpContactID = Details.EmpContactID;
                }

                ListEClass.Add(Summury);
            }

            return Json(ListEClass);
        }

        public ActionResult EmployeeContactAdd(int phonetype, string phonenumber, int Empid)
        {
            var empContact = new tbl_Employee_Contact();
            empContact.EmployeeID = Empid;
            empContact.PhoneTypeID = phonetype;
            empContact.PhoneNumber = phonenumber;
            db.tbl_Employee_Contact.AddObject(empContact);
            db.SaveChanges();
            var test = "";
            return Json(test);
        }

        public JsonResult EmployeeContactEdit(int empcontactID)
        {
            var empContactEdit = (from p in db.tbl_Employee_Contact where p.EmpContactID == empcontactID select p).Single();

            return Json(new { EmpContactID = empContactEdit.EmpContactID, PhoneTypeID = empContactEdit.PhoneTypeID, PhoneNumber = empContactEdit.PhoneNumber }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeContactDelete(int id)
        {
            try
            {
                var employeeContact = db.tbl_Employee_Contact.First(e => e.EmpContactID == id);
                db.tbl_Employee_Contact.DeleteObject(employeeContact);
                db.SaveChanges();

                return Json(new { message = "Contact number deleted with success!", state = true });
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return Json(new { message = "An error occured while deleting the contact number, please try again!", state = false });
            }
        }

        public ActionResult EmployeeContactUpdate(int phonetype, string phonenumber, int empcontactID)
        {
            var empContactUpdate = (from p in db.tbl_Employee_Contact where p.EmpContactID == empcontactID select p).FirstOrDefault();
            empContactUpdate.PhoneTypeID = phonetype;
            empContactUpdate.PhoneNumber = phonenumber;
            db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        public ActionResult EmployeeInformation(int frid, FormCollection formcollection)
        {
            if (formcollection["EmployeeID"] == null)
                return View();
            Int32 id;

            Int32 FranchiseId = Convert.ToInt32(formcollection["FranchiseID"]);

            if (FranchiseId == 0)
                FranchiseId = frid;

            var employeeTablet = string.IsNullOrWhiteSpace(formcollection["hdnTablet"]) ? null : formcollection["hdnTablet"].ToString();

            if (employeeTablet != null)
            {
                if (employeeTablet == "Select Tablet")
                {
                    employeeTablet = null;
                }
            }

            string type = Convert.ToString(formcollection["hdnType"]);
            if (type == "Add")
            {
                tbl_Employee Emp = new tbl_Employee();
                Emp.FranchiseID = FranchiseId;
                Emp.Employee = formcollection["Employee"];
                Emp.Address = formcollection["Address"];
                Emp.City = formcollection["City"];
                Emp.State = formcollection["State"];
                Emp.Postal = formcollection["Postal"];
                Emp.Country = formcollection["Country"];
                if (formcollection["BirthDate"] != "")
                {
                    Emp.BirthDate = Convert.ToDateTime(formcollection["BirthDate"]);
                }
                if (formcollection["Anniversary"] != "")
                {
                    Emp.Anniversary = Convert.ToDateTime(formcollection["Anniversary"]);
                }
                Emp.Spouse = formcollection["Spouse"];
                Emp.Email = formcollection["Email"];

                Emp.EmerContact = formcollection["EmerContact"];
                Emp.EmerAddress = formcollection["EmerAddress"];
                Emp.EmerCity = formcollection["EmerCity"];
                Emp.EmerState = formcollection["EmerState"];
                Emp.EmerPostal = formcollection["EmerPostal"];
                Emp.EmerCountry = formcollection["EmerCountry"];
                Emp.EmerPrimaryPhone = formcollection["EmerPrimaryPhone"];
                Emp.EmerSecondaryPhone = formcollection["EmerSecondaryPhone"];

                if (string.Compare(formcollection["ActiveYN"], "false") == 0)
                    Emp.ActiveYN = false;
                else if (string.Compare(formcollection["ActiveYN"], "true,false") == 0)
                    Emp.ActiveYN = true;
                else
                    throw new ArgumentNullException(formcollection["ActiveYN"]);

                //Emp.PayTypeID = int.Parse((formcollection["PayType"]));

                if (formcollection["ReceivesCommission"] != null)
                {
                    if (string.Compare(formcollection["ReceivesCommission"], "false") == 0)
                    {
                        Emp.ReceivesCommission = false;
                        Emp.LaborRate = 0;
                        Emp.PartRate = 0;
                    }
                    else if (string.Compare(formcollection["ReceivesCommission"], "true,false") == 0)
                    {
                        Emp.ReceivesCommission = true;
                        Emp.LaborRate = float.Parse((formcollection["LaborRate"]));
                        Emp.PartRate = float.Parse((formcollection["LaborRate"]));
                    }
                    else
                        throw new ArgumentNullException(formcollection["ReceivesCommission"]);
                }
                else
                {
                    Emp.ReceivesCommission = false;
                    Emp.LaborRate = 0;
                    Emp.PartRate = 0;
                }

                if (formcollection["IsHourlyRate"] != null)
                {
                    if (string.Compare(formcollection["IsHourlyRate"], "false") == 0)
                    {
                        Emp.IsHourlyRate = false;
                        Emp.HourlyRate = 0;
                    }
                    else if (string.Compare(formcollection["IsHourlyRate"], "true,false") == 0)
                    {
                        Emp.IsHourlyRate = true;
                        Emp.HourlyRate = float.Parse((formcollection["HourlyRate"]));
                    }
                    else
                        throw new ArgumentNullException(formcollection["IsHourlyRate"]);
                }
                else
                {
                    Emp.IsHourlyRate = false;
                    Emp.HourlyRate = 0;
                }

                if (formcollection["IsWeeklySalary"] != null)
                {
                    if (string.Compare(formcollection["IsWeeklySalary"], "false") == 0)
                    {
                        Emp.IsWeeklySalary = false;
                        Emp.WeeklySalary = 0;
                    }
                    else if (string.Compare(formcollection["IsWeeklySalary"], "true,false") == 0)
                    {
                        Emp.IsWeeklySalary = true;
                        Emp.WeeklySalary = float.Parse((formcollection["WeeklySalary"]));
                    }
                    else
                        throw new ArgumentNullException(formcollection["IsWeeklySalary"]);
                }
                else
                {
                    Emp.IsWeeklySalary = false;
                    Emp.WeeklySalary = 0;
                }

                if (string.Compare(formcollection["ExemptYN"], "false") == 0)
                    Emp.ExemptYN = false;
                else if (string.Compare(formcollection["ExemptYN"], "true,false") == 0)
                    Emp.ExemptYN = true;
                else
                    throw new ArgumentNullException(formcollection["ExemptYN"]);

                Emp.DriversLic = formcollection["DriversLic"];
                if (formcollection["DLExpDate"] != "")
                {
                    Emp.DLExpDate = Convert.ToDateTime(formcollection["DLExpDate"]);
                }
                Emp.SS_ = formcollection["SS_"];
                Emp.ProLisc = formcollection["ProLisc"];
                if (formcollection["ProLiscExpDate"] != "")
                {
                    Emp.ProLiscExpDate = Convert.ToDateTime(formcollection["ProLiscExpDate"]);
                }
                Emp.ProLevel = formcollection["ProLevel"];
                if (formcollection["HiredDate"] != "")
                {
                    Emp.HiredDate = Convert.ToDateTime(formcollection["HiredDate"]);
                }
                if (formcollection["TerminatedDate"] != "")
                {
                    Emp.TerminatedDate = Convert.ToDateTime(formcollection["TerminatedDate"]);
                }
                if (string.Compare(formcollection["ServiceProYN"], "false") == 0)
                    Emp.ServiceProYN = false;
                else if (string.Compare(formcollection["ServiceProYN"], "true,false") == 0)
                    Emp.ServiceProYN = true;
                else
                    throw new ArgumentNullException(formcollection["ServiceProYN"]);

                if (string.Compare(formcollection["HVACSalesYN"], "false") == 0)
                {
                    Emp.HVACSalesYN = false;
                    Emp.UserKey = null;
                }
                else if (string.Compare(formcollection["HVACSalesYN"], "true,false") == 0)
                {
                    Emp.HVACSalesYN = true;
                    Emp.UserKey = string.IsNullOrEmpty(formcollection["UserKey"]) ? null : formcollection["UserKey"];
                }

                else
                    throw new ArgumentNullException(formcollection["HVACSalesYN"]);

                Emp.VacationEarned = Convert.ToInt32(formcollection["VacationEarned"]);
                Emp.VacationUsed = Convert.ToInt32(formcollection["VacationUsed"]);
                Emp.SickEarned = Convert.ToInt32(formcollection["SickEarned"]);
                Emp.SickUsed = Convert.ToInt32(formcollection["SickUsed"]);
                Emp.PersonalEarned = Convert.ToInt32(formcollection["PersonalEarned"]);
                Emp.PersonalUsed = Convert.ToInt32(formcollection["PersonalUsed"]);
                Emp.OtherEarned = Convert.ToInt32(formcollection["OtherEarned"]);
                Emp.OtherUsed = Convert.ToInt32(formcollection["OtherUsed"]);
                Emp.DisplayOrder = Convert.ToInt32(formcollection["DisplayOrder"]);

                db.tbl_Employee.AddObject(Emp);
                db.SaveChanges();

                var newlyAddedEmployeeID = Emp.EmployeeID;

                // Tablet Assignment
                if (employeeTablet != null)
                {
                    // Update employee tablet assignment.                    
                    char[] delimiterChars = { '(' };
                    char[] charsToTrim = { ')' };
                    var tabletNumber = employeeTablet.Split(delimiterChars)[1];
                    tabletNumber = tabletNumber.TrimEnd(charsToTrim);

                    var franchiseTablet = (from item in db.tbl_Franchise_Tablets
                                           where ((item.FranchiseID == FranchiseId) && (item.TabletNumber == tabletNumber))
                                           select item).FirstOrDefault();

                    if (franchiseTablet != null)
                    {
                        // Update existing tablet assignment.
                        franchiseTablet.EmployeeID = newlyAddedEmployeeID;
                        db.SaveChanges();
                    }
                }

                //var employee = (from p in db.tbl_Employee select p).Last();
                //id = employee.EmployeeID;
                return RedirectToAction("EmployeeList");
            }
            else
            {
                id = Convert.ToInt32(formcollection["EmployeeID"]);
                var Emp = (from p in db.tbl_Employee where p.EmployeeID == id select p).FirstOrDefault();

                if (Emp == null)
                    return View();

                Emp.Employee = formcollection["Employee"];
                Emp.Address = formcollection["Address"];
                Emp.City = formcollection["City"];
                Emp.State = formcollection["State"];
                Emp.Postal = formcollection["Postal"];
                Emp.Country = formcollection["Country"];
                if (formcollection["BirthDate"] != "")
                {
                    Emp.BirthDate = Convert.ToDateTime(formcollection["BirthDate"]);
                }
                if (formcollection["Anniversary"] != "")
                {
                    Emp.Anniversary = Convert.ToDateTime(formcollection["Anniversary"]);
                }
                Emp.Spouse = formcollection["Spouse"];
                Emp.Email = formcollection["Email"];

                Emp.EmerContact = formcollection["EmerContact"];
                Emp.EmerAddress = formcollection["EmerAddress"];
                Emp.EmerCity = formcollection["EmerCity"];
                Emp.EmerState = formcollection["EmerState"];
                Emp.EmerPostal = formcollection["EmerPostal"];
                Emp.EmerCountry = formcollection["EmerCountry"];
                Emp.EmerPrimaryPhone = formcollection["EmerPrimaryPhone"];
                Emp.EmerSecondaryPhone = formcollection["EmerSecondaryPhone"];

                if (string.Compare(formcollection["ActiveYN"], "false") == 0)
                    Emp.ActiveYN = false;
                else if (string.Compare(formcollection["ActiveYN"], "true,false") == 0)
                    Emp.ActiveYN = true;
                else
                    throw new ArgumentNullException(formcollection["ActiveYN"]);

                //Emp.PayTypeID = int.Parse((formcollection["PayType"]));

                if (formcollection["ReceivesCommission"] != null)
                {
                    if (string.Compare(formcollection["ReceivesCommission"], "false") == 0)
                    {
                        Emp.ReceivesCommission = false;
                        Emp.CommissionRate = 0;
                        Emp.LaborRate = 0;
                        Emp.PartRate = 0;
                    }
                    else if (string.Compare(formcollection["ReceivesCommission"], "true,false") == 0)
                    {
                        Emp.ReceivesCommission = true;
                        Emp.CommissionRate = decimal.Parse(formcollection["LaborRate"]);
                        Emp.LaborRate = float.Parse((formcollection["LaborRate"]));
                        Emp.PartRate = float.Parse((formcollection["LaborRate"]));
                    }
                    else
                        throw new ArgumentNullException(formcollection["ReceivesCommission"]);
                }
                else
                {
                    Emp.ReceivesCommission = false;
                    Emp.LaborRate = 0;
                    Emp.PartRate = 0;
                }

                if (formcollection["IsHourlyRate"] != null)
                {
                    if (string.Compare(formcollection["IsHourlyRate"], "false") == 0)
                    {
                        Emp.IsHourlyRate = false;
                        Emp.HourlyRate = 0;
                    }
                    else if (string.Compare(formcollection["IsHourlyRate"], "true,false") == 0)
                    {
                        Emp.IsHourlyRate = true;
                        Emp.HourlyRate = float.Parse((formcollection["HourlyRate"]));
                    }
                    else
                        throw new ArgumentNullException(formcollection["IsHourlyRate"]);
                }
                else
                {
                    Emp.IsHourlyRate = false;
                    Emp.HourlyRate = 0;
                }

                if (formcollection["IsWeeklySalary"] != null)
                {
                    if (string.Compare(formcollection["IsWeeklySalary"], "false") == 0)
                    {
                        Emp.IsWeeklySalary = false;
                        Emp.WeeklySalary = 0;
                    }
                    else if (string.Compare(formcollection["IsWeeklySalary"], "true,false") == 0)
                    {
                        Emp.IsWeeklySalary = true;
                        Emp.WeeklySalary = float.Parse((formcollection["WeeklySalary"]));
                    }
                    else
                        throw new ArgumentNullException(formcollection["IsWeeklySalary"]);
                }
                else
                {
                    Emp.IsWeeklySalary = false;
                    Emp.WeeklySalary = 0;
                }

                if (string.Compare(formcollection["ExemptYN"], "false") == 0)
                    Emp.ExemptYN = false;
                else if (string.Compare(formcollection["ExemptYN"], "true,false") == 0)
                    Emp.ExemptYN = true;
                else
                    throw new ArgumentNullException(formcollection["ExemptYN"]);

                Emp.DriversLic = formcollection["DriversLic"];
                if (formcollection["DLExpDate"] != "")
                {
                    Emp.DLExpDate = Convert.ToDateTime(formcollection["DLExpDate"]);
                }
                Emp.SS_ = formcollection["SS_"];
                Emp.ProLisc = formcollection["ProLisc"];
                if (formcollection["ProLiscExpDate"] != "")
                {
                    Emp.ProLiscExpDate = Convert.ToDateTime(formcollection["ProLiscExpDate"]);
                }
                Emp.ProLevel = formcollection["ProLevel"];
                if (formcollection["HiredDate"] != "")
                {
                    Emp.HiredDate = Convert.ToDateTime(formcollection["HiredDate"]);
                }
                if (formcollection["TerminatedDate"] != "")
                {
                    Emp.TerminatedDate = Convert.ToDateTime(formcollection["TerminatedDate"]);
                }
                if (string.Compare(formcollection["ServiceProYN"], "false") == 0)
                    Emp.ServiceProYN = false;
                else if (string.Compare(formcollection["ServiceProYN"], "true,false") == 0)
                    Emp.ServiceProYN = true;
                else
                    throw new ArgumentNullException(formcollection["ServiceProYN"]);

                if (string.Compare(formcollection["HVACSalesYN"], "false") == 0)
                {
                    Emp.HVACSalesYN = false;
                    Emp.UserKey = null;
                }
                else if (string.Compare(formcollection["HVACSalesYN"], "true,false") == 0)
                {
                    Emp.HVACSalesYN = true;
                    Emp.UserKey = string.IsNullOrEmpty(formcollection["UserKey"]) ? null : formcollection["UserKey"];
                }

                else
                    throw new ArgumentNullException(formcollection["HVACSalesYN"]);

                Emp.VacationEarned = Convert.ToInt32(formcollection["VacationEarned"]);
                Emp.VacationUsed = Convert.ToInt32(formcollection["VacationUsed"]);
                Emp.SickEarned = Convert.ToInt32(formcollection["SickEarned"]);
                Emp.SickUsed = Convert.ToInt32(formcollection["SickUsed"]);
                Emp.PersonalEarned = Convert.ToInt32(formcollection["PersonalEarned"]);
                Emp.PersonalUsed = Convert.ToInt32(formcollection["PersonalUsed"]);
                Emp.OtherEarned = Convert.ToInt32(formcollection["OtherEarned"]);
                Emp.OtherUsed = Convert.ToInt32(formcollection["OtherUsed"]);
                Emp.DisplayOrder = Convert.ToInt32(formcollection["DisplayOrder"]);

                db.SaveChanges();

                // Tablet Assignment
                if (employeeTablet != null)
                {
                    // Update employee tablet assignment.                    
                    char[] delimiterChars = { '(' };
                    char[] charsToTrim = { ')' };
                    var tabletNumber = employeeTablet.Split(delimiterChars)[1];
                    tabletNumber = tabletNumber.TrimEnd(charsToTrim);

                    var franchiseTablet = (from item in db.tbl_Franchise_Tablets
                                           where ((item.FranchiseID == FranchiseId) && (item.TabletNumber == tabletNumber))
                                           select item).FirstOrDefault();

                    if (franchiseTablet != null)
                    {
                        // Update existing tablet assignment.
                        franchiseTablet.EmployeeID = id;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("EmployeeInformation", new { id = id, frid = FranchiseId.ToString() });
            }
        }

        //[HttpPost]
        //public ActionResult EmployeeInformation(tbl_Employee employee)
        //{
        //    if (employee.EmployeeID != 0)
        //    {
        //        // Edit Mode
        //        UpdateModel(employee);
        //        db.SaveChanges();
        //        return RedirectToAction("EmployeeInformation", new { id = employee.EmployeeID, frid = employee.FranchiseID.ToString() });
        //    }
        //    else
        //    {
        //        // Insert Mode
        //        db.AddTotbl_Employee(employee);
        //        db.SaveChanges();
        //        return RedirectToAction("EmployeeInformation", new { id = employee.EmployeeID, frid = employee.FranchiseID.ToString() });
        //    }
        //}

        public ActionResult EmployeeAwards(int id)
        {
            ViewBag.EmpId = id;
            List<tbl_Employee_Awards> objemployeeAward = (from e in db.tbl_Employee_Awards where e.EmployeeID == id select e).ToList();

            List<tbl_Award> objAwards = (from e in db.tbl_Award select e).ToList();
            ViewBag.awards = objAwards;
            //List<tbl_Employee_Awards> objEmployeeAwards = db.tbl_Employee_Awards.Where(ea => ea.EmployeeID == id).ToList();
            return View();
        }
        public JsonResult DeleteRows(Int32[] id, int employeeid)
        {

            foreach (var item in id)
            {

                var objemployeeAward = (from empAwards in db.tbl_Employee_Awards where empAwards.EmpAwardID == item && empAwards.EmployeeID == employeeid select empAwards).First();
                db.tbl_Employee_Awards.DeleteObject(objemployeeAward);
                db.SaveChanges();
            }

            var objemployee2 = (from e in db.tbl_Award
                                join e1 in db.tbl_Employee_Awards on e.AwardID equals e1.AwardID
                                where e1.EmployeeID == employeeid
                                select new
                                {
                                    e1.AwardID,
                                    e1.Comments,
                                    e.Award,
                                    e1.DateReceived,
                                    e1.EmpAwardID

                                }).ToList();
            return Json(objemployee2);
            //return RedirectToAction("EmployeeAwardsGrid("+employeeid+",'','','')");


        }

        public JsonResult EmployeeAwardsGrid(int id, string awardid, string rdate, string comments)
        {
            if (awardid != "" && rdate != "" && comments != "")
            {
                var objEmpAward = new tbl_Employee_Awards();

                objEmpAward.EmployeeID = id;
                objEmpAward.AwardID = Convert.ToInt32(awardid);
                objEmpAward.DateReceived = Convert.ToDateTime(rdate);
                objEmpAward.Comments = comments;
                db.tbl_Employee_Awards.AddObject(objEmpAward);
                db.SaveChanges();
            }
            var objemployee2 = (from e in db.tbl_Award
                                join e1 in db.tbl_Employee_Awards on e.AwardID equals e1.AwardID
                                where e1.EmployeeID == id
                                select new
                                {
                                    e1.AwardID,
                                    e1.Comments,
                                    e.Award,
                                    e1.DateReceived,
                                    e1.EmpAwardID

                                }).ToList();

            return Json(objemployee2);
        }

        public ActionResult EmployeeReminders(FormCollection formcollection)
        {


            ViewBag.Date = DateTime.Now;
            ViewBag.ReminderDate = System.DateTime.Now.AddDays(30);
            ViewBag.ReminderType = "Employee Dates";
            string Remindertype = ViewBag.ReminderType;
            DateTime Date = Convert.ToDateTime(ViewBag.ReminderDate);

            return View();
        }

        public PartialViewResult PartialEmployeeReminders(string code)
        {
            try
            {
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                var FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();

                ViewBag.Date = DateTime.Now;
                ViewBag.ReminderDate = System.DateTime.Now.AddDays(30);

                //Set Reminder Type

                Session["selectedFranchiseId"] = FranchiseID;
                Session["selectedFranchiseCode"] = code;
                Session["Code"] = code;

                ViewBag.ReminderType = "Employee Dates";
                string Remindertype = ViewBag.ReminderType;
                DateTime Date = Convert.ToDateTime(ViewBag.ReminderDate);
                return PartialView("PartialEmployeeReminders", FranchiseID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult LoadReminderData(string franchisid, string Date, string Remindertype)
        {
            //Set Reminder Type
            int frid = Convert.ToInt32(franchisid);
            DateTime today = System.DateTime.Now;
            DateTime Date1 = Convert.ToDateTime(Date);

            //Load Data Code
            var objEmpReview = new List<EmployeeReview>();
            if (Remindertype == "Reviews")
            {
                var ReviewsList = (from e in db.tbl_HR_Reviews where e.CompletedYN == false && e.ReviewDate >= today && e.ReviewDate <= Date1 && e.FranchiseID == frid orderby e.ReviewDate select e);
                foreach (var Review in ReviewsList)
                {
                    var oEmpReview = new EmployeeReview();
                    int empid = Review.EmployeeID;
                    var objemployee = (from e in db.tbl_Employee where e.EmployeeID == empid select e).FirstOrDefault();
                    var objForm = (from frm in db.tbl_HR_Forms where frm.FormID == Review.FormID select frm).FirstOrDefault();
                    oEmpReview.EmployeeName = objemployee.Employee;
                    oEmpReview.ReviewDate = Convert.ToDateTime(Review.ReviewDate);
                    if (objForm != null)
                        oEmpReview.Form = objForm.Form;

                    oEmpReview.Subject = Review.Subject;
                    objEmpReview.Add(oEmpReview);
                }
            }
            else
            {
                var EmployeeList = (from e in db.tbl_Employee where e.FranchiseID == frid orderby e.Employee select e);
                foreach (var employee in EmployeeList)
                {
                    var oEmpReview = new EmployeeReview();
                    DateTime enddate = Date1;
                    if (employee.BirthDate != null)
                    {
                        DateTime BirthDate = Convert.ToDateTime(employee.BirthDate);
                        if (today.Month <= BirthDate.Month && today.Month <= enddate.Month)
                        {
                            if ((today.Month == BirthDate.Month && today.Day <= BirthDate.Day) || (today.Month < BirthDate.Month && today.Day <= enddate.Day))
                            {
                                int empid = employee.EmployeeID;
                                oEmpReview.EmployeeID = employee.EmployeeID;
                                var objemployee = (from e in db.tbl_Employee where e.EmployeeID == empid select e).FirstOrDefault();
                                oEmpReview.EmployeeName = objemployee.Employee;
                                oEmpReview.DateReason = "Birthdate";
                                oEmpReview.BirthDate = employee.BirthDate;
                                objEmpReview.Add(oEmpReview);

                            }
                        }
                    }
                    if (employee.Anniversary != null)
                    {

                        DateTime Anniversary = Convert.ToDateTime(employee.Anniversary);
                        if (today.Month <= Anniversary.Month && Anniversary.Month <= enddate.Month)
                        {
                            if ((today.Month == Anniversary.Month && today.Day <= Anniversary.Day) || (today.Month < Anniversary.Month && Anniversary.Day <= enddate.Day))
                            {

                                oEmpReview.EmployeeID = employee.EmployeeID;
                                var objemployee = (from e in db.tbl_Employee where e.EmployeeID == employee.EmployeeID select e).FirstOrDefault();
                                oEmpReview.EmployeeName = objemployee.Employee;
                                oEmpReview.DateReason = "Anniversary";
                                oEmpReview.Anniversary = employee.Anniversary;
                                objEmpReview.Add(oEmpReview);
                            }

                        }
                    }
                    if (employee.HiredDate != null)
                    {

                        DateTime HireDate = Convert.ToDateTime(employee.HiredDate);
                        if (today.Month <= HireDate.Month && HireDate.Month <= enddate.Month)
                        {
                            if ((today.Month == HireDate.Month && today.Day <= HireDate.Day) || (today.Month < HireDate.Month && HireDate.Day <= enddate.Day))
                            {
                                oEmpReview.EmployeeID = employee.EmployeeID;
                                var objemployee = (from e in db.tbl_Employee where e.EmployeeID == employee.EmployeeID select e).FirstOrDefault();
                                oEmpReview.EmployeeName = objemployee.Employee;
                                oEmpReview.DateReason = "Employment Anniversary";
                                oEmpReview.HiredDate = employee.HiredDate;
                                objEmpReview.Add(oEmpReview);
                            }

                        }
                    }
                    if (employee.DLExpDate != null)
                    {

                        DateTime dlExpDate = Convert.ToDateTime(employee.DLExpDate);
                        if (today.Month <= dlExpDate.Month && dlExpDate.Month <= enddate.Month)
                        {
                            if ((today.Month == dlExpDate.Month && today.Day <= dlExpDate.Day) || (today.Month < dlExpDate.Month && dlExpDate.Day <= enddate.Day))
                            {
                                oEmpReview.EmployeeID = employee.EmployeeID;
                                var objemployee = (from e in db.tbl_Employee where e.EmployeeID == employee.EmployeeID select e).FirstOrDefault();
                                oEmpReview.EmployeeName = objemployee.Employee;
                                oEmpReview.DateReason = "DL Exp Date";
                                oEmpReview.DLExpDate = employee.DLExpDate;
                                objEmpReview.Add(oEmpReview);
                            }
                        }
                    }
                    if (employee.ProLiscExpDate != null)
                    {

                        DateTime ProLiscExpDate = Convert.ToDateTime(employee.ProLiscExpDate);
                        if (today.Month <= ProLiscExpDate.Month && ProLiscExpDate.Month <= enddate.Month)
                        {
                            if ((today.Month == ProLiscExpDate.Month && today.Day <= ProLiscExpDate.Day) || (today.Month < ProLiscExpDate.Month && ProLiscExpDate.Day <= enddate.Day))
                            {
                                oEmpReview.EmployeeID = employee.EmployeeID;
                                var objemployee = (from e in db.tbl_Employee where e.EmployeeID == employee.EmployeeID select e).FirstOrDefault();
                                oEmpReview.EmployeeName = objemployee.Employee;
                                oEmpReview.DateReason = "Pro License Exp Date";
                                oEmpReview.ProLisc = employee.ProLisc;
                                objEmpReview.Add(oEmpReview);
                            }
                        }
                    }


                }

            }
            return Json(objEmpReview);
        }

        public ActionResult TimeCardSummary()
        {

            return View();
        }

        public ActionResult TimeCardSummaryEmployeeData(int FranchiseeId)
        {
            var EmployeeList = (from p in db.tbl_Employee
                                where p.ActiveYN == true && p.FranchiseID == FranchiseeId
                                orderby p.Employee
                                select new
                                {
                                    EmployeeID = p.EmployeeID,
                                    Employee = p.Employee
                                }).ToList();
            var listItems = "";
            for (var i = 0; i < EmployeeList.Count(); i++)
            {
                listItems += "<option value='" + EmployeeList[i].EmployeeID + "'>" + EmployeeList[i].Employee + "</option>";
            }

            return Json(listItems);
        }

        public ActionResult TimeCardSummaryEmployeeSelected(int FranchiseeId)
        {
            var EmployeeSelected = (from p in db.tbl_Employee where p.ActiveYN == true && p.FranchiseID == FranchiseeId orderby p.Employee select p).ToList();
            return Json(EmployeeSelected);
        }

        public ActionResult TimeCardDetailDayData(string Empid, string DetailDay, DateTime date, int FranchiseeId)
        {
            DateTime PayrollDate = date;
            DateTime currentDate;
            switch (DetailDay)
            {
                case "Sunday":
                    currentDate = PayrollDate.AddDays(-6);
                    ViewBag.DayDetails = "Sunday Details";

                    break;
                case "Monday":
                    currentDate = PayrollDate.AddDays(-5);
                    ViewBag.DayDetails = "Monday Details";

                    break;
                case "Tuesday":
                    currentDate = PayrollDate.AddDays(-4);
                    ViewBag.DayDetails = "Tuesday Details";

                    break;
                case "Wednesday":
                    currentDate = PayrollDate.AddDays(-3);
                    ViewBag.DayDetails = "Wednesday Details";

                    break;
                case "Thursday":
                    currentDate = PayrollDate.AddDays(-2);
                    ViewBag.DayDetails = "Thursday Details";

                    break;
                case "Friday":
                    currentDate = PayrollDate.AddDays(-1);
                    ViewBag.DayDetails = "Friday Details";

                    break;
                case "Saturday":
                    currentDate = PayrollDate;
                    ViewBag.DayDetails = "Saturday Details";

                    break;
                default:
                    currentDate = PayrollDate.AddDays(-6);
                    ViewBag.DayDetails = "Sunday Details";

                    break;
            }

            if (Empid == null || Empid == "")
                return Json("");

            int EmployeeId = Convert.ToInt32(Empid);
            DateTime strDate = Convert.ToDateTime(currentDate.ToShortDateString() + " " + "12:00:00 AM");
            DateTime endDate = Convert.ToDateTime(currentDate.ToShortDateString() + " " + "11:59:59 PM");
            var timeclock = (from p in db.tbl_HR_TimeClock
                             join p1 in db.tbl_HR_TimeClock_Status on p.TimeClockStatusID equals p1.TimeClockStatusID
                             where p.EmployeeID == EmployeeId &&
                             p.DateTimeStatusChanged >= strDate && p.DateTimeStatusChanged <= endDate &&
                             p.FranchiseID == FranchiseeId
                             select new
                             {
                                 p.Comments,
                                 p.DateTimeStatusChanged,
                                 p1.TimeClockStatusDesc,
                                 p.TimeClockID
                             }).ToList();
            return Json(timeclock);
        }

        public JsonResult GetEmployeePayrollInformation(int id)
        {
            try
            {
                var employeeInformation = db.tbl_Employee.FirstOrDefault(e => e.EmployeeID == id);
                if (employeeInformation != null)
                {
                    return Json(new
                    {
                        WeeklySalary = employeeInformation.WeeklySalary,
                        HourlyRate = employeeInformation.HourlyRate,
                        LaborRate = employeeInformation.LaborRate
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        WeeklySalary = 0,
                        HourlyRate = 0
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return Json(new { Success = false, Error = "" });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadEmployeeList()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "MyEmployeeList.xlsx");
        }

        /// <summary>
        /// Save Employee Timesheet.
        /// Called by AJAX call from TimeCardSummary
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <param name="DateTimeWeekOf"></param>
        /// <param name="SundayHoursString"></param>
        /// <param name="MondayHoursString"></param>
        /// <param name="TuesdayHoursString"></param>
        /// <param name="WednesdayHoursString"></param>
        /// <param name="ThursdayHoursString"></param>
        /// <param name="FridayHoursString"></param>
        /// <param name="SaturdayHoursString"></param>
        /// <returns></returns>
        public ActionResult SaveEmployeeTimeSheet(
                                                 int EmployeeID
                                                , DateTime DateTimeWeekOf
                                                , string SundayHoursString
                                                , string MondayHoursString
                                                , string TuesdayHoursString
                                                , string WednesdayHoursString
                                                , string ThursdayHoursString
                                                , string FridayHoursString
                                                , string SaturdayHoursString
            )
        {
            try
            {
                // TODO: Validate input
                // transform input
                decimal sundayHours = decimal.Parse(SundayHoursString);
                decimal mondayHours = decimal.Parse(MondayHoursString);
                decimal tuesdayHours = decimal.Parse(TuesdayHoursString);
                decimal wednesdayHours = decimal.Parse(WednesdayHoursString);
                decimal thursdayHours = decimal.Parse(ThursdayHoursString);
                decimal fridayHours = decimal.Parse(FridayHoursString);
                decimal saturdayHours = decimal.Parse(SaturdayHoursString);

                // Save the timesheet
                SiteBlue.Business.Payroll.TimeSheetService.SaveTimeSheet(EmployeeID, DateTimeWeekOf, sundayHours, mondayHours, tuesdayHours, wednesdayHours, thursdayHours, fridayHours, saturdayHours);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return Json("SUCCESS");
        }

        public ActionResult GetEmployeeTimeSheet(
                                         string EmployeeIDString
                                        , string DateTimeWeekOfString
                                        )
        {
            try
            {
                // TODO: Validate input
                // transform input
                int employeeID = int.Parse(EmployeeIDString);
                DateTime dateTimeWeekOf = DateTime.Parse(DateTimeWeekOfString);

                // ViewModel to Return
                Models.TimeCardSummary timeCardToReturn = new TimeCardSummary();
                List<tbl_HR_TimeSheet> existingTimeSheetList = (from timeSheet in db.tbl_HR_TimeSheet
                                                                where timeSheet.EmployeeID == employeeID
                                                                   && timeSheet.WeekEndingDateOn == dateTimeWeekOf
                                                                select timeSheet).ToList<tbl_HR_TimeSheet>();
                if (existingTimeSheetList.Count > 0)
                {
                    // There's an existing timesheet.  Plaster it on the ViewModel and return it 
                    tbl_HR_TimeSheet existingTimeSheet = existingTimeSheetList.First<tbl_HR_TimeSheet>();
                    timeCardToReturn.TimeSheetSundayHours = existingTimeSheet.SundayHours;
                    timeCardToReturn.TimeSheetMondayHours = existingTimeSheet.MondayHours;
                    timeCardToReturn.TimeSheetTuesdayHours = existingTimeSheet.TuesdayHours;
                    timeCardToReturn.TimeSheetWednesdayHours = existingTimeSheet.WednesdayHours;
                    timeCardToReturn.TimeSheetThursdayHours = existingTimeSheet.ThursdayHours;
                    timeCardToReturn.TimeSheetFridayHours = existingTimeSheet.FridayHours;
                    timeCardToReturn.TimeSheetSaturdayHours = existingTimeSheet.SaturdayHours;
                }

                return Json(timeCardToReturn);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult EmployeeHistoryList(string employeeID)
        {
            using (var context = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var eid = int.Parse(employeeID);

                var employeeHistoryData = (from log in context.AuditLogs
                                           join ae in context.Audit_Employee on log.AuditID equals ae.AuditID
                                           where log.EntityID == employeeID && log.EntityType == "tbl_Employee"
                                           select new
                                           {
                                               log.UserKey,
                                               log.AuditDate,
                                               log.Type,
                                               log.EntityType,
                                               ae.Attribute,
                                               ae.NewValue,
                                               ae.OldValue
                                           }).OrderBy(o => o.AuditDate).ToList();

                var employeeHistoryList = new List<EmployeeHistoryInfo>();

                for (var i = 0; i < employeeHistoryData.Count; i++)
                {
                    var from = employeeHistoryData[i].OldValue;
                    var to = employeeHistoryData[i].NewValue;

                    var changedby = "";
                    try
                    {
                        changedby = Membership.GetUser(new Guid(employeeHistoryData[i].UserKey)).UserName;
                    }
                    catch (Exception)
                    {
                        changedby = "N/A";
                    }

                    employeeHistoryList.Add(new EmployeeHistoryInfo
                    {
                        FieldName = employeeHistoryData[i].Attribute,
                        TableName = employeeHistoryData[i].EntityType,
                        ChangeType = employeeHistoryData[i].Type,
                        Date = employeeHistoryData[i].AuditDate.ToShortDateString(),
                        Time = employeeHistoryData[i].AuditDate.ToShortTimeString(),
                        isTablet = "No",
                        ChangedBy = changedby,
                        From = from,
                        To = to
                    });
                }

                return Json(employeeHistoryList);
            }
        }

        public JsonResult GetTechnicians(int id)
        {
            using (var db = new EightHundredEntities())
            {
                var technicians = from t in db.tbl_Franchise_Tablets
                                  join e in db.tbl_Employee
                                      on t.EmployeeID equals e.EmployeeID
                                  where e.FranchiseID == id
                                  orderby e.Employee, t.TabletNumber
                                  select new { key = t.EmployeeID, label = e.Employee + " (" + t.TabletNumber + ")" };

                return Json(technicians.ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

    }
}