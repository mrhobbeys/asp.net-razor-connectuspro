using System.Collections.Generic;
namespace SiteBlue.Areas.CallCenter.Models
{
    public class Incoming
    {
        public int Id { get; set; }
        public string CPCode { get; set; }
        public string RawDialInNumber { get; set; }
        public string DialInNumber { get; set; }
        public string CustomerPhone { get; set; }
        public string CallScript { get; set; }
        public Dictionary<int, string> JobPriorities { get; set; }
        public Dictionary<int, string> PaymentTypes { get; set; }
        public bool Valid { get; set; }
        public bool StatTrackingInvalid { get; set; }
        public Dictionary<int, string> AvailableFranchises { get; set; }
        public bool FranchiseCanChange { get { return AvailableFranchises.Count > 1; } }
    }
}