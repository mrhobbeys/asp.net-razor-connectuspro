using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class Aging
    {
        public string Invoicenumber { get; set; }
        public string InvoiceDate { get; set; }
        public DateTime? InvoiceDates { get; set; }
        public string CreditTerms { get; set; }
        public string DueDate { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public int? DaysLate { get; set; }
        
        public int? FranchiseID { get; set; }
    }
}