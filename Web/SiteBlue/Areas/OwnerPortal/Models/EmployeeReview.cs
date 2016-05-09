using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class EmployeeReview
    {
        public string DateReason { get; set; }
        public string EmployeeName { get; set; }
        public string Form { get; set; }
        public DateTime? BirthDate { get; set; }
        public System.Nullable<System.DateTime> Anniversary { get; set; }
        public System.Nullable<System.DateTime> HiredDate { get; set; }
        public System.Nullable<System.DateTime> DLExpDate { get; set; }
        public string ProLisc { get; set; }

        public int ReviewID { get; set; }
        public int FranchiseID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime ReviewDate { get; set; }
        public int FormID { get; set; }
        public bool CompletedYN { get; set; }
        public string Comments { get; set; }
        public string Subject { get; set; }
        public byte[] timestamp { get; set; }
    }
}