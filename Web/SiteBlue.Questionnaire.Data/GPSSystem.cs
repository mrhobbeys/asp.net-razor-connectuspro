using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("GPSSystem")]
    public class GPSSystem
    {
        [Key]
        [ScaffoldColumn(false)]
        public int GPSSystemId { get; set; }

        [DisplayName("GPS system")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string GPSSystemName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public virtual List<BusinessInformation> BusinessInformations { get; set; }
    }
}
