using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_SpecialOffer")]
    public class SpecialOffer
    {
        [Key]
        [ScaffoldColumn(false)]
        public int SpecialOfferId { get; set; }

        [DisplayName("Special offer")]
        [Required(ErrorMessage = "Required")]
        public string SpecialOfferName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }
    }
}