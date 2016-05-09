using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.PayrollSetup
{
    public class PayrollSpiff
    {
        public int PayrollSetupID { get; internal set; }
        public int ServiceProID { get; internal set; }
        public int JobCodeID { get; internal set; }
        public int PayType { get; internal set; }
        public decimal Rate { get; internal set; }
        public DateTime? DateExpires { get; internal set; }
        public string Comments { get; internal set; }
        public bool AddOn { get; internal set; }
        public bool Active { get; internal set; }

    }
}
