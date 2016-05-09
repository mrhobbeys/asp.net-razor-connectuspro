using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SiteBlue.Areas.SecurityGuard.Models
{
    public class MembershipConnection : DbContext
    {
        public DbSet<UserFranchise_guard> UserFranchise { get; set; }

        public DbSet<MembershipFranchise_guard> MembershipFranchise { get; set; }
    }
}