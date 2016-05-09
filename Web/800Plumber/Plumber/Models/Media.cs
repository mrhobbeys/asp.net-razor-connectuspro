using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Media")]
    public class Media
    {
        [Key]
        [ScaffoldColumn(false)]
        public int MediaId { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        [DisplayName("Sub title")]
        public string SubTitle { get; set; }

        [DisplayName("Content")]
        public string Content { get; set; }

        [DisplayName("Media type")]
        public int? MediaType { get; set; }

        [DisplayName("Extension")]
        public int? MediaExtensionId { get; set; }

        [DisplayName("URL")]
        public string MediaPath { get; set; }

        [DisplayName("Publication date")]
        public DateTime? PublicationDate { get; set; }

        [DisplayName("Archived")]
        public bool IsArchived { get; set; }

        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; }

        public virtual MediaExtension MediaExtension { get; set; }

    }
}