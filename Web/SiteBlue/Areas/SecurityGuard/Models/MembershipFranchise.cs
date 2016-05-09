using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SiteBlue.Areas.SecurityGuard.Models
{
    //[Table("tbl_Franchise")]
    [Table("ARCHIVE_tbl_Franchise")]
    public class MembershipFranchise_guard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Franchise ID")]
        public int FranchiseID { get; set; }

        [Display(Name = "Franchise Number")]
        [MaxLength(10)]
        public string FranchiseNumber { get; set; }

        public virtual List<UserFranchise_guard> UserFranchises { get; set; }
    }


    public class UserDetailViewModel
    {
        # region Properties

        [Display(Name = "User ID")]
        public Guid UserID { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        # endregion
    }

    public class UserRoleViewModel
    {
        # region Properties

        [Display(Name = "Role ID")]
        public Guid RoleID { get; set; }

        [Display(Name = "Role name")]
        public string RoleName { get; set; }

        # endregion
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}