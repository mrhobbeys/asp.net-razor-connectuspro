using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DHTMLX.Export.Excel;
using SecurityGuard.Services;
using SiteBlue.Areas.OwnerPortal.Models;
using SiteBlue.Areas.SecurityGuard.Models;
using SiteBlue.Business;
using SiteBlue.Business.Alerts;
using SiteBlue.Business.Invoice;
using SiteBlue.Business.Job;
using SiteBlue.Controllers;
using SiteBlue.Data.EightHundred;
using System.Data.Objects.SqlClient;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class InvoiceController : SiteBlueBaseController
    {
        private readonly MembershipService membership = new MembershipService(Membership.Provider);
        private MembershipUser _currentUser;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _currentUser = _currentUser ?? membership.GetUser(User.Identity.Name);
        }

        //
        // GET: /OwnerPortal/Invoice/

        private MembershipConnection memberShipContext = new MembershipConnection();
        //static string DefaultCompamyName = default(String);

        string gbZeeBusinessModel = "Franchise";
        int MyJobID;
        string sqlConString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["OwnerPortalContext"].ToString();
        List<string> lstjobsid;

        public static Hashtable table = new Hashtable();

        public ActionResult Index()
        {
            return RedirectToAction("InvoiceList");
        }

        public ActionResult TaxRate()
        {
            int franchiseId = 0;

            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                if (Request.QueryString["jobid"] != null)
                {
                    int jobid = Convert.ToInt32(Request.QueryString["jobid"]);
                    tbl_Job job = (from j in ctx.tbl_Job where j.JobID == jobid select j).Single();
                    franchiseId = job.FranchiseID;
                    ViewBag.txt_TaxPartPercentage = String.Format("{0:0.00}", job.TaxPartPercentage);
                    ViewBag.txt_TaxLaborPercentage = String.Format("{0:0.00}", job.TaxLaborPercentage);
                }

                List<tbl_TaxRates> TaxRateList = (from b in ctx.tbl_TaxRates where b.FranchiseId == franchiseId select b).ToList();
                ViewBag.taxrates = TaxRateList;
            }
            return View();
        }

        public void UpdateStatus(string Status, string JobId, InvoiceFinancialDetail FinanceDatas)
        {
            if (string.IsNullOrEmpty(JobId)) return;

            int jobID = Convert.ToInt32(JobId);
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                tbl_Job objjob = (from j in ctx.tbl_Job where j.JobID == jobID select j).FirstOrDefault();
                if (objjob != null)
                {
                    if (FinanceDatas != null)
                    {
                        objjob.SubTotal = Convert.ToDecimal(FinanceDatas.SubTotal);
                        objjob.TaxAmount = Convert.ToDecimal(FinanceDatas.Tax);
                        objjob.TaxLaborPercentage = (float)Convert.ToDouble(FinanceDatas.TaxLaborPercent);
                        objjob.TaxPartPercentage = (float)Convert.ToDouble(FinanceDatas.TaxPartPercent);
                        objjob.TotalSales = Convert.ToDecimal(FinanceDatas.Total);
                        objjob.Balance = Convert.ToDecimal(FinanceDatas.Balance);
                    }

                    objjob.StatusID = Convert.ToInt32(Status);

                    var statuses = new int[] {12, 16, 6, 7};
                    
                    if (statuses.Contains(objjob.StatusID) && !objjob.CallCompleted.HasValue)
                    {
                        tbl_Job_Estimates est = null;
                        //If declined estimate set the "completed date" to the date of the estimate.
                        if (objjob.StatusID == 16)
                            est = ctx.tbl_Job_Estimates.Where(j => j.JobID == jobID).OrderByDescending(e => e.EstimateDate).FirstOrDefault();

                        objjob.CallCompleted = est == null ? DateTime.Now : est.EstimateDate;    
                    }
                        

                    //objjob.InvoicedDate = DateTime.Now;
                    ctx.SaveChanges();

                    AlertType? type = null;
                    switch (objjob.StatusID)
                    {
                        case 10:
                            type = AlertType.CallRescheduled;
                            break;
                        case 12:
                            type = AlertType.CustomerCancellation;
                            break;
                    }

                    if (type.HasValue)
                        AbstractBusinessService.Create<AlertEngine>(UserInfo.UserKey).SendAlert(type.Value, objjob.FranchiseID);

                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.ExpiresAbsolute = DateTime.Now.AddMonths(-1);
                }
            }
        }

        public void UpdateJobType(string JobType, string JobId)
        {
            if (string.IsNullOrEmpty(JobId)) return;

            int jobID = Convert.ToInt32(JobId);
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                tbl_Job objjob = (from j in ctx.tbl_Job where j.JobID == jobID select j).FirstOrDefault();

                if (objjob == null) return;

                objjob.BusinessTypeID = Convert.ToInt32(JobType);
                ctx.SaveChanges();
            }
        }

        public void UpdateServicePro(string EmployeeID, string JobId)
        {
            if (string.IsNullOrEmpty(JobId)) return;

            int jobID = Convert.ToInt32(JobId);
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                tbl_Job objjob = (from j in ctx.tbl_Job where j.JobID == jobID select j).FirstOrDefault();

                if (objjob == null) return;

                objjob.ServiceProID = Convert.ToInt32(EmployeeID);
                ctx.SaveChanges();
            }
        }

        public ActionResult InvoiceDetails(string btnclicknext, FormCollection frmcollection)
        {
            var detailmodel = new InvoiceDetailInfo();

            int iIndex;
            lstjobsid = (List<string>)Session["JobsId"];
            var membership = new MembershipService(Membership.Provider);
            MembershipUser user = membership.GetUser(User.Identity.Name);
            Guid userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);

            bool isCorporate = User.IsInRole("Corporate");
            bool isTicket = User.IsInRole("TicketRuler");
            bool isCompany = User.IsInRole("CompanyOwner");

            detailmodel.IsCorporate = isCorporate;
            detailmodel.isCompany = isCompany;
            var objmodcommon = new mod_common(UserInfo.UserKey);

            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                if (Request.QueryString["JobId"] != null)
                {
                    MyJobID = Convert.ToInt32(Request.QueryString["JobId"]);

                    var jobdata = (from j in ctx.tbl_Job
                                   where j.JobID == MyJobID
                                   select new
                                   {
                                       j.FranchiseID,
                                       j.ServiceProID,
                                       j.TaxAuthorityID,
                                       j.TaxPartPercentage,
                                       j.TaxLaborPercentage
                                   })
                                       .Single();

                    if (!UserInfo.Franchises.Any(f => f.FranchiseID == jobdata.FranchiseID))
                    {
                        detailmodel.Authorized = false;
                        return View(detailmodel);
                    }

                    detailmodel.Authorized = true;

                    var TaxRateList = (from b in ctx.tbl_TaxRates
                                       where b.FranchiseId == jobdata.FranchiseID
                                       select b)
                                       .ToList();

                    //if (TaxRateList.Any(q => q.PartsAmount == jobdata.TaxPartPercentage && q.LaborAmount == jobdata.TaxLaborPercentage))
                    if (TaxRateList.Any(q => q.TaxRateID == jobdata.TaxAuthorityID))
                    {
                        //var selTaxRateID = TaxRateList.First(q => q.PartsAmount == jobdata.TaxPartPercentage && q.LaborAmount == jobdata.TaxLaborPercentage).TaxRateID;
                        var selTaxRateID = jobdata.TaxAuthorityID;
                        detailmodel.Financial.TaxRateList = new SelectList(TaxRateList, "TaxRateID", "TaxDescription", selTaxRateID);
                    }
                    else
                    {
                        TaxRateList.Add(new tbl_TaxRates
                        {
                            TaxRateID = 0,
                            TaxDescription = "-"
                        });
                        detailmodel.Financial.TaxRateList = new SelectList(TaxRateList, "TaxRateID", "TaxDescription", 0);
                    }

                    //Only populating the Active Service Providers.
                    List<tbl_Employee> EmployeeList =
                        (from b in ctx.tbl_Employee where b.FranchiseID == jobdata.FranchiseID && b.ActiveYN select b).ToList();
                    detailmodel.EmployeeList = new SelectList(EmployeeList, "EmployeeID", "Employee", jobdata.ServiceProID);

                    if (btnclicknext == "3")
                    {
                        //tbl_Job objjob = (from j in DB.tbl_Job where j.JobID == MyJobID select j).Single();
                        var frm = new FormCollection();
                        frm = frmcollection;
                    }
                    if (btnclicknext == "2")
                    {
                        tbl_Job objjob = (from j in ctx.tbl_Job where j.JobID == MyJobID select j).FirstOrDefault();

                        if (objjob != null)
                        {
                            objjob.StatusID = Convert.ToInt32(frmcollection["ddlstatus"]);
                            //objjob.InvoicedDate = DateTime.Now;
                            ctx.SaveChanges();

                            AlertType? type = null;
                            switch (objjob.StatusID)
                            {
                                case 10:
                                    type = AlertType.CallRescheduled;
                                    break;
                                case 12:
                                    type = AlertType.CustomerCancellation;
                                    break;
                            }

                            if (type.HasValue)
                                AbstractBusinessService.Create<AlertEngine>(UserInfo.UserKey).SendAlert(type.Value, objjob.FranchiseID);

                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.ExpiresAbsolute = DateTime.Now.AddMonths(-1);
                        }
                    }

                    detailmodel.CurrentJobID = MyJobID.ToString();

                    if (lstjobsid != null)
                    {
                        iIndex = lstjobsid.IndexOf(MyJobID.ToString());

                        if (btnclicknext == null)
                        {
                            if (iIndex == 0)
                            {
                                if (lstjobsid.Count() > 1)
                                {
                                    iIndex = iIndex + 1;
                                    detailmodel.NextJobID = Convert.ToString(lstjobsid[iIndex]);
                                    detailmodel.PreviousJobID = "0";
                                    detailmodel.StyleFlag = "0";
                                }
                                else
                                {
                                    detailmodel.NextJobID = "0";
                                    detailmodel.PreviousJobID = "0";
                                    detailmodel.StyleFlag = "0";
                                    detailmodel.StyleFlag1 = "0";
                                }
                            }
                            else
                            {
                                if (iIndex == lstjobsid.Count() - 1)
                                {
                                    detailmodel.NextJobID = "0";
                                    detailmodel.StyleFlag1 = "0";
                                }
                                else
                                {
                                    detailmodel.NextJobID = Convert.ToString(lstjobsid[iIndex + 1]);
                                    detailmodel.StyleFlag1 = "1";
                                }

                                if (Convert.ToInt32(iIndex - 1) > 0)
                                {
                                    detailmodel.PreviousJobID = Convert.ToString(lstjobsid[iIndex - 1]);
                                }
                                else
                                {
                                    detailmodel.PreviousJobID = "0";
                                }
                            }
                        }

                        if (btnclicknext == "0")
                        {
                            if (iIndex == 0)
                            {
                                detailmodel.NextJobID = Convert.ToString(lstjobsid[iIndex + 1]);
                                detailmodel.PreviousJobID = "0";
                                detailmodel.StyleFlag = "0";
                            }
                            else
                            {
                                if (lstjobsid.Count > iIndex + 1)
                                {
                                    detailmodel.NextJobID = Convert.ToString(lstjobsid[iIndex + 1]);
                                }
                                else
                                {
                                    detailmodel.NextJobID = Convert.ToString(lstjobsid[iIndex]);
                                }

                                detailmodel.PreviousJobID = Convert.ToString(lstjobsid[iIndex - 1]);
                                detailmodel.StyleFlag = "1";
                            }
                        }

                        if (btnclicknext == "1")
                        {

                            if (Convert.ToInt32(iIndex + 1) == lstjobsid.Count())
                            {
                                detailmodel.NextJobID = "0";
                                detailmodel.PreviousJobID = Convert.ToString(lstjobsid[iIndex - 1]);
                                detailmodel.StyleFlag1 = "0";
                            }
                            else
                            {
                                detailmodel.NextJobID = Convert.ToString(lstjobsid[iIndex + 1]);
                                detailmodel.PreviousJobID = Convert.ToString(lstjobsid[iIndex - 1]);
                                detailmodel.StyleFlag1 = "1";
                            }
                        }
                    }
                    else
                    {
                        detailmodel.StyleFlag = "0";
                        detailmodel.StyleFlag1 = "0";

                        MyJobID = Convert.ToInt32(Request.QueryString["JobId"]);
                    }
                }

                try
                {
                    var job = (from j in ctx.tbl_Job
                               where j.JobID == MyJobID
                               select new
                                          {
                                              j.BusinessTypeID,
                                              j.AreaID,
                                              j.StatusID,
                                              j.JobID,
                                              j.JobTypeID,
                                              j.WarrantyType1,
                                              j.WarrantyLen1,
                                              j.WarrantyType2,
                                              j.WarrantyLen2,
                                              j.CallReason,
                                              j.ServiceProID,
                                              j.CallCompleted,
                                              j.CustomerID,
                                              j.LocationID,
                                              j.CallTaken,
                                              j.CallSourceID,
                                              j.Diagnostics,
                                              j.Recommendations,
                                              j.SubTotal,
                                              j.TaxAmount,
                                              j.TaxPartPercentage,
                                              j.TaxLaborPercentage,
                                              j.TotalSales,
                                              j.Balance,
                                              j.RequestedTime,
                                              j.ScheduleStart,
                                              j.DispatchedDate,
                                              j.DispatcherID,
                                              j.ActualStart,
                                              j.ActualEnd,
                                              j.FranchiseID,
                                              j.CustomerComments,
                                              j.LockedYN,
                                              j.ServiceID,
                                              j.WSRCompletedDate,
                                              j.RescheduleReason
                                          })
                        .Single();

                    detailmodel.JobInfo.CustomerID = job.CustomerID;

                    var membershipType = (from mt in ctx.tbl_Member_Type
                                          where mt.ActiveYN == true
                                          select new
                                          {
                                              mt.MemberTypeID,
                                              mt.MemberType
                                          }).ToList();

                    var customerMemberInfo = (from m in ctx.tbl_Customer_Members
                                              where m.CustomerID == job.CustomerID
                                              select new
                                              {
                                                  m.MemberTypeID,
                                                  m.StartDate,
                                                  m.EndDate
                                              }).FirstOrDefault();

                    if (customerMemberInfo != null)
                    {
                        detailmodel.MemberInfo.MemberTypeList = new SelectList(membershipType, "MemberTypeID", "MemberType", customerMemberInfo.MemberTypeID);
                        detailmodel.MemberInfo.StartDate = customerMemberInfo.StartDate;
                        detailmodel.MemberInfo.ExpiryDate = customerMemberInfo.EndDate;
                    }
                    else
                    {
                        detailmodel.MemberInfo.MemberTypeList = new SelectList(membershipType, "MemberTypeID", "MemberType");
                        detailmodel.MemberInfo.StartDate = null;
                        detailmodel.MemberInfo.ExpiryDate = null;
                    }

                    detailmodel.JobInfo.LockDate = job.WSRCompletedDate == null ? "Not Locked" : job.WSRCompletedDate.Value.ToShortDateString();

                    var serviceType = (from st in ctx.tbl_Services
                                       where st.ActiveYN == true
                                       select st).ToList();

                    detailmodel.JobInfo.ServiceTypeList = new SelectList(serviceType, "ServiceID", "ServiceName", job.ServiceID);

                    List<tbl_Customer_BusinessType> businesstype = (from bt in ctx.tbl_Customer_BusinessType select bt).ToList();
                    detailmodel.JobInfo.BusinessTypeList = new SelectList(businesstype, "BusinessTypeID", "BusinessType",
                                                                          job.BusinessTypeID.ToString());

                    detailmodel.JobInfo.DBAName = objmodcommon.Get_Job_DBAName(job.AreaID);

                    var statusdata = (from s in ctx.tbl_Job_Status
                                      select s);

                    var statuslist = statusdata.ToList();

                    if (isCompany)
                    {
                        if (job.StatusID == 6)
                            statuslist = statusdata.Where(q => q.StatusID == 6 || q.StatusID == 7).ToList();

                        if (job.StatusID == 13)
                            statuslist = statusdata.Where(q => q.StatusID == 13 || q.StatusID == 16).ToList();
                    }

                    if (isCorporate)
                    {
                        if (job.StatusID == 13)
                            statuslist = statusdata.Where(q => q.StatusID == 13 || q.StatusID == 16).ToList();

                        if (job.StatusID == 10)
                            statuslist = statusdata.Where(q => q.StatusID == 10 || q.StatusID == 12).ToList();
                    }

                    if (isTicket)
                        statuslist = statusdata.ToList();

                    statuslist = statuslist.OrderBy(q => q.Status).ToList();

                    detailmodel.JobInfo.InvoiceStatusList = new SelectList(statuslist, "StatusID", "Status",
                                                                           job.StatusID.ToString());

                    if (job.BusinessTypeID == 2)
                    {
                        ViewBag.rescomm = "Residential";
                    }
                    else if (job.BusinessTypeID == 3)
                    {
                        ViewBag.rescomm = "Commercial";
                    }
                    else
                    {
                        ViewBag.rescomm = "Residential";
                    }


                    //load job info
                    ViewBag.JobID = job.JobID;
                    ViewBag.tmpBusTypeID = job.BusinessTypeID;
                    ViewBag.txt_status = objmodcommon.getStatus(job.StatusID);

                    detailmodel.JobInfo.InvoiceNum = job.JobID.ToString();
                    detailmodel.JobInfo.JobType = objmodcommon.GetJobTypeName(job.JobTypeID);

                    //IQueryable<tbl_Job_Technicians> jobtech =
                    //    (from t in ctx.tbl_Job_Technicians where t.JobID == job.JobID && t.PrimaryYN == true select t);
                    detailmodel.JobInfo.ServicePro = objmodcommon.Get_Employee_Name(job.ServiceProID);
                    //if (jobtech.Count() > 0)
                    //{
                    //    foreach (var jobrec in jobtech)
                    //    {
                    //        detailmodel.JobInfo.ServicePro = objmodcommon.Get_Employee_Name(jobrec.ServiceProID);
                    //    }
                    //}
                    //else
                    //{
                    //    ViewBag.txt_ServicePro = "N/A";
                    //    detailmodel.JobInfo.ServicePro = "N/A";
                    //}

                    detailmodel.JobInfo.WarrantyType1 = objmodcommon.GetWarrantyType(job.WarrantyType1);
                    detailmodel.JobInfo.WarrantyLength1 = objmodcommon.GetWarrantyLength(job.WarrantyLen1);
                    detailmodel.JobInfo.WarrantyType2 = objmodcommon.GetWarrantyType(job.WarrantyType2);
                    detailmodel.JobInfo.WarrantyLength2 = objmodcommon.GetWarrantyLength(job.WarrantyLen2);

                    //MyServiceProID = job.ServiceProID

                    detailmodel.JobInfo.CallReason = job.CallReason;

                    ViewBag.Technician = objmodcommon.Get_Employee_Name(job.ServiceProID);

                    if ((job.CallCompleted == null))
                    {
                        detailmodel.JobInfo.JobCompleted = "  /  /";
                    }
                    else
                    {
                        detailmodel.JobInfo.JobCompleted =
                            DateTime.Parse(job.CallCompleted.ToString()).ToShortDateString();
                    }

                    //load customer billto
                    tbl_Customer customer = objmodcommon.GetCustomers(job.CustomerID);
                    tbl_Locations billto = objmodcommon.getBillTo(job.CustomerID);

                    if ((!(customer == null)
                         && !(billto == null)))
                    {
                        string customername = objmodcommon.GetCustomerName(customer);
                        IQueryable<tbl_Contacts> contactlist =
                            (from c in ctx.tbl_Contacts
                             where job.CustomerID == job.CustomerID && c.LocationID == billto.LocationID
                             select c);

                        detailmodel.JobInfo.CustomerBillTo = (customername + ("\r\n"
                                                                              + (billto.Address + ("\r\n"
                                                                                                   +
                                                                                                   (billto.City + (", "
                                                                                                                   +
                                                                                                                   (billto
                                                                                                                        .
                                                                                                                        State +
                                                                                                                    (" "
                                                                                                                     +
                                                                                                                     (billto
                                                                                                                          .
                                                                                                                          PostalCode +
                                                                                                                      ("  "
                                                                                                                       +
                                                                                                                       (billto
                                                                                                                            .
                                                                                                                            Country +
                                                                                                                        "\r\n")))))))))));

                        if ((customer.EMail != ""))
                        {
                            detailmodel.JobInfo.CustomerBillTo = (detailmodel.JobInfo.CustomerBillTo +
                                                                  ("\r\n" + (customer.EMail + "\r\n")));
                        }
                        else
                        {
                            detailmodel.JobInfo.CustomerBillTo = (detailmodel.JobInfo.CustomerBillTo +
                                                                  ("\r\n" + (customer.EMail + "\r\n")));
                        }

                        detailmodel.CustomerEmail = customer.EMail;

                        foreach (var contactrec in contactlist)
                        {
                            detailmodel.JobInfo.CustomerBillTo = (detailmodel.JobInfo.CustomerBillTo +
                                                                  (objmodcommon.Format_PhoneNumber(
                                                                      contactrec.PhoneNumber) + "\r\n"));
                        }

                        var member = (from c in ctx.tbl_Customer_Members where c.CustomerID == job.CustomerID select c);
                        foreach (var rec in member)
                        {
                            ViewBag.lbl_member = true;
                        }
                    }

                    //load job location
                    var joblocation = objmodcommon.getLocation(job.LocationID);
                    if (!(joblocation == null))
                    {
                        var locationcontact = (from c in ctx.tbl_Contacts where c.LocationID == job.LocationID select c);

                        detailmodel.JobInfo.JobLocation = (joblocation.LocationName + ("\r\n"
                                                                                       + (joblocation.Address + ("\r\n"
                                                                                                                 +
                                                                                                                 (joblocation
                                                                                                                      .
                                                                                                                      City +
                                                                                                                  (", "
                                                                                                                   +
                                                                                                                   (joblocation
                                                                                                                        .
                                                                                                                        State +
                                                                                                                    (" "
                                                                                                                     +
                                                                                                                     (joblocation
                                                                                                                          .
                                                                                                                          PostalCode +
                                                                                                                      ("\r\n" +
                                                                                                                       "\r\n"))))))))));

                        foreach (var locationcontactrec in locationcontact)
                        {
                            detailmodel.JobInfo.JobLocation = (detailmodel.JobInfo.JobLocation +
                                                               (objmodcommon.Format_PhoneNumber(
                                                                   locationcontactrec.PhoneNumber) + "\r\n"));
                        }
                    }

                    //load call info
                    detailmodel.JobInfo.CallReceived = job.CallTaken;

                    if ((gbZeeBusinessModel == "ConnectusPro"))
                    {
                        tbl_Referral referral = objmodcommon.getConnectusReferral(job.CallSourceID.GetValueOrDefault());
                        if (referral != null)
                        {
                            detailmodel.JobInfo.Referral = referral.ReferralType;
                        }
                    }
                    else
                    {
                        tbl_Referral referral = objmodcommon.getReferral(job.CallSourceID.GetValueOrDefault());
                        if (referral != null)
                        {
                            detailmodel.JobInfo.Referral = referral.ReferralType;
                        }
                    }

                    //service comments
                    detailmodel.ServiceComment.Diagnosis = job.Diagnostics;
                    detailmodel.ServiceComment.Recommendations = job.Recommendations;
                    detailmodel.ServiceComment.CustomerComments = job.CustomerComments;
                    detailmodel.ServiceComment.CallNotes = job.RescheduleReason;

                    //Financial

                    IQueryable<tbl_Customer_Info> customerinfo =
                        (from i in ctx.tbl_Customer_Info
                         where i.CustomerID == customer.CustomerID && i.FranchiseID == job.FranchiseID
                         select i);
                    foreach (var InfoRec in customerinfo)
                    {
                        IQueryable<tbl_Customer_CreditTerms> terms =
                            (from t in ctx.tbl_Customer_CreditTerms
                             where t.CreditTermsID == InfoRec.CreditTermsID
                             select t);

                        foreach (var termsrec in terms)
                        {
                            detailmodel.Financial.Term = termsrec.CreditTerms;
                        }

                        detailmodel.Financial.CreditLimit = InfoRec.CreditLimit;
                    }

                    try
                    {
                        var customerbalance = (from h in ctx.tbl_Job
                                               where h.CustomerID == job.CustomerID
                                               group h by h.CustomerID
                                                   into g
                                                   select new
                                                              {
                                                                  TotalBalance = g.Sum(x => x.Balance)
                                                              })
                            .Single();

                        detailmodel.Financial.CustomerBalance =
                            String.Format("{0:C}", customerbalance.TotalBalance).Replace("$", "");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    detailmodel.Financial.SubTotal = String.Format("{0:C}", job.SubTotal).Replace("$", "");
                    detailmodel.Financial.Tax = String.Format("{0:C}", job.TaxAmount).Replace("$", "");
                    detailmodel.Financial.TaxPartPercent = String.Format("{0:0.000}", job.TaxPartPercentage);
                    detailmodel.Financial.TaxLaborPercent = String.Format("{0:0.000}", job.TaxLaborPercentage);
                    detailmodel.Financial.Total = String.Format("{0:C}", job.TotalSales).Replace("$", "");

                    decimal? totalpayment = ctx.tbl_Payments.Where(q => q.JobID == job.JobID).Sum(q => q.PaymentAmount);

                    detailmodel.Financial.TotalPaid = String.Format("{0:C}", totalpayment ?? 0).Replace("$", "");

                    if (job.StatusID == 7)
                    {
                        if (!isTicket)
                        {
                            detailmodel.CloseTicketflag = "1";
                            detailmodel.IsItClose = "1";
                        }
                        else
                        {
                            if (job.LockedYN)
                            {
                                detailmodel.CloseTicketflag = "1";
                                detailmodel.IsItClose = "1";
                            }
                        }
                    }

                    detailmodel.Financial.PaymentType = objmodcommon.getPaymentType_Actual(MyJobID);
                    detailmodel.Financial.Balance = String.Format("{0:C}", job.Balance).Replace("$", "");

                    detailmodel.Payment.JobID = job.JobID;
                    var paymenttypelist = (from pt in ctx.tbl_Payment_Types select pt).ToList();
                    detailmodel.Payment.PaymentTypeList = new SelectList(paymenttypelist, "PaymentTypeId", "PaymentType");

                    // schedule
                    if (job.RequestedTime != null)
                    {
                        detailmodel.JobInfo.Requested = job.RequestedTime.Replace("am", "").Replace("pm", "");
                    }

                    detailmodel.JobInfo.ScheduleTime = job.ScheduleStart;

                    if (!(job.DispatchedDate == null))
                    {
                        detailmodel.JobInfo.Dispatched = job.DispatchedDate.ToString();
                    }
                    else
                    {
                        detailmodel.JobInfo.Dispatched = "";
                    }

                    detailmodel.JobInfo.DispatchedBy = objmodcommon.GetDisplayName(job.DispatcherID);

                    /*DateTime? jobstart = null;
                    DateTime? jobend = null;
                    if (!(job.ActualStart == null))
                    {
                        jobstart = job.ActualStart;
                        ViewBag.txt_Start = job.ActualStart.Value.Hour + ":" + job.ActualStart.Value.Minute;
                    }
                    else
                    {
                        ViewBag.txt_Start = "";
                    }

                    if (!(job.ActualEnd == null))
                    {
                        jobend = job.ActualEnd;
                        ViewBag.txt_End = job.ActualEnd.Value.Hour + ":" + job.ActualEnd.Value.Minute;
                    }
                    else
                    {
                        ViewBag.txt_End = "";
                    }

                    if ((!(job.ActualEnd == null) && !(job.ActualStart == null)))
                    {

                        ViewBag.txt_JobTime = objmodcommon.DateDiff(mod_common.DateInterval.Minute, jobend, jobstart).ToString();
                    }
                    else
                    {
                        ViewBag.txt_JobTime = "";
                    }*/

                    tbl_Job_HappyScores happyrec = (from j in ctx.tbl_Job_HappyScores where j.JobID == MyJobID select j).FirstOrDefault();
                    if (happyrec != null)
                    {
                        detailmodel.Head.NPS_Score = Convert.ToString(happyrec.ScoreValue);
                        detailmodel.Head.NPS_Comment = happyrec.ScoreComment;

                        if ((happyrec.ScoreValue == 11))
                        {
                            detailmodel.Head.NPS_Score = "X";
                        }

                        detailmodel.Head.Technician_Score = happyrec.TechScoreValue ?? 0;
                        detailmodel.Head.Technician_Comment = happyrec.TechScoreComment;

                        detailmodel.Head.Sched_Score = happyrec.SchedulerValue ?? 0;
                        detailmodel.Head.Sched_Comment = happyrec.SchedulerComment;

                        detailmodel.Head.IPad_Score = happyrec.iPadScoreValue ?? 0;
                        detailmodel.Head.IPad_Comment = happyrec.iPadScoreComment;
                    }

                    var pblist = ctx.tbl_PriceBook.Where(q => q.FranchiseID == job.FranchiseID).ToList();
                    detailmodel.MngJobTask.PriceBookList = new SelectList(pblist, "PriceBookID", "BookName");

                    var priceBookId = 0;
                    if (pblist.Count > 0)
                        priceBookId = pblist.First().PriceBookID;

                    var tclist = (from b in ctx.tbl_PB_JobCodes
                                  join c in ctx.tbl_PB_SubSection on b.SubSectionID equals c.SubsectionID
                                  join d in ctx.tbl_PB_Section on c.SectionID equals d.SectionID
                                  join e in ctx.tbl_PriceBook on d.PriceBookID equals e.PriceBookID
                                  where e.FranchiseID == job.FranchiseID && e.ActiveBookYN && e.PriceBookID == priceBookId
                                  orderby b.JobCode
                                  select new
                                  {
                                      b.JobCodeID,
                                      Code = b.JobCode + " - " + b.JobCodeDescription
                                  })
                                  .ToList();
                    detailmodel.MngJobTask.TaskCodeList = new SelectList(tclist, "JobCodeID", "Code");

                    var plist = (from s in ctx.tbl_PB_Parts
                                 join t in ctx.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                                 join m in ctx.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                                 where m.FranchiseID == job.FranchiseID && t.ActiveBookYN && t.PriceBookID == priceBookId
                                 orderby m.PartCode
                                 select new
                                 {
                                     s.PartID,
                                     PCode = m.PartCode + " - " + m.PartName
                                 })
                                 .ToList();
                    detailmodel.MngJobTask.PartList = new SelectList(plist, "PartID", "PCode");

                    var aclist = (from ac in ctx.tbl_Account_Codes
                                  select new
                                  {
                                      ac.AccountCode,
                                      AccountValue = "[" + ac.AccountCode + "] " + ac.AccountName
                                  })
                                  .ToList();

                    detailmodel.MngJobTask.AccountCodeList = new SelectList(aclist, "AccountCode", "AccountValue");

                    bool isOwner = HttpContext.User.IsInRole("CompanyOwner");
                    detailmodel.isOwner = isOwner;

                    bool swapBranding = false;

                    if (isOwner)
                    {
                        userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
                        using (var memCtx = new MembershipConnection())
                        {
                            swapBranding = memCtx.UserFranchise
                                .Where(uf => uf.UserId == userId)
                                .ToArray()
                                .Select(
                                    uf =>
                                    (from f in ctx.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).
                                        SingleOrDefault())
                                .Any(f => f.FranchiseTypeID == 6);
                        }
                    }

                    ViewBag.SwapBranding = swapBranding;
                    return View(detailmodel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public ActionResult PrintInvoice(int id, bool forCustomer)
        {
            var contentType = "application/pdf";
            var result = AbstractBusinessService.Create<InvoiceService>(UserInfo.UserKey).GetInvoicePdf(id, forCustomer);
            var fileName = result.ResultData.Key;
            var data = result.ResultData.Value;

            if (!result.Success)
            {
                contentType = "text/plain";
                fileName = "error.txt";
                data = Encoding.Unicode.GetBytes(result.Message);
            }

            return new FileStreamResult(new MemoryStream(data), contentType) { FileDownloadName = fileName };
        }

        [HttpPost]
        public JsonResult SendInvoice(int id, string to)
        {
            OperationResult<bool> result;

            try
            {
                result = AbstractBusinessService.Create<InvoiceService>(UserInfo.UserKey).SendToCustomer(id, to);
            }
            catch (Exception ex)
            {
                //TODO: Log exception;
                return Json(new { Success = false, Message = "Unhandled exception in the invoice delivery service: " + ex.Message });
            }

            return Json(new { Success = result.Success, Message = result.Message });
        }

        public ActionResult GetImage(int id, int PictureID)
        {
            var imageBytes = RetrieveImage(id, PictureID);

            if (imageBytes == null || imageBytes.Length == 0)
                imageBytes = System.IO.File.ReadAllBytes(Server.MapPath("/Content/images/noimage.png"));

            var ms = new MemoryStream(imageBytes);
            Response.Clear();
            Response.Expires = 0;
            Response.AddHeader("Content-Length", Convert.ToString(ms.Length));
            Response.BufferOutput = false;

            return new FileStreamResult(ms, "image/png");
        }

        public byte[] RetrieveImage(int id, int PictureID)
        {
            using (var db = GetContext())
            {
                var sigs = db.tbl_Job.Select(j => new { j.JobID, j.AuthorizationToStart, j.AcceptedBy }).SingleOrDefault(j => j.JobID == id);

                return PictureID == 0 ? sigs.AuthorizationToStart : sigs.AcceptedBy;
            }
        }

        public ActionResult Jobtask(int jobsid)
        {
            int jobID = jobsid;
            var lsttaskdetails = new List<JobTaskDetails>();
            JobTaskDetails summaryjobtaskdetails;
            var objjobtaskdetails = new JobTaskDetails();

            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var objjobtask = (from jobtask in ctx.tbl_Job_Tasks
                                  join jobtaskparts in ctx.tbl_Job_Task_Parts on jobtask.JobTaskID equals jobtaskparts.JobTaskID into parts
                                  from p in parts.DefaultIfEmpty()
                                  where jobtask.JobID == jobID
                                  select new
                                  {
                                      JobTaskPartsID = p == null ? 0 : p.JobTaskPartsID,
                                      jobtask.AccountCode,
                                      jobtask.JobTaskID,
                                      jobtask.Quantity,
                                      jobtask.JobCode,
                                      jobtask.JobCodeDescription,
                                      jobtask.Price,
                                      p.PartName,
                                      partquantity = p == null ? 0 : p.Quantity,
                                      partprice = p == null ? 0 : p.Price,
                                      p.PartCode,
                                      jobtask.AddOnYN,
                                      jobtask.MemberYN
                                  });

                foreach (var jt in objjobtask)
                {
                    objjobtaskdetails.addonyn = jt.AddOnYN;
                    objjobtaskdetails.JobTaskPartsID = jt.JobTaskPartsID;
                    objjobtaskdetails.jobtaskid = jt.JobTaskID;
                    objjobtaskdetails.taskQty = jt.Quantity;
                    objjobtaskdetails.Code = jt.JobCode;
                    objjobtaskdetails.TaskDescription = jt.JobCodeDescription;
                    objjobtaskdetails.Unit = jt.Price;
                    objjobtaskdetails.Line = jt.Price * jt.Quantity;
                    objjobtaskdetails.memberyn = jt.MemberYN;

                    if (jt.AddOnYN == false)
                    {
                        objjobtaskdetails.Part = jt.PartCode + " NoAuth!";
                    }
                    else
                    {
                        objjobtaskdetails.Part = jt.PartCode ?? string.Empty;
                    }

                    if (jt.AccountCode == "0" || jt.AccountCode == "" || jt.AccountCode == "00000")
                    {
                        objjobtaskdetails.PartDesc = jt.PartName + " NoAcct!";
                    }
                    else
                    {
                        objjobtaskdetails.PartDesc = jt.PartName ?? string.Empty;
                    }


                    //objjobtaskdetails.PartDesc = jt.PartName;
                    objjobtaskdetails.PartQty = jt.partquantity;
                    objjobtaskdetails.Price = jt.partprice;

                    summaryjobtaskdetails = new JobTaskDetails
                      {
                          addonyn = objjobtaskdetails.addonyn,
                          JobTaskPartsID = objjobtaskdetails.JobTaskPartsID,
                          jobtaskid = objjobtaskdetails.jobtaskid,
                          taskQty = objjobtaskdetails.taskQty,
                          Code = objjobtaskdetails.Code,
                          TaskDescription = objjobtaskdetails.TaskDescription,
                          Unit = objjobtaskdetails.Unit,
                          Line = objjobtaskdetails.Line,
                          Part = objjobtaskdetails.Part,
                          PartDesc = objjobtaskdetails.PartDesc,
                          PartQty = objjobtaskdetails.PartQty,
                          Price = objjobtaskdetails.Price,
                          strLine = string.Format("{0:C}", objjobtaskdetails.Line),
                          strUnit = string.Format("{0:C}", objjobtaskdetails.Unit),
                          strPrice = string.Format("{0:C}", objjobtaskdetails.Price),
                          memberyn = objjobtaskdetails.memberyn
                      };

                    lsttaskdetails.Add(summaryjobtaskdetails);

                }
            }

            return Json(lsttaskdetails);
        }

        public ActionResult HeaderData()
        {
            int frid = 0;
            if (Request.QueryString["ddlFrenchise"] != null && Request.QueryString["ddlFrenchise"] != "")
            {
                frid = Convert.ToInt32(Request.QueryString["ddlFrenchise"]);
            }

            var reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                MembershipUser user = membership.GetUser(User.Identity.Name);
                Guid userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                bool isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
                {
                    ViewBag.FunctionName = "doOnLoad()";
                    var FranchiseeList = ctx.tbl_Franchise
                                       .Where(f => assignedFranchises.Contains(f.FranchiseID) == true)
                                       .OrderBy(f => f.FranchiseNUmber)
                                       .Select(d => new { FranchiseID = d.FranchiseID, FranchiseNumber = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                       .ToList();

                    ViewBag.frenchise = FranchiseeList;

                    if (frid > 0)
                    {
                        ViewBag.FranchiseeId = frid;
                        ViewBag.FranchiseeNumber = ctx.tbl_Franchise.Where(f => assignedFranchises.Contains(frid) == true)
                                                   .Select(d => new { FranchiseNumber = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) }).FirstOrDefault().FranchiseNumber;
                    }
                    else
                    {
                        ViewBag.FranchiseeId = FranchiseeList[0].FranchiseID;
                        ViewBag.FranchiseeNumber = FranchiseeList[0].FranchiseNumber;
                    }
                }
            }
            else
            {
                return new RedirectResult("/SGAccount/LogOn");
            }
            return PartialView("Header");
        }

        public ActionResult InvoiceList()
        {
            if (User.Identity.Name != "")
            {
                using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
                {
                    List<tbl_Job_Status> statusdata = ctx.tbl_Job_Status.ToList();

                    var ts = new System.TimeSpan(7, 0, 0, 0);
                    var model = new InvoiceSearch
                                              {
                                                  StatusList = new SelectList(statusdata, "StatusID", "Status"),
                                                  StartDate = DateTime.Now.Subtract(ts),
                                                  EndDate = DateTime.Now
                                              };

                    return View(model);
                }
            }

            return new RedirectResult("/SGAccount/LogOn");
        }

        public ActionResult InvoiceData(string StartDate, string EndDate, string Status, int FranchiseID)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            DateTime startdate = DateTime.Parse(StartDate);
            //Increasing the End Date by one day.
            DateTime enddate = DateTime.Parse(EndDate).AddDays(1);

            var objmodcommon = new mod_common(UserInfo.UserKey);

            var JobsId = new List<string>();

            var sb = new StringBuilder();
            sb.Append("<rows>");
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var joblist = (from j in DB.tbl_Job
                               join c in DB.tbl_Customer on j.CustomerID equals c.CustomerID into tc
                               from c in tc.DefaultIfEmpty()
                               join l in DB.tbl_Locations on j.LocationID equals l.LocationID into tl
                               from l in tl.DefaultIfEmpty()
                               join js in DB.tbl_Job_Status on j.StatusID equals js.StatusID into tjs
                               from js in tjs.DefaultIfEmpty()
                               join jt in DB.tbl_Job_Type on j.JobTypeID equals jt.JobTypeID into tjt
                               from jt in tjt.DefaultIfEmpty()
                               join e in DB.tbl_Employee on j.ServiceProID equals e.EmployeeID into te
                               from e in te.DefaultIfEmpty()
                               where j.CallTaken >= startdate && j.CallTaken < enddate && j.FranchiseID == FranchiseID
                               select new
                               {
                                   j.JobID,
                                   j.StatusID,
                                   j.SubTotal,
                                   j.Balance,
                                   j.CustomerComments,
                                   c.CustomerName,
                                   c.CompanyName,
                                   js.Status,
                                   jt.JobType,
                                   ClosedDate = j.InvoicedDate,
                                   CompletedDate = j.CallCompleted,
                                   JobLocation = l.Address,
                                   Tech = e.Employee
                               })
                            .ToList();

                if (Status != null && Status != "" && Convert.ToInt16(Status) > 0)
                {
                    joblist = (from c in joblist where c.StatusID == Convert.ToInt16(Status) select c).ToList();
                }

                if (joblist.Count() > 0)
                {


                    foreach (var job in joblist)
                    {
                        JobsId.Add(job.JobID.ToString());

                        const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                        const string dateFormat = "<cell><![CDATA[{0:D}]]></cell>";
                        const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                        sb.AppendFormat("<row id='{0}'>", job.JobID);
                        sb.AppendFormat(moneyFormat, job.SubTotal);
                        sb.AppendFormat(formatStr, job.JobID);
                        sb.AppendFormat(formatStr, objmodcommon.GetCustomerName(job.CustomerName, job.CompanyName));
                        sb.AppendFormat(formatStr, job.JobLocation);
                        sb.AppendFormat(formatStr, job.Status);
                        sb.AppendFormat(dateFormat, job.CompletedDate);
                        sb.AppendFormat(dateFormat, job.ClosedDate);
                        sb.AppendFormat(formatStr, job.Tech);
                        sb.AppendFormat(moneyFormat, job.Balance);
                        sb.AppendFormat(formatStr, job.JobType);
                        sb.AppendFormat(formatStr, job.CustomerComments);
                        sb.Append("</row>");
                    }

                    Session["JobsId"] = JobsId;
                }
                sb.Append("</rows>");
            }

            return Json(sb.ToString());
        }

        public ActionResult ConTasktoAuthNoAuth(int jobtaskid, int jobtaskpartid)
        {
            string strmessage = "";
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                tbl_Job_Tasks jobtask = (from jt in DB.tbl_Job_Tasks where jt.JobTaskID == jobtaskid select jt).FirstOrDefault();
                tbl_Job_Task_Parts jobtaskpart = (from jtp in DB.tbl_Job_Task_Parts where jtp.JobTaskID == jobtaskid && jtp.JobTaskPartsID == jobtaskpartid select jtp).FirstOrDefault();

                int tmpJobCodeID = jobtask.JobCodeID;
                string tmpJobCode = jobtask.JobCode;
                int tmpBusTypeID = (from job in DB.tbl_Job where job.JobID == jobtask.JobID select job.BusinessTypeID).Single();

                if (jobtask.AddOnYN == false)
                {
                    jobtask.AddOnYN = true;
                }
                if (jobtask.AccountCode == "0" || jobtask.AccountCode == "" || jobtask.AccountCode == "00000")
                {
                    jobtask.AccountCode = "0000"; // objmodcommon.Get_Account_Code_ByCode(tmpJobCode, tmpBusTypeID);
                    if (jobtask.AccountCode == "00000")
                    {
                        strmessage = "This task code does not have a corretc price book Account code link, see your price book manager";
                    }
                }

                if (jobtask.AccountCode == null)
                {
                    jobtask.AccountCode = "00000";
                    strmessage = "This task code does not have a corretc price book Account code link, see your price book manager";
                }

                DB.SaveChanges();
            }

            return Json(strmessage);
        }
        public ActionResult GetJobCodedata(string jobcodeid)
        {
            int ijobcodeid = Convert.ToInt32(jobcodeid);
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var TaskList = DB.tbl_PB_JobCodes.Select(b => new { b.JobCodeID, b.JobCode, b.JobCodeDescription, b.JobAddonMemberPrice })
                                                 .FirstOrDefault(b => b.JobCodeID == ijobcodeid);
                return Json(TaskList);
            }

        }
        public ActionResult GetJobPartsdata(string partsid)
        {
            int partid = Convert.ToInt32(partsid);
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var Partslist = (from s in DB.tbl_PB_Parts
                                 join m in DB.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                                 where s.PartID == partid
                                 orderby m.PartCode
                                 select new { m.PartCode, m.PartName, s.PartAddonMemberPrice }).FirstOrDefault();

                return Json(Partslist);
            }
        }

        public ActionResult GetTaxRateByid(string TaxRateID)
        {
            var taxrate = new List<tbl_TaxRates>();
            var objmodcommon = new mod_common(UserInfo.UserKey);

            if (Convert.ToInt32(TaxRateID) != 0)
                taxrate = objmodcommon.GetTaxRate(Convert.ToInt32(TaxRateID)).ToList();

            return Json(taxrate);
        }

        public void UpdateTaxrate(string Job_id, float taxpartpercentage, float taxlaborpercentage)
        {

            int jobid = Convert.ToInt32(Job_id);
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                tbl_Job job = (from j in DB.tbl_Job where j.JobID == jobid select j).FirstOrDefault();
                if (job != null)
                {
                    job.TaxLaborPercentage = taxpartpercentage;
                    job.TaxPartPercentage = taxlaborpercentage;
                    DB.SaveChanges();
                }
            }
        }

        public decimal? UpdateFinancial(string JobId, string JobSubTotal, string Taxrate, string TaxPP, string TaxLP, string JobTotal, string Balance)
        {
            if (!string.IsNullOrEmpty(JobId))
            {
                int ijobid = Convert.ToInt32(JobId);

                using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
                {
                    tbl_Job objjob = (from j in DB.tbl_Job where j.JobID == ijobid select j).FirstOrDefault();
                    if (objjob != null)
                    {
                        objjob.SubTotal = Convert.ToDecimal(JobSubTotal);
                        objjob.TaxAmount = Convert.ToDecimal(Taxrate);
                        objjob.TaxPartPercentage = (float)Convert.ToDouble(TaxPP);
                        objjob.TaxLaborPercentage = (float)Convert.ToDouble(TaxLP);
                        objjob.TotalSales = Convert.ToDecimal(JobTotal);
                        objjob.Balance = Convert.ToDecimal(Balance);

                        DB.SaveChanges();

                        return (from h in DB.tbl_Job
                                where h.CustomerID == objjob.CustomerID
                                group h by h.CustomerID
                                    into g
                                    select new
                                    {
                                        TotalBalance = g.Sum(x => x.Balance)
                                    })
                                    .Single().TotalBalance;
                    }
                }
            }

            return null;
        }

        public ActionResult PaymentType(int jobid)
        {
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                List<tbl_Payment_Types> lstPaymentType = (from pt in DB.tbl_Payment_Types select pt).ToList();
                tbl_Payments paymentid = (from p in DB.tbl_Payments where p.JobID == jobid select p).FirstOrDefault();
                ViewBag.PaymentTypeList = lstPaymentType;
                if (paymentid != null)
                {
                    ViewBag.paymentid = paymentid.PaymentID.ToString();
                }
                else
                {
                    ViewBag.paymentid = "0";

                }
            }
            return View();
        }

        public void UpdatePaymentType(string Job_id, string PaymentID)
        {
            int jobid = Convert.ToInt32(Job_id);
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                List<tbl_Payments> lstPayments = (from p in DB.tbl_Payments where p.JobID == jobid select p).ToList();
                var objpayment = new Payments();
                if (lstPayments.Count() > 0)
                {
                    foreach (var item in lstPayments)
                    {
                        objpayment.PaymentID = Convert.ToInt32(PaymentID);
                    }
                }
            }
        }

        public ActionResult Invoicing()
        {
            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var FranchiseeList = (from p in DB.tbl_Franchise
                                      join o in DB.tbl_Franchise_Owner on p.OwnerID equals o.OwnerID
                                      orderby p.FranchiseNUmber
                                      select new
                                                 {
                                                     FranchiseID = p.FranchiseID,
                                                     FranchiseNumber = p.FranchiseNUmber + " - " + o.LegalName
                                                 }).ToList();
                ViewBag.frenchise = FranchiseeList;
                tbl_Franchise objFranchisee = (from e in DB.tbl_Franchise
                                               join o in DB.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
                                               where e.FranchiseID == 56
                                               select e).FirstOrDefault();
                ViewBag.FranchiseeId = objFranchisee.FranchiseID;
                ViewBag.FranchiseeNumber = objFranchisee.FranchiseNUmber + "-" + objFranchisee.LegalName;
            }

            return View();
        }

        public ActionResult InvoiceResult(string statusList, string franchiseid, string strselected)
        {

            int fid = 0;
            int jobID = 0;
            string billto = "";
            string joblocation = "";
            string status = "";
            string closeddate = "";
            string completeddate = "";
            string Tech = "";
            double Jobamt = 0.0;
            double balance = 0.0;
            string jobtype = "";
            string comments = "";
            var reportDetails = new List<Details>();
            var detail = new Details();
            Details summarydetails;
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            var JobsId = new List<string>();

            if (franchiseid != "")
            {
                fid = Convert.ToInt32(franchiseid);
            }


            if (statusList == "" || statusList == null)
            {
                statusList = "Completed";
            }

            //////////////////////////////////////////////////////////////////////////////

            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var objmodcommon = new mod_common(UserInfo.UserKey);
                DateTime? WSRDate = DateTime.Now;
                if (strselected == "Current Weeks Closed Sales")
                {
                    //if (this.lv_Invoicing.Items.Count > 0) {
                    IQueryable<tbl_WSR> wsrlist2 = from w in DB.tbl_WSR where w.FranchiseID == fid select w;
                    DateTime? lastwsrdate = Convert.ToDateTime("1/1/1900");
                    foreach (var wsr_loopVariable in wsrlist2)
                    {

                        lastwsrdate = wsr_loopVariable.LastWSRDate;
                    }
                    System.DateTime currentday = DateTime.Now.Date;

                    string dayofweek = DateTime.Today.DayOfWeek.ToString();
                    TimeSpan tmpspn;
                    switch (dayofweek)
                    {
                        case "Sunday":
                            tmpspn = new TimeSpan(-1, 0, 0, 0);
                            WSRDate = currentday.Add(tmpspn); //DateAndTime.DateAdd(DateInterval.Day, -1, currentday);
                            break;
                        case "Monday":
                            tmpspn = new TimeSpan(-2, 0, 0, 0);
                            WSRDate = currentday.Add(tmpspn); //DateAndTime.DateAdd(DateInterval.Day, -2, currentday);
                            break;
                        case "Tuesday":
                            tmpspn = new TimeSpan(-3, 0, 0, 0);
                            WSRDate = currentday.Add(tmpspn); //DateAndTime.DateAdd(DateInterval.Day, -3, currentday);
                            break;
                        case "Wednesday":
                            tmpspn = new TimeSpan(-4, 0, 0, 0);
                            WSRDate = currentday.Add(tmpspn); //DateAndTime.DateAdd(DateInterval.Day, -4, currentday);
                            break;
                        case "Thursday":
                            tmpspn = new TimeSpan(-5, 0, 0, 0);
                            WSRDate = currentday.Add(tmpspn); //DateAndTime.DateAdd(DateInterval.Day, -5, currentday);
                            break;
                        case "Friday":
                            tmpspn = new TimeSpan(-6, 0, 0, 0);
                            WSRDate = currentday.Add(tmpspn); // DateAndTime.DateAdd(DateInterval.Day, -6, currentday);
                            break;
                        case "Saturday":
                            tmpspn = new TimeSpan(-7, 0, 0, 0);
                            WSRDate = currentday.Add(tmpspn); // DateAndTime.DateAdd(DateInterval.Day, -7, currentday);
                            break;
                    }

                    if (objmodcommon.DateDiff(mod_common.DateInterval.Day, lastwsrdate, WSRDate) > 0)
                    {
                        //if (Interaction.MsgBox("This will move all the jobs from the window onto your weekly sales report for the week of " + WSRDate + ".  Would you like to continue?", MsgBoxStyle.YesNo, "Create Weekly Sales Report") == MsgBoxResult.Yes) {

                        int i = 0;
                        List<tbl_Job> Closedjoblist = (from j in DB.tbl_Job
                                                       join s in DB.tbl_Job_Status on j.StatusID equals s.StatusID
                                                       where s.Status == "Closed" && j.FranchiseID == fid && j.WSRCompletedDate == null
                                                       select j).ToList();

                        while (i < Closedjoblist.Count())
                        {
                            //int jobid = this.lv_Invoicing.Items(i).SubItems(0).Text;
                            int jobid = Closedjoblist[i].JobID;
                            tbl_Job job = objmodcommon.getJob(jobid);
                            DateTime? callcompleted = job.CallCompleted;
                            if (objmodcommon.DateDiff(mod_common.DateInterval.Day, callcompleted, WSRDate) >= 0)
                            {
                                job.WSRCompletedDate = WSRDate;
                                job.LockedYN = true;
                                DB.SaveChanges();

                                IQueryable<tbl_WSR> wsrlist = from w in DB.tbl_WSR where w.FranchiseID == job.FranchiseID select w;
                                if (wsrlist.Count() == 0)
                                {
                                    var rec = new tbl_WSR();
                                    rec.FranchiseID = job.FranchiseID;
                                    rec.LastWSRDate = WSRDate;
                                    DB.tbl_WSR.AddObject(rec);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    foreach (var wsr_loopVariable in wsrlist)
                                    {

                                        wsr_loopVariable.LastWSRDate = WSRDate;
                                        DB.SaveChanges();
                                    }
                                }
                            }
                            i += 1;
                        }
                        //this.lbl_Name.Text = "Current Weeks Closed Sales";
                        //LoadScreen = "Closed";
                        //Setup_List_Containers();
                        //Load_List();
                        //}
                    }
                    else
                    {
                        ViewBag.lblmessage = "Can't create Weekly Sales Report for " + WSRDate + ".  This week already exists.";
                    }

                    //}
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////

                List<tbl_Job> joblist = (from j in DB.tbl_Job
                                         join s in DB.tbl_Job_Status on j.StatusID equals s.StatusID
                                         where s.Status == statusList && j.FranchiseID == fid
                                         select j).ToList();


                if (statusList == "Completed" || statusList == "Active" || statusList == "Appointment")
                {
                    joblist = (from j in joblist where j.InvoicedDate == null select j).ToList();

                }
                else if (statusList == "Closed")
                {
                    joblist = (from j in joblist where j.WSRCompletedDate == null select j).ToList();

                }


                //var joblist = (from j in DB.tbl_Job
                //               join s in DB.tbl_Job_Status on j.StatusID equals s.StatusID 
                //               where s.Status == statusList && j.FranchiseID == fid && j.InvoicedDate == null
                //               select j).ToList();


                if (joblist.Count() > 0)
                {
                    foreach (var job in joblist)
                    {
                        JobsId.Add(job.JobID.ToString());

                        jobID = job.JobID;
                        tbl_Customer Customer = objmodcommon.GetCustomers(job.CustomerID);
                        if (Customer != null)
                        {

                            billto = textInfo.ToTitleCase(objmodcommon.GetCustomerName(Customer).ToString());
                        }
                        tbl_Locations location = objmodcommon.getLocation(job.LocationID);
                        if (location != null)
                        {
                            joblocation = location.Address.ToString();
                        }
                        else
                        {
                            joblocation = "";
                        }
                        status = objmodcommon.getStatus(job.StatusID);
                        if (job.InvoicedDate != null)
                        {
                            closeddate = job.InvoicedDate.ToString();
                        }
                        else
                        {
                            closeddate = "";
                        }
                        if (job.CallCompleted != null)
                        {
                            completeddate = job.CallCompleted.ToString();
                        }
                        else
                        {
                            completeddate = "";
                        }
                        if (job.JobTypeID != 0)
                        {
                            jobtype = (from jt in DB.tbl_Job_Type where jt.JobTypeID == job.JobTypeID select new { jt.JobType }).FirstOrDefault().JobType.ToString();
                        }
                        Tech = objmodcommon.Get_Employee_Name(job.ServiceProID);
                        Jobamt = (double)job.SubTotal;
                        balance = (double)job.Balance;
                        comments = job.CustomerComments;

                        detail.Balance = job.Balance;
                        detail.BillTo = billto;

                        detail.ClosedDate = objmodcommon.ParseNullableDateTime(closeddate);
                        detail.Comments = comments;
                        detail.CompletedDate = objmodcommon.ParseNullableDateTime(completeddate);
                        detail.Invoicenumber = job.JobID;
                        detail.JobAmt = Jobamt;
                        detail.JobLocation = joblocation;
                        detail.JobType = jobtype;
                        detail.Status = status;
                        detail.Tech = Tech;
                        if (detail.ClosedDate != null) { detail.shortdatestring = detail.ClosedDate.Value.ToShortDateString(); } else { detail.shortdatestring = ""; }
                        if (detail.CompletedDate != null) { detail.completedshortdatestring = detail.CompletedDate.Value.ToShortDateString(); } else { detail.completedshortdatestring = ""; }
                        summarydetails =
                          new Details
                          {
                              Balance = detail.Balance,
                              strBalance = string.Format("{0:C}", detail.Balance),
                              BillTo = detail.BillTo,
                              ClosedDate = detail.ClosedDate,
                              Comments = detail.Comments,
                              CompletedDate = detail.CompletedDate,
                              Invoicenumber = detail.Invoicenumber,
                              JobAmt = detail.JobAmt,
                              strJobAmt = string.Format("{0:C}", detail.JobAmt),
                              JobLocation = detail.JobLocation,
                              JobType = detail.JobType,
                              Status = detail.Status,
                              Tech = detail.Tech,
                              shortdatestring = detail.shortdatestring,
                              completedshortdatestring = detail.completedshortdatestring
                          };
                        reportDetails.Add(summarydetails);

                    }
                }
            }

            return Json(reportDetails);
        }

        public ActionResult InovicePayment()
        {

            int franchiseID = Convert.ToInt32(Session["FranchiseId"]);
            if (franchiseID == 0)
            {
                franchiseID = 56;
                ViewBag.FranchiseId = franchiseID;
            }
            else
            {
                ViewBag.FranchiseId = franchiseID;
            }
            return View();
        }
        public ActionResult GetInvoiceJob(string id)
        {

            int FranchiseID = Convert.ToInt32(id);
            var objInvoicePayment = new List<InvoicePayment>();

            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                //Where p.CustomerID = MyCustomerID AndAlso p.Balance > 0 AndAlso p.FranchiseID = MyFranchiseID
                //TBR and put customerID condition in below query
                IQueryable<tbl_Job> joblist = (from p in DB.tbl_Job where p.Balance > 0 && p.FranchiseID == FranchiseID select p);
                foreach (var item in joblist)
                {
                    var oInvoicePayment = new InvoicePayment();
                    oInvoicePayment.InvoiceNumber = item.JobID.ToString();

                    Location objLocation =
                        (from l in DB.Locations where l.LocationId == item.LocationID select l).FirstOrDefault();
                    if (objLocation != null)
                        oInvoicePayment.Address = objLocation.Address;
                    else
                        oInvoicePayment.Address = "";
                    oInvoicePayment.InvoicedDate = item.InvoicedDate;
                    oInvoicePayment.TotalSales = item.TotalSales;
                    oInvoicePayment.Balance = item.Balance;
                    objInvoicePayment.Add(oInvoicePayment);
                }
                var emptyList = new List<string>();
                ViewBag.EmptyList = emptyList;
                //List<string> lst = new List<string>() { "a", "b" };
            }
            return Json(objInvoicePayment);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadInvoiceList()
        {
            var generator = new ExcelWriter();
            string xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            MemoryStream stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "SelectedTickets.xlsx");
        }

        public ActionResult UpdateComments(int id, string note)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                tbl_Job job = ctx.tbl_Job.First(q => q.JobID == id);

                try
                {
                    job.CustomerComments = note;

                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public ActionResult UpdateDiagnosis(int id, string note)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                tbl_Job job = ctx.tbl_Job.First(q => q.JobID == id);

                try
                {
                    job.Diagnostics = note;

                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        [HttpPost, Authorize(Roles = "Corporate")]
        public JsonResult SaveInvoiceDetailsScore(int? jobid, int? npsScore, string npsComment, int? techScore, string techComment, int? scheduleScore, string scheduleComment, int? ipadScore, string ipadComment)
        {
            using (var dbContext = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var jobScore = (from job in dbContext.tbl_Job_HappyScores
                                where job.JobID == jobid
                                select job).FirstOrDefault();

                if (jobScore == null)
                {
                    // Get the job info.
                    var jobInfo = (from j in dbContext.tbl_Job
                                   where j.JobID == jobid
                                   select new { j.FranchiseID, j.RequestedDate }).FirstOrDefault();

                    // Create new job score/comment.
                    jobScore = new tbl_Job_HappyScores();
                    jobScore.JobID = jobid.Value;
                    jobScore.FranchiseID = jobInfo == null ? 0 : jobInfo.FranchiseID;
                    jobScore.ScoreValue = npsScore ?? 0;
                    jobScore.ScoreComment = npsComment;
                    jobScore.TechScoreValue = techScore ?? 0;
                    jobScore.TechScoreComment = techComment;
                    jobScore.SchedulerValue = scheduleScore ?? 0;
                    jobScore.SchedulerComment = scheduleComment;
                    jobScore.iPadScoreValue = ipadScore ?? 0;
                    jobScore.iPadScoreComment = ipadComment;

                    var encoding = new System.Text.ASCIIEncoding();
                    var timestamp = encoding.GetBytes(DateTime.Now.ToString());
                    jobScore.timestamp = timestamp;

                    jobScore.JobDate = jobInfo == null ? null : jobInfo.RequestedDate;

                    dbContext.tbl_Job_HappyScores.AddObject(jobScore);
                    var rowsAffected = dbContext.SaveChanges();

                    return Json(new
                                {
                                    Success = true,
                                    Message = string.Format("Score/Comments for job '{0}' added.", jobid),
                                    ResultData = ""
                                });
                }
                else
                {
                    // Update existing job score/comment.
                    jobScore.ScoreValue = npsScore ?? 0;
                    jobScore.ScoreComment = npsComment;
                    jobScore.TechScoreValue = techScore ?? 0;
                    jobScore.TechScoreComment = techComment;
                    jobScore.SchedulerValue = scheduleScore ?? 0;
                    jobScore.SchedulerComment = scheduleComment;
                    jobScore.iPadScoreValue = ipadScore ?? 0;
                    jobScore.iPadScoreComment = ipadComment;

                    dbContext.SaveChanges();

                    return Json(new
                                {
                                    Success = true,
                                    Message = string.Format("Score/Comments for job '{0}' updated.", jobid),
                                    ResultData = ""
                                });
                }
            }
        }

        [HttpPost, Authorize(Roles = "CompanyOwner")]
        public JsonResult UpdateServiceType(int? jobId, int? serviceId)
        {
            using (var dbContext = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var job = (from j in dbContext.tbl_Job
                           where j.JobID == jobId
                           select j).FirstOrDefault();

                if (job != null)
                {
                    job.ServiceID = serviceId;
                    dbContext.SaveChanges();

                    return Json(new
                    {
                        Success = true,
                        Message = string.Format("Service Type for job '{0}' updated.", jobId),
                        ResultData = ""
                    });
                }

                return Json(new
                {
                    Message = string.Format("Service Type for job '{0}' not found.", jobId),
                    ResultData = "",
                    Success = false
                });
            }
        }

        public ActionResult JobStatusHistory(int jobid)
        {
            var lststatushistory = new List<JobStatusHistory>();

            DateTime lastDate = Convert.ToDateTime("1/1/2000").Date;
            int laststaus = 0;

            using (var DB = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var joblist = (from j in DB.tbl_Job_Status_History
                               join S in DB.tbl_Job_Status on j.StatusID equals S.StatusID
                               where j.JobID == jobid
                               orderby j.StatusDateChanged
                               select new { j, S })
                               .ToList();

                foreach (var item in joblist)
                {
                    string changefromto = item.j.ChangedfromTo;
                    lastDate = item.j.StatusDateChanged.Value;

                    if (item.j.ChangedField == "ServiceProID")
                    {
                        string[] stringSeparators = new string[] { "=>" };
                        string[] fromto = item.j.ChangedfromTo.Trim().Split(stringSeparators, StringSplitOptions.None);

                        int v1 = Convert.ToInt32(fromto[0]);
                        int v2 = Convert.ToInt32(fromto[1]);

                        var f = DB.tbl_Employee.FirstOrDefault(q => q.EmployeeID == v1);
                        var t = DB.tbl_Employee.FirstOrDefault(q => q.EmployeeID == v2);

                        changefromto = ((f == null) ? "N/A" : f.Employee) + " => " + ((t == null) ? "N/A" : t.Employee);
                    }

                    if (item.j.ChangedField == "StatusID")
                    {
                        string[] stringSeparators = new string[] { "=>" };
                        string[] fromto = item.j.ChangedfromTo.Trim().Split(stringSeparators, StringSplitOptions.None);

                        int v1 = Convert.ToInt32(fromto[0]);
                        int v2 = Convert.ToInt32(fromto[1]);

                        var f = DB.tbl_Job_Status.FirstOrDefault(q => q.StatusID == v1);
                        var t = DB.tbl_Job_Status.FirstOrDefault(q => q.StatusID == v2);

                        changefromto = ((f == null) ? "N/A" : f.Status) + " => " + ((t == null) ? "N/A" : t.Status);
                    }

                    if (item.j.ChangedField == "ServiceID")
                    {
                        string[] stringSeparators = new string[] { "=>" };
                        string[] fromto = item.j.ChangedfromTo.Trim().Split(stringSeparators, StringSplitOptions.None);

                        int v1 = Convert.ToInt32(fromto[0]);
                        int v2 = Convert.ToInt32(fromto[1]);

                        var f = DB.tbl_Services.FirstOrDefault(q => q.ServiceID == v1);
                        var t = DB.tbl_Services.FirstOrDefault(q => q.ServiceID == v2);

                        changefromto = ((f == null) ? "N/A" : f.ServiceName) + " => " + ((t == null) ? "N/A" : t.ServiceName);
                    }

                    if (item.j.ChangedField == "TaxAuthorityID")
                    {
                        string[] stringSeparators = new string[] { "=>" };
                        string[] fromto = item.j.ChangedfromTo.Trim().Split(stringSeparators, StringSplitOptions.None);

                        int v1 = Convert.ToInt32(fromto[0]);
                        int v2 = Convert.ToInt32(fromto[1]);

                        var f = DB.tbl_TaxRates.FirstOrDefault(q => q.TaxRateID == v1);
                        var t = DB.tbl_TaxRates.FirstOrDefault(q => q.TaxRateID == v2);

                        changefromto = ((f == null) ? "N/A" : f.TaxDescription) + " => " + ((t == null) ? "N/A" : t.TaxDescription);
                    }

                    if (item.j.ChangedField == "BusinessTypeID")
                    {
                        string[] stringSeparators = new string[] { "=>" };
                        string[] fromto = item.j.ChangedfromTo.Trim().Split(stringSeparators, StringSplitOptions.None);

                        int v1 = Convert.ToInt32(fromto[0]);
                        int v2 = Convert.ToInt32(fromto[1]);

                        var f = DB.tbl_Customer_BusinessType.FirstOrDefault(q => q.BusinessTypeID == v1);
                        var t = DB.tbl_Customer_BusinessType.FirstOrDefault(q => q.BusinessTypeID == v2);

                        changefromto = ((f == null) ? "N/A" : f.BusinessType) + " => " + ((t == null) ? "N/A" : t.BusinessType);
                    }

                    lststatushistory.Add(new JobStatusHistory
                    {
                        statuses = item.S.Status,
                        statuschangeddate = item.j.StatusDateChanged.Value.ToShortDateString(),
                        time = item.j.StatusDateChanged.Value.ToShortTimeString(),
                        changedto = changefromto,
                        field = (item.j.ChangedField == "RescheduleReason") ? "UpdateNotes" : item.j.ChangedField,
                        bywhom = item.j.ChangedBy,
                        tablet = item.j.ChangedOnTabletYN ? "YES" : "NO"
                    });
                }

                if (laststaus != 6 && laststaus != 7)
                {
                    var jobrec = (from J in DB.tbl_Job where J.JobID == jobid select new { J.CallCompleted, J.StatusID }).Single();

                    if (jobrec.CallCompleted > lastDate)
                    {
                        lststatushistory.Add(new JobStatusHistory
                        {
                            statuses = "Completed",
                            statuschangeddate = jobrec.CallCompleted.Value.ToShortDateString(),
                            time = jobrec.CallCompleted.Value.ToShortTimeString(),
                            changedto = "",
                            field = "",
                            bywhom = "",
                            tablet = "---"
                        });
                    }
                }
            }

            return Json(lststatushistory);
        }

        public ActionResult HistoryList(int jobid)
        {
            string strxml = "";
            var objmodcommon = new mod_common(UserInfo.UserKey);

            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                int LocationID = (from jobs in ctx.tbl_Job
                                  where jobs.JobID == jobid
                                  select jobs.LocationID)
                                .FirstOrDefault();

                var objhistory = new History();
                var joblist = (from j in ctx.tbl_Job
                               where j.LocationID == LocationID
                               select new
                               {
                                   j.JobID,
                                   j.JobTypeID,
                                   j.StatusID,
                                   j.ServiceDate,
                                   j.TotalSales,
                                   j.Balance
                               });

                strxml = strxml + "<rows>";

                int i = 0;
                foreach (var job in joblist)
                {
                    objhistory.jobsid = Convert.ToString(job.JobID);
                    objhistory.JobType = objmodcommon.GetJobTypeName(job.JobTypeID);
                    objhistory.Status = objmodcommon.getStatus(job.StatusID);
                    objhistory.Date1 = job.ServiceDate;
                    objhistory.Gross = string.Format("{0:C}", job.TotalSales);
                    objhistory.Balance = string.Format("{0:C}", job.Balance);
                    IQueryable<tbl_Job_Technicians> jobtech1 =
                        (from t in ctx.tbl_Job_Technicians where t.JobID == job.JobID && t.PrimaryYN == true select t);
                    if (jobtech1.Count() >= 1)
                    {
                        string Servicepro;
                        foreach (var jobrec1 in jobtech1)
                        {
                            Servicepro = objmodcommon.Get_Employee_Name(jobrec1.ServiceProID);
                            objhistory.ServicedBy = Servicepro;
                        }

                    }

                    strxml = strxml + "<row id='" + i + "'>";
                    strxml = strxml + "<cell><![CDATA[" + objhistory.jobsid + "]]></cell>";
                    strxml = strxml + "<cell><![CDATA[" + objhistory.JobType + "]]></cell>";
                    strxml = strxml + "<cell><![CDATA[" + objhistory.Status + "]]></cell>";
                    strxml = strxml + "<cell><![CDATA[" + objhistory.Date1 + "]]></cell>";
                    strxml = strxml + "<cell><![CDATA[" + objhistory.Gross + "]]></cell>";
                    strxml = strxml + "<cell><![CDATA[" + objhistory.Balance + "]]></cell>";
                    strxml = strxml + "<cell><![CDATA[" + objhistory.ServicedBy + "]]></cell>";
                    strxml = strxml + "</row>";
                    i = i + 1;
                    //}

                }
            }
            strxml = strxml + "</rows>";
            return Json(strxml);
        }

        public ActionResult JobHistoryList(string jobid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                int jid = int.Parse(jobid);

                var jobhistorydata = (from log in ctx.AuditLogs
                                      join aj in ctx.Audit_Job on log.AuditID equals aj.AuditID
                                      where log.EntityID == jobid && log.EntityType == "tbl_Job"
                                      select new
                                      {
                                          log.UserKey,
                                          log.AuditDate,
                                          log.Type,
                                          log.EntityType,
                                          aj.Attribute,
                                          aj.NewValue,
                                          aj.OldValue
                                      })
                                      .Concat(
                                      from log in ctx.AuditLogs
                                      join pj in ctx.Audit_Payment on log.AuditID equals pj.AuditID
                                      join p in ctx.tbl_Payments on log.EntityID equals SqlFunctions.StringConvert((double)p.PaymentID).Trim()
                                      where p.JobID == jid && log.EntityType == "tbl_Payments"
                                      select new
                                      {
                                          log.UserKey,
                                          log.AuditDate,
                                          log.Type,
                                          log.EntityType,
                                          pj.Attribute,
                                          pj.NewValue,
                                          pj.OldValue
                                      })
                                      .OrderBy(q => q.AuditDate)
                                      .ToList();

                var jobhistorylist = new List<JobHistoryInfo>();

                for (var i = 0; i < jobhistorydata.Count; i++)
                {
                    string from = jobhistorydata[i].OldValue;
                    string to = jobhistorydata[i].NewValue;

                    if (jobhistorydata[i].Attribute == "ServiceProID")
                    {
                        int v1 = Convert.ToInt32(from);
                        int v2 = Convert.ToInt32(to);

                        var f = ctx.tbl_Employee.FirstOrDefault(q => q.EmployeeID == v1);
                        var t = ctx.tbl_Employee.FirstOrDefault(q => q.EmployeeID == v2);

                        from = (f == null) ? "N/A" : f.Employee;
                        to = (t == null) ? "N/A" : t.Employee;
                    }

                    if (jobhistorydata[i].Attribute == "StatusID")
                    {
                        int v1 = Convert.ToInt32(from);
                        int v2 = Convert.ToInt32(to);

                        var f = ctx.tbl_Job_Status.FirstOrDefault(q => q.StatusID == v1);
                        var t = ctx.tbl_Job_Status.FirstOrDefault(q => q.StatusID == v2);

                        from = (f == null) ? "N/A" : f.Status;
                        to = (t == null) ? "N/A" : t.Status;
                    }

                    if (jobhistorydata[i].Attribute == "PaymentTypeID")
                    {
                        int v1 = Convert.ToInt32(from);
                        int v2 = Convert.ToInt32(to);

                        var f = ctx.tbl_Payment_Types.FirstOrDefault(q => q.PaymentTypeId == v1);
                        var t = ctx.tbl_Payment_Types.FirstOrDefault(q => q.PaymentTypeId == v2);

                        from = (f == null) ? "N/A" : f.PaymentType;
                        to = (t == null) ? "N/A" : t.PaymentType;
                    }

                    string changedby = "";
                    try
                    {
                        changedby = Membership.GetUser(new Guid(jobhistorydata[i].UserKey)).UserName;
                    }
                    catch (Exception)
                    {
                        changedby = "N/A";
                    }

                    jobhistorylist.Add(new JobHistoryInfo
                    {
                        FieldName = jobhistorydata[i].Attribute,
                        TableName = jobhistorydata[i].EntityType,
                        ChangeType = jobhistorydata[i].Type,
                        Date = jobhistorydata[i].AuditDate.ToShortDateString(),
                        Time = jobhistorydata[i].AuditDate.ToShortTimeString(),
                        isTablet = "No",
                        ChangedBy = changedby,
                        From = from,
                        To = to
                    });
                }

                return Json(jobhistorylist);
            }
        }

    }
}
