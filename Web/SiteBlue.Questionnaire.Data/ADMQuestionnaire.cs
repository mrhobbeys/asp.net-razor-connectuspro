using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("adm_Questionnaire")]
    public class ADMQuestionnaire
    {
        [Key]
        [ScaffoldColumn(false)]
        public int QuestionnaireId { get; set; }

        [DisplayName("Section")]
        [Required(ErrorMessage = "Required")]
        public int SectionId { get; set; }

        [DisplayName("Question reference")]
        [Required(ErrorMessage = "Required")]
        public string QuestionId { get; set; }

        [DisplayName("Question")]
        public string Question { get; set; }

        [DisplayName("Display question (Y/N)")]
        [DefaultValue(true)]
        public bool IsDisplayed { get; set; }

        [DisplayName("Deleted (Y/N)")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
