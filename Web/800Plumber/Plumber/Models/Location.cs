using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Location")]
    public class Location
    {
        [Key]
        [ScaffoldColumn(false)]
        public int LocationId { get; set; }

        [DisplayName("Franchise")]
        public int? FranchiseID { get; set; }

        [Required(ErrorMessage = "Required")]
        [DisplayName("1-800-PLUMBER of")]
        public string LocationName { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("State")]
        public string State { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        [DisplayName("ZIP")]
        public string ZipCode { get; set; }

        [DisplayName("Local Phone Number:")]
        public string LocalPhoneNumber { get; set; }

        [DisplayName("License Information")]
        public string LicenseInformation { get; set; }

        [DisplayName("Google")]
        public string GoogleLink { get; set; }

        [DisplayName("Yahoo")]
        public string YahooLink { get; set; }

        [DisplayName("Facebook")]
        public string FacebookLink { get; set; }

        [DisplayName("Template")]
        public int TemplateId { get; set; }

        [DisplayName("Meta tag: Title")]
        [StringLength(500, ErrorMessage = "Validation error - Length: 500")]
        public string MetaTitle { get; set; }

        [DisplayName("Meta tag: Description")]
        [StringLength(500, ErrorMessage = "Validation error - Length: 500")]
        public string MetaDescription { get; set; }

        [DisplayName("Meta tag: Keywords")]
        [StringLength(500, ErrorMessage = "Validation error - Length: 500")]
        public string MetaKeywords { get; set; }

        [DisplayName("Archived")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public virtual Franchise Franchise { get; set; }

        public virtual Template Template { get; set; }

        public virtual List<CityServed> Cities { get; set; }

        public virtual List<LocationService> LocationServices { get; set; }

        public virtual List<LocationServiceCategory> LocationServiceCategories { get; set; }

        public virtual List<LocationCoupon> LocationCoupons { get; set; }

        public virtual List<LocationZip> LocationZips { get; set; }

        public virtual List<LocationImage> LocationImages { get; set; }
    }
}