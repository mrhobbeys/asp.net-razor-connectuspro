using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Controllers
{
    public class HomeController : Controller
    {
        private PlumberContext db = new PlumberContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmergencyServiceAvailable()
        {
            return View(new ServiceModel().GetServices());
        }

        public ActionResult ComplimentaryEvaluation()
        {
            return View(new ServiceModel().GetServices());
        }

        public ActionResult NoSurprise()
        {
            return View(new ServiceModel().GetServices());
        }

        private List<ZipList> AllPostalCodes()
        {
            return db.PostalCode.ToList();
        }

        public JsonResult getLocationZips(string zipCode, int? serviceId)
        {
            // Zip Code not found in the database
            var zipCodeCount = db.PostalCode.Where(pc => pc.FranchiseZipID == zipCode).Count();
            if (zipCodeCount == 0) // Zip code not arrived in the selected area
                return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=1" });

            if (serviceId.HasValue)
            {
                // Electrical
                if (serviceId.Value == 4)
                {
                    // Relative to Greenville
                    var greenville = db.Franchise.Find();
                    //var greenvileeZips = db.PostalCode.Where(pc => pc.FranchiseZipID == zipCode)
                        
                }
            }


            return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=1" });
        }

        public JsonResult GetLocationByZipCodeAndService(string zipCode, int? serviceId)
        {
            // Zip Code not found in the database
            var zipCodes = db.LocationZip.Where(lz => lz.ZipCode == zipCode).ToList();
            if ((zipCodes == null) || (zipCodes.Count == 0))
                return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=1" });

            List<Location> location;
            if (serviceId.HasValue)
            {
                List<int> servicesToCheck;
                switch (serviceId.Value)
                {
                    case 1:
                        servicesToCheck = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
                        break;

                    case 2:
                        servicesToCheck = new List<int>() { 14 };
                        break;

                    case 3:
                        servicesToCheck = new List<int>() { 14 };
                        break;

                    case 4:
                        // -1 should be GreenVille
                        servicesToCheck = new List<int>() { 5 };
                        break;

                    case 5:
                        servicesToCheck = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                        break;

                    case 6:
                        // Handyman
                        return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=2" });

                    case 7:
                        // Drain Cleaning
                        servicesToCheck = new List<int>() { 5 };
                        break;

                    case 8:
                        servicesToCheck = new List<int>() { 15 };
                        break;

                    default:
                        return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=1" });
                }

                foreach (var element in servicesToCheck)
                {
                    location = db.Location
                    .Where(l => l.LocationZips.Any(lz => lz.ZipCode == zipCode))
                    .Where(l => l.LocationServices.Any(ls => ls.ServiceId == element))
                    .ToList();

                    if ((location != null) && (location.Count > 0))
                    {
                        return Json(new { url = "/Locations/Special/" + location[0].LocationId });
                    }
                }
                // No result, redirect the user to the login page
                return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=2" });
            }
            else
            {
                location = db.Location
                    .Where(l => l.LocationZips.Any(lz => lz.ZipCode == zipCode))
                    .ToList();

                if ((location != null) && (location.Count > 0))
                {
                    //return RedirectToAction("Special", "Locations", new { id = location[0].LocationId, area = "" });
                    return Json(new { url = "/Locations/Special/" + location[0].LocationId });
                }
                else
                {
                    //return RedirectToAction("Index", "Home", new { area = "" });
                    return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=2" });
                }
            }
            //return View();
        }

        //public JsonResult FindNearestLocation(string zipCode, int? serviceId)
        //{
        //    // Zip Code not found in the database
        //    var zipCodes = db.PostalCode.Where(lz => lz.FranchiseZipID == zipCode).ToList();
        //    if ((zipCodes == null) || (zipCodes.Count == 0))
        //        return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=1" });
        //    else
        //    {
        //        // Get the location
        //        if (serviceId.HasValue)
        //        {
        //            int franchiseId = zipCodes[0].FranchiseID;
        //            var location = db.Location.Where(l => l.FranchiseID == franchiseId).SingleOrDefault();
        //            if (location.LocationServiceCategories.Any(ls => ls.ServiceCategoryId == serviceId.Value))
        //                return Json(new { url = "/Locations/Special/" + location.LocationId });
        //            else
        //                return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=2" });
        //        }
        //        else
        //        {
        //            int franchiseId = zipCodes[0].FranchiseID;
        //            var location = db.Location.Where(l => l.FranchiseID == franchiseId).SingleOrDefault();
        //            return Json(new { url = "/Locations/Special/" + location.LocationId });
        //        }
        //    }



        //    return Json(new { url = "/" });
        //}

        public JsonResult FindNearestLocation(string zipCode, int? serviceId)
        {
            // Zip Code not found in the database
            var zipCodes = db.PostalCode.Where(lz => lz.FranchiseZipID == zipCode).ToList();
            if ((zipCodes == null) || (zipCodes.Count == 0))
                return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=1" });
            else if (zipCodes.Count == 1)
            {
                // Get the location
                if (serviceId.HasValue)
                {
                    int franchiseId = zipCodes[0].FranchiseID;
                    var location = db.Location.Where(l => l.FranchiseID == franchiseId).SingleOrDefault();
                    
                    //if (location.LocationServiceCategories.Any(ls => ls.ServiceCategoryId == serviceId.Value))
                    if (zipCodes[0].Franchise.FranchiseServices.Any(fs => fs.ServiceID == serviceId.Value))
                        return Json(new { url = "/Locations/Special/" + location.LocationId });
                    else
                        return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=2&service=" + serviceId.Value });
                }
                else
                {
                    int franchiseId = zipCodes[0].FranchiseID;
                    var location = db.Location.Where(l => l.FranchiseID == franchiseId).SingleOrDefault();
                    return Json(new { url = "/Locations/Special/" + location.LocationId });
                }
            }
            else
            {
                foreach (var item in zipCodes)
                {
                    int franchiseId = item.FranchiseID;
                    var location = db.Location.Where(l => l.FranchiseID == franchiseId).SingleOrDefault();

                    if (location != null)
                    {
                        if (serviceId.HasValue)
                        {
                            if (item.Franchise.FranchiseServices.Any(fs => fs.ServiceID == serviceId.Value))
                                return Json(new { url = "/Locations/Special/" + location.LocationId });
                            else
                                return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=2&service=" + serviceId.Value });
                        }
                        else
                        {
                            return Json(new { url = "/Locations/Special/" + location.LocationId });
                        }
                    }
                }
                return Json(new { url = "/NotYetArrived?zip=" + zipCode + "&error=1" });
            }
        }
    }
}
