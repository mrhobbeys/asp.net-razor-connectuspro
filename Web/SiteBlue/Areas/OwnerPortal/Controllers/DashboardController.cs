using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class DashboardController : Controller
    {
        //
        // GET: /OwnerPortal/Dashboard/

        public ActionResult Index()
        {
            return View();
        }

    }
}
