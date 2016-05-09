using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("View_PB_LaborParts_per_Jobid")]
    public class LaborTaskView : Labor
    {
        [Key]
        public int JobCodeID { get; set; }
    }
}