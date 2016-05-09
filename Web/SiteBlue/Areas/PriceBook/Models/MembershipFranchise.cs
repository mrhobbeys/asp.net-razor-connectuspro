using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_Franchise")]
    public class MembershipFranchise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Franchise ID")]
        public int FranchiseID { get; set; }

        [Display(Name = "Franchise Number")]
        [MaxLength(10)]
        public string FranchiseNumber { get; set; }

        public virtual List<UserFranchise> UserFranchises { get; set; }
    }
}