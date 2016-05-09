using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.MyFinances.Models
{
    public class JobDetail
    {
        public int JobID { get; set; }
        public string InvoiceNumber { get; set; }
        public string Address { get; set; }
        public string Age { get; set; }
        public Nullable<DateTime> InvoicedDate { get; set; }
        public decimal TotalSales{ get; set; }
        public decimal Balance{ get; set; }
        public decimal AppliedAmt { get; set; }
        //public int MyProperty { get; set; }

    }
}