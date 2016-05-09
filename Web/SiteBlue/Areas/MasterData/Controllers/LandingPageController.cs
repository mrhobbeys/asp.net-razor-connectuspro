using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Data.EightHundred;
using SiteBlue.Areas.PriceBook.Models;
using System.Web.Security;
using MembershipConnection = SiteBlue.Areas.SecurityGuard.Models.MembershipConnection;
using SecurityGuard.Services;

namespace SiteBlue.Areas.MasterData.Controllers
{
    [Authorize]
    public class LandingPageController : Controller
    {
        private MembershipUser _currentUser;
        private readonly MembershipService membership = new MembershipService(Membership.Provider);
        //
        // GET: /MasterData/Home/
        EightHundredEntities db = new EightHundredEntities();
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _currentUser = _currentUser ?? membership.GetUser(User.Identity.Name);
        }
        
        public ActionResult Index()
        {
            var isOwner = HttpContext.User.IsInRole("CompanyOwner");
            ViewBag.Locked = isOwner;

            var swapBranding = false;

            if (isOwner)
            {

                var userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
                using (var ctx = new MembershipConnection())
                {
                    swapBranding = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .ToArray()
                                            .Select(uf => (from f in db.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).SingleOrDefault())
                                            .Any(f => f.FranchiseTypeID == 6);
                }
            }

            ViewBag.SwapBranding = swapBranding;

            return View();
        }

        public ActionResult MyCalls()
        {
           
            var isOwner = HttpContext.User.IsInRole("CompanyOwner");
            ViewBag.Locked = isOwner;

            var swapBranding = false;

            if (isOwner)
            {

                var userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
                using (var ctx = new MembershipConnection())
                {
                    swapBranding = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .ToArray()
                                            .Select(uf => (from f in db.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).SingleOrDefault())
                                            .Any(f => f.FranchiseTypeID == 6);
                }
            }

            ViewBag.SwapBranding = swapBranding;

            return View();
        }
        public ActionResult DailyBudget()
        {
            var isOwner = HttpContext.User.IsInRole("CompanyOwner");
            ViewBag.Locked = isOwner;

            var swapBranding = false;

            if (isOwner)
            {

                var userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
                using (var ctx = new MembershipConnection())
                {
                    swapBranding = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .ToArray()
                                            .Select(uf => (from f in db.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).SingleOrDefault())
                                            .Any(f => f.FranchiseTypeID == 6);
                }
            }

            ViewBag.SwapBranding = swapBranding;

            return View();
        }

        public ActionResult MyFinances()
        {
            var isOwner = HttpContext.User.IsInRole("CompanyOwner");
            ViewBag.Locked = isOwner;

            var swapBranding = false;

            if (isOwner)
            {

                var userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
                using (var ctx = new MembershipConnection())
                {
                    swapBranding = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .ToArray()
                                            .Select(uf => (from f in db.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).SingleOrDefault())
                                            .Any(f => f.FranchiseTypeID == 6);
                }
            }

            ViewBag.SwapBranding = swapBranding;

            return View();
        }

    }
}
