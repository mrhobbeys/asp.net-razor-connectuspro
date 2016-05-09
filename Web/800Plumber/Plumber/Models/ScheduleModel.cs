using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    public class ScheduleModel
    {
        [DisplayName("ZIP or POSTAL")]
        public string ZipCode { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "You did not enter a valid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DisplayName("Preferred date")]
        [DataType(DataType.Date)]
        public DateTime? PreferredDate { get; set; }

        [DisplayName("Preferred time")]
        public string PreferredTime { get; set; }

        [DisplayName("Type of service")]
        public string TypeOfService { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        [DisplayName("Enter Your Subject:")]
        public string Subject { get; set; }
    }
}