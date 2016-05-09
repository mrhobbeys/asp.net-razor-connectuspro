using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class Franchisee
    {
        public int FranchiseID { get; set; }
        public string FranchiseNUmber { get; set; }
        public int ProspectID { get; set; }
        public int OwnerID { get; set; }
        public int ConceptID { get; set; }
        public int FranchiseTypeID { get; set; }
        public int FranchiseStatusID { get; set; }
        public int SalesRepID { get; set; }
        public int SupportRepID { get; set; }
        public int DivisionID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> RenewalDate { get; set; }
        public string WebSite { get; set; }
        public string EMail { get; set; }
        public string LegalName { get; set; }
        public string LegalAddress { get; set; }
        public string LegalAddress2 { get; set; }
        public string LegalCity { get; set; }
        public string LegalState { get; set; }
        public string LegalPostal { get; set; }
        public int LegalCountryID { get; set; }
        public string ShipName { get; set; }
        public string ShipCompany { get; set; }
        public string ShipAddress { get; set; }
        public string ShipAddress2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipPostal { get; set; }
        public int ShipCountryID { get; set; }
        public string MailName { get; set; }
        public string MailCompany { get; set; }
        public string MailAddress { get; set; }
        public string MailAddress2 { get; set; }
        public string MailCity { get; set; }
        public string MailState { get; set; }
        public string MailPostal { get; set; }
        public int MailCountryID { get; set; }
        public string OfficeName { get; set; }
        public string OfficeCompany { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeAddress2 { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeState { get; set; }
        public string OfficePostal { get; set; }
        public int OfficeCountryID { get; set; }
        public string GeneralNotes { get; set; }
        public int TimeOffset { get; set; }
        public byte[] TimeStamp { get; set; }

        public int FranchiseContactID { get; set; }
        public string ContactName { get; set; }
        public int PhoneTypeID { get; set; }
        public string PhoneType { get; set; }
        public string PhoneNumber { get; set; }
    }
}