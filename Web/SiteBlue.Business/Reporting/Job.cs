using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class Job
    {
        public int Id { get; private set; }
        public int TechId { get; private set; }
        public string Tech { get; set; }
        public int JobPriorityId { get; private set; }
        public DateTime? Completed { get; private set; }
        public DateTime? EstimateDate { get; private set; }
        public int StatusId { get; private set; }
        public string Status { get; private set; }
        public string Address { get; private set; }
        public int CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public int ServiceId { get; private set; }
        public string Service { get; private set; }

        public decimal SubTotal { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal TotalSales { get; private set; }
        public decimal Balance { get; private set; }

        public bool IsEstimate
        {
            get
            {
                return EstimateDate.HasValue; // AND no payments
            }
        }

        public bool IsOutstandingEstimate
        {
            get { return IsEstimate && StatusId == 13; }
        }

        public bool IsRecall
        {
            get { return (JobPriorityId == 4 /*OR recall referral*/) && SubTotal == 0; }
        }

        public bool IsComplete
        {
            get { return IsCompleteInternal && !IsRecall; }
        }

        private bool IsCompleteInternal
        {
            get { return (StatusId == 6 || StatusId == 7); } 
        }

        protected void CopyFrom(vRpt_Job job)
        {
            Id = job.TicketNumber;
            TotalSales = job.TotalSales;
            SubTotal = job.SubTotal;
            TaxAmount = job.TaxAmount;
            JobPriorityId = job.JobPriorityID;
            TechId = job.ServiceProID;
            Balance = job.Balance;
            StatusId = job.StatusID;
            Status = job.Status;
            Completed = job.CallCompleted;
            EstimateDate = job.EstimateDate;
            Address = job.JobAddress;
            CustomerId = job.CustomerID;
            CustomerName = job.CustomerName;
            ServiceId = job.ServiceID.GetValueOrDefault();
            Service = job.ServiceName;
        }

        internal static Job MapFromModel(vRpt_Job job)
        {
            var j = new Job();
            j.CopyFrom(job);
            return j;
        }
    }
}
