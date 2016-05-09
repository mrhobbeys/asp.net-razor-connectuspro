using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class ProductController : Controller
    {
        //private OwnerPortalContext db = new OwnerPortalContext();

        ////
        //// GET: /OwnerPortal/Product/

        //public ViewResult Index()
        //{
        //    return View(db.Product.ToList());
        //}

        ////
        //// GET: /OwnerPortal/Product/Details/5

        //public ViewResult Details(long id)
        //{
        //    Product product = db.Product.Find(id);
        //    return View(product);
        //}

        ////
        //// GET: /OwnerPortal/Product/Create

        //public ActionResult Create()
        //{
        //    return View();
        //} 

        ////
        //// POST: /OwnerPortal/Product/Create

        //[HttpPost]
        //public ActionResult Create(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Product.Add(product);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");  
        //    }

        //    return View(product);
        //}
        
        ////
        //// GET: /OwnerPortal/Product/Edit/5
 
        //public ActionResult Edit(long id)
        //{
        //    Product product = db.Product.Find(id);
        //    return View(product);
        //}

        ////
        //// POST: /OwnerPortal/Product/Edit/5

        //[HttpPost]
        //public ActionResult Edit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(product).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(product);
        //}

        ////
        //// GET: /OwnerPortal/Product/Delete/5
 
        //public ActionResult Delete(long id)
        //{
        //    Product product = db.Product.Find(id);
        //    return View(product);
        //}

        ////
        //// POST: /OwnerPortal/Product/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(long id)
        //{            
        //    Product product = db.Product.Find(id);
        //    db.Product.Remove(product);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}