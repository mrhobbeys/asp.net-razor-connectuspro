using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("ZipCode")]
    public class ZipCode
    {
        [Key]
        [ScaffoldColumn(false)]
        public long ZipCodeId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("ZIP")]
        [Required(ErrorMessage = "Required")]
        public string ZipCodeNumber { get; set; }

        public double? TaxRate { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
