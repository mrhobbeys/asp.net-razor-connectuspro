using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PriceBook")]
    public class PriceBooks
    {
        [Key]
        [Display(Name = "PriceBook ID")]
        public int PriceBookID { get; set; }

        [Display(Name = "Franchise ID")]
        [ForeignKey("Franchise")]
        public int FranchiseID { get; set; }

        public virtual Franchise Franchise { get; set; }

        [Display(Name = "Concept ID")]
        public int ConceptID { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime? EffectiveDate { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50)]
        public string BookName { get; set; }

        [Display(Name = "Comments")]
        public string BookComments { get; set; }

        [Display(Name = "Active")]
        public bool ActiveBookYN { get; set; }

        public double? MemberPricePercent { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
    }
}