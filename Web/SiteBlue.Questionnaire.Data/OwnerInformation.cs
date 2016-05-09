using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("OwnerInformation")]
    public class OwnerInformation
    {
        [Key]
        [ScaffoldColumn(false)]
        public long OwnerInformationId { get; set; }

        [DisplayName("Questionnaire")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public long QuestionnaireId { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string LastName { get; set; }

        [DisplayName("Name of the business")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string BusinessName { get; set; }

        [DisplayName("Home address")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string HomeAddress { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string City { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Required")]
        public int? StateId { get; set; }

        [DisplayName("ZIP")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string ZipCode { get; set; }

        [DisplayName("Cell phone")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string CellPhone { get; set; }

        [DisplayName("Home phone")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string HomePhone { get; set; }

        [DisplayName("Email address")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string EmailAddress { get; set; }

        public virtual Questionnaire Questionnaire { get; set; }

        public virtual State State { get; set; }
    }
}
