using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_CityServed")]
    public class CityServed
    {
        [Key]
        [ScaffoldColumn(false)]
        public int CityServedId { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Required")]
        public int LocationId { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "Required")]
        public string City { get; set; }

        public virtual Location Location { get; set; }
    }
}