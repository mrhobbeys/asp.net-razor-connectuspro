using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("AdvertisedPhoneNumber")]
    public class AdvertisedPhoneNumber
    {
        [Key]
        [ScaffoldColumn(false)]
        public long AdvertisedPhoneNumberId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("Advertised phone number")]
        [Required(ErrorMessage = "Required")]
        public string AdvertisedPhone { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
