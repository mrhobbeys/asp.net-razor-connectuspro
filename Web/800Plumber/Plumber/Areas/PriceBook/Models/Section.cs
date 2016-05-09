using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_Section")]
    public class Section
    {
        [Key]
        [Display(Name = "Section ID")]
        public int SectionID { get; set; }

        [Display(Name="Price Book ID")]
        [ForeignKey("PriceBook")]
        public int PriceBookID { get; set; }

        public PriceBooks PriceBook { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50)]
        public string SectionName { get; set; }

        [Display(Name = "Active")]
        public bool ActiveYN { get; set; }

        public int MFlag { get; set; }

        public virtual ICollection<SubSection> SubSections { get; set; }
    }
}