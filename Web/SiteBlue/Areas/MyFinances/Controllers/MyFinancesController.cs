using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Areas.SecurityGuard.Models;
using System.Drawing;
using SiteBlue.Controllers;
using System.IO;
using SiteBlue.Data.EightHundred;
using System.Threading;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using SiteBlue.Areas.MyFinances.Models;
using System.Resources;
using System.Reflection;
using SiteBlue.Business;
using System.Web.Routing;
using System.Text;

using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.BusinessSolutions.SmallBusinessAccounting;
using Interop.QBFC10;
using DHTMLX.Export.Excel;
using OpenExcel.OfficeOpenXml;
using System.Xml;
using System.Data;

//using Microsoft.BusinessSolutions.SmallBusinessAccounting.Loader;


namespace SiteBlue.Areas.MyFinances.Controllers
{
    [Authorize]
    public class MyFinancesController : SiteBlueBaseController
    {
        private DateTime startdate = DateTime.Now.AddDays(-7);
        private DateTime enddate = DateTime.Now;
        //private bool FormIsLoading = true;
        decimal MyJobTotal;
        int MyJobCount;
        decimal MyJobAvg;
        decimal MyJobRes;
        Int32 MyJobResCount;
        decimal MyJobResPercent;
        decimal MyJobCom;
        Int32 MyJobComCount;
        decimal MyJobComPercent;
        private string LoadScreen;


        //public  ISbaObjects sbaObject;
        internal ISmallBusinessInstance smallBusinessInstance = default(ISmallBusinessInstance);

        /// <summary>
        /// Account Adjustment
        /// </summary>

        //string gbZeeAcctType = "MSA";
        internal bool SBASuccess = false;
        //internal ResourceManager isvResources;
        const string resourceFILE = "800Plumber.SBAResources";
        private QBSessionManager sessMgr;
        public short gbQBMasterVersion = 10;
        string gbQBFileName = "";
        string gbQBAppName = "";
        string gbQBJournalRefNumber = "";
        string gbQBCustomerFullName = "";
        string gbQBJournalMemoPrefix = "";
        string gbQBARAcct = "Accounts Receivable";
        string gbQBJournalSalesMemoPrefix = "";

        EightHundredEntities db = new EightHundredEntities();
        private MembershipConnection memberShipContext = new MembershipConnection();

        public ActionResult Index(FormCollection formcollection)
        {
            return View();
        }
        public string getAge(DateTime closedDate)
        {

            if (DateTime.Compare(DateTime.Today, closedDate) > 0)
                return "Current";
            else if (DateTime.Compare(DateTime.Today.AddDays(-60), closedDate) > 0)
                return "30-60";
            else if (DateTime.Compare(DateTime.Today.AddDays(-90), closedDate) > 0)
                return "60-90";
            else
                return "Over 90";
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadMyFinances()
        {
            int k = 0;
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);

            //
            XmlElement root;
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(xml);
            root = dom.DocumentElement;

            var table = new DataTable();
            DataColumn column;
            DataRow row;

            var columns = root.GetElementsByTagName("column");
            for (int i = 0; i < columns.Count; i++)
            {
                column = new DataColumn();
                column.ColumnName = columns[i].InnerText;
                table.Columns.Add(column);
            }

            var rows = root.GetElementsByTagName("row");
            for (int i = 0; i < rows.Count; i++)
            {
                var cells = rows[i].ChildNodes;
                row = table.NewRow();
                for (int j = 0; j < cells.Count; j++)
                {
                    row[j] = cells[j].InnerText;
                }
                table.Rows.Add(row);
            }

            var sb = new System.Text.StringBuilder();
            sb.Append("<table border='" + "2px" + "'b>");
            // write column headings
            sb.Append("<tr >");
            foreach (DataColumn col in table.Columns)
            {
                sb.Append("<td bgcolor =#E3EFFF ><b>" + col.ColumnName + "</b></td>");
                //sb.Append("<td><b><font face=Arial size=2>" + col.ColumnName + "</font></b></td>");
            }
            sb.Append("</tr>");

            // write table data
            string bgcolor = "";
            foreach (DataRow dr in table.Rows)
            {
                if (k % 2 == 0)
                {
                    bgcolor = "white";
                }
                else
                {
                    bgcolor = "#E3EFFF";

                }
                sb.Append("<tr >");
                foreach (DataColumn col in table.Columns)
                {
                    sb.Append("<td bgcolor=" + bgcolor + ">" + dr[col].ToString() + "</td>");
                    //sb.Append("<td><font face=Arial size=14px>" + dr[col].ToString() + "</font></td>");
                }
                sb.Append("</tr>");
                k++;
            }
            sb.Append("</table>");

            //this.Response.AddHeader("Content-Disposition", "MyFinances.xls");
            //this.Response.ContentType = "application/vnd.ms-excel";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(buffer, generator.ContentType, "MyFinances.xls");

            //

            //var stream = generator.Generate(xml);
            //return File(stream.ToArray(), generator.ContentType, "MyFinances.xlsx");
        }





        //[HttpPost, ValidateInput(false)]
        //public ActionResult DownloadMyFinances()
        //{
        //    var generator = new ExcelWriter();
        //    var xml = this.Request.Form["grid_xml"];
        //    xml = this.Server.UrlDecode(xml);
        //    MemoryStream stream = generator.Generate(xml);
        //    return File(stream.ToArray(), generator.ContentType, "MyFinances.xlsx");
        //}

        public ActionResult databyCompanyCode(string code)
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

                var lastDeposit = (from p in db.tbl_Accounting_DepositSlipHistory where p.FranchiseID == FranchiseID orderby p.DepositSlipID descending select p).FirstOrDefault();
                if (lastDeposit != null)
                {
                    ViewBag.clientname = lastDeposit.PerformedBy;
                    ViewBag.lastDepositDate = lastDeposit.DepositSlipDate;
                }
                return PartialView("BankingDeposits", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult UserCompanyCode()
        {
            IMembershipService membershipService;
            IAuthenticationService authenticationService;
            membershipService = new MembershipService(Membership.Provider);
            authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());

            MembershipUser user = membershipService.GetUser(User.Identity.Name);
            var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            var isCorporate = User.IsInRole("Corporate");
            string username = user.UserName;
            int[] assignedFranchises;

            var DefaultCompamyName = default(String);
            var DefaultCompanyID = default(int);

            DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                  where g.FranchiseID == 56 && g.UserId == userId
                                  select g.Franchise.FranchiseNumber).FirstOrDefault();
            if (DefaultCompamyName != null)
            {
                DefaultCompamyName = (from g in db.tbl_Franchise
                                      where g.FranchiseNUmber == DefaultCompamyName
                                      select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
            }
            if (DefaultCompamyName == null)
            {
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();

                DefaultCompamyName = (from g in db.tbl_Franchise
                                      where g.FranchiseNUmber == DefaultCompamyName
                                      select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
            }

            DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                where g.FranchiseID == 56 && g.UserId == userId
                                select g.Franchise.FranchiseID).FirstOrDefault();
            if (DefaultCompanyID == 0)
            {
                DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                    where g.UserId == userId
                                    select g.Franchise.FranchiseID).FirstOrDefault();
            }


            if (RouteData.Values["id"] != null)
            {
                int companyCodeID = int.Parse(Convert.ToString(RouteData.Values["id"]));
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == companyCodeID && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                    where g.FranchiseID == companyCodeID && g.UserId == userId
                                    select g.Franchise.FranchiseID).FirstOrDefault();
            }

            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == userId)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }

            var model = new GrantCompaniesToUser
            {
                UserName = username,
                GrantedCompanyCode =
                db.tbl_Franchise
                               .Where(f => assignedFranchises.Contains(f.FranchiseID))
                               .OrderBy(f => f.FranchiseNUmber)
                               .Select(d => new SelectListItem
                               {
                                   Text = string.Concat(d.LegalName, " - ", d.FranchiseNUmber),
                                   Value = System.Data.Objects.SqlClient.SqlFunctions.StringConvert((double)d.FranchiseID)
                               })
                               .ToList(),
                defaultCompanyName = DefaultCompamyName,
                defaultCompanyID = DefaultCompanyID
            };
            return PartialView("CompanyCodeUser", model);

        }

        public ActionResult HeaderData()
        {
            List<Technician> reportTechnicians = new List<Technician>();
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
                var FranchiseeList = db.tbl_Franchise
                                   .Where(f => assignedFranchises.Contains(f.FranchiseID) == true)
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { FranchiseID = d.FranchiseID, FranchiseNumber = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToList();

                ViewBag.frenchise = FranchiseeList;

                ViewBag.FranchiseeId = FranchiseeList[0].FranchiseID;
                ViewBag.FranchiseeNumber = FranchiseeList[0].FranchiseNumber;


            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return PartialView("Header");
        }

        public ActionResult BankingDeposits()
        {
            ViewBag.HistoryDeposit = "false";
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
        public ActionResult Next(int frid, string DepositID, string myHistoryDepoist)
        {
            int myDepositID = Convert.ToInt32(DepositID);
            int flag = 0;
            List<tbl_Deposits> depositlist;
            if (myHistoryDepoist == "true")
            {
                depositlist = (from d in db.tbl_Deposits where d.DepositID > myDepositID && d.FranchiseID == frid orderby d.DepositID select d).ToList();
                foreach (var item in depositlist)
                {
                    myHistoryDepoist = "true";
                    myDepositID = item.DepositID;
                    flag = 1;
                    break;
                }
            }
            else
            {
                depositlist = (from d in db.tbl_Deposits where d.FranchiseID == frid orderby d.DepositID descending select d).ToList();
                foreach (var rec in depositlist)
                {
                    myHistoryDepoist = "true";
                    myDepositID = rec.DepositID;
                    flag = 1;
                    break;
                }
            }
            return Json(new { XMLDepositList = depositlist, historyDeposit = myHistoryDepoist, MyDepositID = myDepositID, BindGrid = flag });
        }
        public ActionResult Previous(int frid, string DepositID, string myHistoryDepoist)
        {
            int myDepositID = Convert.ToInt32(DepositID);
            int flag = 0;
            List<tbl_Deposits> depositlist;
            if (myHistoryDepoist == "true")
            {
                depositlist = (from d in db.tbl_Deposits where d.DepositID < myDepositID && d.FranchiseID == frid orderby d.DepositID descending select d).ToList();
                foreach (var item in depositlist)
                {
                    myHistoryDepoist = "true";
                    myDepositID = item.DepositID;
                    flag = 1;
                    break;
                }
            }
            else
            {
                depositlist = (from d in db.tbl_Deposits where d.FranchiseID == frid orderby d.DepositID descending select d).ToList();
                foreach (var rec in depositlist)
                {
                    myHistoryDepoist = "true";
                    myDepositID = rec.DepositID;
                    flag = 1;
                    break;
                }
            }
            return Json(new { XMLDepositList = depositlist, historyDeposit = myHistoryDepoist, MyDepositID = myDepositID, BindGrid = flag });
        }

        public ActionResult BankingDepositsData(int frId, string depositID, string historyDeposit)
        {
            int myDepositID = Convert.ToInt32(depositID);
            if (depositID == null)
                depositID = "0";

            if (historyDeposit == null)
                historyDeposit = "false";

            var paymentlist = (from p in db.tbl_Payments
                               join p1 in db.tbl_Job on p.JobID equals p1.JobID
                               join p2 in db.tbl_Customer on p1.CustomerID equals p2.CustomerID into p2_2
                               join p3 in db.tbl_Locations on p1.LocationID equals p3.LocationID into p3_3
                               join p4 in db.tbl_Deposits on p.DepositID equals p4.DepositID into p4_4
                               join p5 in db.tbl_Payment_Types on p.PaymentTypeID equals p5.PaymentTypeId into p5_5
                               from p2 in p2_2.DefaultIfEmpty()
                               from p3 in p3_3.DefaultIfEmpty()
                               from p4 in p4_4.DefaultIfEmpty()
                               from p5 in p5_5.DefaultIfEmpty()
                               where p.FranchiseID == frId
                               select new
                               {
                                   p.DepositID,
                                   p.PaymentID,
                                   p4.DepositNotes,
                                   p1.CustomerID,
                                   p.JobID,
                                   p2.CustomerName,
                                   p3.Address,
                                   p.DepositStatus,
                                   p4.DepositDate,
                                   p4.DepositNumber,
                                   p.PaymentAmount,
                                   p5.PaymentType
                               });

            if (historyDeposit == "true")
                paymentlist = (from p in paymentlist where p.DepositID == myDepositID select p);
            else
                paymentlist = (from p in paymentlist where p.DepositID == 0 select p);

            var total = string.Format("{0:C}", paymentlist.Sum(q => q.PaymentAmount ?? 0));

            return Json(new { paymentlist = paymentlist.ToList(), total = total });
        }

        public ActionResult CloseDeposit(string[] paymentids, tbl_Deposits depositdata)
        {
            if (depositdata == null)
                return Json("0");

            depositdata.CreateDate = DateTime.Now;
            db.tbl_Deposits.AddObject(depositdata);
            db.SaveChanges();

            int myDepositID = depositdata.DepositID;
            for (int i = 0; i < paymentids.Count(); i++)
            {
                int ID = Convert.ToInt32(paymentids[i]);
                var mypayment = (from p in db.tbl_Payments where p.PaymentID == ID select p).FirstOrDefault();
                mypayment.DepositStatus = true;
                mypayment.DepositID = myDepositID;
            }
            db.SaveChanges();

            return Json(myDepositID.ToString());
        }

        public ActionResult AccountingExport()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View();


        }
        public PartialViewResult AccExport(string code)
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



                return PartialView("AccExport", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult WeeklySalesReport()
        {
            //List<Technician> reportTechnicians = new List<Technician>();
            //if (User.Identity.Name != "")
            //{
            //    var membership = new MembershipService(Membership.Provider);
            //    var user = membership.GetUser(User.Identity.Name);
            //    var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            //    var isCorporate = User.IsInRole("Corporate");
            //    int[] assignedFranchises;
            //    DefaultCompamyName = (from g in memberShipContext.UserFranchise
            //                          where g.FranchiseID == 56 && g.UserId == userId
            //                          select g.Franchise.FranchiseNumber).FirstOrDefault();
            //    if (DefaultCompamyName == null)
            //    {
            //        DefaultCompamyName = (from g in memberShipContext.UserFranchise
            //                              where g.UserId == userId
            //                              select g.Franchise.FranchiseNumber).FirstOrDefault();
            //    }
            //    using (var ctx = new MembershipConnection())
            //    {
            //        assignedFranchises = ctx.UserFranchise
            //                                .Where(uf => uf.UserId == userId)
            //                                .Select(f => f.FranchiseID)
            //                                .ToArray();
            //    }

            //    var franchises = db.tbl_Franchise
            //                       .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
            //                       .OrderBy(f => f.FranchiseNUmber)
            //                       .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
            //                       .ToArray();


            //    ViewBag.frenchise = franchises;

            //    if (Session["code"] == null)
            //    {
            //        ViewBag.code = DefaultCompamyName;
            //        Session["code"] = null;
            //    }
            //}
            //else
            //{

            //    return new RedirectResult("/SGAccount/LogOn");

            //}
            DateTime currentday = DateTime.Now;
            DateTime WSRDate;

            string dayofweek = currentday.DayOfWeek.ToString();
            switch (dayofweek)
            {
                case "Sunday":
                    WSRDate = currentday.AddDays(-1);
                    ViewBag.WeekDate = WSRDate.ToShortDateString();
                    break;
                case "Monday":
                    WSRDate = currentday.AddDays(-2);
                    ViewBag.WeekDate = WSRDate.ToShortDateString();
                    break;
                case "Tuesday":
                    WSRDate = currentday.AddDays(-3);
                    ViewBag.WeekDate = WSRDate.ToShortDateString();
                    break;
                case "Wednesday":
                    WSRDate = currentday.AddDays(-4);
                    ViewBag.WeekDate = WSRDate.ToShortDateString();
                    break;
                case "Thursday":
                    WSRDate = currentday.AddDays(-5);
                    ViewBag.WeekDate = WSRDate.ToShortDateString();
                    break;
                case "Friday":
                    WSRDate = currentday.AddDays(-6);
                    ViewBag.WeekDate = WSRDate.ToShortDateString();
                    break;
                case "Saturday":
                    WSRDate = currentday.AddDays(-7);
                    ViewBag.WeekDate = WSRDate.ToShortDateString();
                    break;
            }

            return View();


        }
        public PartialViewResult WeekSalReport(string code)
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




                return PartialView("WeekSalReport", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult WeekSalReportData(DateTime WeekDate, int frId)
        {
            DateTime strstartdate = WeekDate.AddDays(-7);

            var HistList = (from H in db.tbl_ACH_Franchisees_Summary
                            where H.FranchiseID == frId
                            && H.WeekEnding >= strstartdate && H.WeekEnding <= WeekDate
                            orderby H.WeekEnding descending
                            select new
                            {
                                WSRDate = H.WeekEnding,
                                TotalSales = H.TotalSales,
                                TotalRoyalty = H.TotalFees,
                                RoyaltyFees = H.TotalRoyalty,
                                MGMTFees = H.TotalManagement,
                                TECHFees = H.TotalTechnology,
                                MRKTFees = H.TotalMarketing,
                                RoyaltyAdj = H.RoyaltyAdjustment,
                                MGMTAdj = H.ManagementAdjustment,
                                TECHAdj = H.TechnologyAdjustment,
                                MRKTAdj = H.MarketingAdjustment
                            });
            return Json(HistList);
        }

        public ActionResult WeeklyClosing()
        {

            if (User.Identity.Name != "")
            {

            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View();


        }
        public PartialViewResult WeekClosing(string code)
        {
            try
            {
                int FranchiseID = 0;
                if (code == null)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var franchise = UserInfo.CurrentFranchise ??
                                        UserInfo.DefaultFranchise ?? UserInfo.Franchises.FirstOrDefault();

                        FranchiseID = franchise == null ? 56 : franchise.FranchiseID;
                    }
                }
                else
                {
                    if (code.LastIndexOf("-") > 0)
                    {
                        code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                    }

                    FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();

                }

                return PartialView("WeekClosing", FranchiseID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CloseWeek(string[] jobids, int Frid)
        {
            var FranchiseID = Frid;
            List<int> jobIDes = new List<int>();
            var objmodcommon = new mod_common(UserInfo.UserKey);

            for (int i = 0; i < jobids.Length; i++)
            {

                int val;
                if (jobids[i].LastIndexOf("=") > 0)
                {
                    val = Convert.ToInt32(jobids[i].Substring(jobids[i].LastIndexOf("=") + 1));
                }
                else
                {
                    val = Convert.ToInt32(jobids[i]);
                }

                jobIDes.Add(val);
            }


            DateTime? WSRDate = DateTime.Now;

            //if (this.lv_Invoicing.Items.Count > 0) {
            var wsrlist2 = from w in db.tbl_WSR where w.FranchiseID == FranchiseID select w;
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
                var Closedjoblist = (from j in db.tbl_Job
                                     join s in db.tbl_Job_Status on j.StatusID equals s.StatusID
                                     where s.Status == "Closed" && j.FranchiseID == FranchiseID && j.WSRCompletedDate == null && jobIDes.Contains(j.JobID)
                                     select j).ToList();
                //Closedjoblist = Closedjoblist.Where(p => jobIDes.Contains(p.JobID.ToString())).ToList();
                int jobid;
                tbl_Job job = new tbl_Job();
                while (i < Closedjoblist.Count())
                {
                    //int jobid = this.lv_Invoicing.Items(i).SubItems(0).Text;
                    jobid = Closedjoblist[i].JobID;
                    //job = objmodcommon.getJob(jobid);
                    job = (from j in db.tbl_Job where j.JobID == jobid select j).FirstOrDefault();
                    DateTime? callcompleted = job.CallCompleted;
                    if (objmodcommon.DateDiff(mod_common.DateInterval.Day, callcompleted, WSRDate) >= 0)
                    {
                        job.WSRCompletedDate = WSRDate;
                        job.LockedYN = true;
                        job.InvoiceNumber = Convert.ToString(job.JobID);

                        var objprice = (from j in db.tbl_Job
                                        join t in db.tbl_Job_Tasks on j.JobID equals t.JobID
                                        join tp in db.tbl_Job_Task_Parts on t.JobTaskID equals tp.JobTaskID
                                        where j.JobID == jobid
                                        select new { t.Price, t.Quantity })
                                        .Distinct();

                        decimal total = 0;
                        foreach (var item in objprice)
                        {
                            decimal dtotal = item.Price * item.Quantity;
                            total = total + dtotal;
                        }

                        var Paymentlist = (from p in db.tbl_Payments where p.JobID == job.JobID select p);
                        decimal totalpayment = 0;
                        foreach (var payment in Paymentlist)
                        {
                            totalpayment = (Convert.ToDecimal(totalpayment) + Convert.ToDecimal(payment.PaymentAmount));
                        }

                        job.SubTotal = total;
                        job.TotalSales = total + job.TaxAmount;
                        job.Balance = job.TotalSales - totalpayment;

                        var wsrlist = (from w in db.tbl_WSR where w.FranchiseID == FranchiseID select w);
                        if (wsrlist.Count() == 0)
                        {
                            tbl_WSR rec = new tbl_WSR();
                            rec.FranchiseID = FranchiseID;
                            rec.LastWSRDate = WSRDate;
                            db.tbl_WSR.AddObject(rec);
                            db.SaveChanges();
                        }
                        else
                        {
                            foreach (var wsr_loopVariable in wsrlist)
                            {

                                wsr_loopVariable.LastWSRDate = WSRDate;

                            }
                            //db.SaveChanges();
                        }
                        int ival = db.SaveChanges();
                    }
                    i += 1;
                }

                tbl_Accounting_WeeklyLockHistory objAWL = new tbl_Accounting_WeeklyLockHistory();
                IMembershipService membershipService;
                IAuthenticationService authenticationService;
                membershipService = new MembershipService(Membership.Provider);
                authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());

                MembershipUser user = membershipService.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                string username = "";
                if (user != null)
                    username = user.UserName;


                objAWL.WeeklyLockID = (from w in db.tbl_Accounting_WeeklyLockHistory orderby w.WeeklyLockID descending select w.WeeklyLockID).Take(1).Single() + 1;
                objAWL.WeeklyLockDate = WSRDate.Value;
                objAWL.FranchiseID = FranchiseID;
                objAWL.PerformedBy = username;
                db.tbl_Accounting_WeeklyLockHistory.AddObject(objAWL);
                db.SaveChanges();


            }
            else
            {
                ViewBag.lblmessage = "Can't create Weekly Sales Report for " + WSRDate + ".  This week already exists.";
            }



        }

        public ActionResult Getweeklyclosing(string franchisid)
        {
            int fid = Convert.ToInt32(franchisid);
            var objmod_common = new mod_common(UserInfo.UserKey);
            Details detail = new Details();
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            var objmodcommon = new mod_common(UserInfo.UserKey);
            var billto = "";
            var joblocation = "";
            var status = "";
            var Tech = "";
            var Jobamt = 0.0;
            var balance = 0.0;
            var comments = "";
            var xmlstring = "";
            var lastdate = "";
            var performby = "";
            try
            {


                var accountinweeklylockhistory = (from a in db.tbl_Accounting_WeeklyLockHistory
                                                  where a.FranchiseID == fid
                                                  orderby a.WeeklyLockID
                                                  descending
                                                  select a).Take(1).FirstOrDefault();

                if (accountinweeklylockhistory != null)
                {
                    lastdate = accountinweeklylockhistory.WeeklyLockDate.ToShortDateString();
                    performby = accountinweeklylockhistory.PerformedBy.ToString();
                }
                else
                {
                    lastdate = "";
                    performby = "";
                }
                var joblist = (from j in db.tbl_Job
                               join s in db.tbl_Job_Status on j.StatusID equals s.StatusID
                               join p1 in db.tbl_Services on j.ServiceID equals p1.ServiceID into p1_1
                               join p2 in db.tbl_Customer on j.CustomerID equals p2.CustomerID into p2_2
                               join p3 in db.tbl_Locations on j.LocationID equals p3.LocationID into p3_3
                               join p4 in db.tbl_Job_Status on j.StatusID equals p4.StatusID into p4_4
                               join p5 in db.tbl_Employee on j.ServiceProID equals p5.EmployeeID into p5_5
                               from p1 in p1_1.DefaultIfEmpty()
                               from p2 in p2_2.DefaultIfEmpty()
                               from p3 in p3_3.DefaultIfEmpty()
                               from p4 in p4_4.DefaultIfEmpty()
                               from p5 in p5_5.DefaultIfEmpty()
                               where j.FranchiseID == fid && j.WSRCompletedDate == null && s.Status == "Closed"
                               select new
                               {
                                   j.JobID,
                                   j.SubTotal,
                                   j.Balance,
                                   j.CustomerComments,
                                   j.InvoicedDate,
                                   j.CallCompleted,
                                   p1.ServiceName,
                                   billto = p2.CompanyName + "" + p2.CustomerName,
                                   p3.Address,
                                   p4.Status,
                                   p5.Employee,
                                   j.ServiceProID,
                                   j.CustomerID,
                                   j.FranchiseID,
                                   j.tbl_Customer
                               });

                xmlstring = xmlstring + "<rows>";
                if (joblist.Count() > 0)
                {
                    int i = 0;

                    foreach (var job in joblist)
                    {
                        i = i + 1;
                        //var servicetypee = (from s in db.tbl_Services where s.ServiceID == job.ServiceID select new { s.ServiceName }).FirstOrDefault();
                        //var Customer = objmod_common.GetCustomers(job.CustomerID);
                        //if (Customer != null)
                        //{

                        //    billto = textInfo.ToTitleCase(objmod_common.GetCustomerName(Customer).ToString());
                        //}
                        //var location = objmod_common.getLocation(job.LocationID);
                        //if (location != null)
                        //{
                        //    joblocation = location.Address;
                        //}
                        //else
                        //{
                        //    joblocation = "";
                        //}

                        //status = objmod_common.getStatus(job.StatusID);
                        //Tech = objmod_common.Get_Employee_Name(job.ServiceProID);
                        if (job.billto != null && job.billto != "")
                        {
                            billto = textInfo.ToTitleCase(objmodcommon.GetCustomerName(job.tbl_Customer)).ToString();
                        }
                        joblocation = job.Address;
                        status = job.Status;
                        Tech = job.Employee;
                        Jobamt = (double)job.SubTotal;
                        balance = (double)job.Balance;
                        comments = job.CustomerComments;
                        detail.ClosedDate = job.InvoicedDate;
                        detail.CompletedDate = job.CallCompleted;

                        if (detail.ClosedDate != null) { detail.shortdatestring = detail.ClosedDate.Value.ToShortDateString(); } else { detail.shortdatestring = ""; }
                        if (detail.CompletedDate != null) { detail.completedshortdatestring = detail.CompletedDate.Value.ToShortDateString(); } else { detail.completedshortdatestring = ""; }


                        xmlstring = xmlstring + "<row id='" + job.JobID + "'>";
                        xmlstring = xmlstring + "<cell><![CDATA[" + "" + "]]></cell>";
                        xmlstring = xmlstring + "<cell><![CDATA[" + job.JobID + "^../../OwnerPortal/Invoice/InvoiceDetails?JobId=" + job.JobID + "]]></cell>";


                        xmlstring = xmlstring + "<cell><![CDATA[" + billto + "^../../OwnerPortal/AllCustomers/CustomerInformation?Custid=" + job.CustomerID + "]]></cell>";

                        xmlstring = xmlstring + "<cell><![CDATA[" + joblocation + "]]></cell>";
                        //xmlstring = xmlstring + "<cell><![CDATA[" + objmod_common.getStatus(job.StatusID) + "]]></cell>";
                        xmlstring = xmlstring + "<cell><![CDATA[" + status + "]]></cell>";

                        xmlstring = xmlstring + "<cell><![CDATA[" + detail.completedshortdatestring + "]]></cell>";
                        xmlstring = xmlstring + "<cell><![CDATA[" + detail.shortdatestring + "]]></cell>";

                        //xmlstring = xmlstring + "<cell><![CDATA[" + Tech + "^../../OwnerPortal/Employee/EmployeeInformation?id=" + job.tbl_Employee.EmployeeID + "]]></cell>";
                        xmlstring = xmlstring + "<cell><![CDATA[" + Tech + "^../../OwnerPortal/Employee/EmployeeInformation?id=" + job.ServiceProID + "&frid=" + job.FranchiseID + "]]></cell>";

                        xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", Jobamt) + "]]></cell>";
                        if (balance > 0)
                        {
                            xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", balance) + "^../../OwnerPortal/Payment/Payment?jobid=" + job.JobID + "]]></cell>";
                        }
                        else
                        {
                            xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", balance) + "]]></cell>";
                        }
                        //if (servicetypee != null) { xmlstring = xmlstring + "<cell><![CDATA[" + servicetypee.ServiceName + "]]></cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[</cell>"; }
                        if (job.ServiceName != null) { xmlstring = xmlstring + "<cell><![CDATA[" + job.ServiceName + "]]></cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[]]></cell>"; }

                        if (comments != null) { xmlstring = xmlstring + "<cell><![CDATA[" + comments + "]]></cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[]]></cell>"; }
                        xmlstring = xmlstring + "</row>";

                    }




                }
                xmlstring = xmlstring + "</rows>";
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            return Json(new { strxml = xmlstring, lastdt = lastdate, perform = performby });

            //return Json(xmlstring);

        }



        public ActionResult SalesTaxReport()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View();


        }
        public PartialViewResult SalesReport(string code)
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



                return PartialView("SalesReport", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ARAgingSummary()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View();
        }
        public PartialViewResult PartialARAgingSummary(string code)
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



                return PartialView("PartialARAgingSummary", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult SalesTaxLiabilityReport()
        {

            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View();
        }

        public ActionResult TaxSaleReportData(int franchisid)
        {

            string xmlstring = "";
            int i = 0;

            xmlstring = xmlstring + "<rows>";

            var XmlData = (from b in db.tbl_Employee
                           join a in db.tbl_Job on b.EmployeeID equals a.ServiceProID
                           where b.City != "" && a.FranchiseID == franchisid
                           //group b by new { city = b.City} into g
                           select new { b.City, a.TotalSales, a.TaxAmount, a.SubTotal, a.TaxLaborPercentage, a.TaxPartPercentage }).Distinct();
            //select new{ qty = g.Sum(o => o.City)});
            var d = (from X in XmlData
                     group X by new { city = X.City } into g
                     select new
                     {
                         city = g.Key.city,
                         totalsales = g.Sum(p => p.TotalSales),
                         taxamount = g.Sum(p => p.TaxAmount),
                         subtotal = g.Sum(p => p.SubTotal),
                         taxlaborpercent = g.Sum(p => p.TaxLaborPercentage),
                         partpercent = g.Sum(p => p.TaxPartPercentage)

                     });
            decimal Total_Sales = 0;
            decimal Non_TaxableSales = 0;
            decimal TaxableSales = 0;
            decimal TaxCollected = 0;
            foreach (var item in d)
            {
                xmlstring = xmlstring + "<row id='" + i + "'>";
                xmlstring = xmlstring + "<cell><![CDATA[" + item.city + "]]></cell>";
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", item.totalsales) + "]]></cell>";
                Total_Sales += item.totalsales;
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", item.taxamount) + "]]></cell>";
                Non_TaxableSales += item.taxamount;
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", item.subtotal) + "]]></cell>";
                TaxableSales += item.subtotal;
                if (item.taxlaborpercent != 0)
                {
                    xmlstring = xmlstring + "<cell><![CDATA[" + item.taxlaborpercent + "%]]></cell>";
                }
                else
                {
                    xmlstring = xmlstring + "<cell><![CDATA[" + item.partpercent + "%]]></cell>";
                }
                decimal taxcollected = item.subtotal * Convert.ToDecimal(item.taxlaborpercent) / 100;
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", taxcollected) + "]]></cell>";
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", taxcollected) + "]]></cell>";
                TaxCollected += taxcollected;
                xmlstring = xmlstring + "</row>";
                i++;
            }
            xmlstring = xmlstring + "<row id='" + i + "'>";
            xmlstring = xmlstring + "<cell><![CDATA[<b>Total</b>]]></cell>";
            xmlstring = xmlstring + "<cell><![CDATA[<b>" + string.Format("{0:c}", Total_Sales) + "</b>]]></cell>";
            xmlstring = xmlstring + "<cell><![CDATA[<b>" + string.Format("{0:c}", Non_TaxableSales) + "</b>]]></cell>";
            xmlstring = xmlstring + "<cell><![CDATA[<b>" + string.Format("{0:c}", TaxableSales) + "</b>]]></cell>";
            xmlstring = xmlstring + "<cell></cell>";
            xmlstring = xmlstring + "<cell><![CDATA[<b>" + string.Format("{0:c}", TaxCollected) + "</b>]]></cell>";
            xmlstring = xmlstring + "<cell><![CDATA[<b>" + string.Format("{0:c}", TaxCollected) + "</b>]]></cell>";
            xmlstring = xmlstring + "</row>";
            xmlstring = xmlstring + "</rows>";

            return Json(xmlstring);
        }

        public ActionResult ARAgingSummaryData()
        {
            TimeSpan tcurrent = new TimeSpan(0, 0, 0, 0);
            TimeSpan t1to30 = new TimeSpan(-30, 0, 0, 0);
            TimeSpan t31to60 = new TimeSpan(-60, 0, 0, 0);
            TimeSpan t61to90 = new TimeSpan(-90, 0, 0, 0);
            var franchise = (from f in db.tbl_Franchise
                             join fo in db.tbl_Franchise_Owner on f.OwnerID equals fo.OwnerID
                             select new { f.FranchiseID, fo.OwnerName });

            string xmlstring = "";
            xmlstring = xmlstring + "<rows>";
            int i = 0;
            foreach (var item in franchise)
            {
                var objSubtotal = (from j in db.tbl_Job
                                   where j.InvoicedDate != null
                                   && j.Balance > 0
                                   && j.FranchiseID == item.FranchiseID
                                   select new { j.SubTotal, j.InvoicedDate }).ToList();



                decimal totals = objSubtotal.Sum(p => p.SubTotal);
                decimal icurrent = objSubtotal.Where(p => Convert.ToDateTime(p.InvoicedDate).Date == Convert.ToDateTime(DateTime.Now).Date).Sum(p => p.SubTotal);
                decimal onetothirty = objSubtotal.Where(p => p.InvoicedDate > DateTime.Now.Add(t1to30)).Sum(p => p.SubTotal);
                decimal thirtytosixty = objSubtotal.Where(p => p.InvoicedDate > DateTime.Now.Add(t31to60) && p.InvoicedDate < DateTime.Now.Add(t1to30)).Sum(p => p.SubTotal);
                decimal sixtytoninty = objSubtotal.Where(p => p.InvoicedDate > DateTime.Now.Add(t61to90) && p.InvoicedDate < DateTime.Now.Add(t31to60)).Sum(p => p.SubTotal);
                decimal overninty = objSubtotal.Where(p => p.InvoicedDate < DateTime.Now.Add(t61to90)).Sum(p => p.SubTotal);

                xmlstring = xmlstring + "<row id='" + i + "'>";
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", item.OwnerName) + "]]></cell>";
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", icurrent) + "]]></cell>";
                if (totals > 0) { xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:0.00}", (icurrent / totals) * 100) + "%</cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[0%]]></cell>"; }
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", onetothirty) + "]]></cell>";
                if (totals > 0) { xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:0.00}", (onetothirty / totals) * 100) + "%</cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[0%]]></cell>"; }

                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", thirtytosixty) + "]]></cell>";
                if (totals > 0) { xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:0.00}", (thirtytosixty / totals) * 100) + "%</cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[0%]]></cell>"; }
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", sixtytoninty) + "]]></cell>";
                if (totals > 0) { xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:0.00}", (sixtytoninty / totals) * 100) + "%</cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[0%]]></cell>"; }
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", overninty) + "]]></cell>";
                if (totals > 0) { xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:0.00}", (overninty / totals) * 100) + "%</cell>"; } else { xmlstring = xmlstring + "<cell><![CDATA[0%]]></cell>"; }
                xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:c}", totals) + "]]></cell>";
                xmlstring = xmlstring + "</row>";
                i = i + 1;



            }

            xmlstring = xmlstring + "</rows>";
            //xmlstring = xmlstring;

            return Json(xmlstring);
        }

        public ActionResult AcountingCalendar()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View();


        }

        public PartialViewResult AcctCalendar(string code)
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



                return PartialView("AcctCalendar", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult AccountsReceivableFunction()
        {
            return RedirectToAction("PaymentMethod2");
            //return View();
        }

        public ActionResult SampleInvoice()
        {

            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View();

        }

        public PartialViewResult SampInvoice(string code)
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



                return PartialView("SampInvoice", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult AccountsAdjustments()
        {
            var paymentTypes = (from item in db.tbl_Payment_Types
                                where item.DepositType.ToLower() == "gl entry"
                                select item).ToList();

            if (paymentTypes != null)
            {
                paymentTypes.Insert(0, new tbl_Payment_Types { PaymentTypeId = 0, PaymentType = "Please Select" });
            }

            var model = new AccountAdjustmentViewModel
            {
                JobID = null,
                AdjustmentAmount = null,
                AdjustmentTypeList = new SelectList(paymentTypes, "PaymentTypeId", "PaymentType"),
                Comment = ""
            };

            //if (User.Identity.Name != "")
            //{
            //    List<string> lstaddjustment = new List<string>();
            //    lstaddjustment.Add("");
            //    lstaddjustment.Add("58000 Plumbing Discount/Coupon/Adjustment");
            //    lstaddjustment.Add("59100 Electrical Discount/Coupon/Adjustment");
            //    lstaddjustment.Add("59000 HVAC      Discount/Coupon/Adjustment");
            //    lstaddjustment.Add("65900 WriteOff");

            //    ViewBag.AdjustmentAccount = lstaddjustment;
            //}
            //else
            //{
            //    return new RedirectResult("/SGAccount/LogOn");
            //}
            return View(model);
        }

        public PartialViewResult AcctAdjustments(string code)
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



                return PartialView("AcctAdjustments", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult BindOpenBalances(int FranchiseeId)
        {
            var Objmodcommon = new mod_common(UserInfo.UserKey);
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            var objmodcommon = new mod_common(UserInfo.UserKey);

            var billto = "";
            var xmlstring = "";
            var joblocation = "";
            var age = "";
            var closedate = "";
            var Tech = "";
            //int techID = 0;
            var phone = "";
            var totalbalance = 0m;
            decimal bal1 = -0.01m;
            decimal bal2 = 0.01m;

            var joblist = (from j in db.tbl_Job

                           join p3 in db.tbl_Locations on j.LocationID equals p3.LocationID into p3_3
                           join p5 in db.tbl_Employee on j.ServiceProID equals p5.EmployeeID into p5_5

                           from p3 in p3_3.DefaultIfEmpty()
                           from p5 in p5_5.DefaultIfEmpty()
                           where !(j.Balance > bal1 && j.Balance < bal2) && j.InvoicedDate != null && j.FranchiseID == FranchiseeId
                           select new
                           {
                               j.JobID,
                               j.CustomerID,
                               j.LocationID,
                               j.InvoicedDate,
                               j.TotalSales,
                               j.Balance,
                               j.ServiceProID,
                               j.tbl_Customer,
                               p3.Address,
                               p5.Employee,
                               j.tbl_Employee,
                               j.FranchiseID

                           }).Distinct();
            xmlstring = xmlstring + "<rows>";
            if (joblist.Count() > 0)
            {

                foreach (var job in joblist)
                {
                    //var Customer = Objmodcommon.GetCustomers(job.CustomerID);
                    //var location = Objmodcommon.getLocation(job.LocationID);
                    //var techlist = (from t in db.tbl_Job_Technicians where t.JobID == job.JobID && t.PrimaryYN == true select new { t.ServiceProID });

                    //var contactlist = (from c in db.tbl_Contacts
                    //                   join p in db.tbl_PhoneType on c.PhoneTypeID equals p.PhoneTypeID into righttableresults
                    //                   from p in righttableresults.DefaultIfEmpty()
                    //                   where (c.CustomerID == job.CustomerID && p.PhoneType == "Primary")
                    //                   select new { c.PhoneNumber });

                    phone = (from c in db.tbl_Contacts
                             join p in db.tbl_PhoneType on c.PhoneTypeID equals p.PhoneTypeID into righttableresults
                             from p in righttableresults.DefaultIfEmpty()
                             where (c.CustomerID == job.CustomerID && p.PhoneType == "Primary")
                             orderby c.PhoneTypeID descending
                             select new { c.PhoneNumber }).FirstOrDefault().PhoneNumber;

                    if (job.tbl_Customer != null)
                    {
                        billto = textInfo.ToTitleCase(Objmodcommon.GetCustomerName(job.tbl_Customer).ToString());
                    }

                    //if (location != null)
                    //{
                    //    joblocation = location.Address;
                    //}
                    if (job.Address != null && job.Address != "")
                    {
                        joblocation = job.Address;
                    }
                    else
                    {
                        joblocation = "";
                    }
                    if (job.InvoicedDate != null)
                    {
                        age = objmodcommon.getAge(job.InvoicedDate);
                        closedate = job.InvoicedDate.Value.ToShortDateString();
                    }
                    else
                    {
                        age = "";
                        closedate = "";
                    }

                    //foreach (var Techrec in techlist)
                    //{
                    //    techID = Techrec.ServiceProID;
                    //}
                    //Tech = objmodcommon.Get_Employee_Name(techID);
                    Tech = job.Employee;
                    //foreach (var contactrec in contactlist)
                    //{
                    //    phone =  contactrec.PhoneNumber;
                    //}


                    xmlstring = xmlstring + "<row id='" + job.JobID + "'>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + " " + "]]></cell>";

                    xmlstring = xmlstring + "<cell><![CDATA[" + job.JobID + "^../../OwnerPortal/Invoice/InvoiceDetails?JobId=" + job.JobID + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + billto + "^../../OwnerPortal/AllCustomers/CustomerInformation?Custid=" + job.CustomerID + "]]></cell>";

                    xmlstring = xmlstring + "<cell><![CDATA[" + joblocation + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + age + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + closedate + "]]></cell>";
                    //xmlstring = xmlstring + "<cell><![CDATA[" + Tech + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + Tech + "^../../OwnerPortal/Employee/EmployeeInformation?id=" + job.ServiceProID + "&frid=" + job.FranchiseID + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", job.TotalSales) + "]]></cell>";

                    //xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", job.Balance) + "]]></cell>";

                    xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", job.Balance) + "^../../OwnerPortal/Payment/Payment?jobid=" + job.JobID + "]]></cell>";

                    //if (job.Balance > 0)
                    //{
                    //    xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", job.Balance) + "^../../OwnerPortal/Payment/Payment?jobid=" + job.JobID + "]]></cell>";
                    //}
                    //else
                    //{
                    //    xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", job.Balance) + "]]></cell>";
                    //}

                    xmlstring = xmlstring + "<cell><![CDATA[" + objmodcommon.Format_PhoneNumber(phone) + "]]></cell>";
                    xmlstring = xmlstring + "</row>";
                    totalbalance = totalbalance + job.Balance;
                }
            }
            xmlstring = xmlstring + "</rows>";

            return Json(new { strxml = xmlstring, total = string.Format("{0:C}", totalbalance) });
            //return Json(xmlstring);
        }

        public ActionResult OpenBalances()
        {
            if (User.Identity.Name != "")
            {

            }
            else
            {
                return new RedirectResult("/SGAccount/LogOn");
            }
            return View();
        }

        public PartialViewResult OpBalances(string code)
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



                return PartialView("OpBalances", FranchiseID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult WriteOffDownFunction()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                List<string> lstaddjustment = new List<string>();
                lstaddjustment.Add("");
                lstaddjustment.Add("58000 Plumbing Discount/Coupon/Adjustment");
                lstaddjustment.Add("59100 Electrical Discount/Coupon/Adjustment");
                lstaddjustment.Add("59000 HVAC      Discount/Coupon/Adjustment");
                lstaddjustment.Add("65900 WriteOff");

                ViewBag.AdjustmentAccount = lstaddjustment;
            }
            else
            {
                return new RedirectResult("/SGAccount/LogOn");
            }

            return View();
        }

        public PartialViewResult OffDownFunction(string code)
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

                return PartialView("OffDownFunction", FranchiseID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PaymentMethod2()
        {
            var FranchiseeList = (from p in db.tbl_Franchise
                                  join o in db.tbl_Franchise_Owner on p.OwnerID equals o.OwnerID
                                  orderby p.FranchiseNUmber
                                  select new
                                  {
                                      FranchiseID = p.FranchiseID,
                                      FranchiseNumber = p.FranchiseNUmber + " - " + o.LegalName
                                  }).ToList();
            ViewBag.frenchise = FranchiseeList;
            var objFranchisee = (from e in db.tbl_Franchise
                                 join o in db.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
                                 where e.FranchiseID == 56
                                 select e).FirstOrDefault();

            ViewBag.FranchiseeId = objFranchisee.FranchiseID;
            ViewBag.FranchiseeNumber = objFranchisee.FranchiseNUmber + "-" + objFranchisee.LegalName;
            List<string> lstaddjustment = new List<string>();
            lstaddjustment.Add("");
            lstaddjustment.Add("58000 Plumbing Discount/Coupon/Adjustment");
            lstaddjustment.Add("59100 Electrical Discount/Coupon/Adjustment");
            lstaddjustment.Add("59000 HVAC      Discount/Coupon/Adjustment");
            lstaddjustment.Add("65900 WriteOff");

            ViewBag.AdjustmentAccount = lstaddjustment;

            var detailmodel = new PaymentInfo();

            var paymenttypelist = (from pt in db.tbl_Payment_Types select pt).ToList();
            detailmodel.PaymentTypeList = new SelectList(paymenttypelist, "PaymentTypeId", "PaymentType");

            return View(detailmodel);
        }

        public PartialViewResult PayMethod2(string code)
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

                return PartialView("PayMethod2", FranchiseID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ARResult(string franchiseid, string strscreeen)
        {
            decimal balance = 0;
            var objmodcommon = new mod_common(UserInfo.UserKey);

            int fid = 0;
            if (franchiseid != "")
            {
                fid = Convert.ToInt32(franchiseid);
            }

            decimal val1 = -0.01m;
            decimal val2 = 0.01m;

            var ARList = (from a in db.tbl_Job
                          join c in db.tbl_Customer on a.CustomerID equals c.CustomerID into c1
                          from c in c1.DefaultIfEmpty()
                          join l in db.tbl_Locations on a.LocationID equals l.LocationID into c2
                          from l in c2.DefaultIfEmpty()
                          join e in db.tbl_Employee on a.ServiceProID equals e.EmployeeID into c3
                          from e in c3.DefaultIfEmpty()
                          where !(a.Balance > val1 && a.Balance < val2) && a.FranchiseID == fid 
                            && a.InvoicedDate != null && a.ServiceProID != 0 && (a.StatusID == 6 || a.StatusID == 7)
                          select new
                          {
                              e.Employee,
                              e.EmployeeID,
                              l.Address,
                              c.CustomerName,
                              c.CompanyName,
                              a.InvoicedDate,
                              a.JobID,
                              a.CustomerID,
                              a.LocationID,
                              a.TotalSales,
                              a.Balance,
                              a.tbl_Customer,
                              a.ServiceProID,
                              a.FranchiseID
                          });

            string _Type = string.Empty;
            if (strscreeen == "" || strscreeen == null)
                LoadScreen = "All";
            else
                LoadScreen = strscreeen;

            DateTime? dt1;
            DateTime? dt2;
            switch (LoadScreen)
            {
                case "Current":
                    {
                        dt1 = objmodcommon.getCurrentDate();
                        ARList = ARList.Where(p => p.InvoicedDate > dt1);
                    } break;
                case "3060":
                    {
                        dt1 = objmodcommon.getCurrentDate();
                        dt2 = objmodcommon.get60dayDate();
                        ARList = ARList.Where(a => a.InvoicedDate <= dt1 && a.InvoicedDate > dt2);
                    } break;
                case "6090":
                    {
                        dt1 = objmodcommon.get60dayDate();
                        dt2 = objmodcommon.get90dayDate();
                        ARList = ARList.Where(a => a.InvoicedDate <= dt1 && a.InvoicedDate > dt2);
                    } break;
                case "Over90":
                    {
                        dt1 = objmodcommon.get90dayDate();
                        ARList = ARList.Where(a => a.InvoicedDate <= dt1);
                    } break;
                default:
                    {

                    } break;
            }

            var sb = new StringBuilder();
            sb.Append("<rows>");
            foreach (var arrec in ARList)
            {
                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:D}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                string phone = (from c in db.tbl_Contacts
                                join p in db.tbl_PhoneType on c.PhoneTypeID equals p.PhoneTypeID into righttableresults
                                from p in righttableresults.DefaultIfEmpty()
                                where (c.CustomerID == arrec.CustomerID && p.PhoneType == "Primary")
                                select c.PhoneNumber)
                               .FirstOrDefault();

                sb.AppendFormat("<row id='{0}'>", arrec.JobID);
                sb.AppendFormat(formatStr, "");
                sb.AppendFormat(formatStr, arrec.JobID + "^/Ownerportal/invoice/invoicedetails?JobId=" + arrec.JobID);

                if (arrec.tbl_Customer != null)
                    sb.AppendFormat(formatStr, objmodcommon.GetCustomerName(arrec.tbl_Customer) + "^/OwnerPortal/AllCustomers/CustomerInformation?Custid=" + arrec.CustomerID);
                else
                    sb.AppendFormat(formatStr, "");

                sb.AppendFormat(dateFormat, arrec.Address);

                if (arrec.InvoicedDate != null)
                {
                    sb.AppendFormat(formatStr, objmodcommon.getAgeArList(arrec.InvoicedDate));
                    sb.AppendFormat(dateFormat, arrec.InvoicedDate);
                }
                else
                {
                    sb.AppendFormat(formatStr, "");
                    sb.AppendFormat(formatStr, "");
                }

                sb.AppendFormat(formatStr, arrec.Employee + "^/ownerportal/Employee/EmployeeInformation?id=" + arrec.ServiceProID + "&amp;frid=" + arrec.FranchiseID);
                sb.AppendFormat(moneyFormat, arrec.TotalSales);
                sb.AppendFormat(formatStr, string.Format("{0:C}", arrec.Balance) + "^javascript:attachURL(" + arrec.JobID + ")^_self");
                sb.AppendFormat(formatStr, objmodcommon.Format_PhoneNumber(phone));
                sb.Append("</row>");

                decimal remainingBalance = Math.Round(arrec.Balance, 2, MidpointRounding.AwayFromZero);
                balance = balance + remainingBalance;
            }
            sb.Append("</rows>");

            return Json(new { strxml = sb.ToString(), totalbalance = string.Format("{0:C}", balance) });
        }

        public ActionResult PopupPaymentMethod2()
        {

            {
                List<Technician> reportTechnicians = new List<Technician>();
                if (User.Identity.Name != "")
                {
                    var membership = new MembershipService(Membership.Provider);
                    var user = membership.GetUser(User.Identity.Name);
                    var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                    var isCorporate = User.IsInRole("Corporate");
                    int[] assignedFranchises;
                    var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.FranchiseID == 56 && g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                    if (DefaultCompamyName == null)
                    {
                        DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                              where g.UserId == userId
                                              select g.Franchise.FranchiseNumber).FirstOrDefault();
                    }
                    using (var ctx = new MembershipConnection())
                    {
                        assignedFranchises = ctx.UserFranchise
                                                .Where(uf => uf.UserId == userId)
                                                .Select(f => f.FranchiseID)
                                                .ToArray();
                    }

                    var franchises = db.tbl_Franchise
                                       .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                       .OrderBy(f => f.FranchiseNUmber)
                                       .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                       .ToArray();

                    ViewBag.frenchise = franchises;

                    if (Session["code"] == null)
                    {
                        ViewBag.code = DefaultCompamyName;
                        Session["code"] = null;
                    }
                }
                else
                {
                    return new RedirectResult("/SGAccount/LogOn");

                }
                return View();

            }
        }

        public PartialViewResult PopupPayMethod2(string code)
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



                return PartialView("PopupPayMethod2", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult PopupWeeklySalesDetail()
        {
            {
                List<Technician> reportTechnicians = new List<Technician>();
                if (User.Identity.Name != "")
                {
                    var membership = new MembershipService(Membership.Provider);
                    var user = membership.GetUser(User.Identity.Name);
                    var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                    var isCorporate = User.IsInRole("Corporate");
                    int[] assignedFranchises;
                    var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.FranchiseID == 56 && g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                    if (DefaultCompamyName == null)
                    {
                        DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                              where g.UserId == userId
                                              select g.Franchise.FranchiseNumber).FirstOrDefault();
                    }
                    using (var ctx = new MembershipConnection())
                    {
                        assignedFranchises = ctx.UserFranchise
                        .Where(uf => uf.UserId == userId)
                        .Select(f => f.FranchiseID)
                        .ToArray();
                    }

                    var franchises = db.tbl_Franchise
                    .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                    .OrderBy(f => f.FranchiseNUmber)
                    .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                    .ToArray();


                    ViewBag.frenchise = franchises;

                    if (Session["code"] == null)
                    {
                        ViewBag.code = DefaultCompamyName;
                        Session["code"] = null;
                    }
                }
                else
                {

                    return new RedirectResult("/SGAccount/LogOn");

                }
                ViewBag.StartDate = startdate.ToShortDateString();
                ViewBag.EndDate = enddate.ToShortDateString();
                return View();

            }

        }

        public PartialViewResult popupWeeklySalesdet(string code)
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
                var ServiceProList = (from E in db.tbl_Employee where E.FranchiseID == FranchiseID orderby E.Employee select E).ToList();
                ViewBag.ServicePro = ServiceProList;
                ViewBag.StartDate = startdate.ToShortDateString();
                ViewBag.EndDate = enddate.ToShortDateString();
                return PartialView("popupWeeklySalesdet", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void Get_ServicePro_WSR_Totals_Local(int tmpServiceproID, DateTime StartDate, DateTime EndDate)
        {
            int JCnt;
            decimal JTot;
            decimal JAvg;
            MyJobRes = 0;
            MyJobCom = 0;
            MyJobResCount = 0;
            MyJobComCount = 0;
            var tmpTotal = (from J in db.tbl_Job
                            join T in db.tbl_Job_Technicians on J.JobID equals T.JobID
                            where T.ServiceProID == tmpServiceproID
                                && T.PrimaryYN == true
                                && J.WSRCompletedDate >= StartDate
                                && J.WSRCompletedDate <= EndDate
                                && J.StatusID == 7
                            select J);

            JCnt = tmpTotal.Count();
            JTot = 0;
            foreach (var JobTotal in tmpTotal)
            {
                JTot = JTot + JobTotal.SubTotal;
                if (JobTotal.JobTypeID == 1)
                {
                    MyJobResCount = MyJobResCount + 1;
                    MyJobRes = MyJobRes + JobTotal.SubTotal;
                }
                else
                {
                    MyJobComCount = MyJobComCount + 1;
                    MyJobCom = MyJobCom + JobTotal.SubTotal;
                }

            }

            if (JCnt > 0)
                JAvg = JTot / JCnt;
            else
                JAvg = 0;

            MyJobTotal = JTot;
            MyJobCount = JCnt;
            MyJobAvg = JAvg;

            if (MyJobResCount > 0 && MyJobRes > 0)
            {
                MyJobResPercent = MyJobRes / (MyJobRes + MyJobCom);
            }
            else
            {
                MyJobResPercent = 0;
            }
            if (MyJobComCount > 0 && MyJobCom > 0)
            {
                MyJobComPercent = MyJobCom / (MyJobRes + MyJobCom);
            }
            else
            {
                MyJobComPercent = 0;
            }


        }

        public ActionResult LoadZeeList(string ServicePro, int id)
        {
            int MyServiceproID;

            Int32 TotalTechJobs = 0;
            double TotalTechSales = 0;

            var MyList = (from Z in db.tbl_Franchise join E in db.tbl_Employee on Z.FranchiseID equals E.FranchiseID where Z.FranchiseID == id orderby E.Employee select E);
            if (ServicePro != "")
            {
                int servicePro = Convert.ToInt32(ServicePro);
                MyList = (from E in db.tbl_Employee where E.EmployeeID == servicePro select E);
            }
            List<ServicePro> objlstServicePro = new List<ServicePro>();

            foreach (var Zee in MyList)
            {
                ServicePro objservicepro = new ServicePro();
                MyServiceproID = Zee.EmployeeID;
                Get_ServicePro_WSR_Totals_Local(MyServiceproID, startdate, enddate);

                objservicepro.Employee = Zee.Employee;
                objservicepro.MyJobTotal = MyJobTotal;
                objservicepro.MyJobCount = MyJobCount;
                objservicepro.MyJobAvg = MyJobAvg;


                TotalTechJobs = TotalTechJobs + MyJobCount;
                TotalTechSales = TotalTechSales + Convert.ToDouble(MyJobTotal);

                objservicepro.TotalTechSales = TotalTechSales;
                objservicepro.TotalTechJobs = TotalTechJobs;

                objlstServicePro.Add(objservicepro);


            }
            //ViewBag.TotalTechSales = TotalTechSales;
            //ViewBag.TotalTechJobs = TotalTechJobs;

            return Json(objlstServicePro);
        }

        private string GetCustomerName(tbl_Customer customer)
        {
            if (customer.CompanyName != "")
            {
                if (customer.CustomerName != "")
                {
                    string temp = customer.CompanyName + " - " + customer.CustomerName;
                    return temp;
                }
                else
                {
                    return (customer.CompanyName);
                }
            }
            else
            {
                return (customer.CustomerName);
            }
        }
        public ActionResult ApplyPayment(string User_AmoutToBeApplied, string GridInvoiceNo, string Grd_AppliedAmt, string Balance, string AmountApplied, string AmountToBeApplied)
        {
            if (AmountApplied == "")
                AmountApplied = "0";
            if (AmountToBeApplied == "")
                AmountToBeApplied = "0";
            if (Grd_AppliedAmt == "")
                Grd_AppliedAmt = "0";
            double currentappliedamount;

            double amountApplied = Convert.ToDouble(AmountApplied);
            if (amountApplied == 0 && Convert.ToDouble(AmountToBeApplied) == 0)
            {
                AmountToBeApplied = (User_AmoutToBeApplied);
                ViewBag.amountToBeApplied = AmountToBeApplied;
            }

            //If amountApplied = 0 And amounttobeApplied = 0 Then
            //    AmountToBeApplied = User_AmoutToBeApplied;
            //     Me.lbl_AmounttobeApplied.Text = FormatCurrency(amounttobeApplied, 2)
            // End If
            if (Grd_AppliedAmt != Balance)
            {
                currentappliedamount = Convert.ToDouble(Grd_AppliedAmt);
                if (Convert.ToDouble(AmountToBeApplied) >= Convert.ToDouble(Balance))
                {
                    currentappliedamount += Convert.ToDouble(Balance);
                    Grd_AppliedAmt = currentappliedamount.ToString();
                    amountApplied = amountApplied + Convert.ToDouble(Balance);
                    AmountToBeApplied = (Convert.ToDouble(AmountToBeApplied) - Convert.ToDouble(Balance)).ToString();
                    //amounttobeApplied = amounttobeApplied - Me.lv_Payments.Items(x).SubItems(columns.balance).Text
                    AmountApplied = amountApplied.ToString();
                }
                else
                {
                    currentappliedamount += Convert.ToDouble(AmountToBeApplied);
                    Grd_AppliedAmt = currentappliedamount.ToString();
                    amountApplied = amountApplied + Convert.ToDouble(AmountToBeApplied);
                    AmountToBeApplied = (Convert.ToInt32(AmountToBeApplied) - Convert.ToInt32(AmountToBeApplied)).ToString();
                    AmountApplied = amountApplied.ToString();

                }

            }

            ViewBag.amountApplied = AmountApplied;
            ViewBag.amountToBeApplied = AmountToBeApplied;

            return Json(new { amountApplied = AmountApplied, amountToBeApplied = AmountToBeApplied });
        }
        public ActionResult LoadReceivePaymentData(string InvoiceNo)
        {
            List<JobDetail> objlstJobDetail = new List<JobDetail>();
            string CustomerName = "";
            if (InvoiceNo != "")
            {
                int invoiceno = Convert.ToInt32(InvoiceNo);
                var customerAndFranchise = (from j in db.tbl_Job where j.JobID == invoiceno select new {j.FranchiseID, j.CustomerID}).FirstOrDefault();
                if (customerAndFranchise != null)
                {
                    tbl_Customer objCustomer = (from c in db.tbl_Customer where c.CustomerID == customerAndFranchise.CustomerID select c).FirstOrDefault();
                    CustomerName = GetCustomerName(objCustomer);
                }
                else
                {

                    ViewBag.OBCustomer = "";
                }

                var joblist = (from p in db.tbl_Job where p.CustomerID == customerAndFranchise.CustomerID && p.Balance > 0 && p.FranchiseID == customerAndFranchise.FranchiseID select p);

                foreach (var job in joblist)
                {
                    JobDetail objJob = new JobDetail();
                    objJob.InvoiceNumber = job.JobID.ToString();
                    tbl_Locations objLocation = (from l in db.tbl_Locations where l.LocationID == job.LocationID select l).FirstOrDefault();
                    if (objLocation != null)
                        objJob.Address = objLocation.Address;
                    else
                        objJob.Address = "";
                    if (job.InvoicedDate != null)
                    {
                        DateTime dt = Convert.ToDateTime(job.InvoicedDate);
                        objJob.Age = getAge(dt);
                        objJob.InvoicedDate = Convert.ToDateTime(job.InvoicedDate);
                    }
                    else
                    {
                        objJob.Age = "";
                        objJob.InvoicedDate = null;
                    }
                    objJob.TotalSales = job.TotalSales;
                    objJob.Balance = job.Balance;
                    objJob.JobID = job.JobID;
                    objJob.AppliedAmt = Convert.ToDecimal(0);
                    objlstJobDetail.Add(objJob);
                }

            }
            if (CustomerName.LastIndexOf("-") > 0)
            {
                CustomerName = CustomerName.Substring(CustomerName.LastIndexOf("-") + 1).Trim();
            }
            //return Json(objlstJobDetail, CustomerName);
            return Json(new { objJobDetails = objlstJobDetail, customerName = CustomerName });

        }
        public ActionResult RecieveMoney(string InvoiceNo)
        {
            ViewBag.invoiceno = InvoiceNo;

            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();

                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                    .Where(uf => uf.UserId == userId)
                    .Select(f => f.FranchiseID)
                    .ToArray();
                }

                var franchises = db.tbl_Franchise
                .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                .OrderBy(f => f.FranchiseNUmber)
                .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {
                return new RedirectResult("/SGAccount/LogOn");
            }

            return View();
        }

        public void PaymentProcess(string payid, string cmd, string JobId, string Paydate, string PayType, string DriveLic, string Checkno, string PayAmount)
        {
            if (!string.IsNullOrEmpty(JobId))
            {
                int ijobid = Convert.ToInt32(JobId);
                int FranchiseID = db.tbl_Job.Select(j => new {j.JobID, j.FranchiseID}).Single(j => j.JobID == ijobid).FranchiseID;
                int paymentid;

                if (cmd == "Update")
                {
                    if (payid != "" && payid != null)
                    {
                        paymentid = Convert.ToInt32(payid);
                        var Payment = (from p in db.tbl_Payments where p.PaymentID == paymentid select p).Single();
                        if (Payment != null)
                        {
                            Payment.JobID = ijobid;
                            Payment.CheckNumber = Checkno;
                            Payment.DepositStatus = false;
                            Payment.PaymentAmount = Convert.ToDecimal(PayAmount);
                            Payment.PaymentDate = Convert.ToDateTime(Paydate);
                            Payment.PaymentTypeID = Convert.ToInt32(PayType);
                            Payment.DriversLicNUm = DriveLic;
                            Payment.FranchiseID = FranchiseID;
                            db.SaveChanges();
                        }
                    }
                }
                if (cmd == "Submit")
                {
                    tbl_Payments Payment = new tbl_Payments();
                    Payment.JobID = ijobid;
                    Payment.CheckNumber = Checkno;
                    Payment.DepositStatus = false;
                    Payment.PaymentAmount = Convert.ToDecimal(PayAmount);
                    Payment.PaymentDate = Convert.ToDateTime(Paydate);
                    Payment.PaymentTypeID = Convert.ToInt32(PayType);
                    Payment.DriversLicNUm = DriveLic;
                    Payment.FranchiseID = FranchiseID;
                    Payment.CreateDate = DateTime.Now;
                    db.tbl_Payments.AddObject(Payment);
                    db.SaveChanges();
                }
                if (cmd == "Delete")
                {
                    if (payid != "" && payid != null)
                    {
                        paymentid = Convert.ToInt32(payid);
                        var Payment = (from p in db.tbl_Payments where p.PaymentID == paymentid select p).Single();
                        if (Payment != null)
                        {
                            db.tbl_Payments.DeleteObject(Payment);
                            db.SaveChanges();
                        }
                    }
                }
            }

        }
        public ActionResult payments(int jobid)
        {
            List<tbl_Payment_Types> lstPaymentType = (from pt in db.tbl_Payment_Types select pt).ToList();
            var paymentid = (from p in db.tbl_Payments where p.JobID == jobid select p).FirstOrDefault();
            if (paymentid != null)
            {
                ViewBag.PaymentTypeId = paymentid.PaymentTypeID;
                ViewBag.PaymentTypeList = lstPaymentType;
                ViewBag.jobsid = jobid.ToString();

                ViewBag.paymentid = paymentid.PaymentID.ToString();
            }
            else
            {
                ViewBag.paymentid = "0";
                ViewBag.PaymentTypeId = "";
                ViewBag.PaymentTypeList = lstPaymentType;
                ViewBag.jobsid = jobid.ToString();
            }
            return View();
        }
        public ActionResult PaymentList(int jobid)
        {
            Pay_ments objpay = new Pay_ments();
            List<Pay_ments> lstpay = new List<Pay_ments>();
            Pay_ments paysummary;

            List<tbl_Payments> PaymentList = (from t in db.tbl_Payments where t.JobID == jobid select t).ToList();
            decimal? total = 0;
            foreach (var p in PaymentList)
            {
                var FranchiseID = p.FranchiseID;
                objpay.paydate = p.PaymentDate.Value.ToShortDateString();
                if (p.PaymentTypeID > 0) { objpay.paytype = (from pt in db.tbl_Payment_Types where pt.PaymentTypeId == p.PaymentTypeID select pt.PaymentType).Single(); } else { objpay.paytype = "N/A"; }
                objpay.paydrivelicno = p.DriversLicNUm;
                objpay.paycheckno = p.CheckNumber;
                objpay.payamt = string.Format("{0:C}", p.PaymentAmount);
                total = total + p.PaymentAmount;
                objpay.paytotal = string.Format("{0:C}", total);
                objpay.paymentid = p.PaymentID.ToString();

                paysummary = new Pay_ments
                {
                    paydate = objpay.paydate,
                    paytype = objpay.paytype,
                    paydrivelicno = objpay.paydrivelicno,
                    paycheckno = objpay.paycheckno,
                    payamt = objpay.payamt,
                    paytotal = objpay.paytotal,
                    paymentid = objpay.paymentid
                };

                lstpay.Add(paysummary);

            }
            return Json(lstpay);
        }
        public PartialViewResult RecMoney(string code, string invoiceno)
        {
            try
            {
                int InvoiceNo = 0;
                if (invoiceno != "")
                {
                    InvoiceNo = Convert.ToInt32(invoiceno);
                }
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                var FranchiseID = (from g in memberShipContext.MembershipFranchise
                               where g.FranchiseNumber == code
                               select g.FranchiseID).FirstOrDefault();
                ViewBag.Code = code;
                ViewBag.InvoiceNo = invoiceno;
                int objCustomerId = (from j in db.tbl_Job where j.JobID == InvoiceNo select j.CustomerID).FirstOrDefault();
                if (objCustomerId != 0)
                {
                    tbl_Customer objCustomer = (from c in db.tbl_Customer where c.CustomerID == objCustomerId select c).FirstOrDefault();
                    ViewBag.OBCustomer = GetCustomerName(objCustomer);
                    List<tbl_Payment_Types> PaymentTypeList = (from p in db.tbl_Payment_Types select p).ToList();
                    ViewBag.PaymentType = PaymentTypeList;
                    ViewBag.Date = DateTime.Now.ToShortDateString();
                }
                else
                {

                    ViewBag.OBCustomer = "";
                    ViewBag.PaymentType = new List<tbl_Payment_Types>();
                    ViewBag.Date = "";
                }

                return PartialView("RecMoney", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public ActionResult ApplyNow(FormCollection formcollection)
        {
            int appliedamt = Convert.ToInt32(formcollection["hdnAppliedAmt"]);
            int amttobeapplied = Convert.ToInt32(formcollection["hdnamounttobeApplied"]);
            int Jobid = Convert.ToInt32(formcollection["hdnInvoiceNo"]);
            string varcode = formcollection["hdnCode"];
            string varinvoiceno = formcollection["hdnInvoiceNo"];
            //string paytype= formcollection["PaymentType"];
            if (amttobeapplied == 0)
            {
                tbl_Payments Payment = new tbl_Payments();
                Payment.JobID = Jobid;
                Payment.PaymentAmount = appliedamt;
                Payment.PaymentDate = Convert.ToDateTime(formcollection["paymentDate"]);
                //tbl_Payment_Types objPaymentType = (from p in db.tbl_Payment_Types where p.PaymentType == paytype select p).FirstOrDefault();
                Payment.PaymentTypeID = Convert.ToInt32(formcollection["PaymentType"]);
                if (formcollection["CheckNo"] != "")
                    Payment.CheckNumber = formcollection["CheckNo"];
                else
                    Payment.CheckNumber = "";
                Payment.FranchiseID = Convert.ToInt32(formcollection["hdnFranchiseId"]);
                Payment.CreateDate = DateTime.Now;
                db.tbl_Payments.AddObject(Payment);
                var job = (from j in db.tbl_Job where j.JobID == Payment.JobID select j).FirstOrDefault();
                decimal tmpbalance = 0;
                tmpbalance = job.Balance;
                if (Convert.ToDecimal(appliedamt) != job.Balance)
                {
                    job.Balance = tmpbalance - appliedamt;
                    db.SaveChanges();
                }
            }

            //       If FormatCurrency(tmpbalance - Me.lv_Payments.Items(x).SubItems(columns.appliedamt).Text, 2) <> job.Balance Then
            //           job.Balance = FormatCurrency(tmpbalance - Me.lv_Payments.Items(x).SubItems(columns.appliedamt).Text, 2)
            //           db.SubmitChanges()
            //       End If

            return RedirectToAction("RecieveMoney", new RouteValueDictionary(
      new { controller = "MyFinances", action = "RecieveMoney", invoiceno = varinvoiceno }));
            //return RedirectToAction("RecMoney", new { code = code, invoiceno = invoiceno });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult generate(string xmldata)
        {
            Session["xmldataNew"] = xmldata;
            var t = "";
            return Json(t);
        }

        //public ActionResult test()
        //{
        //    var generator = new ExcelWriter();
        //    //var xml = Session["xmldataNew"].ToString();
        //    var xml = "<?xml version=\"1.0\"?><AUTHRESP><RESPONSE>3</RESPONSE><AUTHPARM1>S9801231</AUTHPARM1><BALLOT></BALLOT></AUTHRESP>";
        //    xml = this.Server.UrlDecode(xml);
        //    var stream = generator.Generate(xml);
        //    return File(stream.ToArray(), generator.ContentType, "filedownloaded.xls");
        //}

        //public ActionResult Export()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("name");
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.Append("<table border='" + "2px" + "'b>");
        //    //write column headings
        //    sb.Append("<tr>");
        //    foreach (System.Data.DataColumn dc in dt.Columns)
        //    {
        //        sb.Append("<td><b><font face=Arial size=2>" + dc.ColumnName + "</font></b></td>");
        //    }
        //    sb.Append("</tr>");

        //    //write table data
        //    foreach (System.Data.DataRow dr in dt.Rows)
        //    {
        //        sb.Append("<tr>");
        //        foreach (System.Data.DataColumn dc in dt.Columns)
        //        {
        //            sb.Append("<td><font face=Arial size=" + "14px" + ">" + dr[dc].ToString() + "</font></td>");
        //        }
        //        sb.Append("</tr>");
        //    }
        //    sb.Append("</table>");

        //    this.Response.AddHeader("Content-Disposition", "Employees.xls");
        //    this.Response.ContentType = "application/vnd.ms-excel";
        //    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        //    return File(buffer, "application/vnd.ms-excel");
        //}

        public ActionResult AccountQBTransfer()
        {
            return View();
        }

        public ActionResult QBDepositData(int frId)
        {
            var Depositlist = (from d in db.tbl_Deposits
                               where d.AccountingTransferDate != null && d.FranchiseID == frId
                               select new
                               {
                                   d.DepositID,
                                   d.DepositNumber,
                                   d.DepositDate,
                                   d.CashAmount,
                                   d.CheckAmount,
                                   d.CreditAmount,
                                   d.OtherAmount,
                                   d.DepositAmount,
                                   d.AccountingTransferDate,
                                   d.AccountingRefNum,
                                   d.AccountingTransactionNum,
                                   d.DepositNotes
                               }).ToList();

            return Json(Depositlist);
        }

        public ActionResult QBWeekEndingData(int frId)
        {
            DateTime weekEnd = Convert.ToDateTime("7/30/2011");
            var HistList = (from H in db.tbl_ACH_Franchisees_Summary
                            where H.FranchiseID == frId
                            where H.MSAPostedYN == false && H.WeekEnding > weekEnd
                            orderby H.WeekEnding descending, H.TransactionGroupID
                            select new
                            {
                                H.ACHZeeSummaryID,
                                H.WeekEnding,
                                H.TotalSales,
                                H.MSAPostedYN,
                                H.MSAPostedDate,
                                H.MSARefNum
                            }).ToList();

            return Json(HistList);
        }

        public ActionResult QBInvoiceData(int frId, string date)
        {

            List<InvoiceDetail> lsttaskdetails = new List<InvoiceDetail>();
            InvoiceDetail detail = new InvoiceDetail();
            int jobID = 0;
            int totalCount = 0;
            var invoicenumber = "";
            var billto = "";
            var joblocation = "";
            string status = "";
            var closeddate = "";
            var Tech = "";
            decimal Jobamt = 0;
            decimal taxamt = 0;
            decimal balance = 0;
            decimal TotalSales = 0;
            decimal totalTax = 0;
            string LongStr = "";
            if (date != "")
            {
                DateTime endDate = Convert.ToDateTime(date);

                var joblist = (from j in db.tbl_Job
                               where j.FranchiseID == frId && j.WSRCompletedDate == endDate
                               select new
                               {
                                   j.JobID,
                                   j.InvoicedDate,
                                   j.WSRCompletedDate,
                                   j.LocationID,
                                   j.CustomerID,
                                   j.SubTotal,
                                   j.Balance,
                                   j.TaxAmount
                               }).ToList();


                foreach (var job in joblist)
                {
                    jobID = job.JobID;
                    invoicenumber = job.JobID.ToString();
                    var Customer = (from p in db.tbl_Customer where p.CustomerID == job.CustomerID select p).FirstOrDefault();
                    if (Customer != null)
                        billto = Customer.CustomerName;
                    var location = (from p in db.tbl_Locations where p.LocationID == job.LocationID select p).FirstOrDefault();
                    if (location != null)
                        joblocation = location.Address;
                    status = "Closed";
                    if (job.InvoicedDate != null)
                        closeddate = job.InvoicedDate.ToString();
                    date = job.WSRCompletedDate.ToString();
                    var techlist = (from t in db.tbl_Job_Technicians where t.JobID == jobID && t.PrimaryYN == true select t).ToList();
                    int techID = 0;
                    foreach (var Techrec in techlist)
                    {
                        techID = Techrec.ServiceProID;
                    }
                    var employee = (from p in db.tbl_Employee where p.EmployeeID == techID select p).FirstOrDefault();
                    if (employee != null)
                        Tech = employee.Employee;

                    Jobamt = job.SubTotal;
                    balance = job.Balance;
                    taxamt = job.TaxAmount;
                    TotalSales = TotalSales + Jobamt;
                    totalTax = totalTax + taxamt;

                    LongStr = billto + " | " + joblocation + " | " + Tech + " | " + date;

                    totalCount = joblist.Count();
                    detail = new InvoiceDetail
                    {
                        jobID = jobID,
                        invoicenumber = invoicenumber,
                        billto = billto,
                        joblocation = joblocation,
                        status = status,
                        closeddate = Convert.ToDateTime(closeddate),
                        WsrDate = Convert.ToDateTime(date),
                        Tech = Tech,
                        Jobamt = Jobamt,
                        balance = balance,
                        taxamt = taxamt,
                        TotalSales = TotalSales,
                        totalTax = totalTax,
                        totalCount = totalCount
                    };
                    lsttaskdetails.Add(detail);
                }
                return Json(lsttaskdetails);
            }

            var nothing = "";
            return Json(nothing);

        }

        public ActionResult SaleTaxRevenueReport(string FranchiseeId)
        {
            int fid = Convert.ToInt32(FranchiseeId);
            var listInfo = (from j in db.tbl_Job
                            join e in db.tbl_Employee on j.ServiceProID equals e.EmployeeID
                            where e.City != "" && j.FranchiseID == fid
                            orderby e.City
                            select new
                            {
                                e.City,
                                j.SubTotal,
                                j.TaxAmount
                            });
            var lstinfo = (from l in listInfo
                           group l by l.City into newinfo
                           select new
                           {
                               city = newinfo.Key,
                               taxablesales = newinfo.Sum(p => p.SubTotal),
                               nontaxablessales = newinfo.Sum(p => p.TaxAmount)

                           });


            var xmlstring = "";
            xmlstring = xmlstring + "<rows>";
            int i = 0;
            decimal dtaxablesales = 0;
            decimal dnontaxablesales = 0;
            if (lstinfo.Count() > 0)
            {
                foreach (var item in lstinfo)
                {

                    xmlstring = xmlstring + "<row id='" + i + "'>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + item.city + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", item.taxablesales) + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", item.nontaxablessales) + "]]></cell>";
                    xmlstring = xmlstring + "<cell><![CDATA[" + string.Format("{0:C}", Convert.ToDecimal(item.taxablesales + item.nontaxablessales)) + "]]></cell>";
                    xmlstring = xmlstring + "</row>";
                    dtaxablesales = dtaxablesales + item.taxablesales;
                    dnontaxablesales = dnontaxablesales + item.nontaxablessales;
                    i = i + 1;
                }
                xmlstring = xmlstring + "<row id='" + i + "'><cell><![CDATA[<b>Totals</b>]]></cell><cell><![CDATA[<b>" + string.Format("{0:C}", dtaxablesales) + "</b>]]></cell><cell><![CDATA[<b>" + string.Format("{0:C}", dnontaxablesales) + "</b>]]></cell><cell><![CDATA[<b>" + string.Format("{0:C}", Convert.ToDecimal(dtaxablesales + dnontaxablesales)) + "</b>]]></cell></row>";
            }

            xmlstring = xmlstring + "</rows>";

            return Json(xmlstring);
        }

        public ActionResult GetCloseDate(int jobid)
        {
            DateTime? closedate = (from j in db.tbl_Job where j.JobID == jobid select new { j.InvoicedDate }).FirstOrDefault().InvoicedDate;
            return Json(closedate.Value.ToShortDateString());
        }


        public ActionResult QBSalesData(int frId, string date)
        {
            List<InvoiceDetail> lsttaskdetails = new List<InvoiceDetail>();
            InvoiceDetail detail = new InvoiceDetail();
            float totalAcctSales = 0;

            var Saleslist = (from t in db.tbl_temp_AccountingTransfer
                             where t.FranchiseID == frId
                             select t).ToList();

            if (date != "")
            {
                DateTime endDate = Convert.ToDateTime(date);
                foreach (var sale in Saleslist)
                {
                    totalAcctSales = totalAcctSales + sale.Amount;
                    detail = new InvoiceDetail
                    {
                        WsrDate = enddate,
                        Account = sale.Account,
                        Amount = sale.Amount,
                        AcctTotal = totalAcctSales
                    };
                    lsttaskdetails.Add(detail);
                }
                return Json(lsttaskdetails);
            }
            var nothing = "";
            return Json(nothing);
        }

        [HttpPost]
        public JsonResult ApplyAdjustmentNew(int? franchiseID, int? invoiceID, decimal? adjustmentAmount, int? adjustmentTypeID, string adjustmentType, string checkNumber, string comments)
        {
            // Process account adjustment.
            if (franchiseID.HasValue && invoiceID.HasValue && adjustmentAmount.HasValue && adjustmentTypeID.HasValue)
            {
                // Validation for 'NSF' Payment Type.
                if (adjustmentType == "NSF")
                {
                    // Validate check and payment amount.
                    if (!string.IsNullOrWhiteSpace(checkNumber))
                    {
                        var checkPaymentInfo = (from p in db.tbl_Payments
                                                where (p.JobID == invoiceID && p.CheckNumber == checkNumber && p.PaymentTypeID == 2)
                                                select p).FirstOrDefault();

                        if (checkPaymentInfo != null)
                        {
                            // Validate NSF Amount
                            if (checkPaymentInfo.PaymentAmount == adjustmentAmount)
                            {
                                // See if NSF for the same check number already exist in the payment table.
                                var nsfCheck = (from p in db.tbl_Payments
                                                where (p.PaymentTypeID == 13 && p.CheckNumber == checkNumber && p.PaymentAmount == adjustmentAmount)
                                                select p).FirstOrDefault();

                                if (nsfCheck != null)
                                {
                                    // NSF payment already made, return.
                                    return Json(new
                                    {
                                        Message = string.Format("NSF adjustment for check number '{0}' is already exist.", checkNumber),
                                        ResultData = "",
                                        Success = false
                                    });
                                }

                                // Add payment information.
                                var nsfPayment = new tbl_Payments
                                {
                                    FranchiseID = franchiseID.Value,
                                    JobID = invoiceID.Value,
                                    PaymentAmount = adjustmentAmount,
                                    PaymentDate = DateTime.Now,
                                    PaymentTypeID = adjustmentTypeID.Value,
                                    DepositStatus = true,
                                    CheckNumber = checkNumber,
                                    DepositID = 0
                                };

                                nsfPayment.CreateDate = DateTime.Now;
                                db.tbl_Payments.AddObject(nsfPayment);

                                var nsfJob = (from j in db.tbl_Job
                                              where j.JobID == invoiceID
                                              select j).Single();

                                if (nsfJob != null)
                                {
                                    // Update balance
                                    nsfJob.Balance = nsfJob.Balance + adjustmentAmount.Value;
                                    // Persist data back in datastore.
                                    db.SaveChanges();
                                }

                                return Json(new
                                {
                                    Message = string.Format("Account adjustment of type '{0}' for job '{1}' processed successfully.", adjustmentType, invoiceID),
                                    ResultData = "",
                                    Success = true
                                });
                            }
                            else
                            {
                                return Json(new
                                {
                                    Message = string.Format("The adjustment amount you entered for check number '{0}' is not valid.", checkNumber),
                                    ResultData = "",
                                    Success = false
                                });
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                Message = string.Format("The check number '{0}' you entered does not exist for job '{1}' .", checkNumber, invoiceID),
                                ResultData = "",
                                Success = false
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = "Please provide check number.",
                            ResultData = "",
                            Success = false
                        });
                    }
                }

                // Add payment information.
                var payment = new tbl_Payments
                {
                    FranchiseID = franchiseID.Value,
                    JobID = invoiceID.Value,
                    PaymentAmount = adjustmentAmount,
                    PaymentDate = DateTime.Now,
                    PaymentTypeID = adjustmentTypeID.Value,
                    DepositStatus = true,
                    DepositID = 0
                };

                payment.CreateDate = DateTime.Now;
                db.tbl_Payments.AddObject(payment);

                var job = (from j in db.tbl_Job
                           where j.JobID == invoiceID
                           select j).Single();

                if (job != null)
                {
                    // Update balance
                    if (adjustmentType == "Refund") //PaymentType is Refund.
                    {
                        //PaymentType is Refund, so reduce customer credit balance.
                        job.Balance = job.Balance - (-adjustmentAmount.Value);
                    }
                    else
                    {
                        job.Balance = job.Balance - adjustmentAmount.Value;
                    }

                    // Persist data back in datastore.
                    db.SaveChanges();
                }

                return Json(new
                                {
                                    Message = string.Format("Account adjustment of type '{0}' for job '{1}' processed successfully.", adjustmentType, invoiceID),
                                    ResultData = "",
                                    Success = true
                                });
            }

            return Json(new
            {
                Message = string.Format("Sorry, we are unable to process your request for job '{0}'.", invoiceID),
                ResultData = "",
                Success = false
            });
        }

        /// <summary>
        /// Account Adjustment 
        /// </summary>
        public string lblmessage = "";
        public ActionResult ApplyAdjustment(int franchiseid, string invoiceid, string amount, string accountid, string comments)
        {
            //gbZeeAcctType = (from fc in db.tbl_Franchise_Contract where fc.FranchiseID == franchiseid select new { fc.AccountingSystemType }).FirstOrDefault().AccountingSystemType.ToString();
            //if (gbZeeAcctType == "QB")
            //{
            //    Apply_Adjustment_ForQB(franchiseid, invoiceid, amount, accountid, comments);
            //}
            //else if (gbZeeAcctType == "PV")
            //{
            //    //do nothing
            //}
            //else
            //{
            //    //MSA system

            //    //adjust job payments
            //    try
            //    {
            //        SBASuccess = false;
            //        //SBAAppLoader.Main();

            //        // set sbaObjects

            //        // Initialize form
            //        //adjust msa accounting 

            //        InitializeSBA_Adjustments(franchiseid, invoiceid, amount, accountid, comments);


            //        if (SBASuccess == true)
            //        {
            //            tbl_Payments newpayment = new tbl_Payments();

            //            newpayment.FranchiseID = Convert.ToInt32(franchiseid);
            //            newpayment.JobID = Convert.ToInt32(invoiceid);
            //            newpayment.PaymentAmount = Convert.ToDecimal(amount);
            //            newpayment.PaymentDate = DateTime.Now;
            //            newpayment.DepositStatus = true;
            //            newpayment.CreateDate = DateTime.Now;
            //            //posted
            //            if (accountid == "65900 WriteOff")
            //            {
            //                newpayment.PaymentTypeID = -1;
            //                //writeoff
            //                newpayment.DepositID = -1;
            //            }
            //            else
            //            {
            //                newpayment.PaymentTypeID = -2;
            //                //discount
            //                newpayment.DepositID = -2;
            //            }
            //            db.tbl_Payments.AddObject(newpayment);
            //            db.SaveChanges();

            //            try
            //            {
            //                int ijobid = Convert.ToInt32(invoiceid);

            //                var Jobrec = (from J in db.tbl_Job where J.JobID == ijobid select J).Single();
            //                Jobrec.Balance = Jobrec.Balance - Convert.ToDecimal(amount);
            //                db.SaveChanges();
            //                lblmessage = "Account adjustment done successfully.";

            //            }
            //            catch (Exception ex)
            //            {
            //                lblmessage = "Unable to update job balance, but we will post the adjustment for you!  Error message is:" + ex.Message;
            //            }

            //        }



            //    }
            //    catch (Exception ex)
            //    {
            //        lblmessage = "Adjustment was NOT posted since we are Unable to update job payments due to:" + ex.Message;
            //    }


            //}
            return Json(lblmessage);
        }

        private bool verifyAdjAccounts(int franchiseid, string invoiceid, string amount, string accountid, string comments)
        {

            try
            {
                //int i = 0;
                string holdprimarykey = "";
                string holdcustPrimKey = "";
                int fid = Convert.ToInt32(franchiseid);
                var customerlist = (from c in db.tbl_AccountingCustomers where c.CustomerName == "800Receivables" && c.FranchiseID == fid select c);
                foreach (var customer_loopVariable in customerlist)
                {

                    holdcustPrimKey = customer_loopVariable.PrimaryKey.ToString();
                }
                if (string.IsNullOrEmpty(holdcustPrimKey))
                {
                    lblmessage = "The CUSTOMER 800Receivables needs to be setup in your accouting program before transferring.";
                    return false;
                }

                holdprimarykey = "";
                var ARAccountlist = (from a in db.tbl_AccountingAccounts where a.AccountDesc == "Accounts Receivable" && a.FranchiseID == fid select a);
                foreach (var araccount_loopVariable in ARAccountlist)
                {

                    holdprimarykey = araccount_loopVariable.PrimaryKey.ToString();
                }
                if (string.IsNullOrEmpty(holdprimarykey))
                {
                    lblmessage = "The Account Accounts Receivable needs to be setup in your accounting program before transferring.";
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                lblmessage = "Verify Accounts: " + ex.Message;
                return false;
            }

        }
       
        public bool Open_QB_Session()
        {
            bool functionReturnValue = false;

            //test for dev vars
            if (System.IO.Directory.Exists("C:\\Development\\TT"))
            {
                gbQBMasterVersion = 8;
                gbQBFileName = "";
            }

            try
            {
                //open a session
                sessMgr = new QBSessionManager();
                sessMgr.OpenConnection("", gbQBAppName);
                sessMgr.BeginSession(gbQBFileName, ENOpenMode.omDontCare);
                functionReturnValue = true;

                try
                {
                    //string maxVersion = null;
                    //maxVersion = sessMgr.QBXMLVersionsForSession(Information.UBound(sessMgr.QBXMLVersionsForSession));


                    //Careful -- Should handle CA4.0, etc. later
                    //if ((maxVersion < "10.0"))
                    //{
                    //     ViewBag.lblmessage = "This sample requires QuickBooks 2005 to support qbXML 4.0, current QuickBooks supports only " + maxVersion + "qbXML 4.0 not supported";
                    //    sessMgr.EndSession();
                    //    sessMgr.CloseConnection();
                    //    return functionReturnValue;
                    //}
                }
                catch (Exception ex)
                {
                    lblmessage = "Unable to find valid QB Session due to:" + ex.Message;
                    functionReturnValue = false;
                    return functionReturnValue;
                }

            }
            catch (Exception ex)
            {
                lblmessage = "Unable to open session due to:" + ex.Message;
                functionReturnValue = false;
            }
            return functionReturnValue;

        }
        public void BuildJournalEntryAddRq_ForAdjustments(IMsgSetRequest requestMsgSet, string myInvoiceID, string MyAdjAmtText, string MyAdjAccounttext)
        {
            try
            {
                string InvoiceID = myInvoiceID;
                double EntryAmt = Convert.ToDouble(MyAdjAmtText);
                //string AccountNum = "";
                string AcctName = "";

                //'58000 Plumbing(Discount / Coupon / Adjustment)
                //'59100 Electrical(Discount / Coupon / Adjustment)
                //'59000 HVAC(Discount / Coupon / Adjustment)
                //'65900 WriteOff()

                //switch (Strings.Mid(MyAdjAccounttext, 1, 5))
                switch (MyAdjAccounttext.Substring(1, 5))
                {
                    case "58000":
                        //AccountNum = "58000";
                        AcctName = "Promotions-Discounts - PLMB";
                        break;
                    case "59100":
                        //AccountNum = "59100";
                        AcctName = "Promotions-Discounts - ELEC";
                        break;
                    case "59000":
                        //AccountNum = "59000";
                        AcctName = "Promotions-Discounts - HVAC";
                        break;
                    case "65900":
                        //AccountNum = "65900";
                        AcctName = "Bad Debts";
                        break;
                    default:
                        //AccountNum = "65900";
                        AcctName = "Bad Debts";
                        break;
                }


                gbQBJournalRefNumber = "SW" + InvoiceID;

                IJournalEntryAdd JournalEntryAddRq = default(IJournalEntryAdd);
                JournalEntryAddRq = requestMsgSet.AppendJournalEntryAddRq();

                //TxnDate, ref, 
                JournalEntryAddRq.TxnDate.SetValue(DateTime.Now);
                if (!string.IsNullOrEmpty(gbQBJournalRefNumber))
                {
                    JournalEntryAddRq.RefNumber.SetValue(gbQBJournalRefNumber);
                }

                //DEBIT 
                IORJournalLine JournalDebitLine = default(IORJournalLine);
                JournalDebitLine = JournalEntryAddRq.ORJournalLineList.Append();
                JournalDebitLine.JournalDebitLine.AccountRef.FullName.SetValue(AcctName);
                JournalDebitLine.JournalDebitLine.EntityRef.FullName.SetValue(gbQBCustomerFullName);
                JournalDebitLine.JournalDebitLine.Amount.SetValue(EntryAmt);
                JournalDebitLine.JournalDebitLine.Memo.SetValue(gbQBJournalMemoPrefix + InvoiceID);

                //CREDIT 
                IORJournalLine JournalCreditLine = default(IORJournalLine);
                JournalCreditLine = JournalEntryAddRq.ORJournalLineList.Append();
                JournalCreditLine.JournalCreditLine.AccountRef.FullName.SetValue(gbQBARAcct);
                JournalCreditLine.JournalCreditLine.EntityRef.FullName.SetValue(gbQBCustomerFullName);
                JournalCreditLine.JournalCreditLine.Amount.SetValue(EntryAmt);
                JournalCreditLine.JournalCreditLine.Memo.SetValue(gbQBJournalSalesMemoPrefix + InvoiceID);



            }
            catch (Exception ex)
            {
                lblmessage = "Unable to addd journ line due to:" + ex.Message;
            }

        }

        public string GetBarWord(string S, int FieldNumber)
        {
            string functionReturnValue = null;
            // Returns the nth word in a specific field.

            int WC = 0;
            int Count = 0;
            int SPos = 0;
            int EPos = 0;
            WC = CountBarWords(S);
            if (FieldNumber < 1 | FieldNumber > WC)
            {
                functionReturnValue = "";
                return functionReturnValue;
            }
            Count = 1;
            SPos = 1;
            for (Count = 2; Count <= FieldNumber; Count++)
            {
                //SPos = Strings.InStr(SPos, S, "|") + 1;
                SPos = S.IndexOf("|", SPos) + 1;
            }
            //EPos = Strings.InStr(SPos, S, "|") - 1;
            EPos = S.IndexOf("|", SPos) - 1;
            if (EPos <= 0)
                EPos = S.Length;
            //functionReturnValue = Strings.Replace(Strings.Trim(Strings.Mid(S, SPos, EPos - SPos + 1)), "'", "");
            functionReturnValue = S.Substring(SPos, EPos - SPos + 1).Trim();

            return functionReturnValue;
        }
        public int CountBarWords(string S)
        {
            int functionReturnValue = 0;
            // Counts the words in a string that are separated by commas.

            int WC = 0;
            int Pos = 0;
            if (S.Length == 0)
            {
                functionReturnValue = 0;
                return functionReturnValue;
            }
            WC = 1;
            Pos = S.IndexOf("|");
            while (Pos > 0)
            {
                WC = WC + 1;
                //Pos = Strings.InStr(Pos + 1, S, "|");
                Pos = S.IndexOf("|", Pos + 1);
            }
            functionReturnValue = WC;
            return functionReturnValue;

        }

        public ActionResult DepositSlip()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDeposits(int? franchiseID)
        {
            if (franchiseID.HasValue)
            {
                var deposits = (from d in db.tbl_Deposits
                                where d.FranchiseID == franchiseID
                                select d);

                var xml = "<rows>";

                foreach (var item in deposits)
                {
                    xml = xml + "<row id='" + item.DepositID + "'>";

                    xml = xml + "<cell><![CDATA[" + item.DepositID + "^../../MyFinances/MyFinances/Index?depositID=" + item.DepositID + "^_self]]></cell>";
                    xml = xml + "<cell><![CDATA[" + item.DepositDate + "]]></cell>";
                    xml = xml + "<cell><![CDATA[" + string.Format("{0:C}", item.DepositAmount) + "]]></cell>";
                    xml = xml + "<cell><![CDATA[" + string.Format("{0:C}", item.CheckAmount) + "]]></cell>";
                    xml = xml + "<cell><![CDATA[" + string.Format("{0:C}", item.CreditAmount) + "]]></cell>";
                    xml = xml + "<cell><![CDATA[" + string.Format("{0:C}", item.OtherAmount) + "]]></cell>";
                    xml = xml + "<cell><![CDATA[" + string.Format("{0:C}", item.CashAmount) + "]]></cell>";
                    xml = xml + "<cell><![CDATA[" + item.DepositNumber + "]]></cell>";
                    xml = xml + "<cell><![CDATA[" + item.DepositNotes + "]]></cell>";

                    xml = xml + "</row>";
                }

                xml = xml + "</rows>";

                return Json(new
                {
                    Message = "",
                    ResultData = xml,
                    Success = true
                });
            }
            else
            {
                return Json(new
                {
                    Message = "Please provide franchise id.",
                    ResultData = "",
                    Success = false
                });
            }
        }
    }
}
