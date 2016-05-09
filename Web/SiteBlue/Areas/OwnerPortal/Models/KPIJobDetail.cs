using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class KPIJobDetail
    {
        public int JobId { get; set; }
        public string BillTo { get; set; }
        public string JobLocation { get; set; }
        public string Status { get; set; }
        public string Tech { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? EstimateDate { get; set; }
        public DateTime? InvoicedDate { get; set; }
        public decimal? JobAmt { get; set; }
        public decimal Balance { get; set; }
        public string Comments { get; set; }
        public decimal SubTotal { get; set; }
        public string JobType { get; set; }
        public int InvoiceNumber { get { return JobId; } }
    }
}