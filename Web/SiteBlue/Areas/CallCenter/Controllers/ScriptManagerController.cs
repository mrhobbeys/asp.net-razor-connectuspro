using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using HVACapp.Areas.HVAC_App.Controllers;
using SiteBlue.Areas.MyCalls.Models;
using SiteBlue.Business;
using SiteBlue.Business.Alerts;
using SiteBlue.Areas.CallCenter.Models;
using SiteBlue.Business.Customer;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.CallCenter.Controllers
{
    [Authorize]
    public class ScriptManagerController : CallCenterController
    {
        private readonly IncomingCallsQAEntities db = new IncomingCallsQAEntities();

        public ViewResult Index()
        {
            return View(db.LookupScripts.ToList());
        }

        public JsonResult LoadResult()
        {
            return Json(db.LookupScripts.ToList(), JsonRequestBehavior.AllowGet);
        }

      
        public ViewResult Bookcalls()
        {
            return View(db.LookupScripts.ToList());
        }

      
        public ViewResult Details(int id)
        {

            LookupScript lookupscript = db.LookupScripts.Single(l => l.LookupId == id);
            return View(lookupscript);


        }
        [ValidateInput(false)] //to support <unavailable> in the query string when a caller has a blocked number.
        public ViewResult Incoming(string id, string id2, bool? fullRender, int? calltracker)
        {
            Incoming vm;

            try
            {
                LookupScript lookupscript;
                var validTrack = false;

                using (var scriptCtx = new IncomingCallsQAEntities())
                {
                    lookupscript = scriptCtx.LookupScripts.Single(l => l.LookupPhoneNr == id);

                    var stat = scriptCtx.StatisticTracks.SingleOrDefault(ct => ct.TrackId == (calltracker == null ? -1 : calltracker.Value));
                    validTrack = stat != null && stat.OptionId == null && stat.Jobid == null;
                }
                
                vm = new Incoming
                         {
                             Id = lookupscript.LookupId,
                             RawDialInNumber = id,
                             DialInNumber = id.TrimStart('1'),
                             CallScript = lookupscript.CallScript,
                             Valid = true,
                             StatTrackingInvalid = !validTrack
                         };

                using (var dbContext = GetContext())
                {
                    vm.CustomerPhone = string.IsNullOrWhiteSpace(id2) ? "ManualEntry" : id2;

                    if (string.Compare(vm.CustomerPhone, "<unavailable>", true) == 0)
                        vm.CustomerPhone = "Blocked";

                    if (fullRender.GetValueOrDefault())
                    {
                        var jobPriorities = dbContext.tbl_Job_Priority.Select(j => new { j.JobPriorityID, j.JobPriority })
                            .OrderBy(j => j.JobPriority)
                            .ToDictionary(k => k.JobPriorityID, v => v.JobPriority);
                       var pmtTypes = dbContext.tbl_Payment_Types.Select(p => new { p.PaymentTypeId, p.PaymentType })
                            .OrderBy(p => p.PaymentType)
                            .ToDictionary(k => k.PaymentTypeId, v => v.PaymentType);
                        vm.JobPriorities = jobPriorities;
                        vm.PaymentTypes = pmtTypes;
                    }

                    var franchises = GetAvailableFranchises(lookupscript.LookupId);

                    vm.AvailableFranchises = dbContext.tbl_Franchise
                                                   .Where(f => franchises.Contains(f.FranchiseID))
                                                   .ToDictionary(f => f.FranchiseID, 
                                                                 f => f.FranchiseNUmber + " - " + f.LegalName);

                    int cpFranchise;
                    var friendlyName = vm.DialInNumber;
                    if (int.TryParse(lookupscript.ConnectusCode, out cpFranchise) && vm.AvailableFranchises.Any(p => p.Key == cpFranchise))
                        friendlyName = vm.AvailableFranchises.Single(p => p.Key == cpFranchise).Value;

                    vm.CPCode = string.Concat(lookupscript.ConnectusCode.Trim(), " - ", friendlyName);
                }
                
            }
            catch (Exception)
            {
                vm = new Incoming()
                         {
                             CallScript =
                                 "Something is wrong -- Unknown number? Number is: " + Request.QueryString["id"],
                             Valid = false
                         };
            }

            vm.CallScript = vm.CallScript.Replace("\"", "\\\"");
            return View(fullRender.GetValueOrDefault() ? "Incoming" : "IncomingSplash", vm);
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CallCenter/ScriptManager/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LookupScript lookupscript)
        {
            if (ModelState.IsValid)
            {
                db.LookupScripts.AddObject(lookupscript);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lookupscript);
        }

        //
        // GET: /CallCenter/ScriptManager/Edit/5

        public ActionResult Edit(int id)
        {
            LookupScript lookupscript = db.LookupScripts.Single(l => l.LookupId == id);
            return View(lookupscript);
        }

        //
        // POST: /CallCenter/ScriptManager/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LookupScript lookupscript)
        {
            if (ModelState.IsValid)
            {
                db.LookupScripts.Attach(lookupscript);
                db.ObjectStateManager.ChangeObjectState(lookupscript, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lookupscript);
        }

        ////
        //// GET: /CallCenter/ScriptManager/Delete/5

        //public ActionResult Delete(int id)
        //{
        //    LookupScript lookupscript = db.LookupScripts.Single(l => l.LookupId == id);
        //    return View(lookupscript);
        //}

        public ActionResult Delete(int id)
        {
            LookupScript lookupscript = db.LookupScripts.Single(l => l.LookupId == id);
            db.LookupScripts.DeleteObject(lookupscript);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[HttpPost, ActionName("Delete")]
        public JsonResult DeleteLookupPhone(int id)
        {
            bool result = false;
            var errorMessage = "";

            LookupScript lookupscript = db.LookupScripts.Single(l => l.LookupId == id);
            try
            {
                db.LookupScripts.DeleteObject(lookupscript);
                db.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(new
            {
                Success = result,
                Message = result ? "The lookup phone with number " + lookupscript.LookupPhoneNr + " has been deleted" : errorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteConfirmed()
        {
            return Json(new
            {
                success = true,
                message = "Deleted"
            });
        }

        public ActionResult CustomerList(int scriptId, string phoneNumber, string name, bool billTo)
        {
            var searchPhoneNumber = FormatPhoneNumberForSearch(phoneNumber);

            return View(SearchCustomerInformation(scriptId, searchPhoneNumber, name, billTo));
        }

        public JsonResult OwnerNotes(int franchiseId)
        {
            using (var dbContext = GetContext())
            {
                var ownerNotes =
                    dbContext.tbl_Dispatch_OwnerNotes.Where(c => c.FranchiseID == franchiseId).Select(
                        c => new {Notes = c.DispatchNotes, Specials = c.DispatchSpecials})
                        .ToArray().FirstOrDefault() ?? new {Notes = string.Empty, Specials = string.Empty};

                return Json(ownerNotes, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDbaByZipCode(int scriptId, string postalCode)
        {
            try
            {
                var franchises = GetAvailableFranchises(scriptId);

                if (postalCode == null)
                    postalCode = string.Empty;

                var zipCode = postalCode.Substring(0, Math.Min(5, postalCode.Length));

                using (var dbContext = GetContext())
                {
                    var zipList = from zip in dbContext.tbl_Franchise_ZipList
                                  join franchise in dbContext.tbl_Franchise
                                  on zip.FranchiseID equals franchise.FranchiseID
                                  where (franchises.Contains(zip.FranchiseID)) &&
                                        zip.FranchiseZipID == zipCode &&
                                        franchise.FranchiseStatusID != 10
                                  select new { zip.FranchiseID, franchise.FranchiseNUmber, franchise.LegalName, zip.City, zip.State };

                    var found = zipList.SingleOrDefault();

                    return found == null 
                            ? Json(new { Success = true, Found = false}) 
                            : Json(new { Success = true, Found = true, FranchiseId = found.FranchiseID, Name = string.Format("{0} - {1}", found.FranchiseNUmber, found.LegalName), City = found.City, State = found.State }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.Message });
            }
        }

        public JsonResult GetFranchiseServices(int franchiseId)
        {
            try
            {
                using (var dbContext = GetContext())
                {
                    var query = from fs in dbContext.tbl_Franchise_Services
                                join s in dbContext.tbl_Services on fs.ServiceID equals s.ServiceID
                                where fs.FranchiseID == franchiseId
                                select new {ServiceId = s.ServiceID, s.ServiceName};
                    var serviceList = query.ToList();
                    return Json(serviceList, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = false });
            }
        }

        public JsonResult GetReferralTypes(int franchiseId)
        {
            try
            {
                using (var dbContext = GetContext())
                {
                    var query = from r in dbContext.tbl_Referral
                                where (r.FranchiseID == franchiseId) && r.activeYN
                                orderby r.ReferralType
                                select new {ID = r.referralID, Type = r.ReferralType};
                    var referralList = query.ToList();
                    return Json(referralList, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = false });
            }
        }

        public JsonResult GetDBAList(int franchiseId)
        {
            try
            {
                using (var dbContext = GetContext())
                {
                    var query = from dba in dbContext.tbl_Dispatch_DBA
                                where dba.FranchiseID == franchiseId
                                select new {ID = dba.DBAID, Name = dba.DBAName};
                    return Json(new {Success = true, DBA = query.ToArray()}, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = false });
            }
        }

        public DateTime GetJobStartDate(int serviceWindowId, DateTime serviceDate)
        {
            using (var dbContext = GetContext())
            {
                var startTime = dbContext.tbl_ServiceWindow.Single(w => w.ServiceWindowID == serviceWindowId).StartTime.ToLongTimeString();
                return DateTime.Parse(serviceDate.ToShortDateString() + " " + startTime);
            }
        }

        private static int[] GetAvailableFranchises(int scriptId)
        {
            
            using (var scriptCtx = new IncomingCallsQAEntities())
            {
                return scriptCtx.Tbl_scriptToFranchiseID
                                      .Where(t => t.ScriptID == scriptId)
                                      .Select(f => f.FranchiseID)
                                      .ToArray();
            }
        }
        private static IEnumerable<CustomerInformation> SearchCustomerInformation(int scriptId, string[] phoneNumbers, string name, bool billTo)
        {
            var usePhone = phoneNumbers != null && phoneNumbers.Length > 0;
            var useName = !string.IsNullOrWhiteSpace(name);

            var franchises = GetAvailableFranchises(scriptId);

            name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;

            using (var dbContext = GetContext())
            {
                Expression<Func<CustomerInformation, bool>> filter = cust => (usePhone || useName) &&
                                                                 (
                                                                     (phoneNumbers.Contains(cust.PhoneNumber) ||
                                                                      phoneNumbers.Contains(cust.SecondaryContactNumber) ||
                                                                      !usePhone) &&
                                                                     (cust.CustomerName.Contains(name) || !useName)
                                                                 );
                return GetCustomers(dbContext, franchises, null, billTo).Where(filter)
                                                                        .OrderBy(c => c.CustomerName)
                                                                        .ToArray();
            }
        }

        private static IQueryable<CustomerInformation> GetCustomers(EightHundredBaseContext dbContext, int[] searchFranchises, int? customerId, bool isBillTo)
        {
            return from cust in dbContext.tbl_Customer
                   join ci in dbContext.tbl_Customer_Info
                   on cust.CustomerID equals ci.CustomerID
                   join l in dbContext.tbl_Locations
                   on cust.CustomerID equals isBillTo ? l.BilltoCustomerID.Value : l.ActvieCustomerID.Value
                   join primaryContact in dbContext.tbl_Contacts
                       on new {cust.CustomerID, PhoneTypeID = 2, l.LocationID} equals
                       new {primaryContact.CustomerID, primaryContact.PhoneTypeID, primaryContact.LocationID}
                   join cellContact in dbContext.tbl_Contacts
                       on new {primaryContact.CustomerID, PhoneTypeID = 4, l.LocationID} equals
                       new {cellContact.CustomerID, cellContact.PhoneTypeID, cellContact.LocationID} into joined
                   let member = (from m in dbContext.tbl_Customer_Members where m.CustomerID == cust.CustomerID && (m.StartDate ?? DateTime.Now) <= DateTime.Now && (m.EndDate ?? DateTime.Now) >= DateTime.Now orderby m.StartDate descending select m).FirstOrDefault()
                   from secContact in joined.DefaultIfEmpty()
                   where searchFranchises.Contains(ci.FranchiseID) && (customerId == null || customerId == cust.CustomerID)
                   select
                       new CustomerInformation
                           {
                               CustomerID = cust.CustomerID,
                               CustomerName = cust.CustomerName,
                               ContactID = primaryContact.ContactID,
                               SecondaryContactID = secContact == null ? 0 : secContact.ContactID,
                               SecondaryContactNumber = secContact == null ? string.Empty : secContact.PhoneNumber,
                               ContactName = primaryContact.ContactName,
                               Address = l.Address,
                               City = l.City,
                               CompanyName = cust.CompanyName,
                               Country = l.Country,
                               Email = cust.EMail,
                               PhoneNumber = primaryContact.PhoneNumber,
                               State = l.State,
                               Zip = l.PostalCode,
                               LocationID = l.LocationID,
                               FranchiseID = ci.FranchiseID,
                               MemberFrom = member == null || !member.StartDate.HasValue ? DateTime.MinValue : member.StartDate.Value,
                               MemberTo = member == null || !member.EndDate.HasValue ? DateTime.MaxValue : member.EndDate.Value,
                               IsMember = member != null &&
                               (!member.StartDate.HasValue || member.StartDate.Value <= DateTime.Now) &&
                               (!member.EndDate.HasValue || member.EndDate.Value >= DateTime.Now)
                           };
        }

        public JsonResult GetCustomerInformation(int scriptId, int customerId, bool billingLocation)
        {
            var franchises = GetAvailableFranchises(scriptId);

            using (var dbContext = GetContext())
            {
                var result = GetCustomers(dbContext, franchises, customerId, billingLocation).Single();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCall(tbl_Calls call)
        {
            call.CallReceivedDate = DateTime.Now;
            try
            {
                using (var dbContext = GetContext())
                {
                    if (call.CallID != 0)
                    {
                        var existingCall = dbContext.tbl_Calls.Single(c => c.CallID == call.CallID);
                        dbContext.ApplyCurrentValues(existingCall.EntityKey.EntitySetName, call);
                    }
                    else
                    {
                        call.CallCompletedTime = DateTime.Now.ToLongTimeString();
                        dbContext.tbl_Calls.AddObject(call);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true, Value = call.CallID });
        }

        public JsonResult SaveJob(tbl_Job job, string trackId)
        {
            var messages = new List<string>();
            var result = new SaveResult();
            result.Success = false;

            try
            {
                job.ScheduleStart = GetJobStartDate(job.ServiceWindowID, job.ServiceDate.GetValueOrDefault());
                job.ScheduleEnd = job.ScheduleStart.GetValueOrDefault().AddHours(job.ServiceLength.GetValueOrDefault());
                job.ServiceProID = 127; // default to N/A

                if (job.CustomerID == 0)
                    messages.Add("No customer selected for this job.");

                if (job.LocationID == 0)
                    messages.Add("No job location selected for this job.");
                
                if (job.ServiceDate.GetValueOrDefault() == default(DateTime))
                    messages.Add("Please select a Service Date for the job.");

                if (job.ExpectedPayTypeID == 7 && !CheckCreditTermsInternal(job.FranchiseID, job.CustomerID))
                    messages.Add("The customer has no credit terms set up with this franchise.  Please select a different payment type.");

                if (job.ScheduleStart < DateTime.Now)
                    messages.Add("The Service Date for this job occurs in the past.");

                using (var jobContext = GetContext())
                {
                    if (job.AreaID != 0)
                    {
                        var dba = jobContext.tbl_Dispatch_DBA.Select(d => new {d.DBAID, d.DBAName, d.FranchiseID}).FirstOrDefault(d => d.DBAID == job.AreaID);
                        if (dba == null)
                            messages.Add("The specified DBA cannot be found.");
                        else
                        {
                            if (dba.FranchiseID != job.FranchiseID)
                                messages.Add(string.Format("The specified DBA ({0}) is not valid for the selected franchise.", dba.DBAName));
                        }
                    }

                    if (messages.Count != 0)
                        result.Messages = messages.ToArray();
                    else
                    {

                        var hasInfoRecord = jobContext.tbl_Customer_Info.Any(c => c.CustomerID == job.CustomerID);

                        if (!hasInfoRecord)
                        {
                            var newInfo = tbl_Customer_Info.Createtbl_Customer_Info(default(int), job.FranchiseID,
                                                                                    job.CustomerID, 2, 0, 2, 0, 0);
                            jobContext.AddTotbl_Customer_Info(newInfo);
                        }

                        job.CallTaker = UserInfo.UserKey.ToString();
                        jobContext.AddTotbl_Job(job);
                        jobContext.SaveChanges();

                        try
                        {
                            int trackNum;
                            if (int.TryParse(trackId, out trackNum))
                            {
                                var stat = db.StatisticTracks.SingleOrDefault(s => s.TrackId == trackNum);
                                if (stat != null)
                                {
                                    stat.Duration = DateTime.Now - stat.StartDate;
                                    stat.OptionId = 1;
                                    stat.Jobid = job.JobID;
                                    db.SaveChanges();
                                }
                            }
                        }catch (Exception ex)
                        {
                            Logger.Log("Could not track call statistic", ex, LogLevel.Error);
                        }

                        result.Success = true;
                        result.SavedId = job.JobID;
                        result.Messages = new[] {"Job saved successfully."};

                    }
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                result.Messages = new[] { ex.Message };
            }

            if (result.Success)
            {
                var type = job.JobPriorityID == 4
                            ? AlertType.RecallBooked
                            : (job.ServiceID == 10 ? AlertType.HvacSalesAlert : AlertType.AppointmentBooked);

                AbstractBusinessService.Create<AlertEngine>(UserInfo.UserKey).SendAlert(type, job.FranchiseID);
            }

            return Json(result);
        }

        //If the user trys to set a service date for same day
        [HttpPost]
        public JsonResult GetServiceWindows(DateTime date)
        {
            using (var dbContext = GetContext())
            {
                //servicewindowid 15 is 8pm and after
                var passList = dbContext.tbl_ServiceWindow
                                        .Where(w => date.Date != DateTime.Today || w.StartTime.Hour > DateTime.Now.Hour || w.ServiceWindowID == 15)
                                        .OrderBy(w => w.SortOrder)
                                        .Select(w => new { Key = w.ServiceWindowID, Value = w.ServiceWindow });
                return Json(passList.ToArray());
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private string formatphonenumber(string phonenumber)
        {
            return "(" + phonenumber.Substring(0, 3) + ") " + phonenumber.Substring(3, 3) +
                                              "-" + phonenumber.Substring(6, 4);
        }


        //phonenumber comes in as 10 consecutive alphas
        private string[] FormatPhoneNumberForSearch(string phoneNumber)
        {
            var digitsOnly = new Regex(@"[^\d]");
            phoneNumber = digitsOnly.Replace(phoneNumber, string.Empty);

            if (string.IsNullOrWhiteSpace(phoneNumber))
                return new string[] { };

            var phoneNumberFormats = new List<string>
                                         {
                                             "(" + phoneNumber.Substring(0, 3) + ") " + phoneNumber.Substring(3, 3) +
                                             "-" + phoneNumber.Substring(6, 4),
                                             phoneNumber.Substring(0, 3) + "-" + phoneNumber.Substring(3, 3) + "-" +
                                             phoneNumber.Substring(6, 4), 
                                             phoneNumber.ToString(), 
                                             phoneNumber.Substring(0, 3) + "-" + phoneNumber.Substring(3, 3) + "-" +
                                             phoneNumber.Substring(6, 4) + " ", 
                                             phoneNumber.Substring(0, 3) + " " + phoneNumber.Substring(3, 3)  +
                                             phoneNumber.Substring(6, 4), 

                                         };
            return phoneNumberFormats.ToArray();
        }

        public ActionResult yMap(string phone)
        {
            var model = new yMapWeather.Models.UserLocation { PhoneNumber = phone };
            Int16 err = 0;
            model.GetUserLocation(ref err);

            return View(model);
        }

        public ActionResult yWeather(string phone)
        {
            var model = new yMapWeather.Models.UserLocation() { PhoneNumber = phone };
            Int16 err = 0;
            model.GetUserLocation(ref err);

            var WOEID = model.GetWOEIDForLocation(ref err);
            var wi = new yMapWeather.Models.WeatherInfo();

            wi.WeatherForcast(WOEID);

            return View(wi);
        }

        public JsonResult GetSchedule(int franchiseId, DateTime requesteddate)
        {
            var s = requesteddate.Date;
            var e = s.AddDays(1);
            using (var dbContext = GetContext())
            {
                var jobs = dbContext.tbl_Job.Where(j => j.FranchiseID == franchiseId && j.ScheduleStart >= s && j.ScheduleStart < e)
                                            .Select(j => new { S = j.ScheduleStart, E = j.ScheduleEnd })
                                            .OrderBy(j => j.S).ToArray();

                var bookedHours = jobs.Select(j => Enumerable.Range(j.S.GetValueOrDefault().Hour, j.E.GetValueOrDefault().Hour)).SelectMany(h => h.ToArray()).Distinct();
                var freeHours = Enumerable.Range(1, 24).Except(bookedHours);

                var json = new
                {
                    total_count = freeHours.Count(),
                    rows = freeHours.Select(h => new
                    {
                        id = h,
                        data = new object[] {
                                (h > 12 ? h-12 : h).ToString() + (h < 12 ? "AM" : "PM") + " - " + 
                                (h < 24 ? (h > 12 ? h-12 : h)+1 : (h > 12 ? h-12 : h)) +  ((h < 24 ? h+1 : h) < 12 ? "AM" : "PM")
                    }
                    }).ToArray()
                };

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckCreditTerms(int franchiseId, int customerId)
        {
            return Json(new { Success = true, HasTerms = CheckCreditTermsInternal(franchiseId, customerId) });
        }

        private bool CheckCreditTermsInternal(int franchiseId, int customerId)
        {
            using (var dbContext = GetContext())
            {
                return (from i in dbContext.tbl_Customer_Info
                        join t in dbContext.tbl_Customer_CreditTerms
                        on i.CreditTermsID equals t.CreditTermsID
                        where t.CreditTerms != "No Terms" &&
                              i.FranchiseID == franchiseId &&
                              i.CustomerID == customerId
                        select i.CreditTermsID).Any();
            }
        }

        [HttpPost]
        public JsonResult UpdateCustomerInformation(int franchiseId, tbl_Customer customer, tbl_Locations location, tbl_Contacts primaryContact, tbl_Contacts secondaryContact, bool isBillTo)
        {
            var result = AbstractBusinessService.Create<CustomerService>(UserInfo.UserKey).SaveCustomer(franchiseId, customer, location, primaryContact, secondaryContact, isBillTo);

            if (!result.Success)
                return Json(new {Success = false, Message = "Could not save customer information: " + result.Message});
            
            return Json(new
                            {
                                Success = true,
                                CustomerId = customer.CustomerID,
                                LocationId = location.LocationID,
                                PrimaryContactId = primaryContact.ContactID,
                                SecondaryContactId = secondaryContact.ContactID
                            });
        }

        [HttpPost]
        public JsonResult SetStatisticTrack()
        {
            var user = MembershipService.GetUser(User.Identity.Name);
            var id = Request.Form["id"];
            var id2 = Request.Form["id2"];
            var stat = new StatisticTrack { CalledNumber = id, DialedNumber = id2, StartDate = DateTime.Now, UserId = (Guid)user.ProviderUserKey };
            db.AddToStatisticTracks(stat);
            db.SaveChanges();
            return Json(new { result = true, trackId = stat.TrackId });
            //return Json(new { result = true, trackId = 1 });
        }

        public JsonResult GetEndOptions()
        {
            return Json(new { result = true, data = db.EndCallOptions.Select(item => new { id = item.OptionId, name = item.OptionName }).ToArray() });
            //return Json(new
            //{
            //    result = true,
            //    data = new[]
            //               {
            //                   new {id = 1, name = "option1"}, 
            //                   new {id = 2, name = "option2"}, 
            //                   new {id = 3, name = "option3"}, 
            //                   new {id = 4, name = "option4"}, 
            //                   new {id = 5, name = "option5"}, 
            //                   new {id = 6, name = "option6"}, 
            //                   new {id = 7, name = "option7"}, 
            //                   new {id = 8, name = "option8"}, 
            //                   new {id = 9, name = "option9"}, 
            //                   new {id = 10, name = "option10"}
            //               }
            //});
        }

        public JsonResult UpdateStatisticAfterEnd()
        {
            var trackId = int.Parse(Request.Form["trackId"]);
            if (db.StatisticTracks.Any(item=>item.TrackId==trackId))
            {
                var optionId = int.Parse(Request.Form["optionId"]);
                var endTime = (new DateTime()).AddMilliseconds(int.Parse(Request.Form["endTime"]))-new DateTime();
                var stat = db.StatisticTracks.Single(item => item.TrackId == trackId);
                stat.Duration = endTime;
                stat.OptionId = optionId;
                db.SaveChanges();
                return Json(new {result = true});
            }
            return Json(new {result = false});
            //return Json(new { result = true });
        }

        public ActionResult ShowStatistic()
        {
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var oList = db.EndCallOptions.Select(item=>new {name = item.OptionName, count = item.StatisticTracks.Count()}).ToArray();
            ViewBag.Data = oSerializer.Serialize(oList).Replace("\\","");
            return View("ShowStatistic");
        }

        public JsonResult GetSummaryDataPer24Hours()
        {
            var end = DateTime.Now;
            var start = end.AddDays(-1);
            var userId = (Guid)MembershipService.GetUser(User.Identity.Name).ProviderUserKey;
            var context = new IncomingCallsQAEntities();

            var st = context.StatisticTracks.Where(item => item.UserId == userId && item.StartDate >= start && item.StartDate <= end).ToList();
            var last = st.Last();
            st.Remove(last);
            var completed = st.Count(item => item.OptionId != null);
            var uncompleted = st.Count(item => item.OptionId == null);


            var stat = context.StatisticTracks.Where(item => item.StartDate >= start && item.StartDate <= end).ToList();
            stat.Remove(last);
            var teamcompleted = stat.Count(item => item.OptionId != null);
            var teamuncompleted = stat.Count(item => item.OptionId == null);

            return
                Json(
                    new
                        {
                            userstat =
                        new[]
                            {
                                new {text = "competed", value = completed, color = "#00FF00"},
                                new {text = "uncompleted", value = uncompleted, color = "#FF0000"}
                            },
                            teamstat =
                        new[]
                            {
                                new {text = "competed", value = teamcompleted, color = "#00FF00"},
                                new {text = "uncompleted", value = teamuncompleted, color = "#FF0000"}
                            }
                        });

            //return
            //    Json(
            //        new
            //        {
            //            userstat = new[] { new { text = "competed", value = 33, color = "#00FF00" }, new { text = "uncompleted", value = 7, color = "#FF0000" } },
            //            teamstat = new[] { new { text = "competed", value = 200, color = "#00FF00" }, new { text = "uncompleted", value = 77, color = "#FF0000" } }
            //        });
        }

        public ActionResult ShowStat24(string id, string id2)
        {
            var userId = (Guid)MembershipService.GetUser(User.Identity.Name).ProviderUserKey;
            //var id = Request.Form["id"];
            //var id2 = Request.Form["id2"];
            var end = DateTime.Now;
            var start = end.AddDays(-1);
            var context = new IncomingCallsQAEntities();
            var list = context.StatisticTracks.Where(
                i => i.UserId == userId && i.StartDate >= start && i.StartDate <= end && i.OptionId == null)
                .Select(
                    t =>
                    new CallStat
                        {
                            TrackId = t.TrackId,
                            CalledNumber = t.CalledNumber,
                            DialedNumber = t.DialedNumber,
                            StartDate = t.StartDate
                        }).ToList();
            var listOfOptions = context.EndCallOptions.Select(item => new Option { OptionId = item.OptionId, OptionName = item.OptionName }).ToList();
            listOfOptions.Add(new Option(){OptionId = -1, OptionName = "None"});
            var model = new Stat24Model { StartDate = start, EndDate = end, UserStat24 = list, AllPossibleOptions = listOfOptions };
            return View(model);
        }


        public ActionResult FakeShowStat24(string id, string id2)
        {
            var model = new Stat24Model
                            {
                                StartDate = DateTime.Now.AddDays(-1),
                                EndDate = DateTime.Now,
                                UserStat24 = new List<CallStat>
                                                 {
                                                     new CallStat
                                                         {
                                                             TrackId = 1,
                                                             CalledNumber = "800XXXXX1",
                                                             DialedNumber = "8000YYYYYY1",
                                                             StartDate = DateTime.Now.AddHours(-1)

                                                         },
                                                     new CallStat
                                                         {
                                                             TrackId = 2,
                                                             CalledNumber = "800XXXXX2",
                                                             DialedNumber = "8000YYYYYY2",
                                                             StartDate = DateTime.Now.AddHours(-1)
                                                         },
                                                     new CallStat
                                                         {
                                                             TrackId = 3,
                                                             CalledNumber = "800XXXXX3",
                                                             DialedNumber = "8000YYYYYY3",
                                                             StartDate = DateTime.Now.AddHours(-1)
                                                         }
                                                 },
                                AllPossibleOptions = new List<Option>
                                                         {
                                                             new Option {OptionId = -1, OptionName = "none"},
                                                             new Option {OptionId = 0, OptionName = "opt0"},
                                                             new Option {OptionId = 1, OptionName = "opt1"},
                                                             new Option {OptionId = 2, OptionName = "opt2"}
                                                         }
                            };
            return View("ShowStat24", model);
        }

        public class CallStat
        {
            public int TrackId { get; set; }
            public string CalledNumber { get; set; }
            public string DialedNumber { get; set; }
            public DateTime StartDate { get; set; }
            public TimeSpan Duration { get; set; }
            public Option Option { get; set; }
        }

        public class Option
        {
            public int OptionId { get; set; }
            public string OptionName { get; set; }
        }

        public class Stat24Model
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<Option> AllPossibleOptions { get; set; }
            public List<CallStat> UserStat24 { get; set; } 
        }

        public ActionResult SaveChangesTracks24()
        {
            string type;
            var trackId = 0;
            try
            {
                trackId = int.Parse(Request.Form["gr_id"]);
                var context = new IncomingCallsQAEntities();
                var track = context.StatisticTracks.Single(item => item.TrackId == trackId);
                track.OptionId = int.Parse(Request.Form["optionId"]);
                context.SaveChanges();
                type = "updated";
            }
            catch (Exception)
            {
                type = "fail";
            }
            return new XmlResult(String.Format("<data><action type=\"{0}\" sid=\"{1}\" tid=\"{2}\"/></data>", type, trackId, trackId));
        }

        public ViewResult FakeIncoming(string id, string id2, bool? fullRender)
        {
            Incoming vm;
            vm = new Incoming
                     {
                         CPCode = "aaa",
                         CustomerPhone = "ManualEntry",
                         CallScript = "aaaa",
                         RawDialInNumber = "YYYYYY"
                     };
            vm.DialInNumber = vm.RawDialInNumber.TrimStart('1');
                vm.Valid = true;
            return View("IncomingSplash", vm);
        }
    }
}
