using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("view_Franchise")]
    public class Franchise
    {
        [Key]
        [ScaffoldColumn(false)]
        public int FranchiseID { get; set; }

        [Column("FranchiseNUmber")]
        [DisplayName("Franchise number")]
        [StringLength(10, ErrorMessage = "Validation error - Length: 10")]
        public string FranchiseNumber { get; set; }

        [DisplayName("Prospect")]
        [Required(ErrorMessage = "Required")]
        public int ProspectID { get; set; }

        [DisplayName("Owner")]
        [Required(ErrorMessage = "Required")]
        public int OwnerID { get; set; }

        [DisplayName("Concept")]
        [Required(ErrorMessage = "Required")]
        public int ConceptID { get; set; }

        [DisplayName("Franchise type")]
        [Required(ErrorMessage = "Required")]
        public int FranchiseTypeID { get; set; }

        [DisplayName("Franchise status")]
        [Required(ErrorMessage = "Required")]
        public int FranchiseStatusID { get; set; }

        [DisplayName("Sale rep.")]
        [Required(ErrorMessage = "Required")]
        public int SalesRepID { get; set; }

        [DisplayName("Support rep.")]
        [Required(ErrorMessage = "Required")]
        public int SupportRepID { get; set; }

        [DisplayName("Division")]
        [Required(ErrorMessage = "Required")]
        public int DivisionID { get; set; }

        [DisplayName("Start date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("Renewal date")]
        public DateTime? RenewalDate { get; set; }

        [DisplayName("Website")]
        [StringLength(150, ErrorMessage = "Validation error - Length: 150")]
        public string WebSite { get; set; }

        [DisplayName("E-mail")]
        [StringLength(100, ErrorMessage = "Validation error - Length: 100")]
        public string EMail { get; set; }

        [DisplayName("Legal name")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string LegalName { get; set; }

        [DisplayName("Legal address")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string LegalAddress { get; set; }

        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string LegalAddress2 { get; set; }

        [DisplayName("Legal city")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string LegalCity { get; set; }

        [DisplayName("Legal state")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string LegalState { get; set; }

        [DisplayName("Legal postal")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string LegalPostal { get; set; }

        [DisplayName("Legal country")]
        [Required(ErrorMessage = "Required")]
        public int LegalCountryID { get; set; }

        [DisplayName("Ship name")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ShipName { get; set; }

        [DisplayName("Ship company")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ShipCompany { get; set; }

        [DisplayName("Ship address")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ShipAddress { get; set; }

        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ShipAddress2 { get; set; }

        [DisplayName("Ship city")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ShipCity { get; set; }

        [DisplayName("Ship state")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ShipState { get; set; }

        [DisplayName("Ship postal")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ShipPostal { get; set; }

        [DisplayName("Ship country")]
        [Required(ErrorMessage = "Required")]
        public int ShipCountryID { get; set; }

        [DisplayName("Mail name")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string MailName { get; set; }

        [DisplayName("Mail company")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string MailCompany { get; set; }

        [DisplayName("Mail address")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string MailAddress { get; set; }

        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string MailAddress2 { get; set; }

        [DisplayName("Mail city")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string MailCity { get; set; }

        [DisplayName("Mail state")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string MailState { get; set; }

        [DisplayName("Mail postal")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string MailPostal { get; set; }

        [DisplayName("Mail country")]
        [Required(ErrorMessage = "Required")]
        public int MailCountryID { get; set; }

        [DisplayName("Office name")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string OfficeName { get; set; }

        [DisplayName("Office company")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string OfficeCompany { get; set; }

        [DisplayName("Office address")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string OfficeAddress { get; set; }

        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string OfficeAddress2 { get; set; }

        [DisplayName("Office city")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string OfficeCity { get; set; }

        [DisplayName("Office state")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string OfficeState { get; set; }

        [DisplayName("Office postal")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string OfficePostal { get; set; }

        [DisplayName("Office country")]
        [Required(ErrorMessage = "required")]
        public int OfficeCountryID { get; set; }

        [DisplayName("General note")]
        public string GeneralNotes { get; set; }

        [DisplayName("Time offset")]
        [Required(ErrorMessage = "Required")]
        public int TimeOffset { get; set; }

        public byte[] TimeStamp { get; set; }

        public virtual List<Location> Locations { get; set; }

        public virtual List<ZipList> PostalCodes { get; set; }

        public virtual List<FranchiseService> FranchiseServices { get; set; }

        //public virtual List<DispatchDBA> DispatchDBAs { get; set; }

        //public virtual List<ServiceWindows> ServiceWindows { get; set; }
    }
}