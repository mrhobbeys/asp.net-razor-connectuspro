using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SiteBlue.Areas.Dispatch.Models
{
    [Table("tbl_technician_tasks")]
    public class Techniciantasks
    {
        [Key]
        [Display(Name = "tasksId")]
        public int tasksId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Details")]
        public string Details { get; set; }

        [Required]
        [Display(Name = "FromDate")]
        public DateTime FromDate { get; set; }

        [Required]
        [Display(Name = "ToDate")]
        public DateTime ToDate { get; set; }

        [Required]
        [Display(Name = "Tags")]
        public string Tags { get; set; }

        [Required]
        [Display(Name = "Custom")]
        public string Custom { get; set; }
    }
}