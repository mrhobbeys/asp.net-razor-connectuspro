using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_TerritoryOwner")]
    public class TerritoryOwner
    {
        [Key]
        [ScaffoldColumn(false)]
        public int TerritoryOwnerId { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string Title { get; set; }

        [DisplayName("Content")]
        [Required(ErrorMessage = "Required")]
        public string Content { get; set; }

        [DisplayName("Image")]
        public string ImageUrl { get; set; }
    }
}