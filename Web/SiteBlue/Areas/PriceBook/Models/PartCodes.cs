using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_Parts_Codes")]
    public class PartCodes
    {
        [Key]
        [Display(Name = "PartCode ID")]
        public string PartCodeID { get; set; }

        public string PartCode { get; set; }

        public bool ActiveYN { get; set; }
    }
}