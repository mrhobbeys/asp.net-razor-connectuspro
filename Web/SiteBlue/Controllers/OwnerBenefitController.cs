﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class OwnerBenefitController : Controller
    {
        //
        // GET: /OwnerBenefit/

        public ActionResult Index()
        {
            ViewBag.Title = "Your Benefit";
            return View();
        }

    }
}