using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("View_PB_LaborpriceperPricebookid")]
    public class LaborPriceBookView : Labor
    {
        [Key]
        public int PriceBookID { get; set; }
    }
}