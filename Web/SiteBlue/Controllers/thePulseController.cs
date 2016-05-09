using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Models;

namespace SiteBlue.Controllers
{
    public class thePulseController : Controller
    {
        //
        // GET: /thePulse/

        public ViewResult Index()
        {
            return View();
        }
    }
}