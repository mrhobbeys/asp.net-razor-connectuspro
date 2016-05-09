using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("UserFranchise")]
    public class UserFranchise
    {
        [Key]
        [ScaffoldColumn(false)]
        public int UserFranchiseID { get; set; }

        [Required(ErrorMessage = "Required")]
        [DisplayName("User")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Required")]
        [DisplayName("Franchise")]
        public int FranchiseID { get; set; }

        public virtual MembershipFranchise Franchise { get; set; }

    }
}