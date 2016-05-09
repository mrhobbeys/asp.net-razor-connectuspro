using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("LicenseType")]
    public class LicenseType
    {
        [Key]
        [ScaffoldColumn(false)]
        public long LicenseTypeId { get; set; }

        [DisplayName("Technician information")]
        [Required(ErrorMessage = "Required")]
        public long TechnicianInformationId { get; set; }

        [DisplayName("License type")]
        [Required(ErrorMessage = "Required")]
        public string LicenseTypeName { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual TechnicianInformation TechnicianInformation { get; set; }
    }
}
