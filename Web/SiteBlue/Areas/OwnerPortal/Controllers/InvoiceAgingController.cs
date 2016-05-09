using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using SiteBlue.Areas.OwnerPortal.Models;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Areas.SecurityGuard.Models;
using ReportManagement;
using SiteBlue.Data.EightHundred;
namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class InvoiceAgingController : PdfViewController
    {
        //
        // GET: /OwnerPortal/InvoiceAging/

        EightHundredEntities db = new EightHundredEntities();
        private MembershipConnection memberShipContext = new MembershipConnection();
        public ActionResult Index(FormCollection formcollection)
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;

                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;
                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View(reportTechnicians);


        }
        public ActionResult databyCompanyCode(string code)
        {
            try
            {
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                int FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();



                return PartialView("charts", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult GetInvoice(int id)
        {

            try
            {
                //Aging invoice = new Aging();
                var result = (from j in db.tbl_Job
                              join c in db.tbl_Customer on j.CustomerID equals c.CustomerID
                              join ci in db.tbl_Customer_Info on c.CustomerID equals ci.CustomerID
                              join cct in db.tbl_Customer_CreditTerms on ci.CreditTermsID equals cct.CreditTermsID
                              join f in db.tbl_Franchise on j.FranchiseID equals f.FranchiseID

                              select new
                              {
                                  InvoiceNumber = j.JobID,
                                  InvoiceDate = j.InvoicedDate,
                                  TotalSales = j.TotalSales,
                                  CustomerName = c.CustomerName,
                                  DueDate = j.RequestedDate,
                                  CreditTerms = cct.CreditTerms,
                                  FranchiseID = f.FranchiseID
                              }).Take(20).ToList();
                int tt;
                var newresult = from r in result.ToList()

                                select new
                                {
                                    InvoiceNumber = r.InvoiceNumber,
                                    InvoiceDate = r.InvoiceDate.Value.ToShortDateString(),
                                    TotalSales = r.TotalSales,
                                    CustomerName = r.CustomerName,
                                    DueDate = r.DueDate.Value.ToShortDateString(),
                                    CreditTerms = (r.CreditTerms == "Net 30 Days" ? tt = (DateTime.Now - Convert.ToDateTime(r.InvoiceDate)).Days - 30 : (DateTime.Now - Convert.ToDateTime(r.InvoiceDate)).Days),
                                    FranchiseID = r.FranchiseID
                                };


                var s = (from p in newresult.Where(k => k.FranchiseID == id)
                         select new
                         {
                             InvoiceNumber = p.InvoiceNumber,
                             InvoiceDate = p.InvoiceDate,
                             TotalSales = p.TotalSales,
                             CustomerName = p.CustomerName,
                             DueDate = p.DueDate,
                             CreditTerms = (p.CreditTerms > 0 ? p.CreditTerms : 0),
                             FranchiseID = p.FranchiseID
                         }).ToList();
                return Json(s);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public PartialViewResult reportview(int id)
        {
            try
            {
                return PartialView("reportview", id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public PartialViewResult charts(int id)
        {
            try
            {
                return PartialView("charts", id);
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public ActionResult UserCompanyCode()
        {
            IMembershipService membershipService;
            IAuthenticationService authenticationService;
            membershipService = new MembershipService(Membership.Provider);
            authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());

            MembershipUser user = membershipService.GetUser(User.Identity.Name);
            var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            var isCorporate = User.IsInRole("Corporate");
            string username = user.UserName;
            int[] assignedFranchises;

            var DefaultCompamyName = default(String);
            var DefaultCompanyID = default(int);

            if (Session["selectedFranchiseId"] != null)
            {
                DefaultCompanyID = Convert.ToInt32(Session["selectedFranchiseId"]);
                DefaultCompamyName = (from g in db.tbl_Franchise
                                      where g.FranchiseID == DefaultCompanyID
                                      select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
                ViewBag.code = DefaultCompamyName;
            }
            else
            {
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName != null)
                {
                    DefaultCompamyName = (from g in db.tbl_Franchise
                                          where g.FranchiseNUmber == DefaultCompamyName
                                          select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
                }
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();

                    DefaultCompamyName = (from g in db.tbl_Franchise
                                          where g.FranchiseNUmber == DefaultCompamyName
                                          select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
                }

                DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                    where g.FranchiseID == 56 && g.UserId == userId
                                    select g.Franchise.FranchiseID).FirstOrDefault();
                if (DefaultCompanyID == 0)
                {
                    DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                        where g.UserId == userId
                                        select g.Franchise.FranchiseID).FirstOrDefault();
                }


                if (RouteData.Values["id"] != null)
                {
                    int companyCodeID = int.Parse(Convert.ToString(RouteData.Values["id"]));
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.FranchiseID == companyCodeID && g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                    DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                        where g.FranchiseID == companyCodeID && g.UserId == userId
                                        select g.Franchise.FranchiseID).FirstOrDefault();
                }
            }

            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == userId)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }

            var model = new GrantCompaniesToUser
            {
                UserName = username,
                GrantedCompanyCode =
                db.tbl_Franchise
                               .Where(f => assignedFranchises.Contains(f.FranchiseID))
                               .OrderBy(f => f.FranchiseNUmber)
                               .Select(d => new SelectListItem
                               {
                                   Text = string.Concat(d.LegalName, " - ", d.FranchiseNUmber),
                                   Value = System.Data.Objects.SqlClient.SqlFunctions.StringConvert((double)d.FranchiseID)
                               })
                               .ToList(),
                defaultCompanyName = DefaultCompamyName,
                defaultCompanyID = DefaultCompanyID
            };

            var swapBranding = false;

            if (HttpContext.User.IsInRole("CompanyOwner"))
            {
                using (var ctx = new MembershipConnection())
                {
                    swapBranding = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .ToArray()
                                            .Select(uf => (from f in db.tbl_Franchise where f.FranchiseID == uf.FranchiseID select f).SingleOrDefault())
                                            .Any(f => f.FranchiseTypeID == 6);
                }
            }

            ViewBag.SwapBranding = swapBranding;

            return PartialView("CompanyCodeUser", model);


        }

        public ActionResult report()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            try
            {

                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;

                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(reportTechnicians);
        }
        public ActionResult bindReport(string code)
        {
            int FranchiseID = (from g in memberShipContext.MembershipFranchise
                               where g.FranchiseNumber == code
                               select g.FranchiseID).FirstOrDefault();
            return View("reportview", FranchiseID);
        }


        public ActionResult bindReport1(int id)
        {
            try
            {
                Int32 id1 = Convert.ToInt32(Session["pdfid"]);
                var pastDate30 = DateTime.Today.AddDays(-30);
                var pastDate60 = DateTime.Today.AddDays(-60);
                var pastDate90 = DateTime.Today.AddDays(-90);
                var a = (from k in db.tbl_Job.Where(k => k.Balance != 0 && k.FranchiseID == id && k.InvoicedDate > DateTime.Now)
                         select k.TotalSales).ToList();

                var a1 = (from k in db.tbl_Job.Where(k => k.Balance != 0 && k.FranchiseID == id && k.InvoicedDate <= DateTime.Now && k.InvoicedDate > pastDate30)
                          select k.TotalSales).ToList();

                var a2 = (from k in db.tbl_Job.Where(k => k.Balance != 0 && k.FranchiseID == id && k.InvoicedDate <= pastDate30 && k.InvoicedDate > pastDate60)
                          select k.TotalSales).ToList();

                var a3 = (from k in db.tbl_Job.Where(k => k.Balance != 0 && k.FranchiseID == id && k.InvoicedDate <= pastDate60 && k.InvoicedDate > pastDate90)
                          select k.TotalSales).ToList();

                var a4 = (from k in db.tbl_Job.Where(k => k.Balance != 0 && k.FranchiseID == id && k.InvoicedDate <= pastDate90)
                          select k.TotalSales).ToList();
                var viewModel = new List<TotalSale>();
                foreach (var course in a)
                {
                    viewModel.Add(new TotalSale
                    {
                        Totalsale0 = course,
                        Total = course
                    });
                }
                foreach (var course in a1)
                {
                    viewModel.Add(new TotalSale
                    {
                        Totalsale1 = course,
                        Total = course
                    });
                }
                foreach (var course in a2)
                {
                    viewModel.Add(new TotalSale
                    {
                        Totalsale2 = course,
                        Total = course
                    });
                }
                foreach (var course in a3)
                {
                    viewModel.Add(new TotalSale
                    {
                        Totalsale3 = course,
                        Total = course
                    });
                }
                foreach (var course in a4)
                {
                    viewModel.Add(new TotalSale
                    {
                        Totalsale4 = course,
                        Total = course
                    });
                }

                return this.ViewPdf("", "pdf", viewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
