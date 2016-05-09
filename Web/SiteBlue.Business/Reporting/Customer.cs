using System;
using SiteBlue.Data.Reporting;

namespace SiteBlue.Business.Reporting
{
    public class Customer
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int ClientId { get; private set; }

        public string BillToAddress { get; private set; }
        public string BillToCity { get; private set; }
        public string BillToState { get; private set; }
        public string BillToCountry { get; private set; }
        public string BillToPostalCode { get; private set; }
        
        public string Email { get; private set; }
        public string PrimaryPhone { get; private set; }
        public string CellPhone { get; private set; }
        
        public DateTime LastVisit { get; private set; }
        public int JobCount { get; private set; }
        public decimal TotalSales { get; private set; }
        public decimal AverageJob { get; private set; }
        public decimal Payments { get; private set; }
        public decimal Balance { get; private set; }
        
        protected void CopyFrom(vRpt_Customer customer)
        {
            Id = customer.CustomerID;
            Name = customer.CustomerName;
            ClientId = customer.ClientID;
            BillToAddress = customer.BillToAddress;
            BillToCity = customer.BillToCity;
            BillToState = customer.BillToState;
            BillToPostalCode = customer.BillToPostalCode;
            BillToCountry = customer.BillToCountry;

            Email = customer.Email;
            PrimaryPhone = customer.PrimaryPhone;
            CellPhone = customer.CellPhone;

            LastVisit = customer.LastVisit;
            JobCount = customer.JobCount;
            TotalSales = customer.TotalSales;
            AverageJob = customer.AverageJob;
            Payments = customer.Payments;
            Balance = customer.Balance;
        }

        internal static Customer MapFromModel(vRpt_Customer customer)
        {
            var c = new Customer();
            c.CopyFrom(customer);
            return c;
        }
    }
}
