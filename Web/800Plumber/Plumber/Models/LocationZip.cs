using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_LocationZip")]
    public class LocationZip
    {
        [Key]
        [ScaffoldColumn(false)]
        public int LocationZipId { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Required")]
        public int LocationId { get; set; }

        [DisplayName("ZIP Code")]
        [Required(ErrorMessage = "Required")]
        [StringLength(15, ErrorMessage = "Validation error - Length: 15")]
        public string ZipCode { get; set; }

        public virtual Location Location { get; set; }
    }
}