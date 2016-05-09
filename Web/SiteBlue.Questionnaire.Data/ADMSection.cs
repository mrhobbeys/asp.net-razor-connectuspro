using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("adm_section")]
    public class ADMSection
    {
        [Key]
        [ScaffoldColumn(false)]
        public int SectionId { get; set; }

        [DisplayName("Section name")]
        [Required(ErrorMessage = "Required")]
        public string SectionName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Deleted (Y/N)")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
