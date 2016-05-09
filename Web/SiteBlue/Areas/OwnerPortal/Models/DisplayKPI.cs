using System;
using System.Collections.Generic;


namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class DisplayKPI
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool ShowAllTechs { get; set; }
        public int FranchiseId { get; set; }
        public string FranchiseName { get; set; }

        public decimal Total_SalesActual { get; set; }
        public decimal Total_SalesEstimates { get; set; }
        public decimal Total_SalesOutstandingEstimates { get; set; }
        public int Total_UpSalesNumber { get; set; }
        public decimal Total_UpSalesCost { get; set; }
        public decimal Total_UpSalesHG { get; set; }
        public int Total_UpSalesBio { get; set; }
        public int Total_DiscountsNumber { get; set; }
        public decimal Total_DiscountsCost { get; set; }
        public int Total_JobsComplete { get; set; }
        public int Total_JobsEstimate { get; set; }
        public int Total_JobsOutstandingEstimate { get; set; }
        public int Total_JobsRecall { get; set; }
        public int Total_ClosingRate { get; set; }
        public int Total_RecoverRate { get; set; }
        public decimal Total_JobsAVG { get; set; }

        public IEnumerable<Technician> TechnicianList { get; set; } 
    }
}