using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
   public class CustomerLedgerSummary
    {
        public int CustomerId { get; private set; }
        public int ClientId { get; private set; }
        
        public decimal? Invoices { get; private set; }
        public decimal? payments { get; private set; }
        public decimal? OutstandingBalance { get; private set; }

        public string CustomerName { get; private set; }


        protected void CopyFrom(vRpt_CustomerLedgerSummary rptCust)
        {
            CustomerId = rptCust.CustomerId;
            ClientId = rptCust.ClientId;
            Invoices = rptCust.Invoices;
            payments = rptCust.Payments;
            OutstandingBalance = rptCust.OutstandingBalance;
        }


        internal static CustomerLedgerSummary MapFromModel(vRpt_CustomerLedgerSummary rptwsr,  string cname)
        {
            var Cust = new CustomerLedgerSummary();
            Cust.CopyFrom(rptwsr);

            Cust.CustomerName = cname;

            return Cust;
        }





























































































































































































































































    }
}
