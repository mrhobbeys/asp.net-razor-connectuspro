using System;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class Memberships
    {
        public int MembershipID { get; private set; }
        public int ClientID { get; private set; }
        
        public int CustomerID { get; private set; }
        public string CustomerName { get; private set; }
        public string Email { get; private set; }
        public string PrimaryPhone { get; private set; }
        public string CellPhone { get; private set; }
        public string BillToAddress { get; private set; }
        public string BillToCity { get; private set; }
        public string BillToState { get; private set; }
        public string BillToPostalCode { get; private set; }
        public string BillToCountry { get; private set; }

        public int MemberTypeID { get; private set; }
        public string MemberType { get; private set; }
        public DateTime? MembershipStartDate { get; private set; }
        public DateTime? MembershipEndDate { get; private set; }

        public int? Req_TIPerYear { get; private set; }
        public int? InspectionCnt { get; private set; }
        public DateTime? LastDateInspected { get; private set; }
        public DateTime? LastCustomerVisit { get; private set; }
        public int CountCustomerVisit { get; private set; }
        
        public int JobCount { get; private set; }
        public decimal TotalSales { get; private set; }
        public decimal AverageJob { get; private set; }
        public decimal Payments { get; private set; }
        public decimal Balance { get; private set; }

        protected void CopyFrom(vRPT_MembershipInfo member)
        {
            MembershipID = member.MembershipID;
            ClientID = member.ClientID;
            
            CustomerID = member.CustomerID;
            CustomerName = member.CustomerName;
            Email = member.Email;
            PrimaryPhone = member.PrimaryPhone;
            CellPhone = member.CellPhone;
            BillToAddress = member.BillToAddress;
            BillToCity = member.BillToCity;
            BillToState = member.BillToState;
            BillToPostalCode = member.BillToPostalCode;
            BillToCountry = member.BillToCountry;

            MemberTypeID = member.MemberTypeID;
            MemberType = member.MemberType;
            MembershipStartDate = member.MembershipStartDate;
            MembershipEndDate = member.MembershipEndDate;
            Req_TIPerYear = member.req_TotalInspectionsPerYear;
            InspectionCnt = member.CountOfInspections;
            LastDateInspected = member.LastDateInspected;
            LastCustomerVisit = member.LastCustomerVisit;
            CountCustomerVisit = member.CountCustomerVisits;

            JobCount = member.JobCount;
            TotalSales = member.TotalSales;
            AverageJob = member.AverageJob;
            Payments = member.Payments;
            Balance = member.Balance;
        }

        internal static Memberships MapFromModel(vRPT_MembershipInfo member)
        {
            var mem = new Memberships();
            mem.CopyFrom(member);
            return mem;
        }
    }
}
