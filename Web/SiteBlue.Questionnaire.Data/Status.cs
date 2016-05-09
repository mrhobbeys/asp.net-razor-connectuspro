using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Status")]
    public class Status
    {
        [Key]
        [ScaffoldColumn(false)]
        public int StatusId { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "Required")]
        public string StatusName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; }

        public virtual List<Questionnaire> Questionnaires { get; set; }
    }
}
