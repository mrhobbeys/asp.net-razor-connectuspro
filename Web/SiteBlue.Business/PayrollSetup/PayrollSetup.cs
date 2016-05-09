using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.PayrollSetup
{
    public class PayrollSetup
    {
        public int FranchiseID { get; internal set; }
        public decimal OvertimeStarts { get; internal set; }
        public int OvertimeMethodID { get; internal set; }
        public decimal OTMultiplier { get; internal set; }
    }
}
