using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Controllers;
using SiteBlue.Data.EightHundred;
using System.Text;
using DHTMLX.Export.Excel;

namespace SiteBlue.Areas.Admin.Controllers
{
    [Authorize(Roles = "Corporate")]
    public class ZipListController : SiteBlueBaseController
    {
        private EightHundredEntities db = new EightHundredEntities();

        // GET: /Admin/ZipList/

        public ActionResult Index()
        {
            return View();
        }

        // POST: /Admin/GetZipList/

        [HttpPost]
        public JsonResult GetZipList(int? franchiseID)
        {
            try
            {
                var ziplist = (from item in db.tbl_Franchise_ZipList
                               where (item.FranchiseID == (franchiseID.HasValue ? franchiseID.Value : UserInfo.CurrentFranchise.FranchiseID))
                               && item.ActiveYN==true
                               select item);

                var sb = new StringBuilder();
                sb.Append("<rows>");
                foreach (var item in ziplist)
                {
                    const string stringFormat = "<cell><![CDATA[{0}]]></cell>";
                    const string dateFormat = "<cell><![CDATA[{0:D}]]></cell>";
                   
                    sb.AppendFormat("<row id='{0}'>", item.ZipID);

                    sb.AppendFormat(stringFormat, item.FranchiseZipID);
                    sb.AppendFormat(dateFormat, item.DateAdded);
                    sb.AppendFormat(stringFormat, item.OwnedYN ? 1 : 0);
                    sb.AppendFormat(stringFormat, item.ServicesYN ? 1 : 0);
                    sb.AppendFormat(stringFormat, item.City);
                    sb.AppendFormat(stringFormat, item.State);
                    sb.AppendFormat(stringFormat, item.Country);
                    sb.AppendFormat(stringFormat, item.CallTakerMessage);
                    sb.AppendFormat(stringFormat, item.ZipID);

                    sb.Append("</row>");
                }
                sb.Append("</rows>");

                return Json(new
                {
                    Message = "",
                    ResultData = sb.ToString(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = ex.Message,
                    ResultData = "",
                    Success = false
                });
            }
        }

        [HttpPost]
        public ActionResult AddUpdateZipList(tbl_Franchise_ZipList[] data)
        {
            try
            {
                int? franchiseID = data[0].FranchiseID;

                for (int i = 0; i < data.Count(); i++)
                {
                    if (data[i].ZipID == 0)
                    {
                        data[i].DateAdded = DateTime.Now;
                        var encoding = new System.Text.ASCIIEncoding();
                        var timestamp = encoding.GetBytes(DateTime.Now.ToString());
                        data[i].timestamp = timestamp;
                        db.tbl_Franchise_ZipList.AddObject(data[i]);
                        db.SaveChanges();
                    }
                    else
                    {
                        var zipID = data[i].ZipID;
                        var zip = db.tbl_Franchise_ZipList.FirstOrDefault(z => z.ZipID == zipID);

                        if (zip != null)
                        {
                            zip.FranchiseZipID = data[i].FranchiseZipID;
                            zip.OwnedYN = data[i].OwnedYN;
                            zip.ServicesYN = data[i].ServicesYN;
                            zip.City = data[i].City;
                            zip.State = data[i].State;
                            zip.Country = data[i].Country;
                            zip.CallTakerMessage = data[i].CallTakerMessage;

                            db.SaveChanges();
                        }
                    }
                }

                return Json(new
                {
                    Message = "Changes Saved Successfully.",
                    ResultData = "",
                    Success = true
                });

                //var ziplist = db.tbl_Franchise_ZipList.Where(z => z.FranchiseID == franchiseID).ToList();

                //var result = from z in ziplist
                //             select new
                //             {
                //                 ZipID = z.ZipID,
                //                 FranchiseZipID = z.FranchiseZipID,
                //                 DateAdded = z.DateAdded,
                //                 OwnedYN = z.OwnedYN,
                //                 ServicesYN = z.ServicesYN,
                //                 City = z.City,
                //                 State = z.State,
                //                 Country = z.Country,
                //                 CallTakerMessage = z.CallTakerMessage
                //             };

                //return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = ex.Message,
                    ResultData = "",
                    Success = false
                });
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadZipList()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "FranchiseZipList.xlsx");
        }

        public ActionResult DeleteZipList(int Id)
        {
            var zip = db.tbl_Franchise_ZipList.FirstOrDefault(z => z.ZipID == Id);
            if (zip != null)
            {
                zip.ActiveYN = false;
                zip.DateRemoved = DateTime.Now;
                db.SaveChanges();
            }
            return Json(new
            {
                Message = "Changes Saved Successfully.",
                ResultData = "",
                Success = true
            });
        }


    }
}