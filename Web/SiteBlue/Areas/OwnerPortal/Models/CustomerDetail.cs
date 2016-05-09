using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class CustomerDetail
    {
        public CustomerDetail()
        {
            customerinfo = new CustomerInfo();
            billaddr = new BillAddress();
            jobhistory = new JobHistory();
            serviceplan = new ServicePlanInfo();
            oppinfo = new OpportunityInfo();
            financedata = new FinancialData();
        }

        public int CustomerID { get; set; }
        public List<SelectListItem> ServiceStatus { get; set; }

        public CustomerInfo customerinfo { get; set; }
        public BillAddress billaddr { get; set; }
        public JobHistory jobhistory { get; set; }
        public ServicePlanInfo serviceplan { get; set; }
        public OpportunityInfo oppinfo { get; set; }
        public FinancialData financedata { get; set; }
    }

    public class CustomerInfo
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PrimaryPhone { get; set; }
        public string CellPhone { get; set; }
        public string EMail { get; set; }
        public string Notes { get; set; }
    }

    public class BillAddress
    {
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

    public class JobHistory
    {
        public DateTime? DateNextJob { get; set; }
        public DateTime? DateLastJob { get; set; }
        public int TotalJobs { get; set; }
    }

    public class ServicePlanInfo
    {
        public SelectList MemberTypeList { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int ServiceVisits { get; set; }
        public DateTime? CreditCardExpiryDate { get; set; }
    }

    public class OpportunityInfo
    {
        public int AgeHome { get; set; }
        public int AgeSystem { get; set; }
        public int AgeHeater { get; set; }
    }

    public class FinancialData
    {
        public string Balance { get; set; }
    }

    public class CustomerHistoryInfo
    {
        public string FieldName { get; set; }
        public string TableName { get; set; }
        public string ChangeType { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string isTablet { get; set; }
        public string ChangedBy { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}