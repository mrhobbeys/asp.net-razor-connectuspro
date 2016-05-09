using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Payroll
{
    public class JobPayroll
    {
        // TODO: PayrollDetailsID?  No, since Payroll Details should have a collection of JobPayroll records
        public int JobID { get; internal set; }
        public decimal JobSubTotal {get;internal set;}
        public decimal TotalCommissionPartsAndLabor {get;internal set;}
        public decimal TotalCommissionSpifs {get;internal set;}

        internal JobPayroll(int JobID, decimal JobSubTotal, decimal TotalCommissionPartsAndLabor, decimal TotalCommissionSpifs)
        {
            this.JobID = JobID;
            this.JobSubTotal = JobSubTotal;
            this.TotalCommissionPartsAndLabor = TotalCommissionPartsAndLabor;
            this.TotalCommissionSpifs = TotalCommissionSpifs;
        }
    }
}

