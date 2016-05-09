using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_Parts")]
    public class Parts
    {
        [Key]
        [Display(Name = "Part ID")]
        public int PartID { get; set; }

        public int PriceBookID { get; set; }

        public int MasterPartID { get; set; }

        public decimal PartCost { get; set; }
        public decimal PartStdPrice { get; set; }
        public decimal PartMemberPrice { get; set; }
        public decimal PartAddonStdPrice { get; set; }
        public decimal PartAddonMemberPrice { get; set; }

        public double Markup { get; set; }
    }
}