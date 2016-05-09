using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("view_services")]
    public class Services
    {
        [Key]
        [ScaffoldColumn(false)]
        //[Column("ServiceID")]
        public int ServiceID { get; set; }

        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        [Required(ErrorMessage = "Required")]
        [DisplayName("Service name")]
        //[Column("ServiceName")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Required")]
        [DefaultValue(true)]
        //[Column("ActiveYN")]
        public bool ActiveYN { get; set; }

        public virtual List<FranchiseService> FranchiseServices { get; set; }
    }
}