using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_Markup")]
    public class Markups
    {
        [Key]
        [Display(Name = "Markup ID")]
        public int MID { get; set; }

        public decimal Markup { get; set; }
        public decimal Lowerbound { get; set; }
        public decimal Upperbound { get; set; }
    }
}