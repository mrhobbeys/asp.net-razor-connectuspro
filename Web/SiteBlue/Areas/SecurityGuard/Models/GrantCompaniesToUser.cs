using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SiteBlue.Areas.SecurityGuard.Models
{
    public class GrantCompaniesToUser
    {
        public MembershipUser User { get; set; }
        public string UserName { get; set; }
        public SelectList AvailibleCompanies { get; set; }
        public SelectList GrantedCompanies { get; set; }
        public string defaultCompanyName { get; set; }
        public int defaultCompanyID { get; set; }
        public List<SelectListItem> GrantedCompanyCode { get; set; }
    }
}