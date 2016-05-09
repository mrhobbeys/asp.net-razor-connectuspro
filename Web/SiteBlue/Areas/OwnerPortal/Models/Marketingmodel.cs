using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class Marketingmodel
    {
        public List<tbl_Referral> refList { get ;set;}
        public List<tbl_coupon> couponList { get; set; } 
    }
}