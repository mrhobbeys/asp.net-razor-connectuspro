using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("OfficePersonnel")]
    public class OfficePersonnel
    {
        [Key]
        [ScaffoldColumn(false)]
        public int OfficePersonnelId { get; set; }

        [DisplayName("Business Information")]
        [Required(ErrorMessage = "Required")]
        public long BusinessInformationId { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string FirstName { get; set; }
                
        [DisplayName("Last name")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string LastName { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Title { get; set; }
                
        [DisplayName("Cell Phone")]
        [Required(ErrorMessage = "Required")]
        [StringLength(20, ErrorMessage = "Validation error - Length: 20")]
        public string CellPhone { get; set; }
        
        [DisplayName("Office Phone")]
        [Required(ErrorMessage = "Required")]
        [StringLength(20, ErrorMessage = "Validation error - Length: 20")]
        public string OfficePhone { get; set; }

        public virtual BusinessInformation BusinessInformation { get; set; }
    }
}
