using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using SiteBlue.Areas.OwnerPortal.Models;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Areas.SecurityGuard.Models;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;
using System.IO;
using SiteBlue.Controllers;
namespace SiteBlue.Areas.MyCalls.Controllers
{
    public class CallScriptsController : SiteBlueBaseController
    {
        //
        // GET: /CallScript/CallScripts/
        EightHundredEntities db = new EightHundredEntities();
        IncomingCallsQAEntities callDB = new IncomingCallsQAEntities();
        private MembershipConnection memberShipContext = new MembershipConnection();
        public int PubFranchiseID;

        public ActionResult Index(FormCollection formcollection)
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



                return PartialView("CallScripts", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PartialViewResult CallScripts(int id)
        {
            try
            {
                return PartialView("CallScripts", id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            ;
            //PubFranchiseID = id;

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
            if (DefaultCompamyName == null)
            {
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
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
                memberShipContext.MembershipFranchise
                .Where(f => assignedFranchises.Contains(f.FranchiseID))
                .OrderBy(f => f.FranchiseNumber)
                .Select(d => new SelectListItem
                {
                    Text = d.FranchiseNumber,
                    Value = System.Data.Objects.SqlClient.SqlFunctions.StringConvert((double)d.FranchiseID)
                })
                .ToList(),
                defaultCompanyName = DefaultCompamyName,
                defaultCompanyID = DefaultCompanyID
            };

            return PartialView("CompanyCodeUser", model);
        }

        public ActionResult DBAList(string frid)
        {
            int frenchiseid = Convert.ToInt32(frid);
            List<tbl_Dispatch_DBA> objDBA = (from dba in db.tbl_Dispatch_DBA where dba.FranchiseID == frenchiseid select dba).ToList();
            return Json(objDBA);
        }

        [HttpPost, Authorize(Roles = "Corporate")]
        public ActionResult Save_DBA(int? franchiseID, string OwnersNotes, string CallAnsweringScript)
        {
            var test = "Already exists in the list..";
            try
            {
                var data = from R in db.tbl_Dispatch_OwnerNotes where R.FranchiseID == franchiseID select R;
                if (data.Count() == 0)
                {
                    // Add new.
                    tbl_Dispatch_OwnerNotes Dispatch_DBA = new tbl_Dispatch_OwnerNotes();
                    Dispatch_DBA.FranchiseID = franchiseID.Value;
                    Dispatch_DBA.DispatchComments = OwnersNotes;
                    Dispatch_DBA.DispatchNotes = OwnersNotes;
                    Dispatch_DBA.DispatchSpecials = CallAnsweringScript;
                    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                    Byte[] timestamp = encoding.GetBytes(DateTime.Now.ToString());
                    Dispatch_DBA.timestamp = timestamp;
                    db.tbl_Dispatch_OwnerNotes.AddObject(Dispatch_DBA);
                    db.SaveChanges();
                    test = "Save Data....";
                }
                else
                {
                    // Update existing.
                    var DispatchOwnerNote = (from p in db.tbl_Dispatch_OwnerNotes where p.FranchiseID == franchiseID select p).Single();
                    DispatchOwnerNote.DispatchNotes = OwnersNotes;
                    DispatchOwnerNote.DispatchSpecials = CallAnsweringScript;
                    db.SaveChanges();
                    test = "Update Data....";
                }
            }
            catch (Exception e)
            {
                test = e.Message;
            }

            return Json(test);
        }

        public ActionResult Save_DBA_Data(String DBA, String FrenchiseID)
        {
            int frenchiseId = Convert.ToInt32(FrenchiseID);
            var test = "Already exists in the list..";
            try
            {
                tbl_Dispatch_DBA objDBA = new tbl_Dispatch_DBA();
                objDBA.FranchiseID = frenchiseId;
                objDBA.DBAName = DBA;
                db.tbl_Dispatch_DBA.AddObject(objDBA);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                test = e.Message;
            }

            return Json(test);
        }

        public ActionResult GetData(int? franchiseId)
        {
            var DBA = (from dba in db.tbl_Dispatch_OwnerNotes where dba.FranchiseID == franchiseId select dba);
            return Json(DBA);
        }

        public ActionResult GetCallAnsweringScript(int? franchiseId)
        {
            var answer = callDB.CallAnsweringScript().Where(c => c.FranchiseId == franchiseId).ToList();
            return Json(answer);
        }
    }
}
