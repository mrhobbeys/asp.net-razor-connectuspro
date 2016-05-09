using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Testimonial")]
    public class Testimonial
    {
        [Key]
        [ScaffoldColumn(false)]
        public int TestimonialId { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        [StringLength(250, ErrorMessage = "Validation error - Length: 250")]
        public string Title { get; set; }

        [DisplayName("Content")]
        public string Content { get; set; }

        [DisplayName("Media type")]
        public int? MediaType { get; set; }

        [DisplayName("Extension")]
        public int? MediaExtensionId { get; set; }

        [DisplayName("URL")]
        public string MediaPath { get; set; }

        [DisplayName("Publication date")]
        [DataType(DataType.DateTime)]
        public DateTime? PublicationDate { get; set; }

        [DisplayName("Archived")]
        [DefaultValue(false)]
        public bool IsArchived { get; set; }

        [DisplayName("Deleted")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public virtual MediaExtension MediaExtension { get; set; }
    }
}