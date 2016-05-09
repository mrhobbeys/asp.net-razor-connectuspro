using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_LocationServiceCategory")]
    public class LocationServiceCategory
    {
        [Key]
        [ScaffoldColumn(false)]
        public int LocationServiceCategoryId { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "required")]
        public int LocationId { get; set; }

        [DisplayName("Service")]
        [Required(ErrorMessage = "Required")]
        public int ServiceCategoryId { get; set; }

        public virtual Location Location { get; set; }

        public virtual ServiceCategory ServiceCategory { get; set; }

    }
}