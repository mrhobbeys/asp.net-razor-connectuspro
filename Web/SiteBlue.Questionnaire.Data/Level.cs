using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Level")]
    public class Level
    {
        [Key]
        [ScaffoldColumn(false)]
        public int LevelId { get; set; }

        [DisplayName("Level")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string LevelName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public virtual List<TechnicianInformation> TechnicianInformations { get; set; }
    }
}
