using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.SecurityGuard.Models
{
    public class UserDetailViewModelOld
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

    public class UserRoleViewModelOld
    {
        # region Properties

        [Display(Name = "Role ID")]
        public Guid RoleID { get; set; }

        [Display(Name = "Role name")]
        public string RoleName { get; set; }

        # endregion
    }

}