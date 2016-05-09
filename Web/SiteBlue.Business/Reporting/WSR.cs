namespace SiteBlue.Business.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SiteBlue.Data.Reporting;

    public class WSR
    {
        public int? TicketNumber { get; private set; }

        public DateTime? WSRCompletedDate { get; private set; }
        public DateTime? CallCompleted { get; set; }

        public int? StatusID { get; private set; }
        public string Status { get; private set; }

        public int? ServiceID { get; private set; }
        public string ServiceName { get; private set; }

        public decimal? TotalSales { get; private set; }
        public decimal? SubTotal { get; private set; }
        public decimal? Balance { get; private set; }
        public decimal? TaxAmount { get; private set; }

        public string TaxDescription { get; private set; }

        public int? BusinessTypeID { get; private set; }
        public string BusinessType { get; private set; }

        public string CustomerName { get; private set; }

        public string JobAddress { get; private set; }
        public string JobCity { get; private set; }
        public string JobState { get; private set; }
        public string JobPostalCode { get; private set; }

        public int? ClientID { get; private set; }

        public int? JobPriorityID { get; private set; }
        public string JobPriority { get; private set; }

        public int? ServiceProID { get; private set; }

        protected void CopyFrom(vRpt_WSR rptwsr)
        {
            TicketNumber = rptwsr.TicketNumber;
            WSRCompletedDate = rptwsr.WSRCompletedDate;
            CallCompleted = rptwsr.CallCompleted;
            StatusID = rptwsr.StatusID;
            Status = rptwsr.Status;
            ServiceID = rptwsr.ServiceID;
            ServiceName = rptwsr.ServiceName;
            TotalSales = rptwsr.TotalSales;
            TaxAmount = rptwsr.TaxAmount;
            SubTotal = rptwsr.SubTotal;
            TaxDescription = rptwsr.TaxDescription;
            BusinessTypeID = rptwsr.BusinessTypeID;
            BusinessType = rptwsr.BusinessType;
            CustomerName = rptwsr.CustomerName;
            JobAddress = rptwsr.JobAddress;
            JobCity = rptwsr.JobCity;
            JobState = rptwsr.JobState;
            JobPostalCode = rptwsr.JobPostalCode;
            ClientID = rptwsr.ClientID;
            JobPriorityID = rptwsr.JobPriorityID;
            JobPriority = rptwsr.JobPriority;
            ServiceProID = rptwsr.ServiceProID;
            Balance = rptwsr.Balance;
        }

        internal static WSR MapFromModel(vRpt_WSR rptwsr)
        {
            var wsr = new WSR();
            wsr.CopyFrom(rptwsr);
            return wsr;
        }
    }
}
