using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace SiteBlue.Areas.Dispatch.Models
{
    [Table("tbl_User")]
    public class User_dispatch
    {
        [Key]
        [Display(Name = "UserID")]
        public int UserID { get; set; }

        [Display(Name = "UserName")]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Display(Name = "UserPassword")]
        [MaxLength(15)]
        public string UserPassword { get; set; }

        [Required]
        [Display(Name = "ActiveYN")]
        public bool ActiveYN { get; set; }

        [Required]
        [Display(Name = "FranchiseID")]
        public int FranchiseID { get; set; }

        [Required]
        [Display(Name = "ProgramAccessID")]
        public int ProgramAccessID { get; set; }

        [Display(Name = "DisplayName")]
        [MaxLength(50)]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "CurrentStatus")]
        public int CurrentStatus { get; set; }

        [Display(Name = "EPIName")]
        [MaxLength(100)]
        public string EPIName { get; set; }

        [Required]
        [Display(Name = "TimeStamp")]
        public byte[] timestamp { get; set; } 
    }
}