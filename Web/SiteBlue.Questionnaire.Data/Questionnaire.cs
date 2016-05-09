using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Questionnaire")]
    public class Questionnaire
    {
        [Key]
        [ScaffoldColumn(false)]
        public long QuestionnaireId { get; set; }

        [DisplayName("User")]
        public Nullable<Guid> UserId { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Creation date")]
        public DateTime? CreationDate { get; set; }

        [DisplayName("Last modification date")]
        public DateTime LastModificationDate { get; set; }

        public virtual Status Status { get; set; }

        public virtual List<AccountingInformation> AccountingInformations { get; set; }

        public virtual List<BusinessInformation> BusinessInformations { get; set; }

        public virtual List<OwnerInformation> OwnerInformations { get; set; }

        public virtual List<TechnicianInformation> TechnicianInformations { get; set; }
    }
}
