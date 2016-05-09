using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using SecurityGuard.Services;
using SiteBlue.Areas.dispatch.Models;
using SiteBlue.Business;
using SiteBlue.Business.Alerts;
using SiteBlue.Business.Job;
using SiteBlue.Business.TechMessaging;
using SiteBlue.Controllers;
using SiteBlue.Core;
using SiteBlue.Data.EightHundred;
using MembershipConnection = SiteBlue.Areas.SecurityGuard.Models.MembershipConnection;
using SiteBlue.Business.Employee;

namespace SiteBlue.Areas.dispatch.Controllers
{
    [Authorize]
    [OutputCache(Duration = 1)]
    public class dispatcherController : SiteBlueBaseController
    {
        private readonly MembershipService _membership = new MembershipService(Membership.Provider);
        private const string DispatchRoleName = "Dispatcher";
        private MembershipUser _currentUser;

        private AuditedJobContext GetAuditedContext()
        {
            if (_currentUser == null || string.IsNullOrWhiteSpace(_currentUser.UserName))
                throw new ApplicationException("User name is not known or is blank.  Cannot instantiate the audited context.");

            return new AuditedJobContext((Guid)_currentUser.ProviderUserKey, _currentUser.UserName, false);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _currentUser = _currentUser ?? _membership.GetUser(User.Identity.Name);
        }

        [Authorize(Roles = "CompanyOwner,Corporate,Dispatcher")]
        public ActionResult Scheduling(FormCollection formCollection)
        {
            ViewBag.Locked = UserInfo.IsOwner;
            ViewBag.SwapBranding = UserInfo.SwapBranding;

            return View();
        }
        
        [Authorize]
        public JsonResult Franchises()
        {
            var userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
            var isCorporate = User.IsInRole("Corporate");
            int[] assignedFranchises;

            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == userId)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }

            using (var db = GetAuditedContext())
            {
                var franchises = db.tbl_Franchise
                    .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                    .OrderBy(f => f.FranchiseNUmber)
                    .Select(d => new {d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName)})
                    .ToArray();

                return Json(new SelectList(franchises, "FranchiseID", "Name"), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Dispatchers()
        {
            var role = new RoleService(Roles.Provider);
            var dispatchers = role.GetUsersInRole(DispatchRoleName).Select(u => _membership.GetUser(u)).OrderBy(u => u.UserName) ;

            return Json(new SelectList(dispatchers, "ProviderUserKey", "UserName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Technicians(int id, DateTime? date)
        {
            using (var db = GetAuditedContext())
            {
                var technicians = (from t in db.tbl_Employee
                                   join h in db.tbl_Dispatch_Schedule
                                   on t.EmployeeID equals h.TechID into techs
                                   from j in techs.DefaultIfEmpty()
                                   join o in db.tbl_Dispatch_QueueTechs
                                   on t.EmployeeID equals o.OnCallTechID into techs2
                                   from j2 in techs2.DefaultIfEmpty()
                                   where t.FranchiseID == id && t.ActiveYN && t.ServiceProYN
                                   orderby t.DisplayOrder, t.Employee
                                   select new { t.Employee, t.EmployeeID, t.ProLevel, j, j2 })
                                   .ToArray();

                var today = (int)(date ?? DateTime.Today).DayOfWeek;

                Func<tbl_Dispatch_Schedule, string> startTime = s => s != null ? (today == 0 ? s.SunStart :
                                                                                  today == 1 ? s.MonStart :
                                                                                  today == 2 ? s.TueStart :
                                                                                  today == 3 ? s.WedStart :
                                                                                  today == 4 ? s.ThuStart :
                                                                                  today == 5 ? s.FriStart :
                                                                                  today == 6 ? s.SatStart : "??") : "??";

                Func<tbl_Dispatch_Schedule, string> endTime = s => s != null ? (today == 0 ? s.SunEnd :
                                                                                  today == 1 ? s.MonEnd :
                                                                                  today == 2 ? s.TueEnd :
                                                                                  today == 3 ? s.WedEnd :
                                                                                  today == 4 ? s.ThuEnd :
                                                                                  today == 5 ? s.FriEnd :
                                                                                  today == 6 ? s.SatEnd : "??") : "??";
                var allData = technicians.Select(t => new { key = t.EmployeeID, t.Employee, t.ProLevel, From = startTime(t.j), To = endTime(t.j), OnCall = t.j2 == null ? string.Empty : "On Call" })
                    .Select(t => new { t.key, t.Employee, t.ProLevel, Times = ((t.From == t.To) ? t.From : t.To), t.OnCall })
                    .Select(t => new { t.key, label = string.Format("{0} - {1} - ({2}) {3}", t.Employee, t.ProLevel, t.Times, t.OnCall) })
                    .ToArray();
                return Json(allData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DispatchQueue(int franchiseId, string viewType)
        {
            var type = (viewType ?? "current").ToLower();
            Expression<Func<tbl_Job, bool>> func = null;

            switch (type)
            {
                case "current":
                    {
                        var statuses = new[] { "Active", "Travel", "Wrap Up", "SentToTablet" };
                        func = j => j.FranchiseID == franchiseId &&
                                                     statuses.Contains(j.tbl_Job_Status.Status);
                    }
                    break;
                case "booked":
                    {
                        func = j => j.FranchiseID == franchiseId && 
                                                     (j.ServiceProID == 0 || j.ServiceProID == 127 || j.tbl_Job_Status.Status == "Booked");
                    }
                    break;
                case "scheduled":
                    {
                        func = j => j.FranchiseID == franchiseId && 
                                                     j.tbl_Job_Status.Status == "Scheduled";
                    }
                    break;
                case "queue":
                    {
                        var statuses = new[] { "Waiting Parts", "Waiting Estimate", "Waiting People", "Waiting Weather", "Re-Scheduled"};
                        func = j => j.FranchiseID == franchiseId && 
                                                     statuses.Contains(j.tbl_Job_Status.Status);
                    }
                    break;
            }

            return View(GetJobViewModelList(func));
        }

        public ActionResult Data()
        {
            return View(ViewBag.Franchises);
        }

        private IEnumerable<JobViewModel> GetJobViewModelList(Expression<Func<tbl_Job, bool>> jobFilter)
        {
            using (var db = GetAuditedContext())
            {
                var jobs = from j in db.tbl_Job.Where(jobFilter)
                           join jobLoc in db.tbl_Locations
                           on j.LocationID equals jobLoc.LocationID
                           join c in db.tbl_Customer
                           on j.CustomerID equals c.CustomerID
                           //uncomment this once we get a proper UK on the contact
                           //join jobPrimContact in db.tbl_Contacts
                           //on new { c.CustomerID, jobLoc.LocationID, PhoneTypeID = 2 } equals new { jobPrimContact.CustomerID, jobPrimContact.LocationID, jobPrimContact.PhoneTypeID }
                           join billLoc in db.tbl_Locations
                           on c.CustomerID equals billLoc.BilltoCustomerID
                           //uncomment this once we get a proper UK on the contact
                           //join billPrimContact in db.tbl_Contacts
                           //on new { c.CustomerID, billLoc.LocationID, PhoneTypeID = 2 } equals new { billPrimContact.CustomerID, billPrimContact.LocationID, billPrimContact.PhoneTypeID }
                           select new JobViewModel
                                     {
                                         JobID = j.JobID,
                                         FranchiseID = j.FranchiseID,
                                         CallReason = j.CallReason,
                                         Start = j.ScheduleStart,
                                         End = j.ScheduleEnd,
                                         Status = j.tbl_Job_Status,
                                         StatusText = j.tbl_Job_Status.Status,
                                         CallName = j.tbl_Calls.CallName,
                                         Address = j.tbl_Calls.CallAddress,
                                         Zip = j.tbl_Calls.CallZipCode,
                                         Email = j.tbl_Calls.Email,
                                         Phone = j.tbl_Calls.CallPhone,
                                         CompanyName = j.tbl_Calls.CompanyName,
                                         Notes = j.RescheduleReason,
                                         ServiceProName = j.tbl_Employee.Employee,
                                         BillToLocation = billLoc,
                                         //would be nice to have a UK on the contact so we get it in the join above!!
                                         BillToContact = db.tbl_Contacts.FirstOrDefault(jc => jc.PhoneTypeID == 2 && billLoc.BilltoCustomerID == jc.CustomerID && jc.LocationID == billLoc.LocationID),
                                         BillToCell = db.tbl_Contacts.FirstOrDefault(jc => jc.PhoneTypeID == 4 && billLoc.BilltoCustomerID == jc.CustomerID && jc.LocationID == billLoc.LocationID),
                                         JobLocation = jobLoc,
                                         //would be nice to have a UK on the contact so we get it in the join above!!
                                         JobLocationContact = db.tbl_Contacts.FirstOrDefault(jc => jc.PhoneTypeID == 2 && (jobLoc.ActvieCustomerID == null ? billLoc.BilltoCustomerID : jobLoc.ActvieCustomerID) == jc.CustomerID && jc.LocationID == jobLoc.LocationID),
                                         PrimaryTechnician = j.tbl_Employee,
                                         Customer = c,
                                         CancelReason = j.tbl_Call_CancelReasons,
                                         DiagnosisNotes = j.Diagnostics ?? string.Empty,
                                         OwnerNotes = j.CustomerComments
                                     };

                var result = jobs.ToList();

                var jobids = result.Select(j => j.JobID).ToArray();
                var secondaryTechs = db.tbl_Job_Technicians.Where(jt => jobids.Contains(jt.JobID) && !jt.PrimaryYN).Select(jt => new { JobID = jt.JobID, Tech = jt.tbl_Employee })
                                                           .GroupBy(jt => jt.JobID)
                                                           .ToDictionary(g => g.Key, g => g.Select(at => at.Tech).ToArray());
                result.ForEach(j => j.SecondaryTechnicians = secondaryTechs.ContainsKey(j.JobID) ? secondaryTechs[j.JobID] : new tbl_Employee[] { });


                return result.ToArray();
            }
        }

        public ActionResult SchedulerData(int franchiseId, DateTime from, DateTime? to)
        {
            var fromDate = from.Date;
            var toDate = (to ?? fromDate).AddDays(1);

            var viewModels = GetJobViewModelList(j => j.FranchiseID == franchiseId &&
                                             fromDate <= j.ScheduleStart &&
                                             toDate > j.ScheduleStart &&
                                             j.ServiceProID != 127);

            return View(viewModels);
        }

        public ActionResult UnassignedJobs(int franchiseId)
        {
            var unassigned = GetJobViewModelList(j => j.ServiceProID == 127 && j.FranchiseID == franchiseId && j.tbl_Job_Status.Status != "Cancelled");
            return View(unassigned.OrderBy(j => j.Start).ToArray());
        }

        public ActionResult GetJobDetails(int id)
        {
            var job = GetJobViewModelList(j => j.JobID == id).Single();

            using (var db = GetAuditedContext())
            {
                //Temporary hack until we can implement data layer security.  Dispatchers should not be able to close/complete jobs.
                var statuses = JobStatusWorkflow.GetAvailableStatuses(UserInfo.UserRoles, job.Status.StatusID);
                ViewBag.Statuses = db.tbl_Job_Status.ToArray().Where(js => statuses.Contains(js.StatusID)).OrderBy(s => s.Status).ToArray();
                ViewBag.CancelReasons = db.tbl_Call_CancelReasons.OrderBy(r => r.CancelReason).ToArray();
                ViewBag.Technicians = db.tbl_Employee.Where(e => e.FranchiseID == job.FranchiseID).OrderBy(r => r.Employee).ToArray();
            }

            return View(job);
        }

        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult SaveNote(int id, string note)
        {
            try
            {
                using (var db = GetAuditedContext())
                {
                    var job = db.tbl_Job.Single(j => j.JobID == id);
                    job.RescheduleReason = string.Concat(note, Environment.NewLine, job.RescheduleReason);
                    db.SaveChanges();
                }
            }
            catch (Exception ex )
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return Json(new {Success = false, msg = "Error saving note: " + ex.Message});
            }

            return Json(new {Success = true, msg = "Success"});
        }


        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult SaveJobDates(int id, DateTime start, DateTime end)
        {
            tbl_Job job;
            using (var db = GetAuditedContext())
            {
                job = db.tbl_Job.SingleOrDefault(j => j.JobID == id);

                if (job == null)
                    return Json(new { Success = false, Start = DateTime.MinValue, End = DateTime.MinValue, Message = "Job not found." });

                try
                {
                    if (end != job.ScheduleEnd && end.Date != start.Date)
                    {
                        return
                            Json(new
                            {
                                Success = false,
                                Start = job.ScheduleStart.GetValueOrDefault(),
                                End = job.ScheduleEnd.GetValueOrDefault(),
                                Message = "A job cannot span days.  Please be sure the job starts and ends on the same day."
                            });

                    }

                    if (start >= end || start.Date != end.Date)
                    {
                        var hours = (job.ScheduleEnd.GetValueOrDefault() - job.ScheduleStart.GetValueOrDefault()).TotalHours;
                        end = start.AddHours(hours);
                    }

                    if (job.ScheduleStart != start || job.ScheduleEnd != end)
                    {
                        job.ScheduleStart = start;
                        job.ScheduleEnd = end;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                        ex = ex.InnerException;

                    return Json(new { Success = false, Start = job.ScheduleStart.GetValueOrDefault(), End = job.ScheduleEnd.GetValueOrDefault(), Message = "Error saving job dates: " + ex.Message });
                }
            }

            return Json(new { Success = true, Start = job.ScheduleStart.GetValueOrDefault(), End = job.ScheduleEnd.GetValueOrDefault(), Message = "Success" });
        }


        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult SaveStatus(int id, int statusId, int? reason)
        {
            int franchiseId;
            try
            {
                using (var db = GetAuditedContext())
                {
                    var job = db.tbl_Job.Single(j => j.JobID == id);
                    franchiseId = job.FranchiseID;

                    job.StatusID = statusId;
                    job.CancelReasonID = reason.GetValueOrDefault();
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return Json(new { Success = false, msg = "Error saving status: " + ex.Message });
            }
            
            AlertType? type = null;
            switch (statusId)
            {
                case 10 :
                    type = AlertType.CallRescheduled;
                    break;
                case 12 :
                    type = AlertType.CustomerCancellation;
                    break;
            }

            if (type.HasValue)
                AbstractBusinessService.Create<AlertEngine>(UserInfo.UserKey).SendAlert(type.Value, franchiseId);

            return Json(new { Success = true, msg = "Success" });
        }

        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult SendToTablet(int id)
        {
            try
            {
                using (var db = GetAuditedContext())
                {
                    var job = db.tbl_Job.Single(j => j.JobID == id);
                    var techTab = db.tbl_Franchise_Tablets.SingleOrDefault(t => t.EmployeeID == job.ServiceProID && t.FranchiseID == job.FranchiseID);

                    if (techTab == null)
                        return Json(new { Success = false, Message = "No tablet assigned to this technician." });

                    var folder = Path.Combine(GlobalConfiguration.SendToTabletDropPath, techTab.TabletNumber);

                    using (var tScope = new TransactionScope())
                    {
                        db.ExecuteStoreCommand("EXEC [GetXMLByJob] @JobID={0}, @Folder={1}", new object[] { id, folder });

                        job.StatusID = 11;
                        db.SaveChanges();
                        tScope.Complete();

                        return Json(new { Success = true, Message = "Job sent to tablet." });
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return Json(new { Success = false, ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult SendTechBio(int id)
        {
            using (var db = GetAuditedContext())
            {
                var qryResult = (from j in db.tbl_Job
                           join c in db.tbl_Customer
                           on j.CustomerID equals c.CustomerID
                           where j.JobID == id
                           select new {j.ServiceProID, c.EMail}).SingleOrDefault();
                
                if (qryResult == null || string.IsNullOrWhiteSpace(qryResult.EMail))
                    return Json(new {Success = false, Message = "Could not find email address for customer or no email is specified."});

                var result = AbstractBusinessService.Create<EmployeeService>(UserInfo.UserKey).SendTechnicianBio(qryResult.ServiceProID, qryResult.EMail);

                return Json(result.Success 
                            ? new { Success = true, Message = "Technician bio sent to customer." } 
                            : new {Success = false, Message = result.Message});
            }
        }

        public JsonResult GetJob(int id)
        {
            using (var db = GetAuditedContext())
            {
                var job = from j in db.tbl_Job
                          where j.JobID == id
                          select new {j.JobID, Start = j.ScheduleStart, End = j.ScheduleEnd, j.CallReason, j.StatusID};

                return Json(job.Single(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult UnassignJob(int id)
        {
            try
            {
                using (var db = GetAuditedContext())
                {
                    var job = db.tbl_Job.Single(j => j.JobID == id);

                    if (job == null)
                        return Json(new {Success = false, Message = "Job not found."});

                    job.ServiceProID = 127;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return Json(new { Success = false, Message = "Error unassigning job: " + ex.Message });
            }

            return Json(new { Success = true, Message = "Success" });
        }

        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult SaveAssignedTechnicians(int id, string action, int techId)
        {
            try
            {
                using (var db = GetAuditedContext())
                {
                    switch ((action ?? string.Empty).ToLower())
                    {
                        case "add":
                            {
                                var newTech = new tbl_Job_Technicians { ServiceProID = techId, JobID = id, PrimaryYN = false };
                                db.AddTotbl_Job_Technicians(newTech);
                                db.SaveChanges();
                            }
                            break;
                        case "remove":
                            {
                                var toDelete = db.tbl_Job_Technicians.SingleOrDefault(jt => jt.JobID == id && jt.ServiceProID == techId);

                                if (toDelete != null)
                                {
                                    db.tbl_Job_Technicians.DeleteObject(toDelete);
                                    db.SaveChanges();
                                }
                            }
                            break;
                        case "primary":
                            {
                                var job = db.tbl_Job.Single(j => j.JobID == id);
                                job.ServiceProID = techId;
                                var toUpdate1 = db.tbl_Job_Technicians.SingleOrDefault(jt => jt.JobID == id && jt.ServiceProID == techId);
                                toUpdate1.PrimaryYN = true;
                                var toUpdate2 = db.tbl_Job_Technicians.SingleOrDefault(jt => jt.JobID == id && jt.PrimaryYN);

                                //We'll get a null ref here if the job is currently assigned to the N/A tech.
                                if (toUpdate2 != null)
                                    toUpdate2.PrimaryYN = false;

                                db.SaveChanges();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return Json(new { Success = false, Message = "Could not update assigned technicians: " + ex.Message });
            }
            return Json(new { Success = true, Message = "Success" });
        }

        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult SaveJob(SaveScheduledJobViewModel toSave)
        {
            try
            {
                using (var db = GetAuditedContext())
                {
                    var job = db.tbl_Job.Single(j => j.JobID == toSave.JobId);

                    if (job == null)
                        return Json(new {Success = false, Message = "Job not found."});

                    job.ServiceProID = toSave.TechnicianId;
                    job.ScheduleStart = toSave.StartTime;
                    job.ScheduleEnd = toSave.EndTime;

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return Json(new { Success = false, Message = "Error saving job: " + ex.Message });
            }

            return Json(new { Success = true, Message = "Success" });
        }

        public JsonResult GetJobHistory(int id)
        {
            using (var db = GetAuditedContext())
            {
                var hist = db.tbl_Job_Status_History.Where(j => j.JobID == id).OrderByDescending(js => js.StatusDateChanged).ToArray();

                var json = new
                {
                    total_count = hist.Length,
                    rows = hist.Select(h => new
                    {
                        id = h.StatusChangedID,
                        data = new object[] {
                                                h.ChangedBy, 
                                                h.ChangedField, 
                                                h.ChangedfromTo, 
                                                h.ChangedOnTabletYN, 
                                                h.StatusDateChanged.GetValueOrDefault().ToString("MM/dd/yy hh:mm:ss tt")
                                            }
                    }).ToArray()
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(Duration = 30, VaryByParam = "none", Location = OutputCacheLocation.Server, NoStore = true)]
        public JsonResult GetTechMessages(int? id)
        {
            using (var db = GetAuditedContext())
            {
                var msgs = (from msg in db.tbl_Dispatch_Message_History
                            where !msg.ProcessedYN && (id == null || id == msg.FranchiseID)
                            orderby msg.MessageDate descending
                            select new
                                       {
                                           TechID = msg.TechID,
                                           TechName = msg.TechName,
                                           Message = msg.Message,
                                           Date = msg.MessageDate,
                                           FranchiseName = msg.FranchiseName,
                                           FranchiseID = msg.FranchiseID,
                                           ID = msg.MessageHistoryID
                                       }).ToArray();

                const string formatStr = "({0}) {1}@{2}: {3}";
                var json = new
                {
                    total_count = msgs.Length,
                    rows = msgs.Select(h => new
                    {
                        id = h.ID,
                        data = new object[] { string.Format(formatStr, h.FranchiseName, h.TechName, h.Date.GetValueOrDefault().ToString("MM/dd hh:mmtt"), h.Message) }
                    }).ToArray()
                };

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Corporate,Dispatcher")]
        public JsonResult RespondToTechMessage(int id, string msg, bool markProcessed)
        {
            OperationResult<bool> result;
            try
            {
                result = AbstractBusinessService.Create<MessagingService>(UserInfo.UserKey).SendMessage(id, msg, markProcessed);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, Message = ex.Message});
            }

            return Json(new { Success = result.ResultData, Message = result.Message });
        }
    }
}
