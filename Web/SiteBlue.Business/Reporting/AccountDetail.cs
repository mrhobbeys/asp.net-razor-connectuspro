using System;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class AccountDetail
    {
        public long uID { get; private set; }

        public int ClientID { get; private set; }
        public string AccountCode { get; private set; }
        public string AccountName { get; private set; }
        public int? JobCount { get; private set; }
        public decimal? TotalSales { get; private set; }
        public string ServiceName { get; private set; }
        public string BusinessType { get; private set; }
        public DateTime? JobDate { get; private set; }
        public DateTime? WSRCompletedDate { get; private set; }

        protected void CopyFrom(vRpt_AccountingDetail account)
        {
            uID = account.uID;

            ClientID = account.ClientID;
            AccountCode = account.AccountCode ?? "N/A";
            AccountName = account.AccountName ?? "N/A";
            JobCount = account.JOBCOUNT;
            TotalSales = account.TotalSales;
            ServiceName = account.ServiceName;
            BusinessType = account.BusinessType;
            JobDate = account.JobDate;
            WSRCompletedDate = account.WSRCompletedDate;
        }

        internal static AccountDetail MapFromModel(vRpt_AccountingDetail account)
        {
            var acnt = new AccountDetail();
            acnt.CopyFrom(account);
            return acnt;
        }
    }
}
