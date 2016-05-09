using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("TechnicianInformation")]
    public class TechnicianInformation
    {
        [Key]
        [ScaffoldColumn(false)]
        public long TechnicianInformationId { get; set; }

        [DisplayName("Questionnaire")]
        [Required(ErrorMessage = "Required")]
        public long QuestionnaireId { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings=true)]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string LastName { get; set; }

        [DisplayName("Home address")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string HomeAddress { get; set; }

        [DisplayName("City")]
        [Column("City")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string HomeCity { get; set; }

        [DisplayName("State")]
        [Column("StateId")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public int? StateId { get; set; }

        [DisplayName("ZIP")]
        [Column("ZipCode")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string HomeZipCode { get; set; }

        [DisplayName("Birthday")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/DD/YYYY}")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        [DisplayName("Email")]
        [Column("EmailAddress")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TechnicianEmail { get; set; }

        [DisplayName("Cell phone")]
        [Column("CellPhone")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TechnicianPhone { get; set; }

        [DisplayName("Emergency contact name")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string EmergencyContactName { get; set; }

        [DisplayName("Emergency Contact Phone")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string EmergencyContactPhone { get; set; }

        [DisplayName("Does this technician do PLUMBING work?")]
        [DefaultValue(false)]
        public bool PlumbingWork { get; set; }

        [DisplayName("Does this technician do HVAC work?")]
        [DefaultValue(false)]
        public bool HVACWork { get; set; }

        [DisplayName("How many years of experience does this technician have in his trade?")]
        public string PlumbingHVACWorkTime { get; set; }

        [DisplayName("Where did he/she receive their training?")]
        public string TrainingReceived { get; set; }

        [DisplayName("Upsell ability rating")]
        public int? LevelId { get; set; }

        [DisplayName("Regular daily schedule ")]
        public string RegularSchedule { get; set; }

        [DisplayName("Technican Bio – This will be sent to your customers.")]
        [StringLength(750, ErrorMessage = "Maximum characters allowed: 750")]
        public string TechnicianBio { get; set; }

        public virtual Questionnaire Questionnaire { get; set; }

        public virtual State State { get; set; }

        public virtual Level Level { get; set; }

        public virtual List<Hvac> Hvacs { get; set; }

        public virtual List<LicenseType> LicenseTypes { get; set; }

        public virtual List<Plumbing> Plumbings { get; set; }
    }
}
