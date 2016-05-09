using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("AddressType")]
    public class AddressType
    {
        [Key]
        [ScaffoldColumn(false)]
        public int AddressTypeId { get; set; }

        [DisplayName("Type")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Valisation error - Length: 50")]
        public string AddressTypeName { get; set; }
        
        [DisplayName("Description")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Valisation error - Length: 255")]
        public string Description { get; set; }

        public virtual List<Address> Addresses { get; set; }
    }
}
