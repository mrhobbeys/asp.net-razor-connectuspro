using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_LocationImage")]
    public class LocationImage
    {
        [Key]
        [ScaffoldColumn(false)]
        public int LocationImageId { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Required")]
        public int LocationId { get; set; }

        [DisplayName("Image Url")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string ImageUrl { get; set; }


        [DisplayName("Alternative Text")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string AlternativeText { get; set; }

        public virtual Location Location { get; set; }
    }
}