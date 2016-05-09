using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_Image")]
    public class TaskImage
    {
        [Key]
        public int ImgID { get; set; }

        public int JobCodeID { get; set; }

        [MaxLength(500)]
        public string ImgName { get; set; }
    }
}