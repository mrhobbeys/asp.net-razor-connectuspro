using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class CompanyInfoViewModel
    {
        public int FranchiseID { get; set; }

        // Legal Information
        public string OwnerName { get; set; }
        public string LegalName { get; set; }
        public string LegalAddress { get; set; }
        public string LegalCity { get; set; }
        public string LegalState { get; set; }
        public string LegalPostal { get; set; }
        public int LegalCountryID { get; set; }

        // Office Information
        public string OfficeName { get; set; }
        public string OfficeCompany { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeState { get; set; }
        public string OfficePostal { get; set; }
        public int OfficeCountryID { get; set; }

        // Shipping Information
        public string ShipName { get; set; }
        public string ShipCompany { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipPostal { get; set; }
        public int ShipCountryID { get; set; }

        // Mailing Information
        public string MailName { get; set; }
        public string MailCompany { get; set; }
        public string MailAddress { get; set; }
        public string MailCity { get; set; }
        public string MailState { get; set; }
        public string MailPostal { get; set; }
        public int MailCountryID { get; set; }

        public SelectList PhoneType { get; set; }
        public SelectList CountryList { get; set; }
    }
}