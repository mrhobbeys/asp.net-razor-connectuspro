using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using SecurityGuard.Services;
using SecurityGuard.Core.Attributes;
using routeHelpers = SecurityGuard.Core.RouteHelpers;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using SiteBlue.Controllers;
using SiteBlue.Areas.SecurityGuard.Models;


namespace SiteBlue.Areas.SecurityGuard.Controllers
{
    [Authorize(Roles = "SecurityGuard")]
    public partial class CompanyCodeController : Controller
    {
        //
        // GET: /SecurityGuard/CompanyCode/

        #region ctors
        private readonly IMembershipService membershipService;
        private readonly IRoleService roleService;
        private MembershipConnection memberShipContext = new MembershipConnection();

        public CompanyCodeController()
        {
            roleService = new RoleService(Roles.Provider);
            membershipService = new MembershipService(Membership.Provider);
        }
        #endregion

        #region Index Method and AutoComplete
        public virtual ActionResult Index(string searchterm, string filterby)
        {
            var viewModel = new ManageUsersViewModel { Users = null };

            if (!string.IsNullOrEmpty(searchterm))
            {
                string query = searchterm + "%";
                if (filterby == "email")
                {
                    viewModel.Users = membershipService.FindUsersByEmail(query);
                }
                else if (filterby == "username")
                {
                    viewModel.Users = membershipService.FindUsersByName(query);
                }
            }

            return View(viewModel);
        }

        public string AutoCompleteForUsers(string q, string filterby)
        {
            var viewModel = new ManageUsersViewModel { Users = null };
            if (!string.IsNullOrEmpty(q))
            {
                string query = q + "%";
                switch (filterby)
                {
                    case "email":
                        viewModel.Users = membershipService.FindUsersByEmail(query);
                        break;
                    case "username":
                        viewModel.Users = membershipService.FindUsersByName(query);
                        break;
                }
            }
            var str = viewModel.Users.Cast<MembershipUser>().Select(item => item.UserName).Aggregate("", (current, a) => current + "\n" + a);
            return str;
        }
        #endregion

        public ActionResult CompanyCodeToUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index");
            }

            MembershipUser user = membershipService.GetUser(User.Identity.Name);

            var AvailableCompanyCodeForUser = from c in memberShipContext.MembershipFranchise
                                              where !(from o in memberShipContext.UserFranchise
                                                      select o.FranchiseID).Contains(c.FranchiseID)
                                              select c;

            var AllCompanyCodeForUser = memberShipContext.UserFranchise.ToList();


            var GrantedCompanyCode = memberShipContext.UserFranchise.Where(uf => uf.UserId == (Guid)user.ProviderUserKey).ToList();


            var model = new GrantCompaniesToUser
            {
                UserName = username,
                AvailibleCompanies =
                    (string.IsNullOrEmpty(username)
                         ? new SelectList(AllCompanyCodeForUser)
                         : new SelectList(AvailableCompanyCodeForUser)),
                GrantedCompanies =
                    (string.IsNullOrEmpty(username)
                         ? new SelectList(new string[] { })
                         : new SelectList(GrantedCompanyCode))
            };

            return View(model);

        }



    }
}
