using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class CustomerList
    {
        
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string EMail { get; set; }
        public int? FranchiseID { get; set; }
        public string FranchiseNUmber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public float Balance { get; set; } 

        public int JobID { get; set; } 
        public string InvoiceNumber { get; set; }
        public DateTime? InvoicedDate { get; set; }

        public int? MemberID { get; set; }
        public string MemberType { get; set; }
        public string Exp_date { get; set; }
        public string Scheduled { get; set; }
        public int AgeRecords { get; set; }
        public string House { get; set; } 
    }
}