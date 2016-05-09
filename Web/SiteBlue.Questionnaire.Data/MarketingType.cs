using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("MarketingType")]
    public class MarketingType
    {
        [Key]
        [ScaffoldColumn(false)]
        public long MarketingTypeId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("Marketing type")]
        [Required(ErrorMessage = "Required")]
        public string MarketingTypeName { get; set; }

        [DisplayName("Associated Referral/Tracking Code")]
        public string ReferralCode { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
