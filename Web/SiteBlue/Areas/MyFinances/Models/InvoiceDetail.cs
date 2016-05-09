using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.MyFinances.Models
{
    public class InvoiceDetail
    {
        public int jobID { get; set; }
        public string invoicenumber { get; set; }
        public string billto { get; set; }
        public string joblocation { get; set; }
        public string status { get; set; }
        public DateTime closeddate { get; set; }
        public string Tech { get; set; }
        public decimal Jobamt { get; set; }
        public decimal balance { get; set; }
        public string jobtype { get; set; }
        public string comments { get; set; }
        public DateTime WsrDate { get; set; }
        public decimal TotalInvSales { get; set; }
        public Int32 TotalJobs { get; set; }

        public decimal taxamt { get; set; }
        public decimal TotalSales { get; set; }
        public decimal totalTax { get; set; }

        public int totalCount { get; set; }

        public string Account { get; set; }
        public float Amount { get; set; }
        public float AcctTotal { get; set; }
        
        
    }
}