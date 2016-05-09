using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_LocationService")]
    public class LocationService
    {
        [Key]
        [ScaffoldColumn(false)]
        public int LocationServiceId { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Required")]
        public int LocationId { get; set; }

        [DisplayName("Service")]
        [Required(ErrorMessage = "Required")]
        public int ServiceId { get; set; }

        public virtual Location Location { get; set; }

        public virtual Service Service { get; set; }
    }
}