using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_SubSection")]
    public class SubSection
    {
        [Key]
        [Display(Name = "Subsection ID")]
        public int SubsectionID { get; set; }

        [ForeignKey("Section")]
        public int SectionID { get; set; }

        public Section Section { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50)]
        public string SubSectionName { get; set; }

        [Display(Name = "Active")]
        public bool ActiveYN { get; set; }

        public int MFlag { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}