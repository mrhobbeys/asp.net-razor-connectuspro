using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_MessageStatus")]
    public class MessageStatus
    {
        [Key]
        [ScaffoldColumn(false)]
        public int MessageStatusId { get; set; }

        [DisplayName("Message status")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        [Required(ErrorMessage = "Required")]
        public string MessageStatusName { get; set; }

        [DisplayName("Description")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Description { get; set; }

        public virtual List<Appointment> Appointments { get; set; }

        public virtual List<CareerApplication> CareerApplications { get; set; }

        public virtual List<ServiceFeedback> ServiceFeedbacks { get; set; }

    }
}