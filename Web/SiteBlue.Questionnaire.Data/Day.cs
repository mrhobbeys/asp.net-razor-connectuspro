using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Day")]
    public class Day
    {
        [Key]
        [ScaffoldColumn(false)]
        public int DayId { get; set; }

        [DisplayName("Day")]
        [Required(ErrorMessage = "Required")]
        public string DayName { get; set; }
    }
}
