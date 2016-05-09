using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_Management")]
    public class Management
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ManagementId { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "Required")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [Required(ErrorMessage = "Required")]
        public string LastName { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Required")]
        public string Description { get; set; }

        [DisplayName("Image")]
        public string ImageUrl { get; set; }

        [DisplayName("Alternative text")]
        public string AlternativeText { get; set; }
    }
}