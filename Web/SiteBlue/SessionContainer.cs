using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using log4net;
using SiteBlue.Areas.SecurityGuard.Models;
using SiteBlue.Data.EightHundred;

namespace SiteBlue
{
    public class SessionContainer
    {
        private const string CONTAINER_KEY = "SiteBlueSessionContainer";
        private const string OWNER_ROLE = "CompanyOwner";
        private const string CORPORATE_ROLE = "Corporate";

        public MembershipUser User { get; private set; }
        public bool SwapBranding { get; private set; }
        public Guid UserKey { get; private set; }
        public IEnumerable<tbl_Franchise> Franchises { get; private set; }
        public tbl_Franchise DefaultFranchise { get; set; }
        public tbl_Franchise CurrentFranchise { get; set; }
        public bool IsCorporate { get; private set; }
        public bool IsOwner { get; private set; }
        public bool ShowInactiveFranchises { get; set; }
        public string[] UserRoles { get; set; }

        public static SessionContainer Create(MembershipUser user)
        {
            try
            {
                if (user == null)
                {
                    Remove();
                    return null;
                }

                HttpContext.Current.Session[CONTAINER_KEY] = new SessionContainer { User = user };
                var inst = GetInstance();

                inst.UserRoles = Roles.GetRolesForUser(user.UserName);
                inst.UserKey = (Guid)(user.ProviderUserKey ?? Guid.Empty);
                inst.Franchises = new tbl_Franchise[] { };
                inst.IsOwner = inst.UserRoles.Contains(OWNER_ROLE);
                inst.IsCorporate = inst.UserRoles.Contains(CORPORATE_ROLE);

                using (var db = new EightHundredEntities())
                {
                    IEnumerable<tbl_Franchise> franchises;
                    if (inst.IsCorporate)
                        franchises = db.tbl_Franchise;
                    else
                    {
                        using (var ctx = new MembershipConnection())
                        {
                            franchises = ctx.UserFranchise
                                .Where(uf => uf.UserId == inst.UserKey)
                                .ToArray()
                                .Select(uf => db.tbl_Franchise.SingleOrDefault(f => f.FranchiseID == uf.FranchiseID))
                                .Where(f => f != null);

                        }
                    }

                    inst.Franchises = franchises.OrderBy(f => f.FranchiseNUmber).Distinct().ToArray();
                }

                inst.SwapBranding = !inst.IsCorporate && inst.Franchises.Any(f => f.FranchiseTypeID == 6);
                inst.DefaultFranchise = inst.Franchises.FirstOrDefault(f => f.FranchiseStatusID == 7 || inst.ShowInactiveFranchises);
                inst.CurrentFranchise = inst.DefaultFranchise;

                return inst;
            }
            catch (Exception ex)
            {
                Logger.Log("Error loading the user's session container info.", ex, LogLevel.Error);
                Remove();
                return null;
            }
        }

        public static void Remove()
        {
            if (HttpContext.Current.Session != null)
                HttpContext.Current.Session.Remove(CONTAINER_KEY);
        }

        public static SessionContainer GetInstance()
        {
            var inst = HttpContext.Current.Session[CONTAINER_KEY] as SessionContainer;

            if (inst == null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var user = Membership.GetUser(HttpContext.Current.User.Identity.Name);
                inst = Create(user);
            }

            return inst;
        }
    }
}