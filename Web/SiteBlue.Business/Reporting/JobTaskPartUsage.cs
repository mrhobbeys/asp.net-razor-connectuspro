using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class JobTaskPartUsage
    {
        public int TicketNumber { get; private set; }

        public DateTime? CallCompleted { get; private set; }
        public string Status { get; private set; }
        public string ServiceName { get; private set; }
        public string BusinessType { get; private set; }

        public string CustomerName { get; private set; }
        public string JobAddress { get; private set; }
        public string JobCity { get; private set; }
        public string JobState { get; private set; }
        public string JobPostalCode { get; private set; }

        public int ClientID { get; private set; }

        public string JobCode { get; private set; }
        public decimal TaskQty { get; private set; }

        public string PartCode { get; private set; }
        public string PartName { get; private set; }
        public decimal PartQty { get; private set; }
        public decimal PartCost { get; private set; }
        public decimal PartTotalCost { get; private set; }
        public decimal PartUnitPrice { get; private set; }
        public decimal PartTotalPrice { get; private set; }

        protected void CopyFrom(vRpt_PartUsagePerJob partusage)
        {
            TicketNumber = partusage.TicketNumber;
            CallCompleted = partusage.CallCompleted;
            Status = partusage.Status;
            ServiceName = partusage.ServiceName;
            BusinessType = partusage.BusinessType;

            CustomerName = partusage.CustomerName;
            JobAddress = partusage.JobAddress;
            JobCity = partusage.JobCity;
            JobState = partusage.JobState;
            JobPostalCode = partusage.JobPostalCode;

            ClientID = partusage.ClientID;

            JobCode = partusage.JobCode;
            TaskQty = partusage.TaskQty;

            PartCode = partusage.PartCode;
            PartName = partusage.PartName;
            PartQty = partusage.PartQty;
            PartCost = partusage.PartCost.GetValueOrDefault();
            PartTotalCost = partusage.PartTotalCost.GetValueOrDefault();
            PartUnitPrice = partusage.PartUnitPrice;
            PartTotalPrice = partusage.PartTotalPrice.GetValueOrDefault();
        }

        internal static JobTaskPartUsage MapFromModel(vRpt_PartUsagePerJob partusage)
        {
            var j = new JobTaskPartUsage();
            j.CopyFrom(partusage);
            return j;
        }
    }
}
