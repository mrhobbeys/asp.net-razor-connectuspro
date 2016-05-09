using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class Pay_ments
    {
        public string paydate { get; set; }
        public string paytype { get; set; }
        public string paydrivelicno { get; set; }
        public string paycheckno { get; set; }
        public string payamt { get; set; }
        public string paytotal { get; set; }
        public string paymentid { get; set; }
        public int statusid { get; set; }
        public int jobId { get; set; }
    }
}