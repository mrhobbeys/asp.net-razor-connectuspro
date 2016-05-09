using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("City")]
    public class City
    {
        [Key]
        [ScaffoldColumn(false)]
        public int CityId { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Required")]
        public int StateId { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "Required")]
        public string CityName { get; set; }

        public virtual State State { get; set; }
    }
}
