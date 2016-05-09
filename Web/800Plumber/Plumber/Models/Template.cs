using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Template")]
    public class Template
    {
        [Key]
        [ScaffoldColumn(false)]
        public int TemplateId { get; set; }

        [DisplayName("Template name")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string TemplateName { get; set; }

        [DisplayName("Description")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Description { get; set; }

        [DisplayName("Deleted")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public virtual List<Location> Locations { get; set; }
    }
}