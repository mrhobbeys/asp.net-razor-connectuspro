using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using System.Data.Entity;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Areas.SecurityGuard.Models;
using SiteBlue.Data.EightHundred;
using SiteBlue.Controllers;
using SiteBlue.Business;
using DHTMLX.Export.Excel;
using System.IO;
using SiteBlue.Data.Reporting;
using SiteBlue.Business.Reporting;
using SiteBlue.Business.PayrollSetup;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class PayrollHistoryController : SiteBlueBaseController
    {
        private MembershipConnection memberShipContext = new MembershipConnection();

        EightHundredEntities db = new EightHundredEntities();
        ReportingEntities rptDB = new ReportingEntities();

        public ActionResult Index()
        {
            return RedirectToAction("GetPayrollHistroy");
        }

        public PartialViewResult PartialPayrollHistory(string code)
        {
            if (code.LastIndexOf("-") > 0)
            {
                code = code.Substring(code.LastIndexOf("-") + 1).Trim();
            }
            var FranchiseID = (from g in memberShipContext.MembershipFranchise
                           where g.FranchiseNumber == code
                           select g.FranchiseID).FirstOrDefault();

            Session["selectedFranchiseId"] = FranchiseID;
            Session["selectedFranchiseCode"] = code;
            Session["Code"] = code;

            return PartialView("PartialPayrollHistory", FranchiseID);
        }

        public ActionResult GetPayrollHistroy()
        {

            return View();
        }

        public ActionResult GetParyrollSummary(string franchisid)
        {
            int frid = Convert.ToInt32(franchisid);
            List<Payroll> payrollList = (from payroll in rptDB.vRpt_Payroll
                                                 where payroll.FranchiseID == frid
                                                 orderby payroll.PayrollDate descending
                                                 select new Payroll()
                                                    {
                                                        PayrollID = payroll.PayrollID,
                                                        PayrollDate = payroll.PayrollDate,
                                                        GrossPay = (float)payroll.GrossPay,
                                                        LockedYN = (payroll.LockDate.HasValue == true)
                                                    }
                                                ).ToList();
            
            return Json(payrollList);
        }

        public ActionResult PayrollProcessing()
        {
            try
            {
                DateTime selectedDate = DateTime.MinValue;
                if (Request.QueryString["dt"] != null)
                    selectedDate = DateTime.Parse(Request.QueryString["dt"]);
                int payrollID = -1;
                if (Request.QueryString["Id"] != null)
                    payrollID = int.Parse(Request.QueryString["Id"]);

                int frid = 0;
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;

                if (isCorporate == true)
                {
                    frid = 56;
                }
                else
                {
                    using (var ctx = new MembershipConnection())
                    {
                        assignedFranchises = ctx.UserFranchise
                                                .Where(uf => uf.UserId == userId)
                                                .Select(f => f.FranchiseID)
                                                .ToArray();
                    }
                    if (assignedFranchises.Count() > 0)
                    {
                        frid = assignedFranchises[0];
                    }

                }

                // Payroll Dates ALWAYS fall on a Saturday.
                // Give them list of the past 5 Saturdays and if they need farther back send them to Payroll History
                List<SelectListItem> payrollDates = new List<SelectListItem>();
                DateTime dateToAdd = DateTime.Now.Date;
                while (dateToAdd.DayOfWeek != DayOfWeek.Saturday)   // Get the next saturday (including today)
                    dateToAdd = dateToAdd.AddDays(1.0);
                for (int i = 0; i < 5; i++)                         // Add 5 Saturdays to the dropdownlist
                {
                    payrollDates.Add(new SelectListItem() { Text = dateToAdd.ToShortDateString(), Value = dateToAdd.ToShortDateString() });
                    dateToAdd = dateToAdd.AddDays(-7.0);
                }
                if (selectedDate != DateTime.MinValue)
                {
                    SelectListItem itemSelectedDate = (from SelectListItem item in payrollDates
                                                    where DateTime.Parse(item.Value) == selectedDate
                                                    select item).FirstOrDefault();
                    if (itemSelectedDate == null)
                    {
                        do
                        {
                            SelectListItem itemToAdd = new SelectListItem() { Text = dateToAdd.ToShortDateString(), Value = dateToAdd.ToShortDateString() };
                            if (dateToAdd == selectedDate)
                            {
                                itemToAdd.Selected = true;
                                selectedDate = DateTime.MinValue;
                            }
                            payrollDates.Add(itemToAdd);
                            dateToAdd = dateToAdd.AddDays(-7.0);
                        } while (selectedDate != DateTime.MinValue);
                    }
                    else
                        itemSelectedDate.Selected = true;
                }

                // Finally add the "Other" option
                payrollDates.Add(new SelectListItem() { Text = "Other", Value = "OTHER_DATE" });

                ViewData["DropDownList_PayrollDates"] = payrollDates;

                vRpt_Payroll objpayroll = (from payroll in rptDB.vRpt_Payroll where payroll.FranchiseID == frid && payroll.PayrollID == payrollID select payroll).FirstOrDefault();
                if (objpayroll != null)
                {
                    DateTime dt = Convert.ToDateTime(objpayroll.PayrollDate);

                    ViewBag.Date = dt.ToShortDateString();
                    ViewBag.PayrollID = objpayroll.PayrollID.ToString();

                    selectedDate = dt;
                    if (selectedDate != DateTime.MinValue)
                    {
                        SelectListItem itemSelectedDate = (from SelectListItem item in payrollDates
                                                           where DateTime.Parse(item.Value) == selectedDate
                                                           select item).FirstOrDefault();
                        if (itemSelectedDate == null)
                        {
                            do
                            {
                                SelectListItem itemToAdd = new SelectListItem() { Text = dateToAdd.ToShortDateString(), Value = dateToAdd.ToShortDateString() };
                                if (dateToAdd == selectedDate)
                                {
                                    itemToAdd.Selected = true;
                                    selectedDate = DateTime.MinValue;
                                }
                                payrollDates.Add(itemToAdd);
                                dateToAdd = dateToAdd.AddDays(-7.0);
                            } while (selectedDate != DateTime.MinValue);
                        }
                        else
                            itemSelectedDate.Selected = true;
                    }

                }
                else
                {
                    ViewBag.PayrollID = "0";
                    ViewBag.Date = System.DateTime.Now.ToShortDateString();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }

        public PartialViewResult PartialPayrollProcessing(string code)
        {

            try
            {
                int frid = 0;
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;

                if (isCorporate == true)
                {
                    frid = 56;
                }
                else
                {
                    using (var ctx = new MembershipConnection())
                    {
                        assignedFranchises = ctx.UserFranchise
                                                .Where(uf => uf.UserId == userId)
                                                .Select(f => f.FranchiseID)
                                                .ToArray();
                    }
                    if (assignedFranchises.Count() > 0)
                    {
                        frid = assignedFranchises[0];
                    }

                }

                tbl_Payroll objpayroll = (from payroll in db.tbl_Payroll where payroll.FranchiseID == frid select payroll).FirstOrDefault();
                if (objpayroll != null)
                {
                    DateTime dt = Convert.ToDateTime(objpayroll.PayrollDate);

                    ViewBag.Date = dt.ToShortDateString();
                    ViewBag.PayrollID = objpayroll.PayrollID.ToString();
                }
                else
                {
                    ViewBag.PayrollID = "0";
                    ViewBag.Date = System.DateTime.Now.ToShortDateString();
                }
                return PartialView("PartialPayrollProcessing");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult GetPayrollDetails(string id, string franchisid)
        {
            return GetPayrollDetailsByDate(id, franchisid, DateTime.MinValue);
        }

        public ActionResult GetPayrollDetailsByDate(string id, string franchisid, DateTime payrollDate)
        {
            int payrollid = Convert.ToInt32(id);
            int payrollSearchid = payrollid;

            vRpt_Payroll objpayroll = null;
            if (payrollid == -1) // Get it by date instead
                objpayroll = (from payroll in rptDB.vRpt_Payroll where payroll.PayrollDate == payrollDate select payroll).FirstOrDefault();
            else
                objpayroll = (from payroll in rptDB.vRpt_Payroll where payroll.PayrollID == payrollSearchid select payroll).FirstOrDefault();

            if (objpayroll != null)
            {
                payrollid = objpayroll.PayrollID;
            }

            List<vRpt_PayrollDetail> objPayrollDetails = (from payrolldetails in rptDB.vRpt_PayrollDetail where payrolldetails.PayrollID == payrollid orderby payrolldetails.EmployeeID select payrolldetails).ToList();
            List<Payroll_Details> objPayroll_Details = new List<Payroll_Details>();

            foreach (var item in objPayrollDetails)
            {
                tbl_Employee objEmployees = (from emp in db.tbl_Employee where emp.EmployeeID == item.EmployeeID select emp).SingleOrDefault();
                Payroll_Details oPayroll_Details = new Payroll_Details();
                oPayroll_Details.PayType = "";
                oPayroll_Details.Employee = objEmployees.Employee;
                oPayroll_Details.JobCount = item.JobCount.Value;
                oPayroll_Details.WeeklySalary = item.WeeklySalary;
                oPayroll_Details.RegularHours = item.RegularHours;
                oPayroll_Details.RegularPay = item.RegularPay;
                oPayroll_Details.OTHours = item.OTHours;
                oPayroll_Details.OTPay = item.OTPay;
                oPayroll_Details.TotalCommission = (float)item.TotalCommission;
                oPayroll_Details.OTAdditCommission = (float)item.OTAdditCommission;
                oPayroll_Details.Adjustment = item.Adjustment;
                oPayroll_Details.GrossPay = (float)item.GrossPay;
                oPayroll_Details.PayrollLockDateString = (item.LockDate.HasValue) ? item.LockDate.Value.ToShortDateString() : null;
                objPayroll_Details.Add(oPayroll_Details);

            }

            return Json(objPayroll_Details);
        }

        public IQueryable<tbl_Job> GetJob(int jobid)
        {
            var Job = (from job in db.tbl_Job where job.JobID == jobid select job);
            return (Job);
        }

        public ActionResult Lock_SelectedWeek(DateTime date, int payrollid, string FranchiseId)
        {
            DateTime dateToLockPayroll = DateTime.Now.Date;

            SiteBlue.Business.Payroll.PayrollService payrollService =
            SiteBlue.Business.Payroll.PayrollService.Create<SiteBlue.Business.Payroll.PayrollService>(UserInfo.UserKey);
            OperationResult<DateTime> result = 
                payrollService.LockPayrollForWeekOnDate(date, dateToLockPayroll, FranchiseId, true, UserInfo.UserKey);
            
            ViewBag.Date = date.ToShortDateString();
            return RedirectToAction("PayrollProcessing", new { dt = date });

        }

        /// <summary>
        /// The Main method for Payroll Processing.
        /// Process Payroll for the given week
        /// </summary>
        /// <param name="date"></param>
        /// <param name="payrollid"></param>
        /// <param name="FranchiseId"></param>
        /// <returns></returns>
        public ActionResult ProcessWeek(DateTime date, int payrollid, string FranchiseId)
        {
            SiteBlue.Business.Payroll.PayrollService payrollService =
                SiteBlue.Business.Payroll.PayrollService.Create<SiteBlue.Business.Payroll.PayrollService>(UserInfo.UserKey);
            OperationResult<SiteBlue.Business.Payroll.Payroll> result = payrollService.ProcessPayrollForWeek(date, payrollid, FranchiseId, true, UserInfo.UserKey);

            return RedirectToAction("PayrollProcessing", new { dt = date });

        }

        public ActionResult EmployeePayrollSummary(int id)
        {
            ViewBag.Empid = id;
            var empdata = db.tbl_Employee.FirstOrDefault(e => e.EmployeeID == id);
            //(from p in db.tbl_Employee where p.EmployeeID == id select p).FirstOrDefault();
            //ViewBag.EmpName = empdata.Employee;
            return View(empdata);
        }

        public ActionResult EmployeePayrollSummaryData(int Empid)
        {
            var PayrollDetailList = (from payrollDetail in rptDB.vRpt_PayrollDetail
                                     join payroll in rptDB.vRpt_Payroll on payrollDetail.PayrollID equals payroll.PayrollID
                                     where payrollDetail.EmployeeID == Empid
                                     select new
                                     {
                                         payrollDetail.EmployeeID,
                                         payrollDetail.PayrollID,
                                         PayrollDate = payroll.PayrollDate.Value,
                                         payrollDetail.JobCount,
                                         payrollDetail.WeeklySalary,
                                         hourlyPay = payrollDetail.RegularPay + payrollDetail.OTPay,
                                         payrollDetail.TotalCommission,
                                         payrollDetail.Adjustment,
                                         payrollDetail.GrossPay
                                     });

            return Json(PayrollDetailList);
        }

        public ActionResult EmployeePayrollDetails(string Empid, string Payrollid)
        {
            int EmployeeId = Convert.ToInt32(Empid);
            int payrollID = Convert.ToInt32(Payrollid);
            ViewBag.payrollId = payrollID;
            var emp = (from p in db.tbl_Employee where p.EmployeeID == EmployeeId select p).FirstOrDefault();
            if (emp != null)
            {
                ViewBag.EmpId = emp.EmployeeID;
                ViewBag.EmpName = emp.Employee;
                ViewBag.CommLabor = emp.LaborRate;
                ViewBag.Parts = emp.PartRate;
            }

            var PayrollDetail = (from p in db.tbl_PayrollDetails where p.PayrollID == payrollID && p.EmployeeID == EmployeeId select p).FirstOrDefault();

            var timestatus = (from p in db.tbl_HR_TimeClock_Status select p).ToList();
            ViewBag.statusList = timestatus;

            return View(PayrollDetail);
        }

        public ActionResult PayrollDetailDayData(string Empid, string DetailDay, string id)
        {
            int pId = Convert.ToInt32(id);
            var payrolllist = (from p in db.tbl_Payroll where p.PayrollID == pId select p).FirstOrDefault();
            DateTime PayrollDate = Convert.ToDateTime(payrolllist.PayrollDate);

            DateTime currentDate;
            switch (DetailDay)
            {
                case "Sunday":
                    currentDate = PayrollDate.AddDays(-6);
                    ViewBag.DayDetails = "Sunday Details";

                    break;
                case "Monday":
                    currentDate = PayrollDate.AddDays(-5);
                    ViewBag.DayDetails = "Monday Details";

                    break;
                case "Tuesday":
                    currentDate = PayrollDate.AddDays(-4);
                    ViewBag.DayDetails = "Tuesday Details";

                    break;
                case "Wednesday":
                    currentDate = PayrollDate.AddDays(-3);
                    ViewBag.DayDetails = "Wednesday Details";

                    break;
                case "Thursday":
                    currentDate = PayrollDate.AddDays(-2);
                    ViewBag.DayDetails = "Thursday Details";

                    break;
                case "Friday":
                    currentDate = PayrollDate.AddDays(-1);
                    ViewBag.DayDetails = "Friday Details";

                    break;
                case "Saturday":
                    currentDate = PayrollDate;
                    ViewBag.DayDetails = "Saturday Details";

                    break;
                default:
                    currentDate = PayrollDate.AddDays(-6);
                    ViewBag.DayDetails = "Sunday Details";

                    break;
            }

            int EmployeeId = Convert.ToInt32(Empid);
            DateTime strDate = Convert.ToDateTime(currentDate.ToShortDateString() + " " + "12:00:00 AM");
            DateTime endDate = Convert.ToDateTime(currentDate.ToShortDateString() + " " + "11:59:59 PM");
            var timeclock = (from p in db.tbl_HR_TimeClock
                             join p1 in db.tbl_HR_TimeClock_Status on p.TimeClockStatusID equals p1.TimeClockStatusID
                             where p.EmployeeID == EmployeeId &&
                             p.DateTimeStatusChanged >= strDate && p.DateTimeStatusChanged <= endDate
                             select new
                             {
                                 p.Comments,
                                 p.DateTimeStatusChanged,
                                 p1.TimeClockStatusDesc,
                                 p.TimeClockID
                             }).ToList();

            return Json(timeclock);
        }

        public ActionResult PayrollJobData(string Empid, string Payrollid)
        {
            int EmployeeId = Convert.ToInt32(Empid);
            int payrollid = Convert.ToInt32(Payrollid);
            var jobpayrolllist = (from j in db.tbl_Job_Payroll
                                  join j1 in db.tbl_Job on j.JobID equals j1.JobID
                                  join j2 in db.tbl_Locations on j1.LocationID equals j2.LocationID
                                  where j.PayrollID == payrollid && j.ServiceProID == EmployeeId
                                  select new
                                  {
                                      j.JobID,
                                      j2.Address,
                                      j1.ServiceDate,
                                      j.JobPartsTotal,
                                      j.TotalCommissionParts,
                                      j.JobLaborTotal,
                                      j.TotalCommissionLabor,
                                      j.TotalCommissionSpifs,
                                      GrossPay = j.TotalCommissionParts + j.TotalCommissionLabor + j.TotalCommissionSpifs
                                  });

            return Json(jobpayrolllist);
        }

        public ActionResult TimeClockEditData(int timeId)
        {
            var time = (from p in db.tbl_HR_TimeClock where p.TimeClockID == timeId select p).ToList();

            var id = (from p in db.tbl_HR_TimeClock where p.TimeClockID == timeId select p).FirstOrDefault();
            ViewBag.statusid = id.TimeClockStatusID;
            var timestatus = (from p in db.tbl_HR_TimeClock_Status select p).ToList();
            ViewBag.statusList = timestatus;
            return Json(time);
        }

        /// <summary>
        /// This is my "Repository Loader".  Not sure where in the solution to put this - BPanjavan
        /// </summary>
        /// <returns></returns>
        private SiteBlue.Areas.OwnerPortal.Models.PayrollSetup GetViewModel()
        {
            SiteBlue.Areas.OwnerPortal.Models.PayrollSetup payrollSetupViewModel = null;

            // Going to need this to set on our view-model
            ReportingService reportingService = ReportingService.Create<ReportingService>(UserInfo.UserKey);
            var arrOvertimeMethod = (from overtimeMethod in reportingService.GetOvertimeMethods()
                                     select new SiteBlue.Areas.OwnerPortal.Models.OvertimeMethod()
                                     {
                                         OvertimeMethodName = overtimeMethod.OvertimeMethodName,
                                         OvertimeMethodID = overtimeMethod.OvertimeMethodID
                                     }
                                    ).ToArray();
            var arrJobCode = (from jobCode in reportingService.GetJobCodes(UserInfo.CurrentFranchise.FranchiseID)
                                     select new SiteBlue.Areas.OwnerPortal.Models.JobCode()
                                     {
                                         JobCodeID = jobCode.JobCodeID,
                                         JobCodeDescription = jobCode.JobCodeDescription,
                                         JobCodeName = jobCode.JobCodeName,
                                         ActiveYN = jobCode.ActiveYN,
                                         FranchiseID = jobCode.FranchiseID,
                                         PriceBookActiveYN = jobCode.PriceBookActiveYN,
                                         PriceBookID = jobCode.PriceBookID,
                                         PriceBookName = jobCode.PriceBookName
                                     }
                                    ).ToArray();
            var arrEmployee = (from employee in reportingService.GetEmployees(UserInfo.CurrentFranchise.FranchiseID)
                               select new SiteBlue.Areas.OwnerPortal.Models.Employees()
                                    {
                                        EmployeeID = employee.EmployeeID,
                                        Employee = employee.EmployeeName,
                                        ActiveYN = employee.ActiveYN,
                                        FranchiseID = employee.FranchiseID,
                                        ServiceProYN = employee.ServiceProYN
                                    }
                               ).ToArray();
            var arrSpiffPayType = (
                                    from spiffPayType in reportingService.GetSpiffPayTypes()
                                    select new SiteBlue.Areas.OwnerPortal.Models.SpiffPayType()
                                        {
                                             SpiffPayTypeID = spiffPayType.SpiffPayTypeID,
                                             SpiffPayTypeName = spiffPayType.SpiffPayTypeName
                                        }
                                    ).ToArray();
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = reportingService.GetPayrollSetupData(UserInfo.CurrentFranchise.FranchiseID);

            // If there is NO existing PayrollSetup, load an empty one
            if (arrPayrollSetup.Length == 0)
            {
                payrollSetupViewModel =
                        new SiteBlue.Areas.OwnerPortal.Models.PayrollSetup(UserInfo.CurrentFranchise.FranchiseID, 0, 0, 0M, null, arrOvertimeMethod,
                                                 arrJobCode, arrEmployee, arrSpiffPayType);
            }
            else
            {
                // Otherwise, load our PayrollSetup Data
                SiteBlue.Business.Reporting.PayrollSetup payrollSetupExisting = arrPayrollSetup[0];
                payrollSetupViewModel =
                        new SiteBlue.Areas.OwnerPortal.Models.PayrollSetup(
                                    UserInfo.CurrentFranchise.FranchiseID,
                                    (decimal)payrollSetupExisting.OvertimeStarts,
                                    payrollSetupExisting.OvertimeMethodID,
                                    (decimal)payrollSetupExisting.OTMultiplier,

                                    (from reportingSpiff in payrollSetupExisting.PayrollSpiffs
                                     select new SiteBlue.Areas.OwnerPortal.Models.PayrollSpiff()
                                     {
                                         Active = reportingSpiff.Active,
                                         AddOn = reportingSpiff.AddOn,
                                         Comments = reportingSpiff.Comments,
                                         DateExpires = reportingSpiff.DateExpires,
                                         DateExpiresFormatted = reportingSpiff.DateExpires.Value.ToShortDateString(),
                                         JobCode = reportingSpiff.JobCode,
                                         JobCodeDescription = reportingSpiff.JobCodeDescription,
                                         JobCodeID = reportingSpiff.JobCodeID,
                                         PayrollSetupID = reportingSpiff.PayrollSetupID,
                                         PayrollSpiffID = reportingSpiff.PayrollSpiffID,
                                         PayType = reportingSpiff.PayType,
                                         PayTypeID = reportingSpiff.PayTypeID,
                                         Rate = reportingSpiff.Rate,
                                         ServiceProID = reportingSpiff.ServiceProID,
                                         Employee = reportingSpiff.Employee
                                     }),

                                    arrOvertimeMethod, arrJobCode, arrEmployee, arrSpiffPayType) { PayrollSetupID = arrPayrollSetup[0].PayrollSetupID };
            }

            return payrollSetupViewModel;
        }

        public ActionResult PayrollSetup(FormCollection collection)
        {
            SiteBlue.Areas.OwnerPortal.Models.PayrollSetup payrollSetupViewModel = GetViewModel();
            ViewBag.PayrollSetupID = payrollSetupViewModel.PayrollSetupID;

            // TODO: Used to populate parts list.  We want to populate with Jobs List.  We'll worry about this later
            var Partslist = (from s in db.tbl_PB_Parts
                             join t in db.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                             join m in db.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                             where t.ActiveBookYN == true
                             select new
                             {
                                 s.PartID,
                                 m.PartCode,
                                 m.PartName,
                                 CodeDesc = m.PartCode + " -" + m.PartName
                             }).Take(10).ToList();
            ViewBag.ddlPartCode = Partslist;

            var PartsDesc = (from s in db.tbl_PB_Parts
                             join t in db.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                             join m in db.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                             where t.ActiveBookYN == true
                             select new
                             {
                                 s.PartID,
                                 m.PartCode,
                                 m.PartName
                             }).FirstOrDefault();
            if (PartsDesc != null)
            {
                ViewBag.txtPartDescriptionAdd = PartsDesc.PartName;
            }

            var ServiceProlist = (from p in db.tbl_Employee where p.ServiceProYN == true select p).ToList();
            ViewBag.ddlServicePro = ServiceProlist;


            List<SelectListItem> itemsPaytype = new List<SelectListItem>();
            itemsPaytype.Add(new SelectListItem
            {
                Text = "Flat Rate",
                Value = "0",
                Selected = true
            });
            itemsPaytype.Add(new SelectListItem
            {
                Text = "Commission",
                Value = "1"
            });
            ViewBag.ddlPayType = itemsPaytype;

            return View(payrollSetupViewModel);
        }

        public ActionResult SavePayrollSetupData(int FranchiseeId, string method, string multiplier, string start)
        {
            int franchiseID = FranchiseeId;
            decimal overtimeStarts = decimal.Parse(start);
            int overtimeMethod = int.Parse(method);
            decimal overtimeMultiplier = decimal.Parse(multiplier);

            PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(UserInfo.UserKey);
            OperationResult<SiteBlue.Business.PayrollSetup.PayrollSetup> result = payrollSetupService.SavePayrollSetup(franchiseID, overtimeStarts, overtimeMethod, overtimeMultiplier);

            var toReturn = (result.Success == true) ?
                                new { Success = result.Success, Message = "Payroll Setup saved" }
                                : new { Success = result.Success, Message = "Error: " + result.Message };
            return Json(toReturn);
        }

        public ActionResult PayrollPartExceptionData(bool checkStatus, string partsDesc, string partsCode, int FranchiseeId)
        {
            // Load spiffs from ViewModel instead of Parts List
            SiteBlue.Areas.OwnerPortal.Models.PayrollSetup payrollSetupViewModel = GetViewModel();
            return Json(payrollSetupViewModel.PayrollSpiffs_JSON);


            ////Session["FranchiseId"] = franchiseId;
            //var PartList = (from e in db.tbl_HR_PartsExceptions
            //                join e1 in db.tbl_PB_Parts on e.PartID equals e1.PartID
            //                join e2 in db.tbl_PB_MasterParts on e1.MasterPartID equals e2.MasterPartID
            //                join e3 in db.tbl_Employee on e.ServiceProID equals e3.EmployeeID into parts_emp
            //                from e3 in parts_emp.DefaultIfEmpty()
            //                where e.FranchiseID == FranchiseeId && e.ActiveYN == checkStatus
            //                select new
            //                {
            //                    e.PartExceptionID,
            //                    e.PartID,
            //                    e2.PartCode,
            //                    e2.PartName,
            //                    Employee = (e3.Employee == null ? "" : e3.Employee),
            //                    e.PayType,
            //                    e.Rate,
            //                    e.AddonYN,
            //                    e.DateExpires,
            //                    e.Comments,
            //                    e.ActiveYN,
            //                    e.ServiceProID
            //                }).ToList();

            //if (partsDesc != null && partsDesc.Trim() != "" && partsDesc != "null")
            //{
            //    PartList = PartList.Where(p => p.PartName.ToLower().Contains(partsDesc.ToLower())).ToList();
            //}
            //if (partsCode != null && partsCode.Trim() != "" && partsCode != "null")
            //{
            //    PartList = PartList.Where(p => p.PartCode.ToLower().Contains(partsCode.ToLower())).ToList();
            //}

            //return Json(PartList);
        }

        public ActionResult PayrollExceptionDataAdd_PartCode(int FranchiseeId)
        {
            var Partslist = (from s in db.tbl_PB_Parts
                             join t in db.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                             join m in db.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                             where t.FranchiseID == FranchiseeId && t.ActiveBookYN == true
                             select new
                             {
                                 s.PartID,
                                 m.PartCode,
                                 m.PartName,
                                 CodeDesc = m.PartCode + " -" + m.PartName
                             }).ToList();

            var listItems = "";
            for (var i = 0; i < Partslist.Count(); i++)
            {
                listItems += "<option value='" + Partslist[i].PartID + "'>" + Partslist[i].PartCode + "</option>";
            }

            return Json(listItems);
        }

        public ActionResult PayrollExceptionDataAdd_PartDesc(int FranchiseeId)
        {
            var PartsDesc = (from s in db.tbl_PB_Parts
                             join t in db.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                             join m in db.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                             where t.FranchiseID == FranchiseeId && t.ActiveBookYN == true
                             select new
                             {
                                 s.PartID,
                                 m.PartCode,
                                 m.PartName
                             }).ToList();
            return Json(PartsDesc);
        }

        public ActionResult PayrollExceptionDataAdd_ServicePro(int FranchiseeId)
        {
            var ServiceProlist = (from p in db.tbl_Employee where p.ServiceProYN == true && p.FranchiseID == FranchiseeId select p).ToList();
            var listItems = "";
            for (var i = 0; i < ServiceProlist.Count(); i++)
            {
                listItems += "<option value='" + ServiceProlist[i].EmployeeID + "'>" + ServiceProlist[i].Employee + "</option>";
            }
            return Json(listItems);
        }

        public ActionResult PayrollSpiffAdd(
                        string strCurrentPayrollSpiffID,
                        string strPayrollSetupID,
                        string strJobCodeID, string strServiceProID ,string  strPayTypeID , string strRate , string strAddOn , 
                        string strDateExpires , string strActive , string strComments)
        {
            try
            {
                int payrollSpiffID = int.Parse(strCurrentPayrollSpiffID);
                int payrollSetupID = int.Parse(strPayrollSetupID);
                int jobCodeID = int.Parse(strJobCodeID);
                int serviceProID = int.Parse(strServiceProID);
                int payTypeID = int.Parse(strPayTypeID);
                decimal rate = decimal.Parse(strRate);
                bool addOn = strAddOn != null;
                DateTime dateExpires = DateTime.Parse(strDateExpires);
                bool active = strActive != null;

                PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(UserInfo.UserKey);
                OperationResult<SiteBlue.Business.PayrollSetup.PayrollSpiff> result = null;

                if (payrollSpiffID == -1)
                    result = payrollSetupService.PayrollSpiff_Add(payrollSetupID, serviceProID, jobCodeID, payTypeID, rate, dateExpires, strComments, addOn, active);
                else
                    result = payrollSetupService.PayrollSpiff_Update(payrollSpiffID, serviceProID, jobCodeID, payTypeID, rate, dateExpires, strComments, addOn, active);

                var toReturn = (result.Success == true) ?
                                    new { Success = result.Success, Message = "Payroll Spiff saved" }
                                    : new { Success = result.Success, Message = "Error: " + result.Message };
                return Json(toReturn);
            }
            catch (Exception ex)
            {
                var toReturn = new { Success = false, Message = "Error: " + ex.Message };
                return Json(toReturn);
            }

        }

        public ActionResult PayrollExceptionDataAdd(int partcode, string partdesc, int servicepro, int paytype, float rate, bool addon, DateTime dateexpires, bool active, string comments, int fid)
        {
            tbl_HR_PartsExceptions partException = new tbl_HR_PartsExceptions();
            partException.PartID = partcode;
            partException.ServiceProID = servicepro;
            partException.PayType = paytype;
            partException.Rate = rate;
            partException.AddonYN = addon;
            partException.DateExpires = dateexpires;
            partException.ActiveYN = active;
            partException.Comments = comments;
            partException.FranchiseID = fid;
            db.tbl_HR_PartsExceptions.AddObject(partException);
            db.SaveChanges();
            var test = "";
            return Json(test);
        }

        public ActionResult PayrollExceptionDataEdit(int partExceptionId)
        {
            SiteBlue.Areas.OwnerPortal.Models.PayrollSetup payrollSetupViewModel = GetViewModel();
            SiteBlue.Areas.OwnerPortal.Models.PayrollSpiff payrollSpiff 
                = (from ps in payrollSetupViewModel.PayrollSpiffs
                   where ps.PayrollSpiffID == partExceptionId
                   select ps).First<SiteBlue.Areas.OwnerPortal.Models.PayrollSpiff>();

            return Json(payrollSpiff);
        }

        public ActionResult PayrollExceptionDataUpdate(int partcode, string partdesc, int servicepro, int paytype, float rate, bool addon, DateTime dateexpires, bool active, string comments, int fid, int partExceptionId)
        {
            var partExceptionUpdate = (from p in db.tbl_HR_PartsExceptions where p.PartExceptionID == partExceptionId select p).FirstOrDefault();
            partExceptionUpdate.PartID = partcode;
            partExceptionUpdate.ServiceProID = servicepro;
            partExceptionUpdate.PayType = paytype;
            partExceptionUpdate.Rate = rate;
            partExceptionUpdate.AddonYN = addon;
            partExceptionUpdate.DateExpires = dateexpires;
            partExceptionUpdate.ActiveYN = active;
            partExceptionUpdate.Comments = comments;
            partExceptionUpdate.FranchiseID = fid;
            db.SaveChanges();
            var test = "";
            return Json(test);
        }
        //public ActionResult Data()
        //{
        //    var context = new OwnerPortalContext();
        //    return View(context.timeClock);
        //}

        //public ActionResult Save(HR_TimeClock timeclock, FormCollection form)
        //{
        //    string action_type = form["!nativeeditor_status"];
        //    long source_id = long.Parse(form["gr_id"]);
        //    long target_id = long.Parse(form["gr_id"]);

        //    var context = new OwnerPortalContext();

        //    try
        //    {
        //        switch (action_type)
        //        {
        //            case "inserted":
        //                context.timeClock.Add(timeclock);
        //                break;
        //            case "deleted":
        //                timeclock = context.timeClock.SingleOrDefault(u => u.TimeClockID == source_id);
        //                context.timeClock.Remove(timeclock);
        //                break;
        //            default: // "updated"                          
        //                timeclock = context.timeClock.SingleOrDefault(u => u.TimeClockID == source_id);
        //                UpdateModel(timeclock);
        //                break;
        //        }
        //        context.SaveChanges();
        //        target_id = timeclock.TimeClockID;
        //    }
        //    catch (Exception e)
        //    {
        //        action_type = "error";
        //        string s = e.ToString();
        //    }
        //    return View(new ActionResponseModel(action_type, source_id, target_id));
        //}

        [HttpPost, ValidateInput(false)]
        public ActionResult ExportToExcel()
        {
            var generator = new ExcelWriter();
            string xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            MemoryStream stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "PayrollData.xlsx");
        }

    }
}
