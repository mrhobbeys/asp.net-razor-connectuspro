using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,ContentManager")]
    public class FranchiseController : Controller
    {
        private PlumberContext db = new PlumberContext();

        public ActionResult Index()
        {
            return View(db.Franchise.ToList());
        }

    }
}
