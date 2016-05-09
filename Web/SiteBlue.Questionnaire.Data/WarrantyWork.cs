using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("WarrantyWork")]
    public class WarrantyWork
    {
        [Key]
        [ScaffoldColumn(false)]
        public long WarrantyWorkId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("Warranty work")]
        [Required(ErrorMessage = "Required")]
        public string WarrantyWorkName { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
