using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SiteBlue.Areas.PriceBook.Models
{
    public class MembershipConnection : DbContext
    {
        public DbSet<UserFranchise> UserFranchise { get; set; }

        public DbSet<MembershipFranchise> MembershipFranchise { get; set; }
    }
}