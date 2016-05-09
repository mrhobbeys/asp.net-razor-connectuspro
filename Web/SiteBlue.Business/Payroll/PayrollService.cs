using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using SiteBlue.Core;
using SiteBlue.Core.Email;
using SiteBlue.Data.EightHundred;

using InvoiceGen = SiteBlue.Business.Enterprise.InvoiceGeneration;
using SiteBlue.Business.Reporting;

namespace SiteBlue.Business.Payroll
{
    public class PayrollService : AbstractBusinessService
    {
        EightHundredEntities db = new EightHundredEntities();

        // instantiate through the AbstractBusinessService.Create
        public PayrollService() { }

        public OperationResult<Payroll> ProcessPayrollForWeek(DateTime date, int payrollidNotUsed, string FranchiseId, bool persistToDataBase, Guid userKey)
        {
            // The date is not getting the end date for the week right now, but we can change that
            while (date.DayOfWeek != DayOfWeek.Saturday)
                date = date.AddDays(1.0);

            // NOTE doesn't make sense to put in a payrollidNotUsed if we have the week, so ignore the variable for now.  Kept there to maintain the parameter signature
            var result = new OperationResult<Payroll>();

            int franchiseid = Convert.ToInt32(FranchiseId);
            DateTime startDate = date.AddDays(-7);
            DateTime holddate = date.AddDays(1);

            // Forget about what's in the database, just perform the function to generate a payroll for this week since we re-process it anyways
            // [Step 1] - Get everything we should need so we don't have to make anymore database calls
            List<tbl_Employee> employeelist = (from e in db.tbl_Employee where e.ActiveYN == true && e.FranchiseID == franchiseid select e).ToList<tbl_Employee>();
            List<tbl_HR_TimeSheet> timeSheetList = (from ts in db.tbl_HR_TimeSheet
                                                    join e in db.tbl_Employee on ts.EmployeeID equals e.EmployeeID
                                                    where
                                                        e.FranchiseID == franchiseid
                                                        && ts.WeekEndingDateOn == date
                                                    select ts).ToList<tbl_HR_TimeSheet>();

            var reportingService = AbstractBusinessService.Create<ReportingService>(userKey);
            // Query Jobs within dates from our reporting layer
            KeyPerformanceIndicator kpiAllFranchise = reportingService.GetKeyPerformanceIndicators(
                    franchiseid, startDate, holddate);
            List<Reporting.Job> jobList = kpiAllFranchise.AllJobs.ToList<Reporting.Job>();
            List<Reporting.JobTask> taskList = kpiAllFranchise.AllTasks.ToList();
            List<Reporting.PayrollSetup> payrollsetuplist = reportingService.GetPayrollSetupData(franchiseid).ToList<Reporting.PayrollSetup>();
            
            // Existing Payroll
            List<tbl_Payroll> existingPayrollList = (from p in db.tbl_Payroll where p.PayrollDate == date select p).ToList<tbl_Payroll>();
            tbl_Payroll existingPayroll = (existingPayrollList.Count == 0) ? null : existingPayrollList[0];
            List<tbl_Job_Payroll> jobPayrollListForExistingPayroll = new List<tbl_Job_Payroll>();
            List<tbl_PayrollDetails> payrollDetailsForExistingPayroll = new List<tbl_PayrollDetails>();
            if (existingPayroll != null)
            {
                jobPayrollListForExistingPayroll = (from j in db.tbl_Job_Payroll where j.PayrollID == existingPayroll.PayrollID select j).ToList<tbl_Job_Payroll>();
                payrollDetailsForExistingPayroll = (from pd in db.tbl_PayrollDetails where pd.PayrollID == existingPayroll.PayrollID select pd).ToList<tbl_PayrollDetails>();
            }

            // [Step 2] - Process a new Payroll
            Payroll payrollResult = new Payroll(date, franchiseid);
            ProcessEmployeesForWeek(payrollResult, employeelist, timeSheetList, jobList, taskList, payrollsetuplist);


            // [Step 3] - Now do the database stuff
            tbl_Payroll payrollToUse = null;
            if (existingPayroll != null)                // There IS an existing payroll
            {
                payrollToUse = existingPayroll;

                // blow away any existing JobPayrolls
                foreach (tbl_Job_Payroll existingJobPayroll in jobPayrollListForExistingPayroll)
                    db.DeleteObject(existingJobPayroll);

                // Blow away any existing PayrollDetails
                foreach (tbl_PayrollDetails existingPayrollDetail in payrollDetailsForExistingPayroll)
                    db.DeleteObject(existingPayrollDetail);
            }
            else
            {
                // Create a new payroll
                payrollToUse = new tbl_Payroll();
                payrollToUse.LockDate = null;
                payrollToUse.PayrollDate = date;
                payrollToUse.FranchiseID = franchiseid;
                db.tbl_Payroll.AddObject(payrollToUse);
            }

            // add all the payroll details
            foreach (PayrollDetail pd in payrollResult.PayrollDetails)
            {
                tbl_PayrollDetails newPayrollDetail = new tbl_PayrollDetails()
                {
                    PayrollID = payrollToUse.PayrollID,

                    EmployeeID = pd.EmployeeID,

                    SundayHours = (float)pd.SundayHours,
                    MondayHours = (float)pd.MondayHours,
                    TuesdayHours = (float)pd.TuesdayHours,
                    WednesdayHours = (float)pd.WednesdayHours,
                    ThursdayHours = (float)pd.ThursdayHours,
                    FridayHours = (float)pd.FridayHours,
                    SaturdayHours = (float)pd.SaturdayHours,

                    OTHours = (float)pd.OTHours,
                    OTPay = (float)pd.OTPay,
                    OTRate = (float)pd.OTRate,
                    RegularHours = (float)pd.RegularHours,
                    RegularPay = (float)pd.RegularPay,
                    RegularRate = (float)pd.RegularRate,
                    WeeklySalary = (float)pd.WeeklySalary
                };
                db.tbl_PayrollDetails.AddObject(newPayrollDetail);

                // now add the job_payroll records
                foreach (JobPayroll jobPayroll in pd.JobPayrolls)
                {
                    tbl_Job_Payroll newJobPayroll = new tbl_Job_Payroll()
                    {
                        JobID = jobPayroll.JobID
                        ,ServiceProID = pd.EmployeeID   // TODO: Get rid of this after we add the foreign key to Payroll Detail
                        ,
                        JobSubTotal = (float)jobPayroll.JobSubTotal
                        ,
                        TotalCommissionPartsAndLabor = jobPayroll.TotalCommissionPartsAndLabor
                        ,
                        TotalCommissionSpifs = (float)jobPayroll.TotalCommissionSpifs
                        ,
                        PayrollID = payrollToUse.PayrollID
                    };
                    db.tbl_Job_Payroll.AddObject(newJobPayroll);
                }
            }

            db.SaveChanges();

            //[Step 4] - Communicate results
            payrollResult.PayrollID = payrollToUse.PayrollID;   // Doesn't make perfect sense but the UI needs this
            result.ResultData = payrollResult;
            result.Success = true;
            if (!result.Success)
                result.Message = "Run Payroll Failed";

            return result;
        }

        public OperationResult<DateTime> LockPayrollForWeekOnDate(DateTime dateOfPayroll, DateTime dateToLock, string FranchiseId, bool persistToDataBase, Guid userKey)
        {
            // The date is not getting the end date for the week right now, but we can change that
            while (dateOfPayroll.DayOfWeek != DayOfWeek.Saturday)
                dateOfPayroll = dateOfPayroll.AddDays(1.0);

            var result = new OperationResult<DateTime>();

            int franchiseid = Convert.ToInt32(FranchiseId);
            DateTime startDate = dateOfPayroll.AddDays(-7);
            DateTime holddate = dateOfPayroll.AddDays(1);

            // [Step 1] - Get everything we should need so we don't have to make anymore database calls
            // Get the Payroll so we can lock it and set the completion date
            List<tbl_Payroll> existingPayrollList = (from p in db.tbl_Payroll where p.PayrollDate == dateOfPayroll select p).ToList<tbl_Payroll>();
            if (existingPayrollList.Count == 0)
                throw new Exception(string.Format("Must have existing payroll for week of [{0}] in order to lock payroll", dateOfPayroll.ToShortDateString()));
            if (existingPayrollList.Count > 1)
                throw new Exception(string.Format("Must have only ONE existing payroll for week of [{0}] in order to lock payroll", dateOfPayroll.ToShortDateString()));
            tbl_Payroll existingPayroll = existingPayrollList[0];
            if (existingPayroll.LockDate.HasValue == true)
                throw new Exception("Cannot lock Payroll.  Payroll has already been locked on: " + existingPayroll.LockDate.Value);

            // [Step 2] - Lock the Payroll - There's very little "Business Functionality" here
            DateTime lockDate = dateToLock;

            // [Step 3] - Persistance
            existingPayroll.LockDate = lockDate;
            db.SaveChanges();

            //[Step 4] - Communicate results
            result.ResultData = lockDate;
            result.Success = true;
            if (!result.Success)
                result.Message = "Lock Payroll Failed";

            return result;
        }


        private void ProcessEmployeesForWeek(Payroll payrollToProcess, List<tbl_Employee> employeeList, List<tbl_HR_TimeSheet> timeSheetList, List<Reporting.Job> jobList, List<Reporting.JobTask> taskList, List<Reporting.PayrollSetup> payrollsetuplist)
        {
            int payrollid = payrollToProcess.PayrollID;
            DateTime date = payrollToProcess.PayrollDate;
            int franchiseid = payrollToProcess.FranchiseID;

            // Process the payroll \for the employees
            foreach (var employeeRec in employeeList)
                ProcessPayrollForEmployee(employeeRec, payrollToProcess, timeSheetList, jobList, taskList, payrollsetuplist);

            var TotalGrossPay = (from t in payrollToProcess.PayrollDetails select t.GrossPay).Sum();
        }

        private void ProcessPayrollForEmployee(tbl_Employee employee, Payroll payrollToProcess, List<tbl_HR_TimeSheet> timeSheetList, List<Reporting.Job> jobList, List<Reporting.JobTask> jobTaskList, List<Reporting.PayrollSetup> payrollsetuplist)
        {
            if (payrollsetuplist.Count == 0)
                throw new Exception("Franchise does not have payroll setup");
            if (payrollsetuplist.Count > 1)
                throw new Exception("Franchise has more than one payroll setup.  System does now know which one to use");
            Reporting.PayrollSetup payrollSetup = payrollsetuplist[0];

            int employeeid = employee.EmployeeID;
            int franchiseid = payrollToProcess.FranchiseID;
            DateTime dateEndOfWeek = payrollToProcess.PayrollDate;

            double Sunday = 0;
            double Monday = 0;
            double Tuesday = 0;
            double Wednesday = 0;
            double Thursday = 0;
            double Friday = 0;
            double Saturday = 0;
            GetHours(employee, payrollToProcess, timeSheetList, ref Sunday, ref Monday, ref Tuesday, ref Wednesday, ref Thursday, ref Friday, ref Saturday);

            // For each of his jobs, create a respective JobPayroll Entity
            List<JobPayroll> jobpayrolllist = (
                                              from Reporting.Job job in jobList
                                              where job.TechId == employee.EmployeeID
                                              select new JobPayroll
                                                    (
                                                    job.Id
                                                    ,job.SubTotal
                                                    , job.SubTotal * employee.CommissionRate / 100M
                                                    , CalculateSpiffForJob(
                                                                            job
                                                                            ,(from jobTask in jobTaskList
                                                                            where jobTask.JobId == job.Id
                                                                            select jobTask
                                                                            ).ToList()
                                                                            ,employee
                                                                            , payrollSetup.PayrollSpiffs
                                                                          )
                                                    )
                                                ).ToList<JobPayroll>();



            double TotalHours = 0;
            double OverTimeHours = 0;
            double RegularHours = 0;
            double RegularRate = 0;
            double OTRate = 0;
            double RegularPay = 0;
            double OTPay = 0;
            double Salary = 0;
            double CommissionRateHour = 0;

            // TODO: Values are default.  Have to change this back when PayrollSetup is working
            int PayrollMethod = 1;  
            double OvertimeStarts = 40;
            double OTMultiplier = 1.5;

            // TODO: Change this out to go off of first payroll setup
            foreach (var payrollsetup in payrollsetuplist)
            {
                PayrollMethod = payrollsetup.OvertimeMethodID;
                OvertimeStarts = payrollsetup.OvertimeStarts;
                OTMultiplier = payrollsetup.OTMultiplier;
            }

            RegularRate = employee.HourlyRate;
            OTRate = RegularRate * OTMultiplier;
            Salary = employee.WeeklySalary;
            if (PayrollMethod == 1)
            {
                TotalHours = Sunday + Monday + Tuesday + Wednesday + Thursday + Friday + Saturday;
                if (TotalHours >= OvertimeStarts)
                {
                    RegularHours = OvertimeStarts;
                    OverTimeHours = TotalHours - RegularHours;
                }
                else
                {
                    RegularHours = TotalHours;
                    OverTimeHours = 0;
                }
            }
            else if (PayrollMethod == 2)
            {
                TotalHours = Sunday + Monday + Tuesday + Wednesday + Thursday + Friday + Saturday;
                if (Sunday >= OvertimeStarts)
                {
                    RegularHours += OvertimeStarts;
                    OverTimeHours += Sunday - OvertimeStarts;
                }
                else
                {
                    RegularHours += Sunday;
                }
                if (Monday >= OvertimeStarts)
                {
                    RegularHours += OvertimeStarts;
                    OverTimeHours += Monday - OvertimeStarts;
                }
                else
                {
                    RegularHours += Monday;
                }
                if (Tuesday >= OvertimeStarts)
                {
                    RegularHours += OvertimeStarts;
                    OverTimeHours += Tuesday - OvertimeStarts;
                }
                else
                {
                    RegularHours += Tuesday;
                }
                if (Wednesday >= OvertimeStarts)
                {
                    RegularHours += OvertimeStarts;
                    OverTimeHours += Wednesday - OvertimeStarts;
                }
                else
                {
                    RegularHours += Wednesday;
                }
                if (Thursday >= OvertimeStarts)
                {
                    RegularHours += OvertimeStarts;
                    OverTimeHours += Thursday - OvertimeStarts;
                }
                else
                {
                    RegularHours += Thursday;
                }
                if (Friday >= OvertimeStarts)
                {
                    RegularHours += OvertimeStarts;
                    OverTimeHours += Friday - OvertimeStarts;
                }
                else
                {
                    RegularHours += Friday;
                }
                if (Saturday >= OvertimeStarts)
                {
                    RegularHours += OvertimeStarts;
                    OverTimeHours += Saturday - OvertimeStarts;
                }
                else
                {
                    RegularHours += Saturday;
                }
            }
            RegularPay = Math.Round(RegularHours * RegularRate,2);
            OTPay = Math.Round(OverTimeHours * OTRate, 2);
            Salary = Math.Round(Salary, 2);

            PayrollDetail newPayrollDetail = new PayrollDetail(
                                                employee.EmployeeID, 
                                                null, 
                                                (decimal)Friday,
                                                (decimal)Monday,
                                                (decimal)OverTimeHours,
                                                (decimal)OTPay,
                                                Math.Round((decimal)OTRate,2),
                                                (decimal)RegularHours,
                                                (decimal)RegularPay,
                                                Math.Round((decimal)RegularRate,2),
                                                (decimal)Sunday,
                                                (decimal)Saturday,
                                                (decimal)Thursday,
                                                (decimal)Tuesday,
                                                (decimal)Wednesday,
                                                (decimal)Salary,
                                                (decimal)CommissionRateHour,
                                                jobpayrolllist);
            
            payrollToProcess.PayrollDetails.Add(newPayrollDetail);
        }

        private void GetHours(tbl_Employee employee, Payroll payrollToProcess, List<tbl_HR_TimeSheet> timeSheetList, ref double Sunday, ref double Monday, ref double Tuesday, ref double Wednesday, ref double Thursday, ref double Friday, ref double Saturday)
        {
            int employeeID = employee.EmployeeID;

            DateTime holdCurrentDate = Convert.ToDateTime("1/1/1900").Date;

            DateTime EndDate = payrollToProcess.PayrollDate;
            DateTime StartDate = EndDate.AddDays(-7);
            DateTime holddate = EndDate.AddDays(1);

            // Get the TimeSheet for the week for this employee
            // TODO: If there is NOT one, raise an alert or something...
            List<tbl_HR_TimeSheet> timeSheetForEmployeeList = (from ts in timeSheetList where ts.EmployeeID == employeeID select ts).ToList<tbl_HR_TimeSheet>();
            if (timeSheetForEmployeeList.Count == 0) // if no timesheet for this employee, don't bother
                return;
            tbl_HR_TimeSheet timeSheet = timeSheetForEmployeeList[0];
            Sunday = (double)timeSheet.SundayHours;
            Monday = (double)timeSheet.MondayHours;
            Tuesday = (double)timeSheet.TuesdayHours;
            Wednesday = (double)timeSheet.WednesdayHours;
            Thursday = (double)timeSheet.ThursdayHours;
            Friday = (double)timeSheet.FridayHours;
            Saturday = (double)timeSheet.SaturdayHours;

            // TODO: Remove old code, instead of getting them from timecard list, go straight off of HR_Timecard records
            /*
            var timeClockReclist = (from t in timeClockReclistAll
                                    where t.EmployeeID == employee.EmployeeID
                                    orderby t.DateTimeStatusChanged ascending
                                    select t);
            foreach (var timeclockrec in timeClockReclist)
            {
                var timeClockStatus =
                    (from s in timeClockStatusList where s.TimeClockStatusID == timeclockrec.TimeClockStatusID select s).FirstOrDefault();

                if (holdCurrentDate == Convert.ToDateTime("1/1/1900"))
                {
                    holdCurrentDate = timeclockrec.DateTimeStatusChanged.Value;
                    if (timeClockStatus.ClockedInYN)
                    {
                        holdClockedInTime = timeclockrec.DateTimeStatusChanged.Value;
                    }
                }
                else if (holdCurrentDate < timeclockrec.DateTimeStatusChanged.Value.Date)
                {
                    if (holdCurrentDate == EndDate.AddDays(-6))
                    {
                        Sunday = hours / 60;
                    }
                    else if (holdCurrentDate == EndDate.AddDays(-5))
                    {
                        Monday = hours / 60;
                    }
                    else if (holdCurrentDate == EndDate.AddDays(-4))
                    {
                        Tuesday = hours / 60;
                    }
                    else if (holdCurrentDate == EndDate.AddDays(-3))
                    {
                        Wednesday = hours / 60;
                    }
                    else if (holdCurrentDate == EndDate.AddDays(-2))
                    {
                        Thursday = hours / 60;
                    }
                    else if (holdCurrentDate == EndDate.AddDays(-1))
                    {
                        Friday = hours / 60;
                    }
                    else if (holdCurrentDate == EndDate)
                    {
                        Saturday = hours / 60;
                    }
                    hours = 0.0;
                    holdCurrentDate = timeclockrec.DateTimeStatusChanged.Value;
                    if (timeClockStatus.ClockedInYN)
                    {
                        holdClockedInTime = timeclockrec.DateTimeStatusChanged.Value;
                    }
                }
                else
                {

                    if (timeClockStatus.ClockedInYN)
                    {
                        holdClockedInTime = timeclockrec.DateTimeStatusChanged.Value;
                    }
                    else
                    {
                        DateTime date1 = timeclockrec.DateTimeStatusChanged.Value;
                        TimeSpan tsDiff = holdClockedInTime.Subtract(date1);
                        var obj = new mod_common(this.UserKey);
                        hours += obj.DateDiff(mod_common.DateInterval.Minute, holdClockedInTime, date1);
                        //hours += tsDiff.Minutes;
                        holdClockedInTime = timeclockrec.DateTimeStatusChanged.Value;

                    }
                }

            }

            string strhour = "";
            if (holdCurrentDate.Date == EndDate.AddDays(-6).Date)
            {
                Sunday = hours / 60;
                strhour = "Sunday" + Sunday;

            }
            else if (holdCurrentDate.Date == EndDate.AddDays(-5).Date)
            {
                Monday = hours / 60;
                strhour = "Monday" + Monday;
            }
            else if (holdCurrentDate.Date == EndDate.AddDays(-4).Date)
            {
                Tuesday = hours / 60;
                strhour = "Tuesday" + Tuesday;
            }
            else if (holdCurrentDate.Date == EndDate.AddDays(-3).Date)
            {
                Wednesday = hours / 60;
                strhour = "Wednesday" + Wednesday;
            }
            else if (holdCurrentDate.Date == EndDate.AddDays(-2).Date)
            {
                Thursday = hours / 60;
                strhour = "Thursday" + Thursday;
            }
            else if (holdCurrentDate.Date == EndDate.AddDays(-1).Date)
            {
                Friday = hours / 60;
                strhour = "Friday" + Friday;
            }
            else if (holdCurrentDate.Date == EndDate.Date)
            {
                Saturday = hours / 60;
                strhour = "Saturday" + Saturday;
            }
             return strhour;

            */
        }

        // Spiff for a job is:
        // For each task, see if any Spiff applies
        //  For each Spiff that applies:
        //      Add the applicable Spiff to the TotalCommissioNSpiffsForJob
        private decimal CalculateSpiffForJob(Reporting.Job job, List<JobTask> jobTaskList, tbl_Employee emp, IEnumerable<PayrollSpiff> payrollSpiffList)
        {
            decimal spiffTotal = 0M;
            foreach(JobTask jobTask in jobTaskList)
            {
                var spiffForJobTaskList = (from spf in payrollSpiffList
                                  where 
                                    spf.JobCode == jobTask.JobCode          // Match jobe code on task
                                    && spf.ServiceProID == emp.EmployeeID   // Spiff is for THIS Emp.
                                    && spf.DateExpires >= job.Completed.Value   // Spiff expiration is within completed date
                                    && spf.Active == true                       // must be active to be applicable
                                        select spf).ToList();
                foreach(PayrollSpiff spiffForJobTask in spiffForJobTaskList)
                {
                    if (spiffForJobTask.PayTypeID == 0)         // Flat
                        spiffTotal += spiffForJobTask.Rate;
                    else if (spiffForJobTask.PayTypeID == 1)    // Commission
                        spiffTotal += spiffForJobTask.Rate * job.SubTotal / 100M;
                    else
                        throw new Exception(
                            string.Format("Cannot calculate Payroll Spiff.  Unrecognized Pay Type: {0}.  Spiff Description: {1}"
                                            ,spiffForJobTask.PayTypeID ,spiffForJobTask.Comments)
                                            );
                }
            }

            return spiffTotal;
        }

        /*
        public OperationResult<bool> SendToCustomer(int id, string to)
        {
            var result = new OperationResult<bool>();
            var invoice = TransformJobToInvoice(id);

            if (invoice == null)
            {
                result.Success = false;
                result.ResultData = false;
                result.Message = string.Format("Job '{0}' not found.", id);
                return result;
            }

            var client = GetInvoiceGen();
            var genResultPdf = GetInvoiceInternal(id);
            var genResultHtml = client.RenderHtml(invoice);
            var docType = invoice.IsEstimate ? "Estimate" : "Invoice";

            if (!genResultPdf.Success || !genResultHtml.Success)
            {
                result.Success = false;
                result.ResultData = false;
                result.Message = string.Format("Could not send {0} to customer: {1}", docType, (genResultPdf.Message ?? genResultHtml.ExceptionMessage));
                return result;
            }

            var html = Encoding.Unicode.GetString(genResultHtml.Data);
            var engine = new EmailEngine();
            var subject = string.Format("{0} - {1}", docType, id);
            var from = invoice.FranchiseTypeID == 6
                ? GlobalConfiguration.PlumberInvoiceFromAddress
                : GlobalConfiguration.ConnectUsProInvoiceFromAddress;


            //Do not send invoices to customers when running in test mode.
            if (GlobalConfiguration.TestMode) to = string.Empty;

            bool sent;
            using (var data = new MemoryStream(genResultPdf.ResultData.Value))
            {
                sent = engine.Send(from, to, null, GlobalConfiguration.InvoiceSendBcc, subject, html,
                                          new[] { new Attachment(data, genResultPdf.ResultData.Key) },
                                          true);
            }

            result.Success = sent;

            if (!result.Success)
                result.Message = string.Format("The {0} failed to send.", docType);

            return result;
        }
        */
    }
}
