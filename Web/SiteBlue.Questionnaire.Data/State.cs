using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("State")]
    public class State
    {
        [Key]
        [ScaffoldColumn(false)]
        public int StateId { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Required")]
        public string StateName { get; set; }

        [DisplayName("State full name")]
        public string FullName { get; set; }
    }
}
