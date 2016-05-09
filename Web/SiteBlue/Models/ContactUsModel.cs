using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace SiteBlue.Models
{
    public class ContactUsModel
    {
        //[Required(ErrorMessage = "You did not enter a Name.")]
        [DisplayName("Enter Your Name:")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "You did not enter a valid email address.", AllowEmptyStrings=true)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "You did not enter a valid email address.")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Enter Your E-mail:")]
        public string EmailAddress { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayName("Enter Your Phone:")]
        public string Phone { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayName("Enter Your Cell Phone: (Private Text Response)")]
        public string CellPhone { get; set; }

        [DisplayName("Enter Your State:")]
        public string State { get; set; }

        [DisplayName("Enter Your Subject:")]
        public string Subject { get; set; }

        //[Required(ErrorMessage = "You did not enter a Body.")]
        [DisplayName("Enter Your Message:")]
        public string Message { get; set; }
    }
}