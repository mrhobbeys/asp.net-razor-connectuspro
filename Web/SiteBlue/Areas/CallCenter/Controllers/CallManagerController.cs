using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.CallCenter.Models;
using SiteBlue.Data;
using SiteBlue.Business;
using SiteBlue.Business.Reporting;

namespace SiteBlue.Areas.CallCenter.Controllers
{
    [Authorize]
    public class CallManagerController : CallCenterController
    {
        //
        // GET: /CallCenter/CallManager/

        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)] //to support <unavailable> in the query string when a caller has a blocked number.
        public ActionResult ResolveACall(string id, string id2, bool? fullRender, int? calltracker)
        {
            ResolveCallViewModel vm;

            try
            {
                LookupScript lookupScript;
                var validTrack = false;

                using (var incomingContext = new IncomingCallsQAEntities())
                {
                    lookupScript = incomingContext.LookupScripts.Single(l => l.LookupPhoneNr == id);

                    var stat = incomingContext.StatisticTracks.SingleOrDefault(t => t.TrackId == (calltracker == null ? -1 : calltracker.Value));
                    validTrack = stat != null && stat.OptionId == null && stat.Jobid == null;
                }

                vm = new ResolveCallViewModel
                {
                    Id = lookupScript.LookupId,
                    DialInNumber = id.TrimStart('1'),
                    RawDialInNumber = id,
                    Valid = true
                };

                using (var context = GetContext())
                {
                    vm.CustomerPhone = string.IsNullOrWhiteSpace(id2) ? "ManualEntry" : id2;

                    if (string.Compare(vm.CustomerPhone, "<unavailable>", true) == 0)
                        vm.CustomerPhone = "Blocked";

                    var franchises = GetAvailableFranchises(lookupScript.LookupId);

                    vm.AvailableFranchises = context.tbl_Franchise
                                                   .Where(f => franchises.Contains(f.FranchiseID))
                                                   .ToDictionary(f => f.FranchiseID,
                                                                 f => f.FranchiseNUmber + " - " + f.LegalName);

                    int cpFranchise;
                    var friendlyName = vm.DialInNumber;
                    if (int.TryParse(lookupScript.ConnectusCode, out cpFranchise) && vm.AvailableFranchises.Any(p => p.Key == cpFranchise))
                        friendlyName = vm.AvailableFranchises.Single(p => p.Key == cpFranchise).Value;

                    vm.CPCode = string.Concat(lookupScript.ConnectusCode.Trim(), " - ", friendlyName);
                }
            }
            catch (Exception)
            {
                vm = new ResolveCallViewModel()
                {
                    CPCode = "Something is wrong -- Unknown number? Number is: " + Request.QueryString["id"],
                    Valid = false
                };
            }

            return View(vm);
        }

        private static int[] GetAvailableFranchises(int scriptId)
        {
            using (var incomingContext = new IncomingCallsQAEntities())
            {
                return incomingContext.Tbl_scriptToFranchiseID
                                      .Where(t => t.ScriptID == scriptId)
                                      .Select(f => f.FranchiseID)
                                      .ToArray();
            }
        }

        public JsonResult GetSummaryDataPer24Hours(string id)
        {
            var end = DateTime.Now;
            var start = end.AddDays(-1);
            var userId = (Guid)MembershipService.GetUser(User.Identity.Name).ProviderUserKey;
            var context = new IncomingCallsQAEntities();

            var stat = context.StatisticTracks.Where(item => item.UserId == userId && item.StartDate >= start && item.StartDate <= end).ToList();
            if (stat.Count > 0)
            {
                var last = stat.Last();
                stat.Remove(last);
            }

            var completed = stat.Count(item => item.OptionId != null);
            var uncompleted = stat.Count(item => item.OptionId == null);

            return Json(
                    new
                    {
                        userstat = new[]
                            {
                                new {text = "completed", value = completed, color = "#00FF00"},
                                new {text = "uncompleted", value = uncompleted, color = "#FF0000"}
                            }
                    });
        }

        public JsonResult GetJobInformation()
        {
            var result = "<rows></rows>";
            return Json(new { data = result });
        }

    }
}
