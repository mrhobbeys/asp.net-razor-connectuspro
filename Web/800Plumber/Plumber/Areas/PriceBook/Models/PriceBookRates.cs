using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PriceBook_Rates")]
    public class PriceBookRates
    {
        [Key]
        [Display(Name="ID")]
        public int PBFactorID { get; set; }

        [ForeignKey("PriceBook")]
        public int PriceBookID { get; set; }

        public PriceBooks PriceBook { get; set; }

        public decimal MemberDiscountRate { get; set; }
        public decimal HourlyOverheadBE { get; set; }
        public decimal FirstHourProductivity { get; set; }
        public decimal AddOnHourProductivity { get; set; }
    }
}