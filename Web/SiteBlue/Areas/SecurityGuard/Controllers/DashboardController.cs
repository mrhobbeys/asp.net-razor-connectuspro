using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Services;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using SiteBlue.Controllers;
using SiteBlue.Areas.SecurityGuard.Models;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;

namespace SiteBlue.Areas.SecurityGuard.Controllers
{
    [Authorize(Roles = "SecurityGuard")]
    public partial class DashboardController : SecurityGuardBaseController
    {

        #region ctors

        private IMembershipService membershipService;
        private IRoleService roleService;
        private MembershipConnection memberShipContext = new MembershipConnection();

        public DashboardController()
        {
            this.roleService = new RoleService(Roles.Provider);
            this.membershipService = new MembershipService(Membership.Provider);
        }

        #endregion


        public virtual ActionResult Index()
        {
            DashboardViewModel viewModel = new DashboardViewModel();
            MembershipUser user = membershipService.GetUser(User.Identity.Name);
            int TotalCompanyCode = 0;
            int TotalUserByCompanyCode = 0;
            int companyCodeID = 0;
            string username = user.UserName;
            var isCorporate = User.IsInRole("Corporate");

            if (RouteData.Values["id"] != null)
            {
                companyCodeID = int.Parse(Convert.ToString(RouteData.Values["id"]));
            }
            else
            {
                companyCodeID = 56;
            }

            TotalUserByCompanyCode = memberShipContext.UserFranchise
                               .Where(f => f.FranchiseID == companyCodeID)
                               .OrderBy(f => f.UserId)
                               .Select(d => d.UserId).Count();

            viewModel.TotalUserCount = TotalUserByCompanyCode.ToString();
            viewModel.TotalUsersOnlineCount = membershipService.GetNumberOfUsersOnline().ToString();


            int[] assignedFranchises;
            var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == userId)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }
            TotalCompanyCode = memberShipContext.MembershipFranchise
                           .Where(f => assignedFranchises.Contains(f.FranchiseID))
                           .OrderBy(f => f.FranchiseNumber)
                           .Select(d => d.FranchiseNumber).Count();


            viewModel.TotalRolesCount = roleService.GetAllRoles().Length.ToString();
            viewModel.TotalCompanyCode = Convert.ToString(TotalCompanyCode);
            return View(viewModel);
        }

        //Running Code
        public ActionResult UserCompanyCode()
        {
            IMembershipService membershipService;
            IAuthenticationService authenticationService;
            membershipService = new MembershipService(Membership.Provider);
            authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());

            MembershipUser user = membershipService.GetUser(User.Identity.Name);
            var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            var isCorporate = User.IsInRole("Corporate");
            string username = user.UserName;
            int[] assignedFranchises;

            var DefaultCompamyName = default(String);
            var DefaultCompanyID = default(int);

            DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                  where g.FranchiseID == 56 && g.UserId == userId
                                  select g.Franchise.FranchiseNumber).FirstOrDefault();
            if (DefaultCompamyName == null)
            {
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
            }

            DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                where g.FranchiseID == 56 && g.UserId == userId
                                select g.Franchise.FranchiseID).FirstOrDefault();
            if (DefaultCompanyID == 0)
            {
                DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                    where g.UserId == userId
                                    select g.Franchise.FranchiseID).FirstOrDefault();
            }


            if (RouteData.Values["id"] != null)
            {
                int companyCodeID = int.Parse(Convert.ToString(RouteData.Values["id"]));
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == companyCodeID && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                    where g.FranchiseID == companyCodeID && g.UserId == userId
                                    select g.Franchise.FranchiseID).FirstOrDefault();
            }

            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == userId)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }

            var model = new GrantCompaniesToUser
            {
                UserName = username,
                GrantedCompanyCode =
                memberShipContext.MembershipFranchise
                               .Where(f => assignedFranchises.Contains(f.FranchiseID))
                               .OrderBy(f => f.FranchiseNumber)
                               .Select(d => new SelectListItem
                               {
                                   Text = d.FranchiseNumber,
                                   Value = SqlFunctions.StringConvert((double)d.FranchiseID)
                               })
                               .ToList(),
                defaultCompanyName = DefaultCompamyName,
                defaultCompanyID = DefaultCompanyID
            };
            return PartialView("CompanyCodeUser", model);


        }

        [Authorize]
        public JsonResult Franchises()
        {
            var membership = new MembershipService(Membership.Provider);
            var user = membership.GetUser(User.Identity.Name);
            var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            var isCorporate = User.IsInRole("Corporate");
            int[] assignedFranchises;

            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == userId)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }

            List<SelectListItem> franchises = memberShipContext.MembershipFranchise
                               .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                               .OrderBy(f => f.FranchiseNumber)
                               .Select(d => new SelectListItem
                               {
                                   Text = d.FranchiseNumber,
                                   Value = SqlFunctions.StringConvert((double)d.FranchiseID)
                               })
                               .ToList();

            return Json(franchises, JsonRequestBehavior.AllowGet);
        }

    }
}
