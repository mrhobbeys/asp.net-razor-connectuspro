using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Payroll
{
    public class PayrollDetail
    {
        internal PayrollDetail(int EmployeeID, int? PayrollID, decimal FridayHours
                    ,decimal MondayHours,decimal OTHours,decimal OTPay,decimal OTRate, decimal RegularHours
                    ,decimal RegularPay,decimal RegularRate, decimal SundayHours,decimal SaturdayHours, decimal ThursdayHours
                    ,decimal TuesdayHours,decimal WednesdayHours,decimal WeeklySalary,decimal CommissionRateHour, List<JobPayroll> JobPayrolls)
        {
            this.EmployeeID = EmployeeID;
            this.PayrollID = PayrollID;
            this.SundayHours = SundayHours;
            this.MondayHours = MondayHours;
            this.TuesdayHours = TuesdayHours;
            this.WednesdayHours = WednesdayHours;
            this.ThursdayHours = ThursdayHours;
            this.FridayHours = FridayHours;
            this.SaturdayHours = SaturdayHours;
            this.OTHours = OTHours;
            this.OTPay = OTPay;
            this.OTRate = OTRate;
            this.RegularHours = RegularHours;
            this.RegularPay = RegularPay;
            this.RegularRate = RegularRate;
            this.WeeklySalary = WeeklySalary;
            this.CommissionRateHour = CommissionRateHour;
            this.JobPayrolls = JobPayrolls;
        }
        
        
        public decimal TotalCommission
        {
            get
            {
                return Math.Round(this.JobPayrolls.Sum<JobPayroll>(jp => jp.TotalCommissionPartsAndLabor),2);
            }
        }
        public decimal CommissionSpifs
        {
            get
            {
                return Math.Round(this.JobPayrolls.Sum<JobPayroll>(jp => jp.TotalCommissionSpifs),2);
            }
        }
        public int JobCount
        {
            get
            {
                return this.JobPayrolls.Count;
            }
        }

        public decimal GrossPay
        {
            get
            {
                return WeeklySalary + RegularPay + OTPay + this.TotalCommission + this.CommissionSpifs + OTAdditCommission; 
            }
        }

        public decimal CommissionOTMultiplier
        {
            get
            {
                return (this.RegularHours + this.OTHours == 0) ? 0M : Math.Round( TotalCommission / (this.RegularHours + this.OTHours), 2 );
            }
        }
        public decimal OTAdditCommission
        {
            get
            {
                return Math.Round( this.CommissionRateHour * this.OTHours * CommissionOTMultiplier, 2);
            }
        }

        public int EmployeeID { get; internal set; }
        public decimal FridayHours {get; internal set;}
        public decimal MondayHours  {get; internal set;}
        public decimal OTHours  {get; internal set;}
        public decimal OTPay  {get; internal set;}
        public decimal OTRate  {get; internal set;}
        public int? PayrollID  {get; internal set;}
        public decimal RegularHours {get; internal set;}
        public decimal RegularPay  {get; internal set;}
        public decimal RegularRate {get; internal set;}
        public decimal SundayHours  {get; internal set;}
        public decimal SaturdayHours { get; internal set; }
        public decimal ThursdayHours  {get; internal set;}
        public decimal TuesdayHours  {get; internal set;}
        public decimal WednesdayHours {get; internal set;}
        public decimal WeeklySalary  {get; internal set;}
        public decimal CommissionRateHour  {get; internal set;}

        public List<JobPayroll> JobPayrolls { get; internal set; }

    }
}
