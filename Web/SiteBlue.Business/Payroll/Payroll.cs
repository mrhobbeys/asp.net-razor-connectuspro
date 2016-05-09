using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Payroll
{
    public class Payroll
    {
        public int PayrollID    { get; internal set; }  // BPanjavan - Ok for now but this should be here
        public int FranchiseID  { get; private set; }  // BPanjavan - Ok for now but this should be here
        public DateTime PayrollDate { get; private set; }

        public List<PayrollDetail> PayrollDetails { get; private set; }

        public Payroll(DateTime date, int franchiseid)
        {
            this.PayrollDate = date;
            this.FranchiseID = franchiseid;
            PayrollDetails = new List<PayrollDetail>();
        }

        public Payroll(int payrollid)
        {
            throw new NotImplementedException("Ha ha not implemented yet!");
        }

        public decimal GrossPay
        {
            get
            {
                return (from pd in this.PayrollDetails select pd.GrossPay).Sum();
            }
        }

    }
}
