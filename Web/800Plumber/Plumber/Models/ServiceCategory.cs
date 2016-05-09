using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_ServiceCategory")]
    public class ServiceCategory
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ServiceCategoryId { get; set; }

        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        [Required(ErrorMessage = "Required")]
        [DisplayName("Service name")]
        public string ServiceCategoryName { get; set; }

        [Required(ErrorMessage = "Required")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public virtual List<Service> Services { get; set; }
    }
}