using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Reporting
{
    /// <summary>
    /// Reporting View-Model for Payroll Setup
    /// </summary>
    public class JobCode
    {
        public int FranchiseID { get; internal set; }
        public int PriceBookID {get;internal set;}
	    public string PriceBookName {get;internal set;}
	    public bool PriceBookActiveYN {get;internal set;}
	    public int JobCodeID {get;internal set;}
	    public bool? ActiveYN {get;internal set;}
	    public string JobCodeName {get;internal set;}
        public string JobCodeDescription { get; internal set; }
    }
}
