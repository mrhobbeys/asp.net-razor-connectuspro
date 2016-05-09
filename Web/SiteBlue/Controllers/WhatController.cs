using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class WhatController : Controller
    {
        //
        // GET: /What/

        public ActionResult Index()
        {
            ViewBag.Current = "What";
            return View();
        }

    }
}
