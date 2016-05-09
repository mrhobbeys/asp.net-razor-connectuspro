using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.SecurityGuard.Models;
using System.Web.Security;
using SecurityGuard.Services;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.LandingPage.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        EightHundredEntities db = new EightHundredEntities();
        private MembershipUser _currentUser;
        private readonly MembershipService membership = new MembershipService(Membership.Provider);

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _currentUser = _currentUser ?? membership.GetUser(User.Identity.Name);
        }

        [Authorize(Roles = "CompanyOwner,Corporate")]
        public ActionResult Index()
        {
            var isOwner = HttpContext.User.IsInRole("CompanyOwner");
            ViewBag.Locked = isOwner;

            var swapBranding = false;

            if (HttpContext.User.IsInRole("CompanyOwner"))
            {
                var userId = _currentUser == null ? Guid.Empty : (Guid)(_currentUser.ProviderUserKey ?? Guid.Empty);
                using (var ctx = new MembershipConnection())
                {
                    using (var db = new EightHundredEntities())
                    {
                        swapBranding = ctx.UserFranchise
                                                .Where(uf => uf.UserId == userId)
                                                .ToArray()
                                                .Select(uf => (from f in db.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).SingleOrDefault())
                                                .Any(f => f.FranchiseTypeID == 6);
                    }
                }
            }

            ViewBag.SwapBranding = swapBranding;

            return View();
        }

        public ActionResult Training()
        {
            var isOwner = HttpContext.User.IsInRole("CompanyOwner");
            ViewBag.Locked = isOwner;

            var swapBranding = false;

            List<Training> trainings;

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

            if (swapBranding)
                trainings = db.Trainings
                .Where(t => t.IsActive == true)
                .Where(t => t.SiteId == 2).ToList()
                .OrderBy(t => t.Title).ToList();
            else
                trainings = db.Trainings
                .Where(t => t.IsActive == true)
                .Where(t => t.SiteId == 1).ToList()
                .OrderBy(t => t.Title).ToList();

            ViewBag.SwapBranding = swapBranding;
            return View(trainings);
        }

        public ActionResult DeleteTraining(int id)
        {
            var training = db.Trainings.First(t => t.TrainingId == id);
            db.Trainings.DeleteObject(training);
            db.SaveChanges();
            return RedirectToAction("Training");
        }

    }
}
