using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Address")]
    public class Address
    {
        [Key]
        [ScaffoldColumn(false)]
        public int AddressId { get; set; }

        [DisplayName("Business Information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("Type")]
        [Required(ErrorMessage = "Required")]
        public int AddressTypeId { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Required")]
        [StringLength(150, ErrorMessage = "Validation error - Length: 150")]
        public string Address1 { get; set; }

        [DisplayName("Address (Line 2)")]
        [StringLength(150, ErrorMessage = "Validation error - Length: 150")]
        public string Address2 { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string City { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Required")]
        public int StateId { get; set; }

        [DisplayName("ZIP")]
        [Required(ErrorMessage = "Required")]
        [StringLength(5, ErrorMessage = "Validation error - Length: 5")]
        public string ZipCode { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }

        public virtual AddressType AddressType { get; set; }

        public virtual State State { get; set; }
    }
}
