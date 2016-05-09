using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Reporting
{
    public class PayrollSpiff
    {
        public int PayrollSpiffID { get; internal set; }
        public int PayrollSetupID { get; internal set; }
        public int ServiceProID { get; internal set; }
        public string Employee { get; internal set; }
        public int JobCodeID { get; internal set; }
        public string JobCode { get; internal set; }
        public string JobCodeDescription { get; internal set; }
        public int PayTypeID { get; internal set; }
        public string PayType { get; internal set; }
        public decimal Rate { get; internal set; }
        public DateTime? DateExpires { get; internal set; }
        public string Comments { get; internal set; }
        public bool AddOn { get; internal set; }
        public bool Active { get; internal set; }

    }
}
