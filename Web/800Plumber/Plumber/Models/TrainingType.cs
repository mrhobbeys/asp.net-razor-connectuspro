using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("view_TrainingType")]
    public class TrainingType
    {
        [Key]
        [ScaffoldColumn(false)]
        public int TrainingTypeId { get; set; }

        [DisplayName("Training type")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string TrainingTypeName { get; set; }

        [DisplayName("Description")]
        [StringLength(500, ErrorMessage = "Validation error - Length: 500")]
        public string Description { get; set; }

        [DisplayName("Icon")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Icon { get; set; }

        [DisplayName("Link text")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ViewText { get; set; }

        [DefaultValue(true)]
        public bool? IsActive { get; set; }

        public virtual List<Training> Trainings { get; set; }
    }
}