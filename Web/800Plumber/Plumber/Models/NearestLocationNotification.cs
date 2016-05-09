using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("NearestLocationNotification")]
    public class NearestLocationNotification
    {
        [Key]
        [ScaffoldColumn(false)]
        public int NearestLocationNotificationId { get; set; }

        [DisplayName("Service")]
        public int? ServiceId { get; set; }

        [DisplayName("Subject")]
        public string Subject { get; set; }

        [DisplayName("ZIP")]
        [Required(ErrorMessage = "Required")]
        public string ZipCode { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Required")]
        public string Email { get; set; }

        public virtual Service Service { get; set; }

    }
}