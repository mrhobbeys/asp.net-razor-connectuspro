using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Dba")]
    public class Dba
    {
        [Key]
        [ScaffoldColumn(false)]
        public long DbaId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("DBA")]
        [Required(ErrorMessage = "Required")]
        public string DbaName { get; set; }

        [DisplayName("Advertised phone number")]
        public string AdvertisedPhoneNumber { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
