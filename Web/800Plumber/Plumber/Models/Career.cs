using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Career")]
    public class Career
    {
        [Key]
        [ScaffoldColumn(false)]
        public int CareerId { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Title { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Required")]
        public string Description { get; set; }

        [DisplayName("Publication Date")]
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [DisplayName("Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; }

        [DisplayName("Deleted")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}