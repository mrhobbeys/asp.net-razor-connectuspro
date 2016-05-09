
namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class MyBudgetInfo
    {
        public const int DaysPerYear = 260;
        public const int MonthsPerYear = 12;

        public decimal SalesAnnual { get; set; }
        public decimal AvgTicketAnnual { get; set; }
        public decimal ClosingRateAnnual { get; set; }
        public int HomeGuardAnnual { get; set; }
        public int BioAnnual { get; set; }
        public decimal PayrollSalesAnnual { get; set; }
        public decimal RecallJobsAnnual { get; set; }
        public int DaysInYear { get { return DaysPerYear; } }
        public int MonthsInYear { get { return MonthsPerYear; } }
    }

}