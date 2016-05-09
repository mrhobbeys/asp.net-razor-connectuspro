using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_Franchise")]
    public class Franchise
    {
        [Key]
        [Display(Name = "Franchise ID")]
        public int FranchiseID { get; set; }

        [Display(Name = "Franchise Number")]
        [MaxLength(10)]
        public string FranchiseNumber { get; set; }

        [Display(Name = "Prospect ID")]
        public int ProspectID { get; set; }

        [Display(Name = "Owner ID")]
        public int OwnerID { get; set; }

        [Display(Name = "Concept ID")]
        public int ConceptID { get; set; }

        [Display(Name = "Franchise Type ID")]
        public int FranchiseTypeID { get; set; }

        [Display(Name = "Franchise Status ID")]
        public int FranchiseStatusID { get; set; }

        [Display(Name = "SalesRep ID")]
        public int SalesRepID { get; set; }

        [Display(Name = "SupportRep ID")]
        public int SupportRepID { get; set; }

        [Display(Name = "Division ID")]
        public int DivisionID { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Renewal Date")]
        public DateTime? RenewalDate { get; set; }

        [Display(Name = "WebSite")]
        [MaxLength(150)]
        public string WebSite { get; set; }

        [Display(Name = "Email")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Display(Name = "Legal Name")]
        [MaxLength(50)]
        public string LegalName { get; set; }

        [Display(Name = "Legal Address")]
        [MaxLength(50)]
        public string LegalAddress { get; set; }

        [Display(Name = "Legal Address2")]
        [MaxLength(50)]
        public string LegalAddress2 { get; set; }

        [Display(Name = "Legal City")]
        [MaxLength(50)]
        public string LegalCity { get; set; }

        [Display(Name = "Legal State")]
        [MaxLength(50)]
        public string LegalState { get; set; }

        [Display(Name = "Legal Postal")]
        [MaxLength(50)]
        public string LegalPostal { get; set; }

        [Display(Name = "Legal Country ID")]
        public int LegalCountryID { get; set; }

        [Display(Name = "Ship Name")]
        [MaxLength(50)]
        public string ShipName { get; set; }

        [Display(Name = "Ship Company")]
        [MaxLength(50)]
        public string ShipCompany { get; set; }

        [Display(Name = "Ship Address")]
        [MaxLength(50)]
        public string ShipAddress { get; set; }

        [Display(Name = "Ship Address2")]
        [MaxLength(50)]
        public string ShipAddress2 { get; set; }

        [Display(Name = "Ship City")]
        [MaxLength(50)]
        public string ShipCity { get; set; }

        [Display(Name = "Ship State")]
        [MaxLength(50)]
        public string ShipState { get; set; }

        [Display(Name = "Ship Postal")]
        [MaxLength(50)]
        public string ShipPostal { get; set; }

        [Display(Name = "Ship Country ID")]
        public int ShipCountryID { get; set; }

        [Display(Name = "Mail Name")]
        [MaxLength(50)]
        public string MailName { get; set; }

        [Display(Name = "Mail Company")]
        [MaxLength(50)]
        public string MailCompany { get; set; }

        [Display(Name = "Mail Address")]
        [MaxLength(50)]
        public string MailAddress { get; set; }

        [Display(Name = "Mail Address2")]
        [MaxLength(50)]
        public string MailAddress2 { get; set; }

        [Display(Name = "Mail City")]
        [MaxLength(50)]
        public string MailCity { get; set; }

        [Display(Name = "Mail State")]
        [MaxLength(50)]
        public string MailState { get; set; }

        [Display(Name = "Mail Postal")]
        [MaxLength(50)]
        public string MailPostal { get; set; }

        [Display(Name = "Mail Country ID")]
        public int MailCountryID { get; set; }

        [Display(Name = "Office Name")]
        [MaxLength(50)]
        public string OfficeName { get; set; }

        [Display(Name = "Office Company")]
        [MaxLength(50)]
        public string OfficeCompany { get; set; }

        [Display(Name = "Office Address")]
        [MaxLength(50)]
        public string OfficeAddress { get; set; }

        [Display(Name = "Office Address2")]
        [MaxLength(50)]
        public string OfficeAddress2 { get; set; }

        [Display(Name = "Office City")]
        [MaxLength(50)]
        public string OfficeCity { get; set; }

        [Display(Name = "Office State")]
        [MaxLength(50)]
        public string OfficeState { get; set; }

        [Display(Name = "Office Postal")]
        [MaxLength(50)]
        public string OfficePostal { get; set; }

        [Display(Name = "Office Country ID")]
        public int OfficeCountryID { get; set; }

        [Display(Name = "General Notes")]
        public string GeneralNotes { get; set; }

        public int TimeOffset { get; set; }

        public virtual ICollection<PriceBooks> PriceBooks { get; set; }
        public virtual ICollection<Franchise_Contacts> FranchiseContacts { get; set; }
    }
}