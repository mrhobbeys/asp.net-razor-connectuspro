using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using DHTMLX.Export.Excel;
using SiteBlue.Areas.Reporting.Models;
using SiteBlue.Business;
using SiteBlue.Business.Reporting;
using SiteBlue.Controllers;
using System.Xml;
using System.Data;
using SiteBlue.Data.Reporting;
using System.Linq;

namespace SiteBlue.Areas.Reporting.Controllers
{
    [Authorize]
    public class ReportingController : SiteBlueBaseController
    {
        public ActionResult Index()
        {
            //if (User.Identity.Name != "")
            //{                
            var TimePeriod = new List<SelectListItem>();
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Week",
                Value = "0",
                Selected = true
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Week",
                Value = "1"
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Month",
                Value = "2"
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Month",
                Value = "3"
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Year",
                Value = "4"
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "The beginning of time",
                Value = "5"
            });
            var timespan = new System.TimeSpan(7, 0, 0, 0);
            var model = new ReportSearch
            {
                FromDate = DateTime.Now.Subtract(timespan),
                ToDate = DateTime.Now.Subtract(timespan),
                TimePeriod = GetTimerPeriodList(0)
            };

            return View(model);
        }

        /// <summary>
        /// This action method get the param val and navigate to job information view 
        /// </summary>
        /// <param name="franchiseId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="timeperiod"></param>
        /// <returns></returns>
        public List<SelectListItem> GetTimerPeriodList(int selected)
        {
            var TimePeriod = new List<SelectListItem>();

            TimePeriod.Add(new SelectListItem
            {
                Text = "This week",
                Value = "0",
                Selected = selected == 0 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last week",
                Value = "1",
                Selected = selected == 1 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This month",
                Value = "2",
                Selected = selected == 2 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Month",
                Value = "3",
                Selected = selected == 3 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This year",
                Value = "4",
                Selected = selected == 4 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "The beginning of time",
                Value = "5",
                Selected = selected == 5 ? true : false
            });

            return TimePeriod;
        }

        public ActionResult JobTaskAccounting()
        {
            return View();
        }

        public ActionResult JobDetailSalesReport(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var selected = 0;
            if (timeperiod == "date")
                timeperiod = "-1";

            if (timeperiod != "date")
                selected = Convert.ToInt32(timeperiod);

            var model = new ReportSearch
            {
                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timeperiod
            };
            return View(model);
        }

        public ActionResult JobTaskPartDetails()
        {
            return View();
        }

        #region JobInforamtion

        public ActionResult JobInformation(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var selected = 0;
            if (timeperiod != "-1")
                selected = Convert.ToInt32(timeperiod);

            var model = new ReportSearch
            {
                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timeperiod
            };
            return View(model);
        }

        public JsonResult GetJobInformation(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            switch (timeperiod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetJobs(franchiseId, from, to);

            var sb = new StringBuilder();
            sb = GetJobInformationXML(result);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetJobInformationXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetJobInformationXML(JobOverview[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int uid = 0;

            var sb = new StringBuilder();
            sb.Append("<rows>");
            foreach (var rec in result)
            {
                if (maxRecords > 0 && uid > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:d}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.AppendFormat("<row id='{0}'>", rec.Id);
                sb.AppendFormat(formatStr, rec.Id + (includeLinks ? ("^/Ownerportal/invoice/invoicedetails?JobId=" + rec.Id) : ""));
                sb.AppendFormat(formatStr, rec.CustomerName + (includeLinks ? ("^/OwnerPortal/AllCustomers/CustomerInformation?Custid=" + rec.CustomerId) : ""));
                sb.AppendFormat(formatStr, rec.Address);
                sb.AppendFormat(formatStr, rec.City);
                sb.AppendFormat(formatStr, rec.PostalCode);
                sb.AppendFormat(formatStr, rec.BusinessType);
                sb.AppendFormat(formatStr, rec.JobPriority);
                sb.AppendFormat(formatStr, rec.Status);
                sb.Append(rec.Completed.HasValue ? string.Format(dateFormat, rec.Completed.Value) : string.Empty);
                sb.AppendFormat(formatStr, rec.IsEstimate);
                sb.AppendFormat(formatStr, rec.IsRecall);
                sb.AppendFormat(formatStr, rec.ReferralType);
                sb.AppendFormat(formatStr, rec.Service);
                sb.AppendFormat(formatStr, rec.Tech + (includeLinks ? ("^/ownerportal/Employee/EmployeeInformation?id=" + rec.TechId + "&frid=" + rec.FranchiseID) : ""));
                sb.AppendFormat(moneyFormat, rec.SubTotal);
                sb.AppendFormat(moneyFormat, rec.Balance);
                sb.AppendFormat(moneyFormat, rec.TaxAmount);
                sb.AppendFormat(formatStr, rec.TaxDescription);
                sb.Append("</row>");

                uid++;
            }
            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadJobInformationExcel(bool bMaxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (bMaxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "MyJobInformationReport.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetJobs(franchiseId, from, to);

                xml = GetJobInformationXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "MyJobInformationReport.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "MyJobInformationReport.xlsx");
            }
        }

        #endregion

        #region CustomerInformation

        public ActionResult CustomerInformation(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var selected = 0;
            if (timeperiod == "date")
                timeperiod = "-1";

            if (timeperiod != "date")
                selected = Convert.ToInt32(timeperiod);

            var model = new ReportSearch
            {
                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timeperiod
            };

            return View(model);
        }

        public JsonResult GetCustomerInformation(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            switch (timeperiod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetCustomers(franchiseId, from, to);

            var sb = new StringBuilder();
            sb = GetCustomerInformationXML(result);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetCustomerInformationXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetCustomerInformationXML(Customer[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int uid = 0;

            var sb = new StringBuilder();
            sb.Append("<rows>");

            foreach (var customerInfo in result)
            {
                if (maxRecords > 0 && uid > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:d}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.Append("<row id='" + customerInfo.Id + "'>");
                sb.AppendFormat(formatStr, customerInfo.Name + ((includeLinks) ? ("^/OwnerPortal/AllCustomers/CustomerInformation?Custid=" + customerInfo.Id) : ""));
                sb.AppendFormat(formatStr, customerInfo.BillToAddress);
                sb.AppendFormat(formatStr, customerInfo.BillToCity);
                sb.AppendFormat(formatStr, customerInfo.BillToCountry);
                sb.AppendFormat(formatStr, customerInfo.BillToPostalCode);
                sb.AppendFormat(formatStr, customerInfo.BillToState);
                sb.AppendFormat(formatStr, customerInfo.PrimaryPhone);
                sb.AppendFormat(formatStr, customerInfo.CellPhone);
                sb.AppendFormat(formatStr, customerInfo.Email);
                sb.AppendFormat(dateFormat, customerInfo.LastVisit);
                sb.AppendFormat(moneyFormat, customerInfo.AverageJob);
                sb.AppendFormat(formatStr, customerInfo.JobCount);
                sb.AppendFormat(moneyFormat, customerInfo.TotalSales);
                sb.AppendFormat(moneyFormat, customerInfo.Balance);
                sb.AppendFormat(moneyFormat, customerInfo.Payments);

                sb.Append("</row>");

                uid++;
            }

            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadCustomerInformationExcel(bool bMaxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (bMaxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "MyCustomerInformationReport.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetCustomers(franchiseId, from, to);

                xml = GetCustomerInformationXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "MyCustomerInformationReport.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "MyCustomerInformationReport.xlsx");
            }
        }

        #endregion

        #region AccountSummary

        public ActionResult AccountingSummary(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var selected = 0;
            if (timeperiod == "date")
                timeperiod = "-1";

            if (timeperiod != "date")
                selected = Convert.ToInt32(timeperiod);

            var model = new ReportSearch
            {
                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timeperiod
            };

            return View(model);
        }

        public JsonResult GetAccountingSummary(int frid, DateTime from, DateTime to, string timeperiod)
        {
            switch (timeperiod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetAccountSummary(frid, from, to);

            var sb = new StringBuilder();
            sb = GetAccountingSummaryXML(result);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetAccountingSummaryXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetAccountingSummaryXML(Account[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int counter = 0;

            var sb = new StringBuilder();
            sb.Append("<rows>");

            foreach (var Accountsummary in result)
            {
                if (maxRecords > 0 && counter > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:d}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.Append("<row id='" + Accountsummary.uID + "'>");
                sb.AppendFormat(formatStr, Accountsummary.AccountCode);
                sb.AppendFormat(formatStr, Accountsummary.AccountName);
                sb.AppendFormat(formatStr, Accountsummary.JobCount);
                sb.AppendFormat(moneyFormat, Accountsummary.TotalSales);
                sb.AppendFormat(formatStr, Accountsummary.ServiceName);
                sb.AppendFormat(formatStr, Accountsummary.BusinessType);

                if (Accountsummary.FirstJobDate.HasValue)
                    sb.AppendFormat(dateFormat, Accountsummary.FirstJobDate.Value);
                else
                    sb.AppendFormat(formatStr, "");

                if (Accountsummary.LastJobDate.HasValue)
                    sb.AppendFormat(dateFormat, Accountsummary.LastJobDate.Value);
                else
                    sb.AppendFormat(formatStr, "");
                if (Accountsummary.WSRCompletedDate.HasValue)
                    sb.AppendFormat(dateFormat, Accountsummary.WSRCompletedDate.Value);
                else
                    sb.AppendFormat(formatStr, "");

                sb.Append("</row>");

                counter++;
            }

            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadAccountSummaryExcel(bool bMaxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (bMaxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "MyAccount.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetAccountSummary(franchiseId, from, to);

                xml = GetAccountingSummaryXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "MyAccount.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "MyAccount.xlsx");
            }
        }

        #endregion

        #region Memberships

        public ActionResult Memberships(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var selected = 0;
            if (timeperiod == "date")
                timeperiod = "-1";

            if (timeperiod != "date")
                selected = Convert.ToInt32(timeperiod);

            var model = new ReportSearch
            {
                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timeperiod
            };
            return View(model);
        }

        public JsonResult GetMemberships(int frid, DateTime from, DateTime to, string timeperiod)
        {
            switch (timeperiod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetMemberships(frid, from, to);

            var sb = new StringBuilder();
            sb = GetMembershipsXML(result);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetMembershipsXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetMembershipsXML(Memberships[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int counter = 0;

            var sb = new StringBuilder();
            sb.Append("<rows>");

            foreach (var memberInfo in result)
            {
                if (maxRecords > 0 && counter > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:d}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.Append("<row id='" + memberInfo.MembershipID + "'>");
                sb.AppendFormat(formatStr, memberInfo.CustomerName + ((includeLinks) ? "^/OwnerPortal/AllCustomers/CustomerInformation?Custid=" + memberInfo.CustomerID : ""));
                sb.AppendFormat(formatStr, memberInfo.Email);
                sb.AppendFormat(formatStr, memberInfo.PrimaryPhone);
                sb.AppendFormat(formatStr, memberInfo.CellPhone);
                sb.AppendFormat(formatStr, memberInfo.BillToAddress);
                sb.AppendFormat(formatStr, memberInfo.BillToCity);
                sb.AppendFormat(formatStr, memberInfo.BillToState);
                sb.AppendFormat(formatStr, memberInfo.BillToPostalCode);
                sb.AppendFormat(formatStr, memberInfo.BillToCountry);
                sb.AppendFormat(formatStr, memberInfo.MemberType);
                if (memberInfo.MembershipStartDate.HasValue)
                    sb.AppendFormat(dateFormat, memberInfo.MembershipStartDate.Value);
                else
                    sb.AppendFormat(formatStr, "");

                if (memberInfo.MembershipEndDate.HasValue)
                    sb.AppendFormat(dateFormat, memberInfo.MembershipEndDate.Value);
                else
                    sb.AppendFormat(formatStr, "");

                sb.AppendFormat(formatStr, memberInfo.Req_TIPerYear);
                sb.AppendFormat(formatStr, memberInfo.InspectionCnt);

                if (memberInfo.LastDateInspected.HasValue)
                    sb.AppendFormat(dateFormat, memberInfo.LastDateInspected.Value);
                else
                    sb.AppendFormat(formatStr, "");

                if (memberInfo.LastCustomerVisit.HasValue)
                    sb.AppendFormat(dateFormat, memberInfo.LastCustomerVisit.Value);
                else
                    sb.AppendFormat(formatStr, "");

                sb.AppendFormat(formatStr, memberInfo.CountCustomerVisit);
                sb.AppendFormat(formatStr, memberInfo.JobCount);
                sb.AppendFormat(moneyFormat, memberInfo.TotalSales);
                sb.AppendFormat(moneyFormat, memberInfo.AverageJob);
                sb.AppendFormat(moneyFormat, memberInfo.Payments);
                sb.AppendFormat(moneyFormat, memberInfo.Balance);
                sb.Append("</row>");

                counter++;
            }

            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadMembershipsExcel(bool maxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (maxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);
                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "MyMember.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetMemberships(franchiseId, from, to);

                xml = GetMembershipsXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "MyMember.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "MyMember.xlsx");
            }
        }

        #endregion

        #region Accounting Detail

        public ActionResult AccountingDetail(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var selected = 0;
            if (timeperiod == "date")
                timeperiod = "-1";

            if (timeperiod != "date")
                selected = Convert.ToInt32(timeperiod);

            var model = new ReportSearch
            {
                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timeperiod
            };
            return View(model);
        }

        public JsonResult GetAccountingDetail(int frid, DateTime from, DateTime to, string timeperiod)
        {
            switch (timeperiod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetAccountingDetail(frid, from, to);

            var sb = new StringBuilder();
            sb = GetAccountingDetailXML(result);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetAccountingDetailXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetAccountingDetailXML(AccountDetail[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int counter = 0;

            var sb = new StringBuilder();
            sb.Append("<rows>");

            foreach (var accountInfo in result)
            {
                if (maxRecords > 0 && counter > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:d}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.Append("<row id='" + accountInfo.uID + "'>");
                sb.AppendFormat(formatStr, accountInfo.AccountCode);
                sb.AppendFormat(formatStr, accountInfo.AccountName);
                sb.AppendFormat(formatStr, accountInfo.JobCount + ((includeLinks) ? ("^/Ownerportal/invoice/invoicedetails?JobId=" + accountInfo.JobCount) : ""));
                sb.AppendFormat(moneyFormat, accountInfo.TotalSales);
                sb.AppendFormat(formatStr, accountInfo.ServiceName);
                sb.AppendFormat(formatStr, accountInfo.BusinessType);

                if (accountInfo.JobDate.HasValue)
                    sb.AppendFormat(dateFormat, accountInfo.JobDate.Value);
                else
                    sb.AppendFormat(formatStr, "");

                if (accountInfo.WSRCompletedDate.HasValue)
                    sb.AppendFormat(dateFormat, accountInfo.WSRCompletedDate.Value);
                else
                    sb.AppendFormat(formatStr, "");

                sb.Append("</row>");

                counter++;
            }

            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadAccountingDetailExcel(bool bMaxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (bMaxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "AccountDetail.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetAccountingDetail(franchiseId, from, to);

                xml = GetAccountingDetailXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "AccountDetail.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "AccountDetail.xlsx");
            }
        }

        #endregion

        #region Part Usage Per Job

        public ActionResult JobTaskPartInformation(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var selected = 0;
            if (timeperiod == "date")
                timeperiod = "-1";

            if (timeperiod != "date")
                selected = Convert.ToInt32(timeperiod);

            var model = new ReportSearch()
            {

                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timeperiod
            };

            return View(model);
        }

        public JsonResult GetJobTaskPartDetails(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            switch (timeperiod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetJobTaskPart(franchiseId, from, to);

            var sb = new StringBuilder();
            sb = GetJobTaskPartUsageXML(result);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetJobTaskPartUsageXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetJobTaskPartUsageXML(JobTaskPartUsage[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int uid = 0;

            StringBuilder sb = new StringBuilder();

            sb.Append("<rows>");

            foreach (var JobInfo in result)
            {
                if (maxRecords > 0 && uid > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:d}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.AppendFormat("<row id='{0}'>", uid);
                sb.AppendFormat(formatStr, JobInfo.TicketNumber + ((includeLinks) ? ("^/Ownerportal/invoice/invoicedetails?JobId=" + JobInfo.TicketNumber) : ""));
                sb.AppendFormat(formatStr, JobInfo.CustomerName);
                sb.AppendFormat(formatStr, JobInfo.JobAddress);
                sb.AppendFormat(formatStr, JobInfo.JobCity);
                sb.AppendFormat(formatStr, JobInfo.JobPostalCode);
                sb.AppendFormat(formatStr, JobInfo.JobState);
                sb.AppendFormat(formatStr, JobInfo.PartCode);
                sb.AppendFormat(formatStr, JobInfo.PartName);
                sb.AppendFormat(formatStr, JobInfo.PartQty);
                sb.AppendFormat(formatStr, JobInfo.ServiceName);
                sb.Append(!JobInfo.CallCompleted.HasValue ? string.Format(formatStr, string.Empty) : string.Format(dateFormat, JobInfo.CallCompleted.Value));
                sb.AppendFormat(formatStr, JobInfo.BusinessType);
                sb.AppendFormat(formatStr, JobInfo.Status);
                sb.AppendFormat(moneyFormat, JobInfo.PartTotalCost);
                sb.AppendFormat(moneyFormat, JobInfo.PartTotalPrice);
                sb.AppendFormat(moneyFormat, JobInfo.PartUnitPrice);
                sb.Append("</row>");
                uid++;
            }
            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadJobTaskPartUsageExcel(bool bMaxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (bMaxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "MyJobTaskPartUsageReport.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetJobTaskPart(franchiseId, from, to);

                xml = GetJobTaskPartUsageXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "MyJobTaskPartUsageReport.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "MyJobTaskPartUsageReport.xlsx");
            }
        }

        #endregion

        #region "WSR Report"

        public ActionResult WSRReport(int? franchiseID, DateTime? from, DateTime? to, string timePeriod)
        {
            var selected = 0;
            if (timePeriod != "-1")
            {
                selected = Convert.ToInt32(timePeriod);
            }
            var TimePeriod = new List<SelectListItem>();
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Week",
                Value = "0",
                Selected = selected == 0 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Week",
                Value = "1",
                Selected = selected == 1 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Month",
                Value = "2",
                Selected = selected == 2 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Month",
                Value = "3",
                Selected = selected == 3 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Year",
                Value = "4",
                Selected = selected == 4 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "The beginning of time",
                Value = "5",
                Selected = selected == 5 ? true : false
            });
            switch (timePeriod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.Value.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }
            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetWSRDates(franchiseID, from, to);
            var WSRDatesList = new SelectList(result);

            var model = new WSRReportViewModel
            {
                FranchiseID = franchiseID.HasValue ? franchiseID.Value : UserInfo.CurrentFranchise.FranchiseID,
                FromDate = from.HasValue ? from.Value : DateTime.Today,
                ToDate = to.HasValue ? to.Value : DateTime.Today,
                TimePeriod = TimePeriod,
                WSRDates = WSRDatesList,
                Range = timePeriod
            };

            return View(model);
        }

        public JsonResult GetWSRReport(int franchiseID, DateTime from, DateTime to, string timePeriod)
        {
            switch (timePeriod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            decimal? subtotal = 0;
            decimal? taxtotal = 0;
            decimal? jobtotal = 0;

            var reportData = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetWSRReport(franchiseID, from, to);

            var sb = new StringBuilder();
            sb = GetWSRReportXML(reportData, out subtotal, out taxtotal, out jobtotal);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new
                {
                    wsrlist = sb.ToString(),
                    subtotal = String.Format("{0:C}", subtotal),
                    taxtotal = String.Format("{0:C}", taxtotal),
                    jobtotal = String.Format("{0:C}", jobtotal),
                    maxLimitExceeded = false
                });
                //return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetWSRReportXML(reportData, out subtotal, out taxtotal, out jobtotal, 1000);
                return Json(new
                {
                    wsrlist = sb.ToString(),
                    subtotal = String.Format("{0:C}", subtotal),
                    taxtotal = String.Format("{0:C}", taxtotal),
                    jobtotal = String.Format("{0:C}", jobtotal),
                    maxLimitExceeded = true
                });
                //return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }

            //var fee = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetWSRFee(franchiseID, from, to);
        }

        public JsonResult getDates(int franchiseId, DateTime? from, DateTime? to, string timePeriod)
        {
            switch (timePeriod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.Value.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetWSRDates(franchiseId, from, to);
            var WSRDatesList = new SelectList(result);
            return Json(result);
        }

        private StringBuilder GetWSRReportXML(WSR[] result, out decimal? subtotal, out decimal? taxtotal, out decimal? jobtotal, int maxRecords = 0, bool includeLinks = true)
        {
            subtotal = 0;
            taxtotal = 0;
            jobtotal = 0;

            int counter = 0;

            var sb = new StringBuilder();

            sb.Append("<rows>");
            foreach (var item in result)
            {
                if (maxRecords > 0 && counter > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:D}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.AppendFormat("<row id='{0}'>", item.TicketNumber);
                sb.AppendFormat(formatStr, item.TicketNumber + ((includeLinks) ? ("^/Ownerportal/invoice/invoicedetails?JobId=" + item.TicketNumber) : ""));
                sb.AppendFormat(dateFormat, item.CallCompleted);
                sb.AppendFormat(dateFormat, item.WSRCompletedDate);
                sb.AppendFormat(formatStr, item.Status);
                sb.AppendFormat(formatStr, item.ServiceName);
                sb.AppendFormat(moneyFormat, item.TotalSales);
                sb.AppendFormat(moneyFormat, item.TaxAmount);
                sb.AppendFormat(moneyFormat, item.SubTotal);
                sb.AppendFormat(moneyFormat, item.Balance);
                sb.AppendFormat(formatStr, item.TaxDescription);
                sb.AppendFormat(formatStr, item.BusinessType);
                sb.AppendFormat(formatStr, item.CustomerName);
                sb.AppendFormat(formatStr, item.JobAddress);
                sb.AppendFormat(formatStr, item.JobCity);
                sb.AppendFormat(formatStr, item.JobState);
                sb.AppendFormat(formatStr, item.JobPostalCode);
                sb.AppendFormat(formatStr, item.ServiceProID + ((includeLinks) ? ("^/ownerportal/Employee/EmployeeInformation?id=" + item.ServiceProID + "&frid=" + item.ClientID) : ""));
                sb.AppendFormat(formatStr, item.JobPriority);
                sb.Append("</row>");

                subtotal += item.SubTotal;
                taxtotal += item.TaxAmount;
                jobtotal += item.TotalSales;

                counter++;
            }
            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadWSRReportToExcel(bool bMaxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod, string flag)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (bMaxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "WSRReport.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetWSRReport(franchiseId, from, to);

                decimal? subtotal = 0;
                decimal? taxtotal = 0;
                decimal? jobtotal = 0;

                xml = GetWSRReportXML(result, out subtotal, out taxtotal, out jobtotal, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "WSRReport.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "WSRReport.xlsx");
            }
        }

        #endregion

        /// <summary>
        /// Marging the headings into rows in grid xml.
        /// </summary>
        /// <param name="HeaderXml"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        private string AddHeaderToRowsInXML(string HeaderXml, string xml)
        {
            XmlElement root;
            XmlDocument dom = new XmlDocument();

            // Loading Column Headings from HeaderXml.
            dom.LoadXml(HeaderXml);
            root = dom.DocumentElement;
            var head = root.FirstChild;

            // Loading Rows from xml.
            dom.LoadXml(xml);
            root = dom.DocumentElement;

            // Insert Headings before Rows in xml.
            root.InsertBefore(head, root.FirstChild);

            return root.OuterXml;
        }

        /// <summary>
        /// This function is obsolete.
        /// </summary>
        /// <param name="HeaderXml"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public StringBuilder GenerateExportTable(string HeaderXml, string xml)
        {
            int k = 0;

            XmlElement root;
            XmlDocument dom = new XmlDocument();

            var table = new DataTable();
            DataColumn column;
            DataRow row;

            //==========================================================//
            // Loading Column Headings from grid_xml.
            //==========================================================//
            dom.LoadXml(HeaderXml);
            root = dom.DocumentElement;

            var columns = root.GetElementsByTagName("column");
            for (int i = 0; i < columns.Count; i++)
            {
                column = new DataColumn();
                column.ColumnName = columns[i].InnerText;
                table.Columns.Add(column);
            }

            //==========================================================//
            // Loading Rows from data records.
            //==========================================================//
            dom.LoadXml(xml);
            root = dom.DocumentElement;

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

            //==========================================================//
            // Generating HTML Table for Exporting Data to Excel.
            //==========================================================//
            var sb = new System.Text.StringBuilder();
            sb.Append("<table border='" + "2px" + "'b>");

            // write column headings
            sb.Append("<tr >");
            foreach (DataColumn col in table.Columns)
            {
                sb.Append("<td bgcolor =#E3EFFF ><b>" + col.ColumnName + "</b></td>");
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
                }
                sb.Append("</tr>");
                k++;
            }
            sb.Append("</table>");

            return sb;
        }


        #region[Customer Ledger Report]

        public ActionResult CustomerLedgerReport(int? franchiseID, DateTime? from, DateTime? to, string timePeriod)
        {
            var selected = 0;
            if (timePeriod != "-1")
            {
                selected = Convert.ToInt32(timePeriod);
            }
            var TimePeriod = new List<SelectListItem>();
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Week",
                Value = "0",
                Selected = selected == 0 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Week",
                Value = "1",
                Selected = selected == 1 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Month",
                Value = "2",
                Selected = selected == 2 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Month",
                Value = "3",
                Selected = selected == 3 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Year",
                Value = "4",
                Selected = selected == 4 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "The beginning of time",
                Value = "5",
                Selected = selected == 5 ? true : false
            });
            var model = new ReportSearch
            {
                FranchiseID = franchiseID.HasValue == true ? franchiseID.Value : UserInfo.CurrentFranchise.FranchiseID,
                FromDate = from.HasValue == true ? from.Value : DateTime.Today,
                ToDate = to.HasValue == true ? to.Value : DateTime.Today,
                TimePeriod = TimePeriod,
                Range = timePeriod
            };
            return View(model);
        }

        public JsonResult GetCustomerLedger(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            switch (timeperiod)
            {
                case "0":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                    to = DateTime.Now;
                    break;
                case "1":
                    from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                    to = from.AddDays(6);
                    break;
                case "2":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                    break;
                case "3":
                    from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    break;
                case "4":
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                    to = DateTime.Now;
                    break;
                case "5":
                    from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                    to = DateTime.Now;
                    break;
            }

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetCustomerLedger(franchiseId, from, to);

            var sb = new StringBuilder();
            sb = GetCustomerLedgerXML(result);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                sb = GetCustomerLedgerXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetCustomerLedgerXML(CustomerLedger[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int uid = 0;

            StringBuilder sb = new StringBuilder();

            sb.Append("<rows>");

            foreach (var rec in result)
            {
                if (maxRecords > 0 && uid > maxRecords)
                    break;

                const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                const string dateFormat = "<cell><![CDATA[{0:D}]]></cell>";
                const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

                sb.AppendFormat("<row id='{0}'>", uid);
                sb.AppendFormat(formatStr, rec.Name + ((includeLinks) ? ("^/OwnerPortal/AllCustomers/CustomerInformation?Custid=" + rec.Id) : ""));
                sb.AppendFormat(formatStr, rec.Email);
                sb.AppendFormat(formatStr, rec.PrimaryPhone);
                sb.AppendFormat(formatStr, rec.CellPhone);
                sb.AppendFormat(dateFormat, rec.BillToAddress);
                sb.AppendFormat(formatStr, rec.BillToCity);

                sb.AppendFormat(formatStr, rec.BillToState);
                sb.AppendFormat(formatStr, rec.BillToPostalCode);
                sb.AppendFormat(formatStr, rec.BillToCountry);
                sb.AppendFormat(dateFormat, rec.LastVisit);
                sb.AppendFormat(formatStr, rec.Type);
                sb.AppendFormat(formatStr, rec.Sequence);
                sb.AppendFormat(formatStr, rec.Amount);
                sb.AppendFormat(moneyFormat, rec.ActualBalance);
                sb.AppendFormat(moneyFormat, rec.StoredBalance);
                sb.AppendFormat(moneyFormat, rec.RunningBalance);
                sb.AppendFormat(dateFormat, rec.RecordedDate);
                sb.AppendFormat(formatStr, rec.IsOutstanding);
                sb.Append("</row>");
                uid++;
            }
            sb.Append("</rows>");

            return sb;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadCustomerLedgerExcel(bool bMaxLimitExceeded, int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
            if (bMaxLimitExceeded == false)
            {
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "MyCustomerLedgerReport.xlsx");
            }
            else
            {
                var HeaderXml = this.Request.Form["grid_xml"];
                HeaderXml = this.Server.UrlDecode(HeaderXml);

                switch (timeperiod)
                {
                    case "0":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
                        to = DateTime.Now;
                        break;
                    case "1":
                        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
                        to = from.AddDays(6);
                        break;
                    case "2":
                        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        to = DateTime.Now;
                        break;
                    case "3":
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case "4":
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Now;
                        break;
                    case "5":
                        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
                        to = DateTime.Now;
                        break;
                }

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetCustomerLedger(franchiseId, from, to);

                xml = GetCustomerLedgerXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "MyCustomerLedgerReport.xlsx");

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(GenerateExportTable(HeaderXml, xml).ToString());

                //return File(buffer, generator.ContentType, "MyCustomerLedgerReport.xlsx");
            }
        }

        #endregion

        public ActionResult CustomerLedgerSummaryReport(int? franchiseID, DateTime? from, DateTime? to, string timePeriod)
        {
            var selected = 0;
            if (timePeriod != "-1")
            {
                selected = Convert.ToInt32(timePeriod);
            }
            var TimePeriod = new List<SelectListItem>();
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Week",
                Value = "0",
                Selected = selected == 0 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Week",
                Value = "1",
                Selected = selected == 1 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Month",
                Value = "2",
                Selected = selected == 2 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "Last Month",
                Value = "3",
                Selected = selected == 3 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "This Year",
                Value = "4",
                Selected = selected == 4 ? true : false
            });
            TimePeriod.Add(new SelectListItem
            {
                Text = "The beginning of time",
                Value = "5",
                Selected = selected == 5 ? true : false
            });
            var model = new ReportSearch
            {
                FranchiseID = franchiseID.HasValue == true ? franchiseID.Value : UserInfo.CurrentFranchise.FranchiseID,
                FromDate = from.HasValue == true ? from.Value : DateTime.Today,
                ToDate = to.HasValue == true ? to.Value : DateTime.Today,
                TimePeriod = TimePeriod,
                Range = timePeriod
            };
            return View(model);
        }


        public JsonResult GetCustomerSummaryLedger(int franchiseId, DateTime from, DateTime to, string timeperiod)
        {
            int strMaxRecord = 0;
            //switch (timeperiod)
            //{
            //    case "0":
            //        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday));
            //        to = DateTime.Now;
            //        break;
            //    case "1":
            //        from = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Sunday + 7));
            //        to = from.AddDays(6);
            //        break;
            //    case "2":
            //        from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //        to = DateTime.Now;
            //        break;
            //    case "3":
            //        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            //        to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            //        break;
            //    case "4":
            //        from = new DateTime(DateTime.Today.Year, 1, 1);
            //        to = DateTime.Now;
            //        break;
            //    case "5":
            //        from = new DateTime(DateTime.Today.Year - 10, 1, 1);
            //        to = DateTime.Now;
            //        break;
            //}

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).getCustomerLedgerInfo(franchiseId);

            //var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).getCustomerLedgerInfo(franchiseId);

            var sb = new StringBuilder();
            sb = GetCustomerLedgerSummaryXML(result, strMaxRecord, true);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var test = serializer.Serialize(sb.ToString());
                return Json(new { data = sb.ToString(), maxLimitExceeded = false });
            }
            catch (Exception)
            {
                // sb = GetCustomerLedgerSummaryXML(result, 1000);
                return Json(new { data = sb.ToString(), maxLimitExceeded = true });
            }
        }

        private StringBuilder GetCustomerLedgerSummaryXML(CustomerLedgerSummary[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int uid = 0;

            StringBuilder sb = new StringBuilder();

            sb.Append("<rows>");

            foreach (var rec in result)
            {
                if (maxRecords > 0 && uid > maxRecords)
                    break;
                 const string formatStr = "<cell><![CDATA[{0}]]></cell>";
                sb.AppendFormat("<row id='{0}'>", uid);
                sb.AppendFormat(formatStr, rec.CustomerId);
                sb.AppendFormat(formatStr, rec.CustomerName + ((includeLinks) ? ("^/OwnerPortal/AllCustomers/CustomerInformation?Custid=" + rec.CustomerId) : ""));
               // sb.AppendFormat(formatStr, rec.CustomerId + (includeLinks ? ("^/Reporting/Reporting/ShowPdf?Custid=" + rec.CustomerId + "&Invoices=" + rec.Invoices + "&payments=" + rec.payments + "&outBalance=" + rec.OutstandingBalance) : ""));                
                sb.AppendFormat(formatStr, rec.ClientId);
                sb.AppendFormat(formatStr, string.Format("{0:C}",rec.Invoices));
                sb.AppendFormat(formatStr, string.Format("{0:C}",rec.payments));
                sb.AppendFormat(formatStr,string.Format("{0:C}", rec.OutstandingBalance));
                sb.Append("</row>");
                uid++;
            }
            sb.Append("</rows>");

            return sb;
        }

        public ActionResult ShowPdf(int custId)
        {
            var model = new CustmerInformationViewModel
             {
                 CustomerId = custId,
                };
            return View(model);
        }


        public JsonResult getLedgerDetails(int CustmerId)
        {
            const string formatStr = "<cell><![CDATA[{0}]]></cell>";
            using (var ctx = new ReportingEntities())
            {
                StringBuilder sb = new StringBuilder();
                IQueryable<vRpt_CustomerLedger> qry = ctx.vRpt_CustomerLedger;                
                qry = qry.Where(c => c.CustomerId == CustmerId);
                
                int uid = 0;
                sb.Append("<rows>");
                foreach (var rec in qry)
                {
                    sb.AppendFormat("<row id='{0}'>", uid);
                    sb.AppendFormat(formatStr, rec.ReferenceId);
                    sb.AppendFormat(formatStr, rec.Type);
                    sb.AppendFormat(formatStr, string.Format("{0:C}", rec.Amount));
                    sb.AppendFormat(formatStr, string.Format("{0:C}", rec.ActualBalance));
                    sb.AppendFormat(formatStr, string.Format("{0:C}",rec.StoredBalance));
                    sb.AppendFormat(formatStr, rec.RecordedDate);
                    sb.AppendFormat(formatStr, string.Format("{0:C}",rec.IsOutstanding));
                    sb.AppendFormat(formatStr, string.Format("{0:C}",rec.RunningBalance));
                    sb.Append("</row>");
                    uid++;
                }
                sb.Append("</rows>");
                return Json(new { data = sb.ToString() });
            }
        }

        public JsonResult getCustomerDetails(int CustomerId)
        {
           // const string formatStr = "<cell><![CDATA[{0}]]></cell>";
             using (var ctx = new ReportingEntities())
            {
                StringBuilder sb = new StringBuilder();
                var qry= AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetCustomersInfo(CustomerId);
                
                int uid = 0;
                foreach (var rec in qry)
                {
                sb.Append(rec.Name);
                uid++;
                }
                
                return Json(new {data = sb.ToString()});
             }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadCustomerLedgerSummaryExcel()
        {
            var generator = new ExcelWriter();

            var xml = string.Empty;
           
                xml = this.Request.Form["grid_xml"];
                xml = this.Server.UrlDecode(xml);

                var stream = generator.Generate(xml);
                return File(stream.ToArray(), generator.ContentType, "MyCustomerLedgerReport.xlsx");
            
        }


      
    }
}
