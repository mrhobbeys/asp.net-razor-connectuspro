using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Plumber.Controllers
{
    public class MembersController : Controller
    {
        //
        // GET: /Membership/

        public ActionResult Plans()
        {
            return View();
        }

    }
}
