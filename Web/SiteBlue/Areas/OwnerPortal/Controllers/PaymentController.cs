using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using SiteBlue.Data.EightHundred;
using SiteBlue.Business;
using System.Web.Routing;
using SiteBlue.Controllers;
using SiteBlue.Business.Job;
using System.Data.Objects.SqlClient;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class PaymentController : SiteBlueBaseController
    {
        //
        // GET: /OwnerPortal/Payment/
        EightHundredEntities DB = new EightHundredEntities();
        int MyPaymentID;

        public ActionResult Payment(int jobid, FormCollection frmcollection, string Command)
        {
            ViewBag.JobID = jobid.ToString();
            int frid = 0;

            if (frmcollection != null)
            {
                if (jobid != 0)
                {
                    frid = (from fid in DB.tbl_Job 
                            where fid.JobID == jobid 
                            select fid.JobID)
                            .FirstOrDefault();
                }
                if (Command == "Save")
                {
                    try
                    {
                        if (Convert.ToInt32(frmcollection["ddlpayment"]) != 0)
                        {

                            if (!string.IsNullOrEmpty(frmcollection["txtamount"]))
                            {
                                tbl_Payments Payment = new tbl_Payments();
                                Payment.JobID = jobid;
                                if (frmcollection["txtcheckno"] != "")
                                    Payment.CheckNumber = frmcollection["txtcheckno"];
                                Payment.DepositStatus = false;
                                Payment.PaymentAmount = Convert.ToDecimal(frmcollection["txtamount"]);
                                Payment.PaymentDate = Convert.ToDateTime(frmcollection["txtdate"]);
                                Payment.PaymentTypeID = Convert.ToInt32(frmcollection["ddlpayment"]);
                                if (frmcollection["txtdriverlic"] != "")
                                    Payment.DriversLicNUm = frmcollection["txtdriverlic"];
                                
                                Payment.FranchiseID = frid;
                                Payment.CreateDate = DateTime.Now;
                                DB.tbl_Payments.AddObject(Payment);
                                DB.SaveChanges();
                                
                                MyPaymentID = Payment.PaymentID;
                                ViewBag.lblmessage = "Record Save Successfully.";
                            }
                            else
                            {
                                ViewBag.lblmessage = "Please provide an amount before trying to add a new payment.";
                            }
                        }
                        else
                        {
                            ViewBag.lblmessage = "Please select a Payment Type before trying to create a new payment.";
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                if (Command == "Update")
                {
                    try
                    {
                        if (frmcollection["hdnpaymentid"] != "")
                        {
                            MyPaymentID = Convert.ToInt32(frmcollection["hdnpaymentid"]);
                            if (Convert.ToInt32(frmcollection["ddlpayment"]) != 0)
                            {
                                if (!string.IsNullOrEmpty(frmcollection["txtamount"]))
                                {
                                    var Payment = (from p in DB.tbl_Payments where p.PaymentID == MyPaymentID select p).Single();

                                    if (Payment.DepositStatus == true)
                                    {
                                        ViewBag.lblmessage = "This payment has been posted to accounting and may not be changed!";
                                        return RedirectToAction("Index", "Employee");
                                    }

                                    Payment.JobID = jobid;
                                    if (frmcollection["txtcheckno"] != "")
                                        Payment.CheckNumber = frmcollection["txtcheckno"];
                                    Payment.DepositStatus = false;
                                    Payment.PaymentAmount = Convert.ToDecimal(frmcollection["txtamount"]);
                                    Payment.PaymentDate = Convert.ToDateTime(frmcollection["txtdate"]);
                                    Payment.PaymentTypeID = Convert.ToInt32(frmcollection["ddlpayment"]);
                                    if (frmcollection["txtdriverlic"] != "")
                                        Payment.DriversLicNUm = frmcollection["txtdriverlic"];

                                    Payment.FranchiseID = frid;
                                    DB.SaveChanges();
                                   
                                    ViewBag.lblmessage = "Record Update Successfully.";

                                }
                                else
                                {
                                    ViewBag.lblmessage = "Please provide an amount before trying to update the selected payment.";
                                }
                            }
                            else
                            {
                                ViewBag.lblmessage = "Please select a Payment Type before trying to update the selected payment.";
                            }
                        }
                        else
                        {
                            ViewBag.lblmessage = "Please select an existing payment before clicking update.";
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                if (Command == "Delete")
                {
                    try
                    {
                        if (frmcollection["hdnpaymentid"] != "")
                        {
                            MyPaymentID = Convert.ToInt32(frmcollection["hdnpaymentid"]);
                            string tmpstr = null;

                            tmpstr = frmcollection["ddlpayment"] + " for " + Convert.ToDecimal(frmcollection["txtamount"]);

                            //if (Interaction.MsgBox("Are you sure you want to delete the selected payment of " + tmpstr + "?", MsgBoxStyle.YesNo, "Delete Payment") == MsgBoxResult.Yes)
                            //{
                            var payment = (from p in DB.tbl_Payments where p.PaymentID == MyPaymentID select p).Single();

                            if (payment.DepositStatus == true)
                            {
                                ViewBag.lblmessage = "This payment has been posted to accounting and may not be deleted!";
                                return RedirectToAction("Index", "Employee");
                            }

                            DB.tbl_Payments.DeleteObject(payment);
                            DB.SaveChanges();
                            ViewBag.lblmessage = "Record Delete Successfully.";
                            MyPaymentID = 0;

                            //}
                        }
                        else
                        {
                            ViewBag.lblmessage = "Please select an existing payment before clicking delete.";
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                var objjob = (from j in DB.tbl_Job
                              where j.JobID == jobid
                              select j)
                              .FirstOrDefault();

                var objprice = (from j in DB.tbl_Job
                                join t in DB.tbl_Job_Tasks on j.JobID equals t.JobID
                                join tp in DB.tbl_Job_Task_Parts on t.JobTaskID equals tp.JobTaskID
                                where j.JobID == jobid
                                select new { t.Price, t.Quantity })
                                .Distinct();

                decimal total = 0;
                foreach (var item in objprice)
                {
                    decimal dtotal = item.Price * item.Quantity;
                    total = total + dtotal;
                }

                var Paymentlist = (from p in DB.tbl_Payments where p.JobID == objjob.JobID select p);
                decimal totalpayment = 0;
                foreach (var payment in Paymentlist)
                {
                    totalpayment = (Convert.ToDecimal(totalpayment) + Convert.ToDecimal(payment.PaymentAmount));
                }

                objjob.SubTotal = total;
                objjob.TotalSales = total + objjob.TaxAmount;
                objjob.Balance = objjob.TotalSales - totalpayment;
                DB.SaveChanges();

            }

            var lstPaymentType = (from pt in DB.tbl_Payment_Types select pt).ToList();

            ViewBag.PaymentTypeList = new SelectList(lstPaymentType, "PaymentTypeId", "PaymentType");
            ViewBag.jobsid = jobid.ToString();
            ViewBag.statusid = (from s in DB.tbl_Job where s.JobID == jobid select s.StatusID).FirstOrDefault();

            var paymentid = (from p in DB.tbl_Payments where p.JobID == jobid select p).FirstOrDefault();
            if (paymentid != null)
            {
                ViewBag.paymentid = paymentid.PaymentID.ToString();
                ViewBag.PaymentTypeId = paymentid.PaymentTypeID;
            }
            else
            {
                ViewBag.paymentid = "0";
                ViewBag.PaymentTypeId = "";
            }

            if (Command != null)
            {
                return RedirectToAction("../../MyFinances/MyFinances/paymentmethod2");
            }
            else
            {
                PaymentInfo modelPaymentInfo = new PaymentInfo();
                modelPaymentInfo.JobID = jobid;
                var paymenttypelist = (from pt in DB.tbl_Payment_Types select pt).ToList();
                modelPaymentInfo.PaymentTypeList = new SelectList(paymenttypelist, "PaymentTypeId", "PaymentType");

                return View(modelPaymentInfo);
            }
        }

        public ActionResult PaymentProcess(string jobsid, string paymentid, string Command, string paydate, string paytype, string Drivelic, string Checkno, string Amt)
        {

            int jobid = Convert.ToInt32(jobsid);
                int frid = 0;
           
                if (jobid != 0)
                {
                    frid = (from fid in DB.tbl_Job where fid.JobID == jobid select new { fid.JobID }).FirstOrDefault().JobID;
                }
                if (Command == "Save")
                {
                    try
                    {
                                tbl_Payments Payment = new tbl_Payments();
                                Payment.JobID = jobid;
                                Payment.CheckNumber = Checkno;
                                Payment.DepositStatus = false;
                                Payment.PaymentAmount = Convert.ToDecimal(Amt);
                                Payment.PaymentDate = Convert.ToDateTime(paydate);
                                Payment.PaymentTypeID = Convert.ToInt32(paytype);
				Payment.CreateDate = DateTime.Now;
                                if (Drivelic != "")
                                    Payment.DriversLicNUm = Drivelic;
                                //Payment.FranchiseID = 38;
                                Payment.FranchiseID = frid;
                                DB.tbl_Payments.AddObject(Payment);
                                DB.SaveChanges();
                                MyPaymentID = Payment.PaymentID;
                                ViewBag.lblmessage = "Record Save Successfully.";
                        
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                if (Command == "Update")
                {
                    try
                    {
                        if (paymentid != "")
                        {
                            MyPaymentID = Convert.ToInt32(paymentid);
                            if (Convert.ToInt32(paytype) != 0)
                            {
                                if (!string.IsNullOrEmpty(Amt))
                                {
                                    var Payment = (from p in DB.tbl_Payments where p.PaymentID == MyPaymentID select p).Single();

                                    if (Payment.DepositStatus == true)
                                    {
                                        ViewBag.lblmessage = "This payment has been posted to accounting and may not be changed!";
                                        return RedirectToAction("Index", "Employee");
                                    }

                                    Payment.JobID = jobid;
                                    if (Checkno != "")
                                        Payment.CheckNumber = Checkno;
                                    Payment.DepositStatus = false;
                                    Payment.PaymentAmount = Convert.ToDecimal(Amt);
                                    Payment.PaymentDate = Convert.ToDateTime(paydate);
                                    Payment.PaymentTypeID = Convert.ToInt32(paytype);
                                    if (Drivelic != "")
                                        Payment.DriversLicNUm = Drivelic;
                                    //Payment.FranchiseID = 38;
                                    Payment.FranchiseID = frid;
                                    DB.SaveChanges();

                                    ViewBag.lblmessage = "Record Update Successfully.";

                                }
                                else
                                {
                                    ViewBag.lblmessage = "Please provide an amount before trying to update the selected payment.";
                                }
                            }
                            else
                            {
                                ViewBag.lblmessage = "Please select a Payment Type before trying to update the selected payment.";
                            }
                        }
                        else
                        {
                            ViewBag.lblmessage = "Please select an existing payment before clicking update.";
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                if (Command == "Delete")
                {
                    try
                    {
                        if (paymentid != "")
                        {
                            MyPaymentID = Convert.ToInt32(paymentid);
                        

                            var payment = (from p in DB.tbl_Payments where p.PaymentID == MyPaymentID select p).Single();

                            if (payment.DepositStatus == true)
                            {
                                ViewBag.lblmessage = "This payment has been posted to accounting and may not be deleted!";
                                return RedirectToAction("Index", "Employee");
                            }

                            DB.tbl_Payments.DeleteObject(payment);
                            DB.SaveChanges();
                            ViewBag.lblmessage = "Record Delete Successfully.";
                            MyPaymentID = 0;

                            //}
                        }
                        else
                        {
                            ViewBag.lblmessage = "Please select an existing payment before clicking delete.";
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                var objjob = (from j in DB.tbl_Job
                              where j.JobID == jobid
                              select j)
                              .FirstOrDefault();

                var objprice = (from j in DB.tbl_Job
                                join t in DB.tbl_Job_Tasks on j.JobID equals t.JobID
                                join tp in DB.tbl_Job_Task_Parts on t.JobTaskID equals tp.JobTaskID
                                where j.JobID == jobid
                                select new { t.Price, t.Quantity })
                                .Distinct();

                decimal total = 0;
                foreach (var item in objprice)
                {
                    decimal dtotal = item.Price * item.Quantity;
                    total = total + dtotal;
                }

                var Paymentlist = (from p in DB.tbl_Payments where p.JobID == objjob.JobID select p);
                decimal totalpayment = 0;
                foreach (var payment in Paymentlist)
                {
                    totalpayment = (Convert.ToDecimal(totalpayment) + Convert.ToDecimal(payment.PaymentAmount));
                }

                objjob.SubTotal = total;
                objjob.TotalSales = total + objjob.TaxAmount;
                objjob.Balance = objjob.TotalSales - totalpayment;
                DB.SaveChanges();

            
            return Json("");

        }

        public ActionResult PaymentList(int jobid)
        {
            var paymentdata = (from p in DB.tbl_Payments
                               join pt in DB.tbl_Payment_Types on p.PaymentTypeID equals pt.PaymentTypeId into tpt
                               from pt in tpt.DefaultIfEmpty()
                               where p.JobID == jobid
                               select new
                               {
                                   p.PaymentDate,
                                   pt.PaymentType,
                                   p.DriversLicNUm,
                                   p.CheckNumber,
                                   p.PaymentAmount,
                                   p.PaymentID,
                                   PaymentTypeId = ((int?)pt.PaymentTypeId) ?? 0
                               }).ToList();

            var paymentlist = (from t in paymentdata
                               select new
                               {
                                   paydate = (t.PaymentDate != null) ? t.PaymentDate.Value.ToShortDateString() : "",
                                   paytype = t.PaymentType ?? "N/A",
                                   paydrivelicno = t.DriversLicNUm,
                                   paycheckno = t.CheckNumber,
                                   payamt = t.PaymentAmount,
                                   paymentid = t.PaymentID,
                                   paytypeid = t.PaymentTypeId
                               });

            var total = string.Format("{0:C}", paymentdata.Sum(q => q.PaymentAmount ?? 0));

            return Json(new { paymentlist = paymentlist, total = total });
        }

        public ActionResult PaymentListMultiple(string jobid)
        {
            jobid = jobid.Substring(0, jobid.Length - 1);

            var paymentlist = new object();
            var total = new object();

            string[] invoiceNo = null;

            invoiceNo = jobid.Split(',');
            int[] values = new int[invoiceNo.Length];

            for (int x = 0; x < invoiceNo.Length; x++)
            {
                values[x] = Convert.ToInt32(invoiceNo[x].ToString());
            }

            var paymentdata = (from p in DB.tbl_Payments
                               join pt in DB.tbl_Payment_Types on p.PaymentTypeID equals pt.PaymentTypeId into tpt
                               from pt in tpt.DefaultIfEmpty()
                               where values.Contains(p.JobID)
                               select new
                               {
                                   p.PaymentDate,
                                   pt.PaymentType,
                                   p.DriversLicNUm,
                                   p.CheckNumber,
                                   p.PaymentAmount,
                                   p.PaymentID,
                                   p.JobID,
                                   PaymentTypeId = ((int?)pt.PaymentTypeId) ?? 0
                               }).ToList();


            total = string.Format("{0:C}", paymentdata.Sum(q => q.PaymentAmount ?? 0));
            paymentlist = (from t in paymentdata
                           select new
                           {
                               paydate = (t.PaymentDate != null) ? t.PaymentDate.Value.ToShortDateString() : "",
                               paytype = t.PaymentType ?? "N/A",
                               paydrivelicno = t.DriversLicNUm,
                               paycheckno = t.CheckNumber,
                               payamt = t.PaymentAmount,
                               paymentid = t.PaymentID,
                               paytypeid = t.PaymentTypeId,
                               jobid = t.JobID
                           });

            return Json(new { paymentlist = paymentlist, total = total });

        }

        public InvoiceFinancialDetail RecalcJobData(int jobid, int customerid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var total = (from j in ctx.tbl_Job
                             join t in ctx.tbl_Job_Tasks on j.JobID equals t.JobID
                             where j.JobID == jobid
                             select new { t.Price, t.Quantity }).ToArray().Sum(x => x.Price * x.Quantity);

                var totalpayment = (from p in ctx.tbl_Payments
                                    where p.JobID == jobid
                                    select p.PaymentAmount).Sum() ?? 0;

                var objjob = (from j in ctx.tbl_Job
                              where j.JobID == jobid
                              select j)
                    .FirstOrDefault();

                objjob.SubTotal = total;
                objjob.TotalSales = total + objjob.TaxAmount;
                objjob.Balance = objjob.TotalSales - totalpayment;
                    
                ctx.SaveChanges();

                var finance = new InvoiceFinancialDetail()
                                  {
                                      SubTotal = String.Format("{0:C}", objjob.SubTotal).Replace("$", ""),
                                      Total = String.Format("{0:C}", objjob.TotalSales).Replace("$", ""),
                                      Balance = String.Format("{0:C}", objjob.Balance).Replace("$", ""),
                                      TotalPaid = String.Format("{0:C}", totalpayment).Replace("$", "")
                                  };

                var customerbalance = (from h in ctx.tbl_Job
                                       where h.CustomerID == customerid
                                       group h by h.CustomerID into g
                                       select new
                                                  {
                                                      TotalBalance = g.Sum(x => x.Balance)
                                                  })
                    .Single();

                finance.CustomerBalance = String.Format("{0:C}", (decimal?)customerbalance.TotalBalance ?? 0).Replace("$", "");

                return finance;
            }
        }

        public InvoiceFinancialDetail RecalcMultipleJobData(int jobid, int customerid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                try
                {
                    var job = (from j in ctx.tbl_Job
                               where j.JobID == jobid
                               select j)
                                  .FirstOrDefault();

                    var totalpayment = (from p in ctx.tbl_Payments
                                        where p.JobID == jobid
                                        select p.PaymentAmount).Sum() ?? 0;

                    job.Balance = job.Balance - totalpayment;

                    ctx.SaveChanges();

                    var finance = new InvoiceFinancialDetail()
                    {
                        SubTotal = String.Format("{0:C}", job.SubTotal).Replace("$", ""),
                        Total = String.Format("{0:C}", job.TotalSales).Replace("$", ""),
                        Balance = String.Format("{0:C}", job.Balance).Replace("$", ""),
                        TotalPaid = String.Format("{0:C}", totalpayment).Replace("$", "")
                    };

                    var customerbalance = (from h in ctx.tbl_Job
                                           where h.CustomerID == customerid
                                           group h by h.CustomerID into g
                                           select new
                                           {
                                               TotalBalance = g.Sum(x => x.Balance)
                                           })
                                           .Single();

                    finance.CustomerBalance = String.Format("{0:C}", customerbalance.TotalBalance).Replace("$", "");
                    
                    return finance;
                }
                catch (Exception)
                {
                }

                return null;
            }
        }

        public ActionResult InsertPayment(tbl_Payments paymentdata)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                try
                {
                    var job = (from j in ctx.tbl_Job
                               where j.JobID == paymentdata.JobID
                               select new
                               {
                                   j.FranchiseID,
                                   j.CustomerID
                               })
                                .First();

                    paymentdata.FranchiseID = job.FranchiseID;
                    paymentdata.DepositStatus = false;
                    paymentdata.CreateDate = DateTime.Now;
                    ctx.tbl_Payments.AddObject(paymentdata);
                    ctx.SaveChanges();

                    InvoiceFinancialDetail finance = RecalcJobData(paymentdata.JobID, job.CustomerID);

                    return Json(finance);
                }
                catch (Exception)
                {
                }

                return Json("fail");
            }
        }

        public ActionResult InsertMultiplePayment(tbl_Payments[] paymentdata, DateTime PaymentDate)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var overpayments = 0;
                var underpayments = 0;
                var finance = new InvoiceFinancialDetail();
                decimal amountRemaining = 0;

                paymentdata = paymentdata.OrderBy(x => x.PaymentDate).ToArray();

                for (var i = 0; i < paymentdata.Length; i++)
                {
                    var jobId = paymentdata[i].JobID;
                    
                    if (overpayments > 0)
                        paymentdata[i].PaymentAmount = amountRemaining;

                    var paymentAmount = Convert.ToDecimal(paymentdata[i].PaymentAmount);

                    var job = (from j in ctx.tbl_Job
                               where j.JobID == jobId
                               select new
                                          {
                                              j.CustomerID,
                                              j.Balance
                                          })
                        .First();

                    var currentBalance = job.Balance;

                    if (paymentAmount > currentBalance)
                    {
                        paymentdata[i].PaymentAmount = currentBalance;

                        amountRemaining = paymentAmount - currentBalance;
                        if (i == paymentdata.Length - 1)
                            paymentdata[i].PaymentAmount += amountRemaining;

                        overpayments++;
                    }

                    if (paymentAmount < currentBalance)
                    {
                        underpayments++;

                        paymentdata[i].PaymentAmount = amountRemaining == 0 ? 0 : amountRemaining;

                        if (i == paymentdata.Length - 1)
                            paymentdata[i].PaymentAmount = amountRemaining == 0 ? 0 : amountRemaining;
                    }

                    if (paymentAmount == currentBalance)
                        paymentdata[i].PaymentAmount = paymentAmount;

                    if (underpayments > 1)
                        paymentdata[i].PaymentAmount = 0;

                    paymentdata[i].PaymentDate = PaymentDate;
                    paymentdata[i].CreateDate = DateTime.Now;
                    ctx.tbl_Payments.AddObject(paymentdata[i]);
                    ctx.SaveChanges();
                    finance = RecalcMultipleJobData(paymentdata[i].JobID, job.CustomerID);
                    i++;

                }

                return Json(finance);
            }
        }

        public ActionResult DeletePayment(tbl_Payments paymentdata)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var payment = (from p in ctx.tbl_Payments where p.PaymentID == paymentdata.PaymentID select p).Single();

                if (payment.DepositStatus == true)
                {
                    return Json("posted");
                }

                ctx.tbl_Payments.DeleteObject(payment);
                ctx.SaveChanges();

                var CustomerID = ctx.tbl_Job.First(q => q.JobID == paymentdata.JobID).CustomerID;

                InvoiceFinancialDetail finance = RecalcJobData(paymentdata.JobID, CustomerID);

                return Json(finance);
            }
        }

        public ActionResult UpdatePayment(tbl_Payments paymentdata)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                try
                {
                    var payment = (from p in ctx.tbl_Payments where p.PaymentID == paymentdata.PaymentID select p).Single();

                    if (payment.DepositStatus == true)
                    {
                        return Json("posted");
                    }

                    payment.DepositStatus = false;
                    payment.PaymentAmount = paymentdata.PaymentAmount;
                    payment.PaymentDate = paymentdata.PaymentDate;
                    payment.PaymentTypeID = paymentdata.PaymentTypeID;
                    payment.CheckNumber = paymentdata.CheckNumber;
                    payment.DriversLicNUm = paymentdata.DriversLicNUm;
                    ctx.SaveChanges();

                    var CustomerID = ctx.tbl_Job.First(q => q.JobID == paymentdata.JobID).CustomerID;

                    InvoiceFinancialDetail finance = RecalcJobData(paymentdata.JobID, CustomerID);

                    return Json(finance);
                }
                catch (Exception)
                {
                }

                return Json("fail");
            }
        }
    }
}
