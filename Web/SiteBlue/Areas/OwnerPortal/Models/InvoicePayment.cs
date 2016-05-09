using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class InvoicePayment
    {
        public string InvoiceNumber { get; set; }
        public string Address { get; set; }
        public System.Nullable<System.DateTime> InvoicedDate { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Balance { get; set; }
        public int JobID { get; set; } 

    }
}