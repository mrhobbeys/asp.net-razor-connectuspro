using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.Title = "Index page";
            return View();
        }

        public ActionResult Error()
        {
            var info = (HandleErrorInfo) ViewData.Model;
            var msg = string.Format("An unhandled exception occurred in controller '{0}' and action '{1}'", info.ControllerName, info.ActionName);
            Logger.Log(msg, info.Exception, LogLevel.Fatal);
            return View();
        }

        public ActionResult Privacy()
        {
            ViewBag.Title = "Privacy";
            return View();
        }
    }
}
