using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using SiteBlue.Data.EightHundred;
using SiteBlue.Data.Reporting;
using System.Data;
using SiteBlue.Data;

namespace SiteBlue.Business.Reporting
{
    public class ReportingService : AbstractBusinessService
    {
        public const int DaysPerYear = 260;
        public const int MonthsPerYear = 12;

        public Budget GetBudget(int franchiseId, DateTime asOfDate)
        {
            var month = GetKeyPerformanceIndicators(franchiseId, new DateTime(asOfDate.Year, asOfDate.Month, 1),
                                        asOfDate.Date.AddDays(1));
            var today = month.GetByDay(asOfDate);

            var m = new Budget(franchiseId, asOfDate.Date)
                        {
                            DailySales = {Actual = today.ActualSales},
                            MonthlySales = {Actual = month.ActualSales},
                            DailyJobs = {Actual = today.CompletedCount},
                            MonthlyJobs = {Actual = month.CompletedCount},
                            DailyCloseRate = {Actual = today.CloseRate},
                            MonthlyCloseRate = {Actual = month.CloseRate},
                            DailyBio = {Actual = today.Bio},
                            MonthlyBio = {Actual = month.Bio},
                            DailyHomeGuard = {Actual = today.HomeGuard},
                            MonthlyHomeGuard = {Actual = month.HomeGuard},
                            DailyRecalls = {Actual = today.RecallCount},
                            MonthlyRecalls = {Actual = month.RecallCount},
                            DailyAverageTicket = {Actual = today.AverageTicket},
                            MonthlyAverageTicket = {Actual = month.AverageTicket}
                        };

            tbl_DailyBudget dailybudget;

            using (var txnCtx = new EightHundredEntities(UserKey))
            {
                dailybudget = txnCtx.tbl_DailyBudget.SingleOrDefault(b => b.FranchiseID == franchiseId);
            }

            if (dailybudget != null)
            {
                m.DailySales.Budget = dailybudget.DailySales.GetValueOrDefault();
                m.MonthlySales.Budget = dailybudget.MonthlySales.GetValueOrDefault();

                m.DailyCloseRate.Budget = dailybudget.AnnualClosingRate.GetValueOrDefault();
                m.MonthlyCloseRate.Budget = m.DailyCloseRate.Budget;

                m.DailyJobs.Budget = (decimal)dailybudget.AnnualJobs.GetValueOrDefault() / DaysPerYear;
                m.MonthlyJobs.Budget = (decimal)dailybudget.AnnualJobs.GetValueOrDefault() / MonthsPerYear;

                m.DailyAverageTicket.Budget = dailybudget.AnnualAvgTicket.GetValueOrDefault();
                m.MonthlyAverageTicket.Budget = dailybudget.AnnualAvgTicket.GetValueOrDefault();

                m.DailyHomeGuard.Budget = dailybudget.DailyHomeGuard.GetValueOrDefault();
                m.MonthlyHomeGuard.Budget = dailybudget.MonthlyHomeGuard.GetValueOrDefault();

                m.DailyBio.Budget = dailybudget.DailyBio.GetValueOrDefault();
                m.MonthlyBio.Budget = dailybudget.MonthlyBio.GetValueOrDefault();

                m.DailyRecalls.Budget = dailybudget.AnnualRecallPercentOfJobs.GetValueOrDefault();
                m.MonthlyRecalls.Budget = dailybudget.AnnualRecallPercentOfJobs.GetValueOrDefault();

                m.DailyPayroll.Budget = dailybudget.DailyPayroll.GetValueOrDefault();
                m.MonthlyPayroll.Budget = dailybudget.MonthlyPayroll.GetValueOrDefault();
            }

            return m;
        }

        public KeyPerformanceIndicator GetKeyPerformanceIndicators(int franchiseId, DateTime fromDateInclusive, DateTime toDateExclusive)
        {
            var hash = GetJobData(franchiseId, null, fromDateInclusive, toDateExclusive, null, new int[] {15});
            var jobs = hash.Select(p => p.Key).ToArray();

            var tasks = hash.SelectMany(p => p.Value).ToArray();
            var kpi = new KeyPerformanceIndicator(franchiseId, jobs, tasks);

            using (var ctx = new EightHundredEntities(UserKey))
            {
                var techs = ctx.tbl_Employee.Select(t => new { t.EmployeeID, t.Employee }).ToArray();
                foreach (var j in kpi.AllJobs)
                {
                    var techId = j.TechId;
                    var tech = techs.Single(t => t.EmployeeID == techId);

                    j.Tech = tech != null ? tech.Employee : string.Empty;
                }
            }

            return kpi;
        }

        #region Job Details

        public JobOverview[] GetJobs(int? franchiseId, DateTime fromDateInclusive, DateTime toDateExclusive)
        {
            var joboverviews = GetJobData(franchiseId, null, fromDateInclusive, toDateExclusive, null, null).Select(
            p => JobOverview.MapFromJob(p.Key)).ToArray();

            using (var ctx = new EightHundredEntities(UserKey))
            {
                var techs = ctx.tbl_Employee.Select(t => new { t.EmployeeID, t.Employee }).ToArray();
                foreach (var j in joboverviews)
                {
                
                    var techId = j.TechId;
                    var tech = techs.FirstOrDefault(t => t.EmployeeID == techId);
                    j.Tech = tech == null ? string.Empty : tech.Employee;
                }
            }

            return joboverviews;

            //return GetJobData(franchiseId, null, fromDateInclusive, toDateExclusive, null).SelectMany(
            // p => JobOverview.MapFromJobAndTasks(p.Key, p.Value)).ToArray();
        }

        private static Dictionary<vRpt_Job, vRpt_JobDetail[]> GetJobData(int? franchiseId, int? techId, DateTime fromDateInclusive, DateTime toDateExclusive, int[] statuses, int[] excludeStatuses)
        {
            DateTime dtFrom = fromDateInclusive.Date;
            DateTime dtTo = toDateExclusive;
            var noStatuses = statuses == null || statuses.Length == 0;
            var noExcludeStatuses = excludeStatuses == null || excludeStatuses.Length == 0;

            if (noStatuses)
                statuses = new[] { int.MinValue };

            if (noExcludeStatuses)
                excludeStatuses = new[] { int.MinValue };

            using (var ctx = new ReportingEntities())
            {
                  var qry = from ji in ctx.vRpt_Jobs
                          where ((ji.CallCompleted >= dtFrom && ji.CallCompleted < dtTo) ||
                                 (ji.EstimateDate >= dtFrom && ji.EstimateDate < dtTo)
                                ) &&
                              ji.ClientID == (franchiseId ?? ji.ClientID) &&
                              (noStatuses || statuses.Contains(ji.StatusID)) &&
                              (noExcludeStatuses || !excludeStatuses.Contains(ji.StatusID))
                          let tasks = from t in ctx.vRpt_JobDetails where t.JobID == ji.TicketNumber select t
                          select new { Job = ji, Tasks = tasks };

                return qry.ToDictionary(j => j.Job, j => j.Tasks.ToArray());
            }
        }

        #endregion

        #region Customer Report

        public Customer[] GetCustomers(int? franchiseId, DateTime? lastVisitFromDateInclusive, DateTime? lastVisitToDateExclusive)
        {
            return GetCustomerData(franchiseId, lastVisitFromDateInclusive, lastVisitToDateExclusive).Select(Customer.MapFromModel).ToArray();
        }

        private static IEnumerable<vRpt_Customer> GetCustomerData(int? franchiseId, DateTime? lastVisitFromDateInclusive, DateTime? lastVisitToDateExclusive)
        {
            DateTime dtFrom = lastVisitFromDateInclusive.Value;
            DateTime dtTo = lastVisitToDateExclusive.Value.Date.AddDays(1);
            using (var ctx = new ReportingEntities())
            {
                //ctx.CommandTimeout = 30; // defaulted to SQL Server Remote Query TimeOut. Make sense only for this instance.
                IQueryable<vRpt_Customer> qry = ctx.vRpt_Customer;

                if (franchiseId.HasValue)
                    qry = qry.Where(c => c.ClientID == franchiseId);

                if (lastVisitFromDateInclusive.HasValue)
                    qry = qry.Where(c => c.LastVisit >= dtFrom);

                if (lastVisitToDateExclusive.HasValue)
                    qry = qry.Where(c => c.LastVisit < dtTo);

                return qry.ToArray();
            }
        }

        #endregion

        #region Customer Ledger

        public CustomerLedger[] GetCustomerLedger(int? franchiseId, DateTime fromDateInclusive, DateTime toDateExclusive)
        {
            var customerledger = GetCustomerLedgerData(franchiseId, fromDateInclusive, toDateExclusive).Select(
                                        p => CustomerLedger.MapFromCustomer(p.Key, p.Value)).ToArray();



            return customerledger;
        }

        private static Dictionary<vRpt_CustomerLedger, vRpt_Customer> GetCustomerLedgerData(int? franchiseId, DateTime? fromDateInclusive, DateTime? toDateExclusive)
        {
            DateTime dtFrom = fromDateInclusive.Value;
            DateTime dtTo = toDateExclusive.Value.Date.AddDays(1);
            using (var ctx = new ReportingEntities())
            {                         
                var qry = from l in ctx.vRpt_CustomerLedger
                          where
                              l.ClientId == (franchiseId ?? l.ClientId)
                              && l.RecordedDate >= dtFrom
                              && l.RecordedDate < dtTo
                          let cust = (from c in ctx.vRpt_Customer
                                      where c.CustomerID == l.CustomerId
                                      select c).FirstOrDefault()
                          select new { Ledger = l, Customer = cust };

                return qry.ToDictionary(c => c.Ledger, c => c.Customer);
            }
        }


        #endregion

        #region Job Task Part Details

        public JobTaskPartUsage[] GetJobTaskPart(int? franchiseId, DateTime? callCompletedFromDateInclusive, DateTime? callCompletedToDateExclusive)
        {
            return GetJobTaskPartData(franchiseId, callCompletedFromDateInclusive, callCompletedToDateExclusive).Select(JobTaskPartUsage.MapFromModel).ToArray();
        }

        private static IEnumerable<vRpt_PartUsagePerJob> GetJobTaskPartData(int? franchiseId, DateTime? callCompletedFromDateInclusive, DateTime? callCompletedToDateExclusive)
        {

            DateTime dtFrom = callCompletedFromDateInclusive.Value;
            DateTime dtTo = callCompletedToDateExclusive.Value.Date.AddDays(1);
            using (var ctx = new ReportingEntities())
            {
                //ctx.CommandTimeout = 30; // defaulted to SQL Server Remote Query TimeOut. Make sense only for this instance.
                IQueryable<vRpt_PartUsagePerJob> qry = ctx.vRpt_PartUsagePerJob;

                if (franchiseId.HasValue)
                    qry = qry.Where(c => c.ClientID == franchiseId);

                if (callCompletedFromDateInclusive.HasValue)
                    qry = qry.Where(c => c.CallCompleted >= dtFrom);

                if (callCompletedToDateExclusive.HasValue)
                    qry = qry.Where(c => c.CallCompleted < dtTo);

                return qry.ToArray();
            }
        }

        #endregion

        #region Account Summary

        public Account[] GetAccountSummary(int? franchiseId, DateTime? lastVisitFromDateInclusive, DateTime? lastVisitToDateExclusive)
        {
            return GetAccountSummaryData(franchiseId, lastVisitFromDateInclusive, lastVisitToDateExclusive).Select(Account.MapFromModel).ToArray();
        }

        private static IEnumerable<vRpt_AccountingSummary> GetAccountSummaryData(int? franchiseId, DateTime? lastVisitFromDateInclusive, DateTime? lastVisitToDateExclusive)
        {

            DateTime dtFrom = lastVisitFromDateInclusive.Value;
            DateTime dtTo = lastVisitToDateExclusive.Value.Date.AddDays(1);
            using (var ctx = new ReportingEntities())
            {
                IQueryable<vRpt_AccountingSummary> qry = ctx.vRpt_AccountingSummary;

                if (franchiseId.HasValue)
                    qry = qry.Where(c => c.ClientID == franchiseId);

                if (lastVisitFromDateInclusive.HasValue)
                    qry = qry.Where(c => c.WSRCompletedDate >= dtFrom);

                if (lastVisitToDateExclusive.HasValue)
                    qry = qry.Where(c => c.WSRCompletedDate < dtTo);

                return qry.ToArray();
            }
        }

        #endregion

        #region Memberships

        public Memberships[] GetMemberships(int? franchiseId, DateTime? lastVisitFromDateInclusive, DateTime? lastVisitToDateExclusive)
        {
            return GetMembershipsData(franchiseId, lastVisitFromDateInclusive, lastVisitToDateExclusive).Select(Memberships.MapFromModel).ToArray();
        }

        private static IEnumerable<vRPT_MembershipInfo> GetMembershipsData(int? franchiseId, DateTime? lastVisitFromDateInclusive, DateTime? lastVisitToDateExclusive)
        {
            DateTime dtFrom = lastVisitFromDateInclusive.Value;
            DateTime dtTo = lastVisitToDateExclusive.Value.Date.AddDays(1);
            using (var ctx = new ReportingEntities())
            {
                IQueryable<vRPT_MembershipInfo> qry = ctx.vRPT_MembershipInfo;

                if (franchiseId.HasValue)
                    qry = qry.Where(c => c.ClientID == franchiseId);

                if (lastVisitFromDateInclusive.HasValue)
                    qry = qry.Where(c => c.MembershipStartDate >= dtFrom);

                if (lastVisitToDateExclusive.HasValue)
                    qry = qry.Where(c => c.MembershipStartDate < dtTo);

                return qry.ToArray();
            }
        }

 #endregion

        #region Accounting Detail

        public AccountDetail[] GetAccountingDetail(int? frid, DateTime? from, DateTime? to)
        {
            return GetAccountingDetailData(frid, from, to).Select(AccountDetail.MapFromModel).ToArray();
        }

        private static IEnumerable<vRpt_AccountingDetail> GetAccountingDetailData(int? frid, DateTime? from, DateTime? to)
        {
            DateTime dtFrom = from.Value;
            DateTime dtTo = to.Value.Date.AddDays(1);

            using (var ctx = new ReportingEntities())
            {
              
                IQueryable<vRpt_AccountingDetail> qry = ctx.vRpt_AccountingDetail;

                if (frid.HasValue)
                    qry = qry.Where(c => c.ClientID == frid);

                if (from.HasValue)
                    qry = qry.Where(c => c.WSRCompletedDate >= dtFrom);

                if (to.HasValue)
                    qry = qry.Where(c => c.WSRCompletedDate < dtTo);

                return qry.ToArray();
            }
        }

    

        #endregion

        #region "WSR Report"

        public WSR[] GetWSRReport(int? franchiseID, DateTime? fromDateInclusive, DateTime? toDateExclusive)
        {
            return GetWSRData(franchiseID, fromDateInclusive, toDateExclusive).Select(WSR.MapFromModel).ToArray();
        }

        private static IEnumerable<vRpt_WSR> GetWSRData(int? franchiseID, DateTime? fromDateInclusive, DateTime? toDateExclusive)
        {
            using (var context = new ReportingEntities())
            {
              var query = context.vRpt_WSR.AsQueryable();

                if (franchiseID.HasValue)
                    query = query.Where(q => q.ClientID == franchiseID);

                if (fromDateInclusive.HasValue)
                    query = query.Where(q => q.WSRCompletedDate  >= fromDateInclusive);

                if (toDateExclusive.HasValue)
                    query = query.Where(q => q.WSRCompletedDate  <= toDateExclusive);
               
          
                return query.ToArray();
            }
        }




        public DateTime[] GetWSRDates(int? franchiseId, DateTime? from, DateTime? to)
        {
            DateTime dtFrom = from.Value;
            DateTime dtTo = to.Value.Date.AddHours(23);
            using (var context = new ReportingEntities())
            {
                var query = context.vRpt_WSR_Dates.Where(q => q.ClientID == franchiseId && q.WSRCompletedDate != null && q.WSRCompletedDate >= dtFrom && q.WSRCompletedDate <= dtTo).Distinct();

                return query.OrderBy(q => q.WSRCompletedDate).Select(q => q.WSRCompletedDate.Value).ToArray();
            }
        }

        public tbl_ACH_Franchisees_Summary GetWSRFee(int frId, DateTime startdate, DateTime enddate)
        {
            using (var context = new EightHundredEntities())
           { 
                return context.tbl_ACH_Franchisees_Summary.SingleOrDefault(q => q.FranchiseID == frId
                    && q.WeekEnding >= startdate && q.WeekEnding <= enddate);
            }
        }

        #endregion


        public CallStates[] GetCallStateReport(DateTime? fromDateInclusive, DateTime? toDateExclusive)
        {
            return GetCallStateData(fromDateInclusive, toDateExclusive).Select(CallStates.MapFromModel).ToArray();
        }

        private static IEnumerable<VADM_CallStats> GetCallStateData(DateTime? from, DateTime? to)
        {
            using (var ctx = new IncomingCallsQAEntities())
            {
                IQueryable<VADM_CallStats> qry = ctx.VADM_CallStats;
                if (from.HasValue)
                    qry = qry.Where(c => c.calltime >= from);

                if (to.HasValue)
                    qry = qry.Where(c => c.calltime < to);

                return qry.ToArray();
            }
        }

        public PayrollSetup[] GetPayrollSetupData(int? franchiseId)
        {
            PayrollSetup[] toReturn = new PayrollSetup[0];
            using (var ctx = new ReportingEntities())
            {
                // Query Payroll Setup Reporting Facade and inflate reporting entities
                IQueryable<vRpt_PayrollSetup> qry = ctx.vRpt_PayrollSetup;
                if (franchiseId.HasValue)
                    qry = qry.Where(payrollSetup => payrollSetup.FranchiseID == franchiseId);
                var leftJoin = from vRpt_PayrollSetup ps in qry
                               join vRpt_PayrollSpiff psp in ctx.vRpt_PayrollSpiff
                                on ps.PayrollSetupID equals psp.PayrollSetupID into JOINPayrollSetupPayrollSpiff
                               from psp in JOINPayrollSetupPayrollSpiff.DefaultIfEmpty()
                               select new
                               {
                                   xPayrollSetup = ps,
                                   xPayrollSpiff = psp != null ? psp : null
                               };
                var y = leftJoin.GroupBy((xPair) => xPair.xPayrollSetup, xPair => xPair)
                        .Select(xGroup => new PayrollSetup()
                        {
                            FranchiseID = xGroup.Key.FranchiseID,
                            PayrollSetupID = xGroup.Key.PayrollSetupID,
                            OvertimeMethod = xGroup.Key.OvertimeMethod,
                            OvertimeMethodID = xGroup.Key.OvertimeMethodID,
                            OvertimeStarts = xGroup.Key.OvertimeStarts,
                            OTMultiplier = xGroup.Key.OTMultiplier,
                            PayrollSpiffs = xGroup.Where(xPair => xPair.xPayrollSpiff != null)
                                                    .Select(xPair =>
                                                               new PayrollSpiff()
                                                               {
                                                                   PayrollSpiffID = xPair.xPayrollSpiff.PayrollSpiffID,
                                                                   PayrollSetupID = xGroup.Key.PayrollSetupID,
                                                                   Active = xPair.xPayrollSpiff.ActiveYN,
                                                                   AddOn = xPair.xPayrollSpiff.AddonYN,
                                                                   Comments = xPair.xPayrollSpiff.Comments,
                                                                   DateExpires = xPair.xPayrollSpiff.DateExpires,
                                                                   JobCode = xPair.xPayrollSpiff.JobCode,
                                                                   JobCodeDescription = xPair.xPayrollSpiff.JobCodeDescription,
                                                                   JobCodeID = xPair.xPayrollSpiff.JobCodeID,
                                                                   PayType = xPair.xPayrollSpiff.PayType,
                                                                   PayTypeID = xPair.xPayrollSpiff.PayTypeID,
                                                                   Rate = xPair.xPayrollSpiff.Rate,
                                                                   ServiceProID = xPair.xPayrollSpiff.ServiceProID,
                                                                   Employee = xPair.xPayrollSpiff.Employee
                                                               }
                                                         )
                        }
                                );
                toReturn = y.ToArray<PayrollSetup>();

            }
            return toReturn;
        }

        public OvertimeMethod[] GetOvertimeMethods()
        {
            using (var ctx = new ReportingEntities())
            {
                var qry = (from vRpt_OvertimeMethod overtimeMethod in ctx.vRpt_OvertimeMethod
                           select new OvertimeMethod()
                           {
                               OvertimeMethodID = overtimeMethod.OvertimeMethodID,
                               OvertimeMethodName = overtimeMethod.OvertimeMethod
                           });
                return qry.ToArray<OvertimeMethod>(); ;
            }
        }

        public JobCode[] GetJobCodes(int? franchiseId)
        {
            using (var ctx = new ReportingEntities())
            {
                IQueryable<vRpt_JobCode> qry = ctx.vRpt_JobCode;
                if (franchiseId.HasValue)
                    qry = from vRpt_JobCode jobCode in qry
                          where jobCode.FranchiseID == franchiseId.Value
                          select jobCode;

                

             

                var toReturn = (from vRpt_JobCode jobCode in qry
                                select new JobCode()
                                {
                                    ActiveYN = jobCode.ActiveYN,
                                    FranchiseID = jobCode.FranchiseID,
                                    JobCodeDescription = jobCode.JobCodeDescription,
                                    JobCodeID = jobCode.JobCodeID,
                                    JobCodeName = jobCode.JobCode,
                                    PriceBookActiveYN = jobCode.PriceBookActiveYN,
                                    PriceBookID = jobCode.PriceBookID,
                                    PriceBookName = jobCode.PriceBookName
                                });
                return toReturn.ToArray<JobCode>(); ;
            }
        }

        public Employee[] GetEmployees(int? franchiseId)
        {
            using (var ctx = new ReportingEntities())
            {
                IQueryable<vRpt_Employee> qry = ctx.vRpt_Employee;
                if (franchiseId.HasValue)
                    qry = from vRpt_Employee employee in qry
                          where employee.FranchiseID == franchiseId.Value
                          select employee;

                var toReturn = (from vRpt_Employee employee in qry
                                select new Employee()
                                {
                                    ActiveYN = employee.ActiveYN,
                                    CommissionRate = employee.CommissionRate,
                                    EmployeeID = employee.EmployeeID,
                                    EmployeeName = employee.Employee,
                                    FranchiseID = employee.FranchiseID,
                                    FranchiseLegalName = employee.FranchiseLegalName,
                                    FranchiseNUmber = employee.FranchiseNUmber,
                                    ServiceProYN = employee.ServiceProYN
                                });
                return toReturn.ToArray<Employee>(); ;
            }
        }

        public SpiffPayType[] GetSpiffPayTypes()
        {
            using (var ctx = new ReportingEntities())
            {
                var qry = (from vRpt_SpiffPayType payType in ctx.vRpt_SpiffPayType
                           select new SpiffPayType()
                           {
                               SpiffPayTypeID = payType.SpiffPayTypeID,
                               SpiffPayTypeName = payType.SpiffPayType
                           });
                return qry.ToArray<SpiffPayType>(); ;
            }
        }

        #region[GoLiveCheck]
        public IEnumerable<GoLiveCheck_Result> getOnlineCheck(int FranchiseId)
        {
            using (var context = new EightHundredEntities())
            {
                var result = context.GoLiveCheck(FranchiseId).AsQueryable();
                return result.ToArray();           
            }
        }

        public CustomerLedgerSummary[] getCustomerLedgerInfo(int franchiseId)
        {
            return getCustomerSummaryData(franchiseId).Select(p => CustomerLedgerSummary.MapFromModel(p.Key, p.Value)).ToArray();
        }


        public static Dictionary<vRpt_CustomerLedgerSummary, string> getCustomerSummaryData(int FranchiseId)
        {
            using (var ctx = new ReportingEntities())
            {
                IQueryable<vRpt_CustomerLedgerSummary> qry = ctx.vRpt_CustomerLedgerSummary;
                
                qry = qry.Where(c => c.ClientId == FranchiseId);

                //var jqry = from l in qry.AsEnumerable()
                //          join c in ctx.vRpt_Customer on l.CustomerId equals c.CustomerID
                //          where l.ClientId == FranchiseId
                //          select new { CustomerLedgerSummary = l, c.CustomerName };

                var jqry = from l in qry.AsEnumerable()
                           where l.ClientId == FranchiseId
                           let customerName = (from c in ctx.vRpt_Customer where c.CustomerID == l.CustomerId select c.CustomerName).FirstOrDefault()
                           select new { CustomerLedgerSummary = l, customerName };

                return jqry.ToDictionary(l => l.CustomerLedgerSummary, l => l.customerName);
            }
        }


        public Customer[] GetCustomersInfo(int CustomerID)
        {
            return getCustomerDetails(CustomerID).Select(Customer.MapFromModel).ToArray();
        }

        public static IEnumerable<vRpt_Customer> getCustomerDetails(int CustomerId)
        {
            using (var ctx = new ReportingEntities())
            {
                IQueryable<vRpt_Customer> qry = ctx.vRpt_Customer;
                qry = qry.Where(c => c.CustomerID==CustomerId);
            
                return qry.ToArray();
            }
        }
        #endregion

    }
}
