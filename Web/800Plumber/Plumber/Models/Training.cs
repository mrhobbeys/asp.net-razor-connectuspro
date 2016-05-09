using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("view_Training")]
    public class Training
    {
        [Key]
        [ScaffoldColumn(false)]
        public int TrainingId { get; set; }

        [DisplayName("Site")]
        [Required(ErrorMessage = "Required")]
        public int SiteId { get; set; }

        [DisplayName("Training type")]
        [Required(ErrorMessage = "Required")]
        public int TrainingTypeId { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Title { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Navigate URL")]
        [StringLength(500, ErrorMessage = "Validation error - Length: 500")]
        public string NavigateUrl { get; set; }

        [DefaultValue(true)]
        public bool? IsActive { get; set; }

        public virtual Site Site { get; set; }

        public virtual TrainingType TrainingType { get; set; }
    }
}