using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteBlue.Data.Reporting;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Business.Reporting
{
    public class JobTask
    {
        public int Id { get; private set; }
        public string JobCode { get; private set; }
        public string Description { get; private set; }
        public decimal Rate { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Amount { get; private set; }
        public bool IsAddOn { get; private set; }
        public int JobId { get; private set; }

        public bool IsMemberPlan { get { return (JobCode.StartsWith("A0") && Amount > 0); } }

        public bool IsBio
        {
            get { return JobCode != null && (JobCode.Contains("B019") || JobCode.Contains("B02000")); }
        }

        public bool IsDiscount
        {
            get { return (Description ?? string.Empty).ToLower().Contains("discount"); }
        }

        public bool IsInspection
        {
            get { return JobCode != null && JobCode.StartsWith("A") && Amount == 0; }
        }

        protected void CopyFrom(vRpt_JobDetail task)
        {
            if (task == null) return;

            Description = task.JobCodeDescription;
            JobCode = task.JobCode;
            Rate = task.Rate;
            Quantity = task.Quantity;
            Amount = task.Amount;
            JobId = task.JobID;
            IsAddOn = task.IsAddOn;
        }

        protected void CopyFrom(tbl_Job_Tasks task)
        {
            if (task == null) return;

            Description = task.JobCodeDescription;
            JobCode = task.JobCode;
            Rate = task.UnitPrice;
            Quantity = task.Quantity;
            Amount = task.Price;
            JobId = task.JobID;
            IsAddOn = task.AddOnYN;
        }

        public static JobTask MapFromModel(vRpt_JobDetail task)
        {
            var t = new JobTask();
            t.CopyFrom(task);
            return t;
        }

        public static JobTask MapFromModel(tbl_Job_Tasks task)
        {
            var t = new JobTask();
            t.CopyFrom(task);
            return t;
        }
    }
}
