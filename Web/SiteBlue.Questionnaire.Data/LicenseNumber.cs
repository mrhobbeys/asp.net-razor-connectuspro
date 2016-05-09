using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("LicenseNumber")]
    public class LicenseNumber
    {
        [Key]
        [ScaffoldColumn(false)]
        public long LicenseNumberId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        [ForeignKey("BusinessInformation")]
        public long BusinnessInformationId { get; set; }

        [DisplayName("License number")]
        [Required(ErrorMessage = "Required")]
        public string LicenseNumberName { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        [DisplayName("Is this number required on your invoices?")]
        [DefaultValue(false)]
        public bool InvoicesRequired { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
