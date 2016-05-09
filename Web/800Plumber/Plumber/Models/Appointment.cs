using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Appointment")]
    public class Appointment
    {

        [Key]
        [ScaffoldColumn(false)]
        public int AppointmentId { get; set; }

        [DisplayName("ZIP OR POSTAL")]
        [Required(ErrorMessage = "Required")]
        [StringLength(15, ErrorMessage = "Validation error - Length: 15")]
        public string PostalCode { get; set; }

        [DisplayName("Name")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string Name { get; set; }

        [DisplayName("Address")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Address { get; set; }

        [DisplayName("City")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string City { get; set; }

        [DisplayName("State")]
        [StringLength(2, ErrorMessage = "Validation error - Length: 2")]
        public string State { get; set; }

        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Email { get; set; }

        [DisplayName("Phone")]
        [StringLength(20, ErrorMessage = "Validation error - Length: 20")]
        public string Phone { get; set; }

        [DisplayName("Preferred Date")]
        [DataType(DataType.Date)]
        public DateTime? PreferredDate { get; set; }

        [DisplayName("Preferred Time")]
        public string PreferredTime { get; set; }

        [DisplayName("Type of Service")]
        public string TypeOfService { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public int MessageStatusId { get; set; }

        public virtual MessageStatus MessageStatus { get; set; }
    }
}