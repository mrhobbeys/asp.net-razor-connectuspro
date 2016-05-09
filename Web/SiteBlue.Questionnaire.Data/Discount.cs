using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        [ScaffoldColumn(false)]
        public long DiscountId { get; set; }

        [DisplayName("Business information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("Discount")]
        [Required(ErrorMessage = "Required")]
        public string DiscountName { get; set; }

        [DisplayName("Expiration date")]
        public DateTime? ExpirationDate { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
