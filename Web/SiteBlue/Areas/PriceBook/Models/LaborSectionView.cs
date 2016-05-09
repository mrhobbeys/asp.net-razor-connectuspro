using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("View_PB_LaborpricePerSectionID")]
    public class LaborSectionView : Labor
    {
        [Key]
        public int SectionID { get; set; }
    }
}