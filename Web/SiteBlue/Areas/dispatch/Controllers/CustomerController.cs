using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.Dispatch.Models;
using System.Data.Linq.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using System.Web.Helpers;
using SiteBlue.Data.EightHundred;
using OpenExcel.OfficeOpenXml;
using DHTMLX.Export.Excel;
using Interop.QBFC10;
using System.Web.Routing;


namespace SiteBlue.Areas.dispatch.Controllers
{
    public class CustomerController : Controller
    {
        // GET: /dispatch/Customer/
        EightHundredEntities db = new EightHundredEntities();
        public ActionResult Index(FormCollection formcollection)
        {
            int MyFranchiseID = 38;
            ViewBag.FrenchiseID = MyFranchiseID;
            var f = (from frn in db.tbl_Franchise select frn).ToList();
            ViewBag.frenchise = f;
            return View(new tbl_Employee[] {});

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadMyFinances()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "MyFinances.xlsx");
        }
    }
}
