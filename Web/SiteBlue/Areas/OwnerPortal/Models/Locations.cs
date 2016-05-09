using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace SiteBlue.Areas.OwnerPortal.Models
{

    [Table("tbl_Locations")]
    public class Locations
    {
        [Key]
        [Display(Name = "LocationID")]
        public int LocationID { get; set; }

        [Required]
        [Display(Name = "BilltoCustomerID")]
        public int BilltoCustomerID { get; set; }

        [Required]
        [Display(Name = "ActvieCustomerID")]
        public int ActvieCustomerID { get; set; }

        [ForeignKey("ActvieCustomerID")]
        public virtual Customers BilltoCustomer { get; set; }

        [Display(Name = "Address")]
        [MaxLength(75)]
        public string Address { get; set; }

        [Display(Name = "City")]
        [MaxLength(50)]
        public string City { get; set; }

        [Display(Name = "State")]
        [MaxLength(2)]
        public string State { get; set; }

        [Display(Name = "PostalCode")]
        [MaxLength(15)]
        public string PostalCode { get; set; }

        [Display(Name = "Country")]
        [MaxLength(50)]
        public string Country { get; set; }

        [Display(Name = "GPS")]
        [MaxLength(50)]
        public string GPS { get; set; }

        [Display(Name = "Directions")]
        [MaxLength(2147483647)]
        public string Directions { get; set; }

        [Display(Name = "LocationNotes")]
        [MaxLength(2147483647)]
        public string LocationNotes { get; set; }

        [Display(Name = "LocationName")]
        [MaxLength(50)]
        public string LocationName { get; set; }

        [Display(Name = "LocationCompany")]
        [MaxLength(50)]
        public string LocationCompany { get; set; }

        [Display(Name = "TimeStamp")]
        public byte[] timestamp { get; set; }

    }
}