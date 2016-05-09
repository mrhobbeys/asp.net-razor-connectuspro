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
using SiteBlue.Data.EightHundred;


namespace SiteBlue.Areas.Admin.Controllers
{
    public class golivecheckController : SiteBlueBaseController
    {
        //
        // GET: /Admin/golivecheck/
        private const string formatStr = "<cell><![CDATA[{0}]]></cell>";
        private const string dateFormat = "<cell><![CDATA[{0:D}]]></cell>";
        private const string moneyFormat = "<cell><![CDATA[{0:C}]]></cell>";
        public ActionResult Index()
        {

            return View();
        }

        public JsonResult getlivecheck(int franchiseId)
        {
            
            using (var db = new EightHundredEntities())
            {
                var result = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).getOnlineCheck(franchiseId);
                int uid = 0;
                var sb = new StringBuilder();
                sb.Append("<rows>");
                foreach (var rec in result)
                {
                    sb.AppendFormat("<row id='{0}'>", uid++);
                    sb.AppendFormat(formatStr, rec.TestDescription);
                    sb.AppendFormat(formatStr, rec.PassFail);
                    sb.AppendFormat(dateFormat, rec.Comments);
                    sb.AppendFormat(formatStr, rec.resolution);
                    sb.Append("</row>");
                    uid++;
                }
                sb.Append("</rows>");

                return Json(new { data = sb.ToString() });
            }


        }


        [HttpPost]
        public ActionResult downloadOnlineCheckToExcel()
        {
            var generator = new ExcelWriter();
            var xml = string.Empty;
            xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "golivecheck.xlsx");
           
        }
     
    }
}
