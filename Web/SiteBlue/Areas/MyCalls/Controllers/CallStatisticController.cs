using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHTMLX.Export.Excel;
using SiteBlue.Areas.MyCalls.Models;
using SiteBlue.Business;
using SiteBlue.Controllers;
using SiteBlue.Data;
using System.Data.Linq;
using System.Web.Security;

namespace SiteBlue.Areas.MyCalls.Controllers
{
    [Authorize]
    public class CallStatisticController : SiteBlueBaseController
    {
        //
        // GET: /MyCalls/CallStatistic/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadDetailStatistic()
        {
            var generator = new ExcelWriter();
            var xml = Request.Form["grid_xml"];
            xml = Server.UrlDecode(xml);
            var stream = generator.Generate(xml);

            return File(stream.ToArray(), generator.ContentType, "Statistic.xlsx");
        }

        public ActionResult DownloadFullStatistic(int? fr)
        {
            var generator = new ExcelWriter();
            var context = new IncomingCallsQAEntities();

            var statisticlist = (from s in context.StatisticTracks
                                 join ls in context.LookupScripts on s.CalledNumber equals ls.LookupPhoneNr into tls
                                 from ls in tls.DefaultIfEmpty()
                                 join eo in context.EndCallOptions on s.OptionId equals eo.OptionId into teo
                                 from eo in teo.DefaultIfEmpty()
                                 where s.CalledNumber != ""
                                 select new
                                 {
                                     seqnr = s.TrackId,
                                     calltime = s.StartDate,
                                     s.CalledNumber,
                                     s.DialedNumber,
                                     s.Duration,
                                     s.Jobid,
                                     s.UserId,
                                     ls.XMLCode,
                                     eo.OptionName
                                 }).OrderBy(q => q.seqnr);

            var xml = "<rows><head><columns>";
            xml += "<column width='50' type='ch' align='center' color='white' sort='int'>Sequence</column>";
            xml += "<column width='150' type='ch' align='center' color='white' sort='date'>Call Time</column>";
            xml += "<column width='100' type='ch' align='center' color='white' sort='str'>Called Number</column>";
            xml += "<column width='100' type='ch' align='center' color='white' sort='str'>Dialed Number</column>";
            xml += "<column width='80' type='ch' align='center' color='white' sort='str'>Duration</column>";
            xml += "<column width='80' type='ch' align='center' color='white' sort='str'>JobID</column>";
            xml += "<column width='100' type='ch' align='center' color='white' sort='str'>User Name</column>";
            xml += "<column width='100' type='ch' align='center' color='white' sort='str'>XML Code</column>";
            xml += "<column width='100' type='ch' align='center' color='white' sort='str'>Option Name</column></columns></head>";

            foreach (var row in statisticlist)
            {
                xml += "<row id='" + row.seqnr + "'>";
                xml += "<cell><![CDATA[" + row.seqnr + "]]></cell>";
                xml += "<cell><![CDATA[" + row.calltime + "]]></cell>";
                xml += "<cell><![CDATA[" + row.CalledNumber + "]]></cell>";
                xml += "<cell><![CDATA[" + row.DialedNumber + "]]></cell>";
                xml += "<cell><![CDATA[" + row.Duration + "]]></cell>";
                xml += "<cell><![CDATA[" + row.Jobid + "]]></cell>";
                xml += "<cell><![CDATA[" + Membership.GetUser(row.UserId).UserName + "]]></cell>";
                xml += "<cell><![CDATA[" + row.XMLCode + "]]></cell>";
                xml += "<cell><![CDATA[" + row.OptionName + "]]></cell>";
                xml += "</row>";
            }
            xml += "</rows>";

            var stream = generator.Generate(xml);

            return File(stream.ToArray(), generator.ContentType, "FullStatistic.xlsx");
        }

        public ActionResult GetInfoForGrid(DateTime? start, DateTime? end, int? fr, int? opt)
        {
            if (!start.HasValue)
                start = new DateTime(2011, 1, 1);

            if (!end.HasValue)
                end = DateTime.Now;

            var franchisesId = GetFranchisesID(fr ?? -1);
            
            var endCallOptions = GetEndCallOptions();
            
            var dic = new Dictionary<InfoNumber, List<KeyValuePair<string, int>>>();

            if (opt != null && opt.Value == 2)
                dic = GetDicOfCallTalkerStat(start, end, franchisesId);
            else
                dic = GetDicOfPhoneStat(start, end, franchisesId);

            var model = new GridModel()
            {
                GridData = dic,
                opt = opt,
                Header = new GridHeader() { Options = endCallOptions }
            };

            return View(model);
        }

        private List<int> GetFranchisesID(int fr)
        {
            var franchisesId = new List<int>();

            if (fr == 0)
                franchisesId = UserInfo.Franchises.Select(i => i.FranchiseID).ToList();
            else
                franchisesId.Add(UserInfo.CurrentFranchise.FranchiseID);

            return franchisesId;
        }

        private Dictionary<InfoNumber, List<KeyValuePair<string, int>>> GetDicOfPhoneStat(DateTime? start, DateTime? end, ICollection<int> franchisesId)
        {
            var context = new IncomingCallsQAEntities();

            var EmptyGuid = Guid.Empty;

            var numbersInfos =
                context.Tbl_scriptToFranchiseID.Where(item => franchisesId.Contains(item.FranchiseID)).Select(
                    item =>
                    new InfoNumber
                        {
                            Description = item.LookupScript.XMLCode,
                            PhoneNumber = item.LookupScript.LookupPhoneNr.Remove(0, 1),
                            UserId = EmptyGuid
                        }).
                    ToList();

            var numbers = numbersInfos.Select(i => i.PhoneNumber);
            
            var dic = new Dictionary<InfoNumber, List<KeyValuePair<string, int>>>();
            var stat =
                context.StatisticTracks.Where(
                    item => numbers.Contains(item.CalledNumber) && item.StartDate >= start.Value && item.StartDate < end.Value).
                    GroupBy(i => i.CalledNumber).
                    ToList();

            
            foreach (var number in stat)
            {
                dic.Add(numbersInfos.First(i => i.PhoneNumber == number.Key),
                        number.GroupBy(i => i.EndCallOption != null ? i.EndCallOption.OptionName : "Unknow").Select(
                            i => new KeyValuePair<string, int>(i.Key, i.Count())).ToList());
            }

            return dic;
        }

        private Dictionary<InfoNumber, List<KeyValuePair<string, int>>> GetDicOfCallTalkerStat(DateTime? start, DateTime? end, ICollection<int> franchisesId)
        {
            var context = new IncomingCallsQAEntities();

            var EmptyGuid = Guid.Empty;

            var PhoneNumbers = 
                context.Tbl_scriptToFranchiseID.Where(item => franchisesId.Contains(item.FranchiseID)).Select(
                    item =>
                    new InfoNumber
                    {
                        Description = item.LookupScript.XMLCode,
                        PhoneNumber = item.LookupScript.LookupPhoneNr.Remove(0, 1),
                        UserId = EmptyGuid
                    }).
                    ToList();

            var objmodcommon = new mod_common(new Guid());

            var numbersInfos =
                PhoneNumbers.Select(
                    item =>
                    new InfoNumber
                    {
                        Description = item.Description,
                        PhoneNumber = item.PhoneNumber,
                        UserId = objmodcommon.GetCallTalkerUserID(item.PhoneNumber)
                    }).
                    ToList();

            var users = numbersInfos.Select(i => i.UserId);

            var dic = new Dictionary<InfoNumber, List<KeyValuePair<string, int>>>();
            var stat =
                context.StatisticTracks.Where(
                    item => users.Contains(item.UserId) && item.StartDate >= start.Value && item.StartDate < end.Value).
                    GroupBy(i => i.UserId).
                    ToList();

            foreach (var user in stat)
            {
                dic.Add(numbersInfos.First(i => i.UserId.ToString() == user.Key.ToString()),
                        user.GroupBy(i => i.EndCallOption != null ? i.EndCallOption.OptionName : "Unknow").Select(
                            i => new KeyValuePair<string, int>(i.Key, i.Count())).ToList());
            }
            
            return dic;
        }

        private static List<string> GetEndCallOptions()
        {
            var context = new IncomingCallsQAEntities();

            var endCallOptions = context.EndCallOptions.Select(i => i.OptionName).ToList();
            endCallOptions.Add("Unknow");

            return endCallOptions;
        }
    }
}
