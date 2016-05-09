using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Controllers
{
    public class SpecialController : Controller
    {
        private PlumberContext db = new PlumberContext();

        public ActionResult Offer(int id)
        {
            var offer = db.Offer.Find(id);
            if (offer != null)
                return View(offer);
            else
                throw new HttpException(404, "Not found");
        }

        public ActionResult Details(int id)
        {
            var specialOffer = new SpecialOfferModel().GetSpecialOffers().Where(so => so.SpecialOfferId == id).SingleOrDefault();
            if (specialOffer != null)
                return View(specialOffer);
            else
            {
                ViewBag.ErrorMessage = "This special offer is not available, please try again";
                return RedirectToAction("Index");
            }
        }

    }
}
