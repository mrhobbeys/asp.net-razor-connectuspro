using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class Calendarmodel
    {
        public int? frachiseid { get; set; }
        public DateTime? date { get; set; }

        public int EmployeeId { get; set; }
        public int FrenchiseID { get; set; }
        public int FrenchiseName { get; set; }
        public string Name { get; set; }
        public int SalesActual { get; set; }
        public decimal SalesEstimates { get; set; }
        public int JobsComplete { get; set; }
        public int JobsEstimate { get; set; }
        public int JobsAVG { get; set; }
        public double JobsRecall { get; set; }
        public int JobsDispatched { get; set; }
        public int UpSalesNumber { get; set; }
        public int UpSalesCost { get; set; }
        public int UpSalesHG { get; set; }
        public int UpSalesBio { get; set; }
        public int DiscountsNumber { get; set; }
        public int ClosingRate { get; set; }
        public decimal DiscountsCost { get; set; }
        public bool IsSummary { get; set; }

        public string maindiv { get; set; }
        public string secondarydiv { get; set; }
        public string secondarydivP { get; set; }
        public string griddiv { get; set; }
        public string trdetails { get; set; }
        public string Actualdiv { get; set; }
        public string Upsalesdiv { get; set; }
        public string EstimateSalesdiv { get; set; }
        public string Discountdiv { get; set; }
        public string Arrowdiv { get; set; }
        
    }
}