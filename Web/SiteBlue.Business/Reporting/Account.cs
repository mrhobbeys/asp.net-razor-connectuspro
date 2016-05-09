using System;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class Account
    {
        public long uID { get; private set; }

        public int ClientID { get; private set; }
        public string AccountCode { get; private set; }
        public string AccountName { get; private set; }
        public int? JobCount { get; private set; }
        public decimal? TotalSales { get; private set; }
        public string ServiceName { get; private set; }
        public string BusinessType { get; private set; }
        public DateTime? FirstJobDate { get; private set; }
        public DateTime? LastJobDate { get; private set; }
        public DateTime? WSRCompletedDate { get; private set; }

        protected void CopyFrom(vRpt_AccountingSummary account)
        {
            uID = account.uID;

            ClientID = account.ClientID;
            AccountCode = account.AccountCode ?? "N/A";
            AccountName = account.AccountName ?? "N/A";
            JobCount = account.JOBCOUNT;
            TotalSales = account.TotalSales;
            ServiceName = account.ServiceName;
            BusinessType = account.BusinessType;
            FirstJobDate = account.FirstJobDate;
            LastJobDate = account.LastJobDate;
            WSRCompletedDate = account.WSRCompletedDate;
        }

        internal static Account MapFromModel(vRpt_AccountingSummary account)
        {
            var acnt = new Account();
            acnt.CopyFrom(account);
            return acnt;
        }
    }
}
