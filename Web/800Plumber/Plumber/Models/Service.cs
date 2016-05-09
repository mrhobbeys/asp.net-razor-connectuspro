using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Service")]
    public class Service
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ServiceId { get; set; }

        [DisplayName("Service")]
        [Required(ErrorMessage = "Required")]
        public string ServiceName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Category")]
        public int? ServiceCategoryId { get; set; }

        public virtual ServiceCategory ServiceCategory { get; set; }

        public virtual List<LocationService> LocationServices { get; set; }
    }
}