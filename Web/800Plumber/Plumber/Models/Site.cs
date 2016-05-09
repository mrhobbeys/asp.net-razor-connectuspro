using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("view_Site")]
    public class Site
    {

        [Key]
        [ScaffoldColumn(false)]
        public int SiteId { get; set; }

        [DisplayName("Site")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string SiteName { get; set; }

        [DisplayName("URL")]
        [Required(ErrorMessage = "Required")]
        [StringLength(500, ErrorMessage = "Validation error - Length: 500")]
        public string Url { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public virtual List<Training> Trainings { get; set; }

    }
}