using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.Admin.Models;
using SiteBlue.Controllers;
using SiteBlue.Business;
using SiteBlue.Business.Reporting;
using System.Text;
using DHTMLX.Export.Excel;
using System.Data;
using System.Xml;

namespace SiteBlue.Areas.Admin.Controllers
{
    [Authorize(Roles = "Corporate")]
    public class ReportingController : SiteBlueBaseController
    {
        private const string formatStr = "<cell><![CDATA[{0}]]></cell>";
        private const string dateFormat = "<cell><![CDATA[{0:D}]]></cell>";
        private const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";

        public ActionResult Index()
        {
            var timePeriod = new List<SelectListItem>();
            timePeriod.Add(new SelectListItem
            {
                Text = "This Week",
                Value = "0",
                Selected = true
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "Last Week",
                Value = "1"
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "This Month",
                Value = "2"
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "Last Month",
                Value = "3"
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "This Year",
                Value = "4"
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "The beginning of time",
                Value = "5"
            });

            var timespan = new System.TimeSpan(7, 0, 0, 0);

            var model = new ReportViewModel
            {
                FromDate = DateTime.Now.Subtract(timespan),
                ToDate = DateTime.Now.Subtract(timespan),
                TimePeriod = timePeriod
            };

            return View(model);
        }

        #region "Detailed Call Stats"

        public ActionResult DetailedCallStats(int franchiseId, DateTime from, DateTime to, string timePeriod)
        {
            var selected = 0;
            if (timePeriod != "-1")
                selected = Convert.ToInt32(timePeriod);

            var model = new ReportViewModel
            {
                FranchiseID = franchiseId,
                FromDate = from,
                ToDate = to,
                TimePeriod = GetTimerPeriodList(selected),
                Range = timePeriod
            };
            return View(model);
        }

        #endregion

        #region "Misc"

        public List<SelectListItem> GetTimerPeriodList(int selected)
        {
            var timePeriod = new List<SelectListItem>();

            timePeriod.Add(new SelectListItem
            {
                Text = "This week",
                Value = "0",
                Selected = selected == 0 ? true : false
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "Last week",
                Value = "1",
                Selected = selected == 1 ? true : false
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "This month",
                Value = "2",
                Selected = selected == 2 ? true : false
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "Last Month",
                Value = "3",
                Selected = selected == 3 ? true : false
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "This year",
                Value = "4",
                Selected = selected == 4 ? true : false
            });
            timePeriod.Add(new SelectListItem
            {
                Text = "The beginning of time",
                Value = "5",
                Selected = selected == 5 ? true : false
            });

            return timePeriod;
        }

        #endregion

        #region Detailed CallStats Report.

        public JsonResult GetCallStateInformation(DateTime from, DateTime to, string timeperiod)
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

            var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetCallStateReport(from, to);

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

        private StringBuilder GetJobInformationXML(CallStates[] result, int maxRecords = 0, bool includeLinks = true)
        {
            int uid = 0;

            var sb = new StringBuilder();
            sb.Append("<rows>");
            foreach (var rec in result)
            {
                if (maxRecords > 0 && uid > maxRecords)
                    break;

                sb.AppendFormat("<row id='{0}'>", uid++);
                sb.AppendFormat(formatStr, rec.UserName);
                sb.AppendFormat(formatStr, rec.CalledNo);
                sb.AppendFormat(dateFormat, rec.CallTime);
                sb.AppendFormat(formatStr, rec.JobId + ((includeLinks == true) ? ("^/Ownerportal/invoice/invoicedetails?JobId=" + rec.JobId) : ""));
                sb.AppendFormat(formatStr, rec.Duration);
                sb.AppendFormat(formatStr, rec.OptionName);
                sb.AppendFormat(formatStr, rec.CalledDescription);
                sb.Append("</row>");

                uid++;
            }
            sb.Append("</rows>");

            return sb;
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadJobInformationExcel(bool bMaxLimitExceeded,DateTime from, DateTime to, string timeperiod)
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

                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetCallStateReport(from, to);

                xml = GetJobInformationXML(result, includeLinks: false).ToString();

                var stream = generator.Generate(AddHeaderToRowsInXML(HeaderXml, xml));
                return File(stream.ToArray(), generator.ContentType, "MyDetailedCallStats.xlsx");    
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
    }
}