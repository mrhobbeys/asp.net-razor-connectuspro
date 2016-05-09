using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class JobOverview : Job
    {
        //public int LocationId { get; private set; }
        //public int TaxAuthorityId { get; private set; }
        //public string TaxAuthority { get; private set; }

        public string City { get; private set; }
        public string State { get; private set; }
        public string PostalCode { get; private set; }

        public int BusinessTypeId { get; private set; }
        public string BusinessType { get; private set; }
        
        public string JobPriority { get; private set; }
        public string TaxDescription { get; private set; }

        public int FranchiseID { get; private set; }

        public int ReferralID { get; private set; }
        public string ReferralType { get; private set; }

        //public string AccountCode { get; private set; }
        //public int TaskId { get; private set; }
        //public string JobCode { get; private set; }
        //public string JobCodeDescription { get; private set; }
        //public decimal TaskQuantity { get; private set; }
        //public decimal TaskPrice { get; private set; }
        //public decimal TaskUnitPrice { get; private set; }
        //public decimal TaskCost { get; private set; }

        //public string PartCode { get; private set; }
        //public decimal PartQuantity { get; private set; }
        //public decimal PartUnitCost { get; private set; }
        //public decimal PartCost { get; private set; }
        //public decimal PartUnitPrice { get; private set; }
        //public decimal PartPrice { get; private set; }

        protected void CopyFrom2(vRpt_Job job)   //, vRpt_JobDetail task)
        {
            CopyFrom(job);

            City = job.JobCity;
            State = job.JobState;
            PostalCode = job.JobPostalCode;

            BusinessTypeId = job.BusinessTypeID;
            BusinessType = job.BusinessType;

            JobPriority = job.JobPriority;
            TaxDescription = job.TaxDescription;

            FranchiseID = job.ClientID;

            ReferralID = job.ReferralID.GetValueOrDefault();
            ReferralType = job.ReferralType;

            //var j = new JobOverview();
            //j.CopyFrom(job);
            //j.JobCode = task.JobCode;
        }

        internal static JobOverview MapFromJob(vRpt_Job job)
        {
            var j = new JobOverview();
            j.CopyFrom2(job);
            
            return j;
        }

        //internal static JobOverview[] MapFromJobAndTasks(vRpt_Job job, vRpt_JobDetail[] tasks)
        //{
        //    var jobs = new List<JobOverview>();
        //    foreach (var t in tasks)
        //    {
        //        var j = new JobOverview();
        //        j.CopyFrom(job, t);
        //        jobs.Add(j);
        //    }

        //    return jobs.ToArray();
        //}
    }
}
