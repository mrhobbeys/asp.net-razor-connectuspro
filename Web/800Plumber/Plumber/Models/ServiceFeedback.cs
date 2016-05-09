using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_ServiceFeedback")]
    public class ServiceFeedback
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ServiceFeedbackId { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Required")]
        public int LocationId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string Name { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Email { get; set; }

        [DisplayName("Comment")]
        [Required(ErrorMessage = "Required")]
        [StringLength(500, ErrorMessage = "Validation error - Length: 500")]
        public string Comment { get; set; }

        [DisplayName("Creation date")]
        [Required(ErrorMessage = "Required")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Status")]
        public int MessageStatusId { get; set; }

        public virtual Location Location { get; set; }

        public virtual MessageStatus MessageStatus { get; set; }
    }
}