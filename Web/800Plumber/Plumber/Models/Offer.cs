using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Offer")]
    public class Offer
    {
        [Key]
        [ScaffoldColumn(false)]
        public int OfferId { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        [DisplayName("Sub title")]
        public string SubTitle { get; set; }

        [DisplayName("Content")]
        [Required(ErrorMessage = "Required")]
        public string Content { get; set; }

        [DisplayName("Image URL")]
        public string ImageUrl { get; set; }

        public virtual List<LocationCoupon> LocationCoupons { get; set; }
    }
}