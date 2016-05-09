using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    public class NotYetArrivedModel
    {
        [DisplayName("ZIP or POSTAL")]
        public string ZipCode { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Email")]
        //[Required(ErrorMessage = "Required")]
        //[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "You did not enter a valid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}