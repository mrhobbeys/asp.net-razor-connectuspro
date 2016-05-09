using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteBlue.Data.Reporting;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Business.Reporting
{
    public class CustomerLedger : Customer
    {
        //=======================================================
        //Already available in Customer class.
        //=======================================================
        //public string CustomerName { get; private set; }
        //public string Email { get; private set; }
        //public string PrimaryPhone { get; private set; }
        //public string CellPhone { get; private set; }
        //public string BillToAddress { get; private set; }
        //public string BillToCity { get; private set; }
        //public string BillToState { get; private set; }
        //public string BillToPostalCode { get; private set; }
        //public string BillToCountry { get; private set; }
        //public DateTime? LastVisit { get; private set; }
        //=======================================================

        public int? CustomerId { get; private set; }
        public int LineId { get; private set; }
        public int ReferenceId { get; private set; }

        public string Type { get; private set; }
        public decimal? Amount { get; private set; }
        public decimal? ActualBalance { get; private set; }
        public decimal StoredBalance { get; private set; }
        public DateTime? RecordedDate { get; private set; }
        public bool? IsOutstanding { get; private set; }
        public decimal? RunningBalance { get; private set; }
        public long? Sequence { get; private set; }

        protected void CopyFromCustomer(vRpt_Customer customer)
        {
            CopyFrom(customer);
        }

        protected void CopyFromLedger(vRpt_CustomerLedger ledger)
        {
            CustomerId = ledger.CustomerId;
            LineId = ledger.LineId;
            ReferenceId = ledger.ReferenceId;
            Type = ledger.Type;
            Amount = ledger.Amount;
            ActualBalance = ledger.ActualBalance;
            StoredBalance = ledger.StoredBalance;
            RecordedDate = ledger.RecordedDate;
            IsOutstanding = ledger.IsOutstanding;
            RunningBalance = ledger.RunningBalance;
            Sequence = ledger.Sequence;
        }

        public static CustomerLedger MapFromCustomer(vRpt_CustomerLedger ledger, vRpt_Customer customer)
        {
            var cl = new CustomerLedger();

            cl.CopyFromCustomer(customer);
            cl.CopyFromLedger(ledger);

            return cl;
        }

    }
}