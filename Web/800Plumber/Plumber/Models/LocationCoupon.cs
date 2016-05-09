using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_LocationCoupon")]
    public class LocationCoupon
    {
        [Key]
        [ScaffoldColumn(false)]
        public int LocationCouponId { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Required")]
        public int LocationId { get; set; }

        [DisplayName("Coupon")]
        [Required(ErrorMessage = "Required")]
        public int OfferId { get; set; }

        public virtual Location Location { get; set; }

        public virtual Offer Offer { get; set; }
    }
}