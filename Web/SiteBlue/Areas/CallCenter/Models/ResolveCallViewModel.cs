using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.CallCenter.Models
{
    public class ResolveCallViewModel
    {
        public int Id { get; set; }
        public string CPCode { get; set; }
        public string RawDialInNumber { get; set; }
        public string DialInNumber { get; set; }
        public string CustomerPhone { get; set; }
        public bool Valid { get; set; }
        public Dictionary<int, string> AvailableFranchises { get; set; }
    }
}