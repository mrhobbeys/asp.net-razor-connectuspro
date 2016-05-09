using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    [Table("tbl_payroll")]
    public class Payroll
    {
        [Key]
        [Display(Name = "Payroll ID")]
        public int PayrollID { get; set; }

        [Display(Name = "Franchise ID")]
        public int FranchiseID { get; set; }

        [Display(Name = "Payroll Date")]
        public System.Nullable<System.DateTime> PayrollDate { get; set; }
        
        [Required]
        [Display(Name = "LockedYN")]
        public bool LockedYN { get; set; }

        [Required]
        [Display(Name = "GrossPay")]
        public float GrossPay { get; set; } 
    }
}