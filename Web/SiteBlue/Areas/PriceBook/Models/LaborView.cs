using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("View_PB_LaborPricePerJobcodeid")]
    public class LaborView
    {
        [Key]
        public int JobCodeID { get; set; }

        public decimal LaborPercentage { get; set; }
    }
}