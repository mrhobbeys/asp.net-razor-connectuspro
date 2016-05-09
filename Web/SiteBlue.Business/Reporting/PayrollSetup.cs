using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Reporting
{
    /// <summary>
    /// Reporting View-Model for Payroll Setup
    /// </summary>
    public class PayrollSetup
    {
        public int FranchiseID { get; internal set; }
        public int PayrollSetupID { get; internal set; }
        public float OvertimeStarts { get; internal set; }
        public int OvertimeMethodID { get; internal set; }
        public string OvertimeMethod { get; internal set; }
        public float OTMultiplier { get; internal set; }

        public IEnumerable<PayrollSpiff> PayrollSpiffs {get;internal set;}
    }
}
