using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class TimeCardSummary
    {
        public decimal TimeSheetSundayHours { get; set; }
        public decimal TimeSheetMondayHours { get; set; }
        public decimal TimeSheetTuesdayHours { get; set; }
        public decimal TimeSheetWednesdayHours { get; set; }
        public decimal TimeSheetThursdayHours { get; set; }
        public decimal TimeSheetFridayHours { get; set; }
        public decimal TimeSheetSaturdayHours { get; set; }
    }
}