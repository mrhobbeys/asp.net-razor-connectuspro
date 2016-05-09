using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("View_LaborpricePerSubsectionID")]
    public class LaborSubSectionView : Labor
    {
        [Key]
        public int SubsectionID { get; set; }
    }
}