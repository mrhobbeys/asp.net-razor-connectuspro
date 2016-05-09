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
using SiteBlue.Data.EightHundred;
using SiteBlue.Business.Alerts;
using System.Transactions;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class OwnerAlertsController : Controller
    {
        EightHundredEntities db = new EightHundredEntities();
        private MembershipConnection memberShipContext = new MembershipConnection();

        public ActionResult Index(FormCollection formcollection)
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

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
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

            return View(reportTechnicians);
        }

        public ActionResult GetReferral(int id)
        {
            var result = from p in db.tbl_OwnerAlerts
                         where p.FranchiseID == id
                         select new
                         {
                             OwnerAlertId = p.OwnerAlertId,
                             Subject = p.Subject,
                             Descriptions = p.Descriptions,
                             EmailData = db.tbl_OwnerAlertDestinations
                                    .Where(c => c.OwnerAlertId == p.OwnerAlertId)
                                    .Select(q => new
                                    {
                                        DestID = q.DestinationID,
                                        DestAddr = q.OwnerAlertDestinationText
                                    }),
                             OwneralertEnabledYN = p.OwneralertEnabledYN
                         };

            return Json(result);
        }

        public ActionResult GetDestination(int ownerAlertId)
        {
            var result = db.tbl_OwnerAlertDestinations
                .Where(c => c.OwnerAlertId == ownerAlertId)
                .OrderByDescending(t => t.DestinationID)
                .Select(q => new
                {
                    DestID = q.DestinationID,
                    DestAddr = q.OwnerAlertDestinationText
                });

            return Json(result);
        }

        public ActionResult Alerts(int id)
        {
            return View();
        }

        public ActionResult ownerAlerts(int id)
        {
            var alerts = from a in db.tbl_OwnerAlerts
                         join f in db.tbl_Franchise on a.FranchiseID equals f.FranchiseID
                         select new
                         {
                             Subject = a.Subject,
                             Description = a.Descriptions,
                             FranchiseID = f.FranchiseID
                         };
            var result = from p in alerts.ToList()
                         where p.FranchiseID == id
                         select new
                         {
                             Subject = p.Subject,
                             Description = p.Description,
                             FranchiseID = p.FranchiseID
                         };

            return Json(result);
        }

        public ActionResult databyCompanyCode(string code)
        {
            if (code.LastIndexOf("-") > 0)
                code = code.Substring(code.LastIndexOf("-") + 1).Trim();

            var FranchiseID = (from g in memberShipContext.MembershipFranchise
                               where g.FranchiseNumber == code
                               select g.FranchiseID).FirstOrDefault();

            return PartialView("owner", FranchiseID);
        }

        public PartialViewResult owner(int id)
        {
            return PartialView("owner", id);
        }

        public int CreateEmailDestination(int ownerAlertId, int communicationTypeId, string emailName, string emailAddress, string destinationAdditionalText)
        {
            if (db.tbl_OwnerAlertDestinations.Any(q => (q.OwnerAlertId == ownerAlertId) &&
                (q.OwnerAlertDestinationName == emailName || q.OwnerAlertDestinationText == emailAddress)))
                return -1;

            // Initialise the Owner Alert Destination
            tbl_OwnerAlertDestinations model = new tbl_OwnerAlertDestinations()
            {
                OwnerAlertId = ownerAlertId,
                OwnerAlertCommunicationTypeID = communicationTypeId,
                OwnerAlertDestinationName = emailName,
                OwnerAlertDestinationText = emailAddress,
                OwnerAlertDestinationAdditionalText = destinationAdditionalText,
                OwneralertEnabledYN = true
            };

            try
            {
                // Insert the object into the database
                db.tbl_OwnerAlertDestinations.AddObject(model);
                db.SaveChanges();

                return model.DestinationID;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return 0;
            }
        }

        public int CreatePhoneDestination(int ownerAlertId, int communicationTypeId, string phoneName, string phoneNumber, string phoneCarrier)
        {
            if (db.tbl_OwnerAlertDestinations.Any(q => (q.OwnerAlertId == ownerAlertId) &&
                q.OwnerAlertDestinationName == phoneName && q.OwnerAlertDestinationText == phoneNumber
                && q.OwnerAlertDestinationAdditionalText == phoneCarrier))
                return -1;

            // Initialise the Owner Alert Destination
            tbl_OwnerAlertDestinations model = new tbl_OwnerAlertDestinations()
            {
                OwnerAlertId = ownerAlertId,
                OwnerAlertCommunicationTypeID = communicationTypeId,
                OwnerAlertDestinationName = phoneName,
                OwnerAlertDestinationText = phoneNumber,
                OwnerAlertDestinationAdditionalText = phoneCarrier,
                OwneralertEnabledYN = true
            };

            try
            {
                // Insert the object into the database
                db.tbl_OwnerAlertDestinations.AddObject(model);
                db.SaveChanges();

                return model.DestinationID;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return 0;
            }
        }

        public string DeleteOwnerAlertDestination(int id)
        {
            var model = db.tbl_OwnerAlertDestinations.First(d => d.DestinationID == id);

            using (var scope = new TransactionScope())
            {
                // Delete object from the database
                db.tbl_OwnerAlertDestinations.DeleteObject(model);
                db.SaveChanges();

                scope.Complete();

                return "Done";
            }
        }

        public string ToogleOwnerAlertState(int ownerAlertId, bool state)
        {
            var model = db.tbl_OwnerAlerts.First(o => o.OwnerAlertId == ownerAlertId);
            try
            {
                model.OwneralertEnabledYN = state;
                UpdateModel(model);
                db.SaveChanges();
                return "Done";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult OwnerAlertDestinations(int ownerAlertId, int communicationTypeId)
        {
            var result = (from o in db.tbl_OwnerAlertDestinations
                          where o.OwnerAlertId == ownerAlertId && o.OwnerAlertCommunicationTypeID == communicationTypeId
                          select new
                          {
                              DestinationID = o.DestinationID,
                              OwnerAlertId = o.OwnerAlertId,
                              OwnerAlertDestinationName = o.OwnerAlertDestinationName,
                              OwnerAlertDestinationText = o.OwnerAlertDestinationText,
                              OwnerAlertDestinationAdditionalText = o.OwnerAlertDestinationAdditionalText
                          });
            
            return Json(result);
        }

    }
}
