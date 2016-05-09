using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("ServicePlan")]
    public class ServicePlan
    {
        [Key]
        [ScaffoldColumn(false)]
        public long ServicePlanId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("Service plan")]
        [Required(ErrorMessage = "Required")]
        public string ServicePlanName { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
