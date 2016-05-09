using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using System.Drawing;
using System.Linq.Expressions;
using SiteBlue.Controllers;
using SiteBlue.Data.EightHundred;
using System.Text;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Areas.SecurityGuard.Models;
using SiteBlue.Models;
using DHTMLX.Export.Excel;
using SiteBlue.Business.Job;
using System.Data.Objects.SqlClient;
using System.Transactions;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class AllCustomersController : SiteBlueBaseController
    {
        //
        // GET: /OwnerPortal/AllCustomers/
        EightHundredEntities db = new EightHundredEntities();

        private MembershipConnection memberShipContext = new MembershipConnection();

        public ActionResult Index()
        {
            ViewBag.custAc = "";
            ViewBag.custName = "";
            ViewBag.ComName = "";
            ViewBag.Address = "";
            ViewBag.State = "";
            ViewBag.city = "";
            ViewBag.Email = "";

            return RedirectToAction("CustomerList");
        }

        public ActionResult CustomerList(FormCollection formcollection)
        {
            ViewBag.ShowAllFranchiseOption = true;
            return View();
        }

        public JsonResult CustomerData(int franchise, string strName, string strAddress, string strphone, bool isMember, int pageNum, int pageSize)
        {
            Session["txtName"] = strName;
            Session["txtAddress"] = strAddress;
            Session["txtPhoneNumber"] = strphone;
            Session["chkmember"] = isMember;

            var startAt = Math.Min(pageNum - 1 * pageSize, 0);

            var customerlist = (from cust in db.tbl_Customer
                                join loc in db.tbl_Locations on cust.CustomerID equals loc.BilltoCustomerID into loc_1
                                join cInfo in db.tbl_Customer_Info on cust.CustomerID equals cInfo.CustomerID into cInfo_1
                                join mInfo in db.tbl_Customer_Members on cust.CustomerID equals mInfo.CustomerID into mInfo_1
                                from loc in loc_1.DefaultIfEmpty()
                                from cInfo in cInfo_1.DefaultIfEmpty()
                                from mInfo in mInfo_1.DefaultIfEmpty()
                                where loc.BilltoCustomerID != null && cInfo.CustomerID != null && (cInfo.FranchiseID == franchise || franchise == 0)
                                select new CustomerList
                                {
                                    FranchiseID = cInfo.FranchiseID,
                                    CustomerID = cust.CustomerID,
                                    CustomerName = cust.CustomerName,
                                    CompanyName = cust.CompanyName,
                                    Address = loc.Address,
                                    State = loc.State,
                                    City = loc.City,
                                    PostalCode = loc.PostalCode,
                                    EMail = cust.EMail,
                                    PhoneNumber = (from c in db.tbl_Contacts where c.PhoneTypeID == 2 && c.CustomerID == cust.CustomerID select c.PhoneNumber).FirstOrDefault(),
                                    MemberType = "HomeGuard",
                                    Exp_date = "02/12/2011",
                                    Scheduled = "Yes",
                                    AgeRecords = 2,
                                    House = "A",
                                    MemberID = mInfo.MemberID
                                });

            if (customerlist.Count() == 0)
                return Json(new { total_count = 0, rows = "" }, JsonRequestBehavior.AllowGet);


            if (strAddress != "null" && strAddress != null && strAddress != "")
                customerlist = (from c in customerlist where c.Address.ToLower().Contains(strAddress.ToLower()) select c);

            if (strName != "null" && strName != null && strName != "")
                customerlist = (from c in customerlist
                                where c.CustomerName.ToLower().Contains(strName.ToLower())
                                || c.CompanyName.ToLower().Contains(strName.ToLower())
                                select c);

            if (strphone != "null" && strphone != null && strphone != "")
                customerlist = (from c in customerlist where (c.PhoneNumber ?? "").Replace("(", "").Replace(")", "").Replace("-", "").Contains(strphone.Replace("(", "").Replace(")", "").Replace("-", "")) select c);

            if (isMember)
                customerlist = (from c in customerlist where c.MemberID != null select c);

            customerlist = customerlist.OrderBy(c => c.CustomerName);

            if (startAt > 0)
                customerlist = customerlist.Skip(startAt);

            customerlist = customerlist.Take(pageSize);

            var json = new
            {
                total_count = customerlist.Count(),
                rows = customerlist.ToList().Select(h => new
                {
                    id = h.CustomerID,
                    data = new object[] {
                                            h.CustomerID, 
                                            h.CustomerName, 
                                            h.CompanyName, 
                                            h.Address, 
                                            h.State,
                                            h.City,
                                            (h.PhoneNumber != null && h.PhoneNumber != "" && h.PhoneNumber.IndexOf('(') == -1 && h.PhoneNumber.Trim().Length >= 10) ? string.Format("({0}) {1}-{2}", h.PhoneNumber.Substring(0, 3), h.PhoneNumber.Substring(3, 3), h.PhoneNumber.Substring(6)) : h.PhoneNumber,
                                            h.EMail,
                                            h.Balance
                                        }
                }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CustomerDetails(int id)
        {
            var customerData = (from cust in db.tbl_Customer
                                join loc in db.tbl_Locations on cust.CustomerID equals loc.BilltoCustomerID into loc_1
                                join cInfo in db.tbl_Customer_Info on cust.CustomerID equals cInfo.CustomerID into cInfo_1
                                from loc in loc_1.DefaultIfEmpty()
                                from cInfo in cInfo_1.DefaultIfEmpty()
                                where cust.CustomerID == id
                                select new
                                {
                                    cInfo.FranchiseID,
                                    cust.CustomerID,
                                    cust.CustomerName,
                                    cust.CompanyName,
                                    cInfo.CreditLimit,
                                    cInfo.CustomerNotes,
                                    loc.Address,
                                    loc.State,
                                    loc.City,
                                    cust.EMail,
                                    loc.PostalCode
                                }).FirstOrDefault();

            ViewBag.custAc = customerData.CustomerID;
            ViewBag.custName = customerData.CustomerName;
            ViewBag.ComName = customerData.CompanyName;
            ViewBag.Address = customerData.Address;
            ViewBag.State = customerData.State;
            ViewBag.city = customerData.City;
            ViewBag.Email = customerData.EMail;

            return Json(customerData);

        }
        public ActionResult CustomerViewData(int id)
        {
            var viewData = (from c in db.tbl_Calls
                            join cust in db.tbl_Customer_Info on c.CustomerID equals cust.CustomerID
                            where c.CustomerID == id
                            select c).ToList();

            return Json(viewData.Count);
        }

        public ActionResult CustomerJobViewData(int id)
        {
            var viewData = (from j in db.tbl_Job
                            join cust in db.tbl_Customer_Info on j.CustomerID equals cust.CustomerID
                            where j.CustomerID == id
                            select j).ToList();

            return Json(viewData.Count);
        }

        public ActionResult JobData(int id)
        {
            var viewData = (from j in db.tbl_Job
                            join cust in db.tbl_Customer on j.CustomerID equals cust.CustomerID
                            where j.CustomerID == id
                            select new
                            {
                                j.JobID,
                                InvoiceNumber = j.JobID,
                                j.InvoicedDate,
                                cust.CustomerName
                            }).ToList();

            return Json(viewData);
        }

        public ActionResult CustomerListUpdate(int custid, string custName, string ComName, string Address, string State, string Postal, string City, string Email, string Jobs, string Calls, decimal CreditLimit, string Scheduled, string Type, string Visits, string MExp, string CCExp, string AgeRecords, string House, string WHeater, string AC, string GenNotes)
        {
            var custdata = (from p in db.tbl_Customer where p.CustomerID == custid select p).FirstOrDefault();
            custdata.CustomerName = custName;
            custdata.CompanyName = ComName;
            custdata.EMail = Email;

            var custLoc = (from l in db.tbl_Locations where l.BilltoCustomerID == custdata.CustomerID select l).FirstOrDefault();
            custLoc.Address = Address;
            custLoc.State = State;
            custLoc.City = City;
            custLoc.PostalCode = Postal;

            var custinfo = (from l in db.tbl_Customer_Info where l.CustomerID == custdata.CustomerID select l).FirstOrDefault();
            custinfo.CreditLimit = CreditLimit;

            db.SaveChanges();

            var test = "";
            return Json(test);
        }

        [HttpPost]
        public ActionResult CheckCustomerID(int custId)
        {
            var iscustomer = db.tbl_Customer.Any(q => q.CustomerID == custId);

            if (iscustomer)
                return Json("exist");

            return Json("none");
        }

        public ActionResult CustomerInformation(int Custid)
        {
            var statusId = 0;

            CustomerDetail customerdetail = new CustomerDetail();
            customerdetail.CustomerID = Custid;

            var ServiceMem = (from p in db.tbl_Member_Type
                              select new
                              {
                                  p.MemberTypeID,
                                  p.MemberType
                              });

            customerdetail.serviceplan.MemberTypeList = new SelectList(ServiceMem, "MemberTypeID", "MemberType");

            var custinfo = (from cust in db.tbl_Customer
                            join loc in db.tbl_Locations on cust.CustomerID equals loc.ActvieCustomerID into loc_1
                            join Bloc in db.tbl_Locations on cust.CustomerID equals Bloc.BilltoCustomerID into Bloc_1
                            join cInfo in db.tbl_Customer_Info on cust.CustomerID equals cInfo.CustomerID into cInfo_1
                            join mInfo in db.tbl_Customer_Members on cust.CustomerID equals mInfo.CustomerID into mInfo_1
                            from loc in loc_1.DefaultIfEmpty()
                            from cInfo in cInfo_1.DefaultIfEmpty()
                            from mInfo in mInfo_1.DefaultIfEmpty()
                            from Bloc in Bloc_1.DefaultIfEmpty()
                            join contact in db.tbl_Contacts on cust.CustomerID equals contact.CustomerID
                            join phonetype in db.tbl_PhoneType on contact.PhoneTypeID equals phonetype.PhoneTypeID
                            where cInfo.CustomerID != null && cust.CustomerID == Custid
                            select new
                            {
                                cust.CustomerName,
                                loc.Address,
                                cust.CompanyName,
                                loc.City,
                                loc.State,
                                loc.PostalCode,
                                BillCustomerName = cust.CustomerName,
                                BillAddress = Bloc.Address,
                                BillCompanyName = cust.CompanyName,
                                BillCity = Bloc.City,
                                BillState = Bloc.State,
                                BillPostalCode = Bloc.PostalCode,
                                PrimaryPhone = contact.PhoneNumber,
                                CellPhone = contact.PhoneNumber,
                                cust.EMail,
                                cInfo.CustomerNotes,
                                MemberInfo = mInfo,
                                cInfo.StatusID
                            }).FirstOrDefault();

            if (custinfo != null)
            {
                //Customer Information
                customerdetail.customerinfo.CustomerName = custinfo.CustomerName;
                customerdetail.customerinfo.Address = custinfo.Address;
                customerdetail.customerinfo.CompanyName = custinfo.CompanyName;
                customerdetail.customerinfo.City = custinfo.City;
                customerdetail.customerinfo.State = custinfo.State;
                customerdetail.customerinfo.PostalCode = custinfo.PostalCode;
                customerdetail.customerinfo.PrimaryPhone = custinfo.PrimaryPhone;
                customerdetail.customerinfo.CellPhone = custinfo.CellPhone;
                customerdetail.customerinfo.EMail = custinfo.EMail;
                customerdetail.customerinfo.Notes = custinfo.CustomerNotes;

                //Bill Address
                customerdetail.billaddr.CustomerName = custinfo.BillCustomerName;
                customerdetail.billaddr.Address = custinfo.BillAddress;
                customerdetail.billaddr.CompanyName = custinfo.BillCompanyName;
                customerdetail.billaddr.City = custinfo.BillCity;
                customerdetail.billaddr.State = custinfo.BillState;
                customerdetail.billaddr.PostalCode = custinfo.BillPostalCode;

                statusId = custinfo.StatusID;

                //Job History
                var LastJob = (from p in db.tbl_Job where p.CustomerID == Custid orderby p.JobEnded select p.JobEnded).FirstOrDefault();
                if (LastJob != null)
                    customerdetail.jobhistory.DateLastJob = LastJob;

                var viewData = (from j in db.tbl_Job
                                join cust in db.tbl_Customer_Info on j.CustomerID equals cust.CustomerID
                                where j.CustomerID == Custid
                                select j).Count();
                customerdetail.jobhistory.TotalJobs = viewData;

                if (custinfo.MemberInfo != null)
                {
                    var memtype = (from p in db.tbl_Customer
                                   join p1 in db.tbl_Customer_Members on p.CustomerID equals p1.CustomerID
                                   join p2 in db.tbl_Member_Type on p1.MemberTypeID equals p2.MemberTypeID
                                   where p.CustomerID == Custid
                                   select new
                                   {
                                       p1.MemberTypeID,
                                       p1.StartDate,
                                       p1.EndDate,
                                       p2.MemberType,
                                       p1.MemberID
                                   }).FirstOrDefault();

                    customerdetail.serviceplan.MemberTypeList = new SelectList(ServiceMem, "MemberTypeID", "MemberType", memtype.MemberTypeID);
                    customerdetail.serviceplan.StartDate = memtype.StartDate;
                    customerdetail.serviceplan.ExpiryDate = memtype.EndDate;

                    var visits = (from p in db.tbl_Member_Visits where p.MemberID == memtype.MemberID select p).Count();
                    customerdetail.serviceplan.ServiceVisits = visits;
                }
            }

            var itemsServicetype = new List<SelectListItem>();
            itemsServicetype.Add(new SelectListItem
            {
                Text = "Normal",
                Value = "0",
                Selected = statusId == 0 ? true : false
            });
            itemsServicetype.Add(new SelectListItem
            {
                Text = "No Service",
                Value = "1",
                Selected = statusId == 1 ? true : false
            });
            itemsServicetype.Add(new SelectListItem
            {
                Text = "COD Only",
                Value = "2",
                Selected = statusId == 2 ? true : false
            });
            itemsServicetype.Add(new SelectListItem
            {
                Text = "Normal",
                Value = "0",
                Selected = statusId == 0 ? true : false
            });
            customerdetail.ServiceStatus = itemsServicetype;

            return View(customerdetail);
        }

        public ActionResult BindInvoiceNumbers(int CustomerId)
        {
            var objJOb = (from e in db.tbl_Job
                          where e.CustomerID == CustomerId
                          orderby e.CallCompleted ascending
                          select new
                          {
                              e.JobID,
                              e.CallCompleted
                          }).ToList();

            return Json(objJOb);
        }

        public ActionResult SaveCustomerInformation(string CustName, string Address, string ComName, string City, string State, string ZipCode, string PhoneNumber, string CellNumber, string Email, string Notes, string MemberType, string StartDate, string ExpiryDate, string ServiceVisits, string CCExpiryDate, string DateNextJob, string DateLastJob, string TotalJobs, string Balance, string AgeofHome, string AgeofSystem, string AgeofHeater, string BCustName, string BComName, string BAddress, string BCity, string BState, string BZipCode, string ServiceStatus, int CustID)
        {
            var custinfo = db.tbl_Customer.FirstOrDefault(p => p.CustomerID == CustID);

            if (custinfo != null)
            {
                using (var scope = new TransactionScope())
                {
                    custinfo.CustomerName = CustName;
                    custinfo.CompanyName = ComName;
                    custinfo.EMail = Email;
                    var location = db.tbl_Locations.FirstOrDefault(p => p.ActvieCustomerID == CustID);
                    if (location != null)
                    {
                        location.Address = Address;
                        location.City = City;
                        location.State = State;
                        location.PostalCode = ZipCode;
                    }

                    var Blocation = db.tbl_Locations.FirstOrDefault(p => p.BilltoCustomerID == CustID);
                    if (Blocation != null)
                    {
                        Blocation.Address = BAddress;
                        Blocation.City = BCity;
                        Blocation.State = BState;
                        Blocation.PostalCode = BZipCode;
                    }

                    var phone = db.tbl_Contacts.FirstOrDefault(p => p.CustomerID == CustID && p.PhoneTypeID == 2);
                    if (phone != null)
                        phone.PhoneNumber = PhoneNumber;

                    var cell = db.tbl_Contacts.FirstOrDefault(p => p.CustomerID == CustID && p.PhoneTypeID == 4);
                    if (cell != null)
                        cell.PhoneNumber = CellNumber;

                    var notes = db.tbl_Customer_Info.FirstOrDefault(p => p.CustomerID == CustID);
                    if (notes != null)
                        notes.CustomerNotes = Notes;
                    notes.StatusID = Convert.ToInt32(ServiceStatus);

                    var member = db.tbl_Customer_Members.FirstOrDefault(p => p.CustomerID == CustID);

                    if (!string.IsNullOrEmpty(MemberType))
                    {
                        if (member != null)
                        {
                            member.StartDate = Convert.ToDateTime(StartDate);
                            member.EndDate = Convert.ToDateTime(ExpiryDate);
                            member.MemberTypeID = Convert.ToInt32(MemberType);
                        }
                        else
                        {
                            var newMember = new tbl_Customer_Members { CustomerID = CustID };

                            if (StartDate != null)
                                newMember.StartDate = Convert.ToDateTime(StartDate);
                            if (ExpiryDate != null)
                                newMember.EndDate = Convert.ToDateTime(ExpiryDate);

                            newMember.MemberTypeID = Convert.ToInt32(MemberType);

                            db.tbl_Customer_Members.AddObject(newMember);
                        }
                    }
                    else
                    {
                        if (member != null)
                            db.tbl_Customer_Members.DeleteObject(member);
                    }

                    db.SaveChanges();

                    scope.Complete();
                }
            }

            return Json(string.Empty);
        }

        public string LoadCustomerData(string strName, string strAddress, string strphone, bool? isMember, string strFrId, string orderByColumn, string orderOrientation)
        {
            StringBuilder response = new StringBuilder();
            int count, index, totalCount;

            //int iAcnum = (strAcnum == "" ? 0 : Convert.ToInt32(strAcnum));
            int iFrId = (strFrId == "" ? 0 : Convert.ToInt32(strFrId));

            if (orderByColumn == null) orderByColumn = "CustomerID";
            if (orderOrientation == null) orderOrientation = "asc";

            if ((Session["strFrId"] != null) && (Session["strFrId"].ToString() != strFrId))
            {
                index = 0;
                Session["posStart"] = null;
                Session["totalCount"] = null;
                Session["count"] = null;
                Session["orderBy"] = null;
                Session["direction"] = null;
            }

            if ((Session["direction"] != null) && (Session["direction"].ToString().Equals(orderOrientation) == false))
            {
                index = 0;
                Session["posStart"] = null;
                Session["totalCount"] = null;
                Session["count"] = null;
                Session["orderBy"] = null;
                Session["direction"] = null;
            }

            Session["strFrId"] = strFrId;
            Session["orderBy"] = orderByColumn;
            Session["direction"] = orderOrientation;

            CustomerList cList = new Models.CustomerList();
            //CustomerList custData;
            List<CustomerList> custDataList = new List<Models.CustomerList>();

            Session["txtName"] = strName;
            Session["txtAddress"] = strAddress;
            Session["txtPhoneNumber"] = strphone;
            Session["chkmember"] = isMember;

            if (iFrId == 0)
                iFrId = 56;

            var customerData = (from cust in db.tbl_Customer
                                join loc in db.tbl_Locations on cust.CustomerID equals loc.ActvieCustomerID into loc_1
                                join cInfo in db.tbl_Customer_Info on cust.CustomerID equals cInfo.CustomerID into cInfo_1
                                join mInfo in db.tbl_Customer_Members on cust.CustomerID equals mInfo.CustomerID into mInfo_1
                                from loc in loc_1.DefaultIfEmpty()
                                from cInfo in cInfo_1.DefaultIfEmpty()
                                from mInfo in mInfo_1.DefaultIfEmpty()
                                where loc.BilltoCustomerID != null && cInfo.CustomerID != null && cInfo.FranchiseID == iFrId
                                select new
                                {
                                    cInfo.FranchiseID,
                                    cust.CustomerID,
                                    cust.CustomerName,
                                    cust.CompanyName,
                                    loc.Address,
                                    loc.State,
                                    loc.City,
                                    loc.PostalCode,
                                    cust.EMail,
                                    PhoneNumber = (from c in db.tbl_Contacts where c.PhoneTypeID == 2 && c.CustomerID == cust.CustomerID select c.PhoneNumber).FirstOrDefault(),
                                    MemberType = "",
                                    Exp_date = "",
                                    Scheduled = "",
                                    AgeRecords = 2,
                                    House = "",
                                    mInfo.StartDate,
                                    mInfo.EndDate
                                }).Distinct().ToList();


            //var customerInfoData = db.tbl_Customer_Info.Where(ci => ci.FranchiseID == iFrId).ToList();
            // Session variables initialisation
            if ((Session["posStart"] != null) && (Session["posStart"].ToString() != ""))
                index = int.Parse(Session["posStart"].ToString());
            else
                index = 0;
            if ((Session["count"] != null) && (Session["count"].ToString() != ""))
            {
                count = int.Parse(Session["count"].ToString());
                Session["count"] = count;
            }
            else
                count = customerData.Count < 20 ? customerData.Count : 20;

            if (index == 0)
            {
                totalCount = customerData.Count;
                Session["totalCount"] = totalCount;
            }
            else
                totalCount = int.Parse(Session["totalCount"].ToString());

            if (orderOrientation == "asc")
                switch (orderByColumn)
                {
                    case "0":
                        customerData = customerData.OrderBy(t => t.CustomerID).Skip(index).Take(count).ToList();
                        break;
                    case "1":
                        customerData = customerData.OrderBy(t => t.CustomerName).Skip(index).Take(count).ToList();
                        break;
                    case "2":
                        customerData = customerData.OrderBy(t => t.CompanyName).Skip(index).Take(count).ToList();
                        break;
                    case "3":
                        customerData = customerData.OrderBy(t => t.Address).Skip(index).Take(count).ToList();
                        break;
                    case "4":
                        customerData = customerData.OrderBy(t => t.State).Skip(index).Take(count).ToList();
                        break;
                    case "5":
                        customerData = customerData.OrderBy(t => t.City).Skip(index).Take(count).ToList();
                        break;
                    case "6":
                        customerData = customerData.OrderBy(t => t.PhoneNumber).Skip(index).Take(count).ToList();
                        break;
                    case "7":
                        customerData = customerData.OrderBy(t => t.EMail).Skip(index).Take(count).ToList();
                        break;
                    //case "8":
                    //    customerData = customerData.OrderBy(t => t.b).Skip(index).Take(count).ToList();
                    //    break;

                    default:
                        customerData = customerData.OrderBy(t => t.CustomerID).Skip(index).Take(count).ToList();
                        break;
                }
            else if (orderOrientation == "des")
                switch (orderByColumn)
                {
                    case "0":
                        customerData = customerData.OrderByDescending(t => t.CustomerID).Skip(index).Take(count).ToList();
                        break;
                    case "1":
                        customerData = customerData.OrderByDescending(t => t.CustomerName).Skip(index).Take(count).ToList();
                        break;
                    case "2":
                        customerData = customerData.OrderByDescending(t => t.CompanyName).Skip(index).Take(count).ToList();
                        break;
                    case "3":
                        customerData = customerData.OrderByDescending(t => t.Address).Skip(index).Take(count).ToList();
                        break;
                    case "4":
                        customerData = customerData.OrderByDescending(t => t.State).Skip(index).Take(count).ToList();
                        break;
                    case "5":
                        customerData = customerData.OrderByDescending(t => t.City).Skip(index).Take(count).ToList();
                        break;
                    case "6":
                        customerData = customerData.OrderByDescending(t => t.PhoneNumber).Skip(index).Take(count).ToList();
                        break;
                    case "7":
                        customerData = customerData.OrderByDescending(t => t.EMail).Skip(index).Take(count).ToList();
                        break;
                    //case "8":
                    //    customerData = customerData.OrderBy(t => t.b).Skip(index).Take(count).ToList();
                    //    break;

                    default:
                        customerData = customerData.OrderByDescending(t => t.CustomerID).Skip(index).Take(count).ToList();
                        break;
                }
            else
                customerData = customerData.OrderBy(t => t.CustomerID).Skip(index).Take(count).ToList();

            if (strAddress != null && strAddress != "")
            {
                customerData = (from c in customerData where c.Address.ToLower().Contains(strAddress.ToLower()) select c).ToList();
            }

            if (strName != null && strName != "")
            {
                customerData = (from c in customerData where c.CustomerName.ToLower().Contains(strName.ToLower()) select c).ToList();
            }

            if (strphone != null && strphone != "")
            {
                customerData = (from c in customerData where (c.PhoneNumber == null ? "" : c.PhoneNumber).Replace("(", "").Replace(")", "").Replace("-", "").Contains(strphone.Replace("(", "").Replace(")", "").Replace("-", "")) select c).ToList();
            }
            if (isMember == true)
            {
                customerData = (from c in customerData where c.StartDate != null select c).ToList();
            }

            response.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><rows total_count='" + totalCount + "' pos='" + index + "'>");
            foreach (var cData in customerData)
            {
                response.Append("<row id='" + index + "'>");
                response.Append("<cell><![CDATA[" + cData.CustomerID + "]]></cell>");
                response.Append("<cell><![CDATA[" + cData.CustomerName + "]]></cell>");
                response.Append("<cell><![CDATA[" + cData.CompanyName + "]]></cell>");
                response.Append("<cell><![CDATA[" + cData.Address + "]]></cell>");
                response.Append("<cell><![CDATA[" + cData.State + "]]></cell>");
                response.Append("<cell><![CDATA[" + cData.City + "]]></cell>");
                if (cData.PhoneNumber != null)
                    response.Append("<cell><![CDATA[" + cData.PhoneNumber + "]]></cell>");
                else
                    response.Append("<cell><![CDATA[</cell>");
                response.Append("<cell><![CDATA[" + cData.EMail + "]]></cell>");
                response.Append("<cell><![CDATA[" + 0 + "]]></cell>");
                response.Append("</row>");

                index++;
            }
            response.Append("</rows>");
            Session["posStart"] = index;

            return response.ToString();

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadCustomerList()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "SelectedCustomers.xlsx");
        }

        [HttpPost]
        public JsonResult CustomerHistoryList(string customerID)
        {
            using (var context = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var cid = int.Parse(customerID); ;

                var customerHistoryData = (from log in context.AuditLogs
                                           join ac in context.Audit_Customer on log.AuditID equals ac.AuditID
                                           where log.EntityID == customerID && log.EntityType == "tbl_Customer"
                                           select new
                                           {
                                               log.UserKey,
                                               log.AuditDate,
                                               log.Type,
                                               log.EntityType,
                                               ac.Attribute,
                                               ac.NewValue,
                                               ac.OldValue
                                           }).Concat(
                                           from log in context.AuditLogs
                                           join ac in context.Audit_Contact on log.AuditID equals ac.AuditID
                                           join c in context.tbl_Contacts on log.EntityID equals SqlFunctions.StringConvert((double)c.ContactID).Trim()
                                           where c.CustomerID == cid && log.EntityType == "tbl_Contacts"
                                           select new
                                           {
                                               log.UserKey,
                                               log.AuditDate,
                                               log.Type,
                                               log.EntityType,
                                               ac.Attribute,
                                               ac.NewValue,
                                               ac.OldValue
                                           }).Concat(
                                           from log in context.AuditLogs
                                           join al in context.Audit_Location on log.AuditID equals al.AuditID
                                           join l in context.tbl_Locations on log.EntityID equals SqlFunctions.StringConvert((double)l.LocationID).Trim()
                                           where (l.ActvieCustomerID == cid || l.BilltoCustomerID == cid) && log.EntityType == "tbl_Locations"
                                           select new
                                           {
                                               log.UserKey,
                                               log.AuditDate,
                                               log.Type,
                                               log.EntityType,
                                               al.Attribute,
                                               al.NewValue,
                                               al.OldValue
                                           }).Concat(
                                           from log in context.AuditLogs
                                           join am in context.Audit_Membership on log.AuditID equals am.AuditID
                                           join cm in context.tbl_Customer_Members on log.EntityID equals SqlFunctions.StringConvert((double)cm.MemberID).Trim()
                                           where cm.CustomerID == cid && log.EntityType == "tbl_Customer_Members"
                                           select new
                                           {
                                               log.UserKey,
                                               log.AuditDate,
                                               log.Type,
                                               log.EntityType,
                                               am.Attribute,
                                               am.NewValue,
                                               am.OldValue
                                           }).OrderBy(o => o.AuditDate).ToList();

                var customerHistoryList = new List<CustomerHistoryInfo>();

                for (var i = 0; i < customerHistoryData.Count; i++)
                {
                    var from = customerHistoryData[i].OldValue;
                    var to = customerHistoryData[i].NewValue;

                    var changedby = "";
                    try
                    {
                        changedby = Membership.GetUser(new Guid(customerHistoryData[i].UserKey)).UserName;
                    }
                    catch (Exception)
                    {
                        changedby = "N/A";
                    }

                    customerHistoryList.Add(new CustomerHistoryInfo
                    {
                        FieldName = customerHistoryData[i].Attribute,
                        TableName = customerHistoryData[i].EntityType,
                        ChangeType = customerHistoryData[i].Type,
                        Date = customerHistoryData[i].AuditDate.ToShortDateString(),
                        Time = customerHistoryData[i].AuditDate.ToShortTimeString(),
                        isTablet = "No",
                        ChangedBy = changedby,
                        From = from,
                        To = to
                    });
                }

                return Json(customerHistoryList);
            }
        }

    }

}
