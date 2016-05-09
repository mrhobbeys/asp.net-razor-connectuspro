using System;
using System.Collections.Generic;
using System.Linq;
using SiteBlue.Data.EightHundred;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class KeyPerformanceIndicator
    {
        public Job[] InconsistentRecords { get; set; }
        private readonly Func<Job, decimal> _salesSum = j => j.SubTotal;
        private readonly Func<JobTask, decimal> _taskSum = t => t.Amount;

        public int FranchiseId { get; private set; }

        public decimal ActualSales { get { return CompletedJobs.Sum(_salesSum); } }
        public decimal EstimateSales { get { return Estimates.Sum(_salesSum); } }
        public decimal OutstandingEstimateSales { get { return OutstandingEstimates.Sum(_salesSum); } }
        public decimal AddOnSales { get { return AddOnTasks.Sum(_taskSum); } }
        public decimal AverageTicket { get { return DispatchCount == 0 ? 0 : ActualSales / DispatchCount; } }
        public int Discounts { get { return DiscountJobs.Count(); } }
        public decimal DiscountSales { get { return DiscountTasks.Sum(_taskSum); } }
        public int CompletedCount { get { return CompletedJobs.Count(); } }
        public int EstimateCount { get { return Estimates.Count(); } }
        public int OutstandingEstimateCount { get { return OutstandingEstimates.Count(); } }
        public int RecallCount { get { return RecallJobs.Count(); } }
        public int DispatchCount { get { return CompletedCount + EstimateCount + RecallCount; } }

        public decimal CloseRate
        {
            get { return DispatchCount == 0 ? 0 : ((decimal)CompletedCount / DispatchCount * 100); }
        }

        public int AddOns { get { return AddOnTasks.Select(t => t.JobId).Distinct().Count(); } }
        public int HomeGuard { get { return HomeGuardTasks.Select(t => t.JobId).Distinct().Count(); } }
        public int Bio { get { return BioTasks.Count(); } }

        private Job[] Jobs { get; set; }
        private JobTask[] Tasks { get; set; }
        private IEnumerable<JobTask> DiscountTasks { get { return Tasks.Where(t => t.IsDiscount && CompletedJobs.Any(dj => dj.Id == t.JobId)); } }
        private IEnumerable<JobTask> HomeGuardTasks { get { return Tasks.Where(t => t.IsMemberPlan && CompletedJobs.Any(j => j.Id == t.JobId)); } }
        private IEnumerable<JobTask> BioTasks { get { return Tasks.Where(t => t.IsBio); } }
        private IEnumerable<JobTask> AddOnTasks { get { return Tasks.Where(t => t.IsAddOn && Jobs.Any(j => j.Id == t.JobId)); } }

        public IEnumerable<Job> AllJobs { get { return RecallJobs.Concat(Estimates).Concat(CompletedJobs); } }
        public IEnumerable<Job> CompletedEstimates { get { return Estimates.Where(j => j.IsComplete); } }
        public IEnumerable<Job> OutstandingEstimates { get { return Estimates.Where(j => j.IsOutstandingEstimate); } }
        public IEnumerable<Job> RecallJobs { get { return Jobs.Where(j => j.IsRecall); } }
        public IEnumerable<Job> Estimates { get { return Jobs.Where(j => j.IsEstimate ); } }
        public IEnumerable<Job> CompletedJobs { get { return Jobs.Where(j => j.IsComplete); } }
        public IEnumerable<Job> AddOnJobs { get { return Jobs.Where(j => AddOnTasks.Any(t => t.JobId == j.Id) && j.IsComplete); } }
        public IEnumerable<Job> DiscountJobs { get { return Jobs.Where(j => DiscountTasks.Any(t => t.JobId == j.Id) && j.IsComplete); } }
        public IEnumerable<JobTask> AllTasks { get { return Tasks; } }
        
        public decimal RecoverRate
        {
            get
            {
                var completedEstimates = (decimal)CompletedEstimates.Count();
                var estimates = (decimal)Estimates.Count();
                return estimates == 0 ? 0 : completedEstimates / estimates * 100;
            }
        }

        internal KeyPerformanceIndicator(int franchiseId, vRpt_Job[] jobs, vRpt_JobDetail[] tasks)
        {
            FranchiseId = franchiseId;
            Jobs = jobs.Select(Job.MapFromModel).ToArray();
            Tasks = tasks.Select(JobTask.MapFromModel).ToArray();

            InconsistentRecords = Jobs.Where(j => !Estimates.Any(e => e.Id == j.Id) && !CompletedJobs.Any(c => c.Id == j.Id) && !RecallJobs.Any(r => r.Id == j.Id)).ToArray();
        }

        private KeyPerformanceIndicator(int franchiseId, Job[] jobs, JobTask[] tasks)
        {
            FranchiseId = franchiseId;
            Jobs = jobs;
            Tasks = tasks;

            InconsistentRecords = Jobs.Where(j => !Estimates.Any(e => e.Id == j.Id) && !CompletedJobs.Any(c => c.Id == j.Id) && !RecallJobs.Any(r => r.Id == j.Id)).ToArray();
        }

        public KeyPerformanceIndicator GetByTechnician(int techId)
        {
            var jobs = Jobs.Where(j => j.TechId == techId).ToArray();
            var tasks = Tasks.Where(t => jobs.Any(j => j.Id == t.JobId)).ToArray();
            return new KeyPerformanceIndicator(FranchiseId, jobs, tasks);
        }

        public KeyPerformanceIndicator GetByDay(DateTime date)
        {
            var f = date.Date;
            var e = f.AddDays(1);

            var jobs = Jobs.Where(j => (j.Completed >= f && j.Completed< e) || (j.EstimateDate >= f && j.EstimateDate < e)).ToArray();
            var tasks = Tasks.Where(t => jobs.Any(j => j.Id == t.JobId)).ToArray();
            return new KeyPerformanceIndicator(FranchiseId, jobs, tasks);
        }
        
        public decimal GetDiscountSales(int jobId)
        {
            return DiscountTasks.Where(t => t.JobId == jobId).Sum(_taskSum);
        }

        public decimal GetAddOnSales(int jobId)
        {
            return AddOnTasks.Where(t => t.JobId == jobId).Sum(_taskSum);
        }

    }
}
