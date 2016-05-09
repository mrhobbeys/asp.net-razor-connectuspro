using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class Payroll_Details
    {
        public int PayrollDetailID { get; set; }
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public float WeeklySalary { get; set; }
        public float RegularHours { get; set; }
        public float RegularRate { get; set; }
        public float RegularPay { get; set; }
        public float OTHours { get; set; }
        public float OTRate { get; set; }
        public float OTPay { get; set; }
        public float CommissionParts { get; set; }
        public float CommissionLabor { get; set; }
        public float CommissionSpifs { get; set; }
        public float TotalCommission { get; set; }
        public float GrossPay { get; set; }
        public float SundayHours { get; set; }
        public float MondayHours { get; set; }
        public float TuesdayHours { get; set; }
        public float WednesdayHours { get; set; }
        public float ThursdayHours { get; set; }
        public float FridayHours { get; set; }
        public float SaturdayHours { get; set; }
        public int JobCount { get; set; }
        public float Adjustment { get; set; }
        public string AdjustmentReason { get; set; }
        public float CommissionOTMultiplier { get; set; }
        public float CommissionRateHour { get; set; }
        public float OTAdditCommission { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Employee { get; set; }
        public string PayType { get; set; }

        public int LocationID { get; set; }
        public string Address { get; set; }
        public System.Nullable<System.DateTime> ServiceDate { get; set; }
        public string PayrollLockDateString { get; set; }
    }
}