using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using SiteBlue.Business.Job;
using SiteBlue.Controllers;
using SiteBlue.Data.EightHundred;
using SiteBlue.Business;
using System.Text;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class JobTaskController : SiteBlueBaseController
    {
        //
        // GET: /OwnerPortal/JobTask/
        
        public static int inoofrows;

        public ActionResult ManageTask(FormCollection frmcollection, int JobId, string command)
        {
            var objmodcommon = new mod_common(UserInfo.UserKey);

            using (var DB = GetContext())
            {
                if (Request.QueryString["JobId"] != null && Request.QueryString["JobId"] != "")
                {
                    ViewBag.jobsid = Request.QueryString["JobId"].ToString();

                    #region "Update Task Part"

                    if (command == "Update")
                    {
                        try
                        {
                            if (Convert.ToString(frmcollection["txtCode"]) != "" &&
                                Convert.ToString(frmcollection["txtCode"]) != null)
                            {
                                tbl_Job_Tasks objjobtask = new tbl_Job_Tasks();
                                int taskid = Convert.ToInt32(frmcollection["hdntaskid"]);

                                objjobtask = (from t in DB.tbl_Job_Tasks
                                              where t.JobID == JobId && t.JobTaskID == taskid
                                              select t)
                                              .FirstOrDefault();

                                if (objjobtask != null)
                                {
                                    if (frmcollection["chkMember"].ToString() == "true,false")
                                    {
                                        objjobtask.MemberYN = true;
                                    }
                                    else
                                    {
                                        objjobtask.MemberYN = false;
                                    }
                                    if (frmcollection["chkAddon"].ToString() == "true,false")
                                    {
                                        objjobtask.AddOnYN = true;
                                    }
                                    else
                                    {
                                        objjobtask.AddOnYN = false;
                                    }

                                    try
                                    {
                                        objjobtask.Quantity = Convert.ToDecimal(frmcollection["txtQty"]);
                                    }
                                    catch (Exception)
                                    {
                                        objjobtask.Quantity = 0;
                                    }

                                    try
                                    {
                                        objjobtask.Price = Convert.ToDecimal(frmcollection["txtPrice"]);
                                    }
                                    catch (Exception)
                                    {
                                        objjobtask.Price = 0;
                                    }

                                    objjobtask.JobCodeDescription = Convert.ToString(frmcollection["txtDescription"]);
                                    objjobtask.JobCode = Convert.ToString(frmcollection["txtCode"]);

                                    try
                                    {
                                        objjobtask.JobCodeID = Convert.ToInt32(frmcollection["ddlcode"]);
                                    }
                                    catch (Exception)
                                    {
                                        objjobtask.JobCodeID = 0;
                                    }
                                }
                            }

                            if (Convert.ToString(frmcollection["txtPartno"]) != "" &&
                                Convert.ToString(frmcollection["txtPartno"]) != null)
                            {

                                int taskid = Convert.ToInt32(frmcollection["hdntaskid"]);
                                int taskpartid = Convert.ToInt32(frmcollection["hdntaskpartid"]);

                                tbl_Job_Task_Parts objjobtaskparts = new tbl_Job_Task_Parts();
                                objjobtaskparts =
                                    (from tp in DB.tbl_Job_Task_Parts
                                     where tp.JobTaskPartsID == taskpartid && tp.JobTaskID == taskid
                                     select tp)
                                     .FirstOrDefault();

                                if (objjobtaskparts != null)
                                {
                                    objjobtaskparts.PartsID = Convert.ToInt32(frmcollection["ddlparts"]);
                                    objjobtaskparts.Quantity = Convert.ToDecimal(frmcollection["txtPartQty"]);
                                    objjobtaskparts.Price = Convert.ToDecimal(frmcollection["txtPartPrice"]);
                                    objjobtaskparts.PartCode = Convert.ToString(frmcollection["txtPartno"]).Trim();
                                    objjobtaskparts.PartName = Convert.ToString(frmcollection["txtPartDescription"]);
                                }
                            }

                            DB.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                                           validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    #endregion

                    #region "Delete Task Part"
                    if (command == "Delete")
                    {
                        using (var scope = new TransactionScope())
                        {
                            var taskid = Convert.ToInt32(frmcollection["hdntaskid"]);

                            if (Convert.ToString(frmcollection["txtPartno"]) != "" &&
                                Convert.ToString(frmcollection["txtPartno"]) != null)
                            {

                                var taskpartid = Convert.ToInt32(frmcollection["hdntaskpartid"]);

                                var objjobtaskparts = (from tp in DB.tbl_Job_Task_Parts
                                                       where tp.JobTaskPartsID == taskpartid
                                                       select tp)
                                    .SingleOrDefault();

                                if (objjobtaskparts != null)
                                    DB.tbl_Job_Task_Parts.DeleteObject(objjobtaskparts);
                            }

                            DB.SaveChanges();

                            //if no parts are left on the task, delete the taks.
                            if (!DB.tbl_Job_Task_Parts.Any(p => p.JobTaskID == taskid))
                            {
                                var task = DB.tbl_Job_Tasks.Single(jt => jt.JobTaskID == taskid);
                                DB.tbl_Job_Tasks.DeleteObject(task);
                            }

                            DB.SaveChanges();
                            scope.Complete();
                        }
                    }
                    #endregion

                    #region "Add Part"
                    if (command == "Add Parts")
                    {
                        try
                        {
                            // && inoofrows > 0
                            if (Convert.ToInt32(frmcollection["ddlparts"]) != 0)
                            {
                                if (frmcollection["hdntaskid"] != "")
                                {
                                    //there is a selected task to add the part to 
                                    int holdpartid = Convert.ToInt32(frmcollection["ddlparts"]);
                                    var addpart = (from a in DB.tbl_PB_Parts
                                                   where a.PartID == holdpartid
                                                   select a)
                                                   .Single();

                                    int holdtaskid = Convert.ToInt32(frmcollection["hdntaskid"]);

                                    var task = (from t in DB.tbl_Job_Tasks
                                                where t.JobTaskID == holdtaskid
                                                select t)
                                                .Single();

                                    tbl_Job_Task_Parts part = new tbl_Job_Task_Parts
                                    {
                                        JobTaskID = holdtaskid,
                                        PartCode = objmodcommon.GetPartCode(holdpartid).Trim(),
                                        PartName = objmodcommon.GetPartName(holdpartid).Trim(),
                                        PartsID = addpart.PartID
                                    };

                                    if (task.MemberYN)
                                    {
                                        if (task.AddOnYN)
                                        {
                                            part.Price = addpart.PartAddonMemberPrice;
                                        }
                                        else
                                        {
                                            part.Price = addpart.PartMemberPrice;
                                        }
                                    }
                                    else
                                    {
                                        if (task.AddOnYN)
                                        {
                                            part.Price = addpart.PartAddonStdPrice;
                                        }
                                        else
                                        {
                                            part.Price = addpart.PartStdPrice;
                                        }
                                    }

                                    part.Quantity = 1;
                                    DB.tbl_Job_Task_Parts.AddObject(part);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    ViewBag.lblmessage = "Please select a task to add the part to.";
                                }
                            }
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                                           validationError.ErrorMessage);
                                }
                            }
                        }
                    }

                    #endregion

                    #region "Add Task"
                    if (command == "Add Code")
                    {
                        try
                        {
                            if (Convert.ToInt32(frmcollection["ddlcode"]) != 0)
                            {
                                int holdjobcodeid = Convert.ToInt32(frmcollection["ddlcode"]);
                                var addtask = (from a in DB.tbl_PB_JobCodes
                                               where a.JobCodeID == holdjobcodeid
                                               select
                                                   new
                                                       {
                                                           a.JobAddonMemberPrice,
                                                           a.JobAddonStdPrice,
                                                           a.JobCost,
                                                           a.JobCode,
                                                           a.JobCodeID,
                                                           a.JobCodeDescription
                                                       })
                                                       .Single();

                                var job = (from j in DB.tbl_Job
                                           where j.JobID == JobId
                                           select new
                                           {
                                               j.CustomerID,
                                               j.BusinessTypeID
                                           })
                                           .Single();

                                var customerID = job.CustomerID;
                                var customer = objmodcommon.GetCustomers(customerID);
                                //get a new task record and save

                                tbl_Job_Tasks task = new tbl_Job_Tasks();
                                if (inoofrows > 0)
                                {
                                    task.AddOnYN = true;

                                    //if the customer is a member do stuff
                                    var member = (from m in DB.tbl_Customer_Members where m.CustomerID == customerID select m);

                                    bool memberyn = false;
                                    foreach (var item in member)
                                    {
                                        memberyn = true;
                                    }

                                    if (memberyn)
                                    {
                                        task.Price = addtask.JobAddonMemberPrice;
                                    }
                                    else
                                    {
                                        task.Price = addtask.JobAddonStdPrice;
                                    }

                                    task.Quantity = 1;
                                }
                                else
                                {
                                    task.AddOnYN = false;
                                    //if the customer is a member do stuff
                                    var member = (from m in DB.tbl_Customer_Members where m.CustomerID == customerID select m);

                                    bool memberyn = false;
                                    foreach (var item in member)
                                    {
                                        memberyn = true;
                                    }

                                    if (memberyn)
                                    {
                                        task.Price = addtask.JobAddonMemberPrice;
                                    }
                                    else
                                    {
                                        task.Price = addtask.JobAddonStdPrice;
                                    }

                                    task.Quantity = 1;

                                }
                                task.AuthorizedYN = true;
                                task.Cost = addtask.JobCost;
                                task.JobCodeID = addtask.JobCodeID;
                                task.JobCode = addtask.JobCode;
                                task.JobCodeDescription = addtask.JobCodeDescription;
                                task.JobID = JobId;

                                int tmpBusID = job.BusinessTypeID;
                                int tmpCodeID = addtask.JobCodeID;
                                task.AccountCode = objmodcommon.Get_Account_Code(tmpCodeID, tmpBusID);

                                //System.Text.Encoding enc = System.Text.Encoding.ASCII;
                                //byte[] ByteArray = enc.GetBytes(DateTime.Now.ToShortTimeString());

                                //task.timestamp = DBNull.Value;
                                DB.tbl_Job_Tasks.AddObject(task);
                                DB.SaveChanges();

                                int myjobcodeID = task.JobCodeID;
                                //now add any parts associated with this task
                                var partlist = (from p in DB.tbl_PB_JobCodes_Details where p.JobCodeID == myjobcodeID select p);

                                bool partadded = false;
                                foreach (var partrec in partlist)
                                {
                                    tbl_Job_Task_Parts part = new tbl_Job_Task_Parts();
                                    part.JobTaskID = task.JobTaskID;
                                    int holdpartid = partrec.PartID;
                                    part.PartCode = objmodcommon.GetPartCode(holdpartid).Trim();
                                    part.PartName = objmodcommon.GetPartName(holdpartid);
                                    part.PartsID = partrec.PartID;

                                    if (task.AddOnYN)
                                    {
                                        if (task.MemberYN)
                                        {
                                            part.Price = partrec.PartAddonMemberPrice;
                                        }
                                        else
                                        {
                                            part.Price = partrec.PartAddonStdPrice;
                                        }
                                    }
                                    else
                                    {
                                        if (task.MemberYN)
                                        {
                                            part.Price = partrec.PartMemberPrice;
                                        }
                                        else
                                        {
                                            part.Price = partrec.PartStdPrice;
                                        }
                                    }
                                    part.Quantity = task.Quantity * partrec.Qty;
                                    DB.tbl_Job_Task_Parts.AddObject(part);
                                    partadded = true;
                                }

                                if (partadded)
                                {
                                    DB.SaveChanges();
                                }

                            }
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                                           validationError.ErrorMessage);
                                }
                            }
                        }

                    }
                    #endregion

                    var total = (from j in DB.tbl_Job
                                 join t in DB.tbl_Job_Tasks on j.JobID equals t.JobID
                                 where j.JobID == JobId
                                 select new { t.Price, t.Quantity }).ToArray().Sum(x => x.Price * x.Quantity);

                    var totalpayment = (from p in DB.tbl_Payments
                                        where p.JobID == JobId
                                        select p.PaymentAmount).Sum() ?? 0;

                    var objjob = (from j in DB.tbl_Job
                                  where j.JobID == JobId
                                  select j)
                                  .FirstOrDefault();

                    objjob.SubTotal = total;
                    objjob.TotalSales = total + objjob.TaxAmount;
                    objjob.Balance = objjob.TotalSales - totalpayment;

                    DB.SaveChanges();
                }

                var jobInfo = (from j in DB.tbl_Job
                               join t in DB.tbl_Employee on j.ServiceProID equals t.EmployeeID into t1
                               from t in t1.DefaultIfEmpty()
                               join tt in DB.tbl_Franchise_Tablets on t.EmployeeID equals tt.EmployeeID into tt1
                               from tt in tt1.DefaultIfEmpty()
                               where j.JobID == JobId
                               select new { j.FranchiseID })
                               .Single();

                var pricebooks = DB.tbl_PriceBook.Where(pb => pb.FranchiseID == jobInfo.FranchiseID).ToArray();
                var priceBookId = Convert.ToInt32(string.IsNullOrEmpty(frmcollection["ddlPriceBook"])
                                            ? "0"
                                            : frmcollection["ddlPriceBook"]);

                if (priceBookId == 0 && pricebooks.Length > 0)
                    priceBookId = pricebooks.First().PriceBookID;

                ViewBag.PriceBooks = pricebooks;
                ViewBag.PriceBookId = priceBookId;

                if (command == "Search Code")
                {
                    string strjobcode = frmcollection["txtjobcode"].ToString();
                    var TaskList = (from b in DB.tbl_PB_JobCodes
                                    join c in DB.tbl_PB_SubSection on b.SubSectionID equals c.SubsectionID
                                    join d in DB.tbl_PB_Section on c.SectionID equals d.SectionID
                                    join e in DB.tbl_PriceBook on d.PriceBookID equals e.PriceBookID
                                    where e.FranchiseID == jobInfo.FranchiseID && e.ActiveBookYN && priceBookId == e.PriceBookID
                                    orderby b.JobCode
                                    select new { b.JobCodeID, Code = b.JobCode + " - " + b.JobCodeDescription });

                    TaskList = TaskList.Where(p => p.Code.Contains(strjobcode));

                    ViewBag.taskcode = TaskList.ToArray();
                }
                else
                {
                    var TaskList = (from b in DB.tbl_PB_JobCodes
                                    join c in DB.tbl_PB_SubSection on b.SubSectionID equals c.SubsectionID
                                    join d in DB.tbl_PB_Section on c.SectionID equals d.SectionID
                                    join e in DB.tbl_PriceBook on d.PriceBookID equals e.PriceBookID
                                    where e.FranchiseID == jobInfo.FranchiseID && e.ActiveBookYN && priceBookId == e.PriceBookID
                                    orderby b.JobCode
                                    select new { b.JobCodeID, Code = b.JobCode + " - " + b.JobCodeDescription });
                    ViewBag.taskcode = TaskList.ToArray();
                }

                if (command == "Search Parts")
                {
                    string strjobparts = frmcollection["txtPartcode"].ToString();
                    var Partslist = (from s in DB.tbl_PB_Parts
                                     join t in DB.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                                     join m in DB.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                                     where m.FranchiseID == jobInfo.FranchiseID && t.ActiveBookYN && priceBookId == t.PriceBookID
                                     orderby m.PartCode
                                     select new { s.PartID, PCode = m.PartCode + " - " + m.PartName });
                    Partslist = Partslist.Where(p => p.PCode.Contains(strjobparts));

                    ViewBag.partlist = Partslist.ToArray();

                }
                else
                {
                    var Partslist = (from s in DB.tbl_PB_Parts
                                     join t in DB.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                                     join m in DB.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                                     where m.FranchiseID == jobInfo.FranchiseID && t.ActiveBookYN && priceBookId == t.PriceBookID
                                     orderby m.PartCode
                                     select new { s.PartID, PCode = m.PartCode + " - " + m.PartName });
                    ViewBag.partlist = Partslist.ToArray();
                }
            }

            return View();

        }

        public ActionResult JobTaskDetails(int jobsid)
        {
            var jobID = jobsid;

            var lsttaskdetails = new List<JobTaskDetails>();
            JobTaskDetails summaryjobtaskdetails;
            var objjobtaskdetails = new JobTaskDetails();

            using (var DB = GetContext())
            {
                var objjobtask = (from jobtask in DB.tbl_Job_Tasks
                                  join jobtaskparts in DB.tbl_Job_Task_Parts on jobtask.JobTaskID equals
                                      jobtaskparts.JobTaskID into parts
                                  from p in parts.DefaultIfEmpty()
                                  where jobtask.JobID == jobID
                                  select new
                                             {
                                                 jobtask.MemberYN,
                                                 JobTaskPartsID = p == null ? 0 : p.JobTaskPartsID,
                                                 jobtask.AccountCode,
                                                 jobtask.JobTaskID,
                                                 jobtask.Quantity,
                                                 jobtask.JobCode,
                                                 jobtask.JobCodeDescription,
                                                 jobtask.Price,
                                                 p.PartName,
                                                 partquantity = p == null ? 0 : p.Quantity,
                                                 partprice = p == null ? 0 : p.Price,
                                                 p.PartCode,
                                                 jobtask.AddOnYN
                                             });

                inoofrows = objjobtask.Count();

                foreach (var jt in objjobtask)
                {
                    objjobtaskdetails.addonyn = jt.AddOnYN;
                    objjobtaskdetails.JobTaskPartsID = jt.JobTaskPartsID;
                    objjobtaskdetails.jobtaskid = jt.JobTaskID;
                    objjobtaskdetails.taskQty = jt.Quantity;
                    objjobtaskdetails.Code = jt.JobCode;
                    objjobtaskdetails.TaskDescription = jt.JobCodeDescription;
                    objjobtaskdetails.Unit = jt.Price;
                    objjobtaskdetails.Line = jt.Price * jt.Quantity;
                    objjobtaskdetails.memberyn = jt.MemberYN;
                    if (jt.AddOnYN == false)
                    {
                        objjobtaskdetails.Part = jt.PartCode + " NoAuth!";
                    }
                    else
                    {
                        objjobtaskdetails.Part = jt.PartCode;
                    }
                    if (jt.AccountCode == "0" || jt.AccountCode == "" || jt.AccountCode == "00000")
                    {
                        objjobtaskdetails.PartDesc = jt.PartName + " NoAcct!";
                    }
                    else
                    {
                        objjobtaskdetails.PartDesc = jt.PartName;
                    }


                    //objjobtaskdetails.PartDesc = jt.PartName;
                    objjobtaskdetails.PartQty = jt.partquantity;
                    objjobtaskdetails.Price = jt.partprice;

                    summaryjobtaskdetails =
                        new JobTaskDetails
                            {
                                addonyn = objjobtaskdetails.addonyn,
                                JobTaskPartsID = objjobtaskdetails.JobTaskPartsID,
                                jobtaskid = objjobtaskdetails.jobtaskid,
                                taskQty = objjobtaskdetails.taskQty,
                                Code = objjobtaskdetails.Code,
                                TaskDescription = objjobtaskdetails.TaskDescription,
                                Unit = objjobtaskdetails.Unit,
                                Line = objjobtaskdetails.Line,
                                Part = objjobtaskdetails.Part,
                                PartDesc = objjobtaskdetails.PartDesc,
                                PartQty = objjobtaskdetails.PartQty,
                                Price = objjobtaskdetails.Price,
                                strLine = string.Format("{0:C}", objjobtaskdetails.Line),
                                strUnit = string.Format("{0:C}", objjobtaskdetails.Unit),
                                strPrice = string.Format("{0:C}", objjobtaskdetails.Price),
                                memberyn = objjobtaskdetails.memberyn,
                                statusid =
                                    (from j in DB.tbl_Job where j.JobID == jobID select j.StatusID).FirstOrDefault()

                            };

                    lsttaskdetails.Add(summaryjobtaskdetails);

                }
            }

            return Json(lsttaskdetails);
        }

        public ActionResult JobTaskList(int jobid, int? actype)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var jobtaskdata = (from jobtask in ctx.tbl_Job_Tasks
                                   join jobtaskparts in ctx.tbl_Job_Task_Parts on jobtask.JobTaskID equals jobtaskparts.JobTaskID into parts
                                   from p in parts.DefaultIfEmpty()
                                   where jobtask.JobID == jobid
                                   select new
                                   {
                                       JobTaskPartsID = ((int?)p.JobTaskPartsID) ?? 0,
                                       jobtask.AccountCode,
                                       jobtask.JobTaskID,
                                       jobtask.Quantity,
                                       jobtask.JobCode,
                                       jobtask.JobCodeDescription,
                                       jobtask.Price,
                                       p.PartName,
                                       PartQuantity = ((decimal?)p.Quantity) ?? 0,
                                       PartPrice = ((decimal?)p.Price) ?? 0,
                                       p.PartCode,
                                       jobtask.AddOnYN,
                                       jobtask.MemberYN
                                   }).ToList();

                var jobtasklist = (from t in jobtaskdata
                                   select new
                                   {
                                       t.JobTaskPartsID,
                                       t.JobTaskID,
                                       t.Quantity,
                                       t.AddOnYN,
                                       t.MemberYN,
                                       t.PartQuantity,
                                       PartPrice = string.Format("{0:C}", t.PartPrice),
                                       Code = t.JobCode,
                                       TaskDesc = t.JobCodeDescription,
                                       Unit = string.Format("{0:C}", t.Price),
                                       Line = string.Format("{0:C}", t.Price * t.Quantity),
                                       Part = (t.AddOnYN) ? t.PartCode ?? "" : t.PartCode,
                                       PartDesc = (t.AccountCode == "0" || t.AccountCode == "" || t.AccountCode == "00000") ? t.PartName + " NoAcct!" : t.PartName ?? "",
                                       t.AccountCode
                                   });

                return Json(jobtasklist);
            }
        }

        public InvoiceFinancialDetail RecalcJobData(int jobid, int customerid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                try
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

                    finance.CustomerBalance = String.Format("{0:C}", customerbalance.TotalBalance).Replace("$", "");

                    return finance;
                }
                catch (Exception)
                {
                }

                return null;
            }
        }

        public ActionResult AddCode(int jobcodeid, int jobid)
        {
            var objmodcommon = new mod_common(UserInfo.UserKey);

            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                try
                {
                    //task
                    var task = (from jc in ctx.tbl_PB_JobCodes
                                where jc.JobCodeID == jobcodeid
                                select new
                                {
                                    jc.JobAddonMemberPrice,
                                    jc.JobAddonStdPrice,
                                    jc.JobCost,
                                    jc.JobCode,
                                    jc.JobCodeID,
                                    jc.JobCodeDescription
                                }).Single();

                    var job = (from j in ctx.tbl_Job
                               where j.JobID == jobid
                               select new
                               {
                                   j.CustomerID,
                                   j.BusinessTypeID
                               }).Single();

                    var newtask = new tbl_Job_Tasks()
                    {
                        Cost = task.JobCost,
                        JobCodeID = task.JobCodeID,
                        JobCode = task.JobCode,
                        JobCodeDescription = task.JobCodeDescription,
                        JobID = jobid,
                        AuthorizedYN = true,
                        AddOnYN = true,
                        Quantity = 1
                    };

                    if (ctx.tbl_Customer_Members.Any(q => q.CustomerID == job.CustomerID))
                        newtask.Price = task.JobAddonMemberPrice;
                    else
                        newtask.Price = task.JobAddonStdPrice;

                    newtask.AccountCode = objmodcommon.Get_Account_Code(task.JobCodeID, job.BusinessTypeID);

                    ctx.tbl_Job_Tasks.AddObject(newtask);
                    ctx.SaveChanges();

                    //part
                    var partlist = (from p in ctx.tbl_PB_JobCodes_Details
                                    where p.JobCodeID == newtask.JobCodeID
                                    select p);

                    bool addflag = false;
                    foreach (var part in partlist)
                    {
                        var newpart = new tbl_Job_Task_Parts()
                        {
                            JobTaskID = newtask.JobTaskID,
                            PartCode = objmodcommon.GetPartCode(part.PartID).Trim(),
                            PartName = objmodcommon.GetPartName(part.PartID),
                            PartsID = part.PartID,
                            Quantity = newtask.Quantity * part.Qty
                        };

                        if (newtask.AddOnYN)
                            newpart.Price = newtask.MemberYN ? part.PartAddonMemberPrice : part.PartAddonStdPrice;
                        else
                            newpart.Price = newtask.MemberYN ? part.PartMemberPrice : part.PartStdPrice;

                        ctx.tbl_Job_Task_Parts.AddObject(newpart);

                        addflag = true;
                    }

                    if (addflag)
                        ctx.SaveChanges();

                    InvoiceFinancialDetail finance = RecalcJobData(jobid, job.CustomerID);

                    return Json(finance);
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                                   validationError.ErrorMessage);
                        }
                    }
                }

                return Json("fail");
            }
        }

        public ActionResult AddPart(int partid, int jobid, int taskid)
        {
            var objmodcommon = new mod_common(UserInfo.UserKey);

            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                try
                {
                    var part = (from p in ctx.tbl_PB_Parts
                                where p.PartID == partid
                                select p)
                                    .Single();

                    var task = (from t in ctx.tbl_Job_Tasks
                                where t.JobTaskID == taskid
                                select t)
                                .Single();

                    var newpart = new tbl_Job_Task_Parts
                    {
                        JobTaskID = taskid,
                        PartCode = objmodcommon.GetPartCode(partid).Trim(),
                        PartName = objmodcommon.GetPartName(partid).Trim(),
                        PartsID = part.PartID,
                        Quantity = 1
                    };

                    if (task.MemberYN)
                        newpart.Price = task.AddOnYN ? part.PartAddonMemberPrice : part.PartMemberPrice;
                    else
                        newpart.Price = task.AddOnYN ? part.PartAddonStdPrice : part.PartStdPrice;
                        
                    ctx.tbl_Job_Task_Parts.AddObject(newpart);
                    ctx.SaveChanges();

                    var customerid = ctx.tbl_Job.FirstOrDefault(q => q.JobID == jobid).CustomerID;

                    InvoiceFinancialDetail finance = RecalcJobData(jobid, customerid);

                    return Json(finance);
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                                   validationError.ErrorMessage);
                        }
                    }
                }

                return Json("fail");
            }
        }

        public ActionResult UpdateJobTask(int jobid, tbl_Job_Tasks taskdata, tbl_Job_Task_Parts partdata)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                try
                {
                    var task = ctx.tbl_Job_Tasks.FirstOrDefault(t => t.JobID == jobid && t.JobTaskID == taskdata.JobTaskID);

                    if (task != null)
                    {
                        task.MemberYN = taskdata.MemberYN;
                        task.AddOnYN = taskdata.AddOnYN;
                        task.Quantity = taskdata.Quantity;
                        task.Price = taskdata.Price;
                        task.JobCodeDescription = taskdata.JobCodeDescription;
                        task.JobCode = taskdata.JobCode;
                        task.JobCodeID = taskdata.JobCodeID;
                    }

                    var part = ctx.tbl_Job_Task_Parts.FirstOrDefault(q => q.JobTaskPartsID == partdata.JobTaskPartsID && q.JobTaskID == taskdata.JobTaskID);

                    if (part != null)
                    {
                        part.PartsID = partdata.PartsID;
                        part.Quantity = partdata.Quantity;
                        part.Price = partdata.Price;
                        part.PartCode = partdata.PartCode;
                        part.PartName = partdata.PartName;
                    }

                    ctx.SaveChanges();

                    var customerid = ctx.tbl_Job.FirstOrDefault(q => q.JobID == jobid).CustomerID;
                    InvoiceFinancialDetail finance = RecalcJobData(jobid, customerid);

                    return Json(finance);
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                                   validationError.ErrorMessage);
                        }
                    }
                }

                return Json("fail");
            }
        }

        public ActionResult DeleteJobTask(int jobid, int taskid, int partid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                using (var scope = new TransactionScope())
                {
                    var part = ctx.tbl_Job_Task_Parts.SingleOrDefault(q => q.JobTaskPartsID == partid);

                    if (part != null)
                    {
                        ctx.tbl_Job_Task_Parts.DeleteObject(part);
                        ctx.SaveChanges();
                    }

                    if (!ctx.tbl_Job_Task_Parts.Any(q => q.JobTaskID == taskid))
                    {
                        var task = ctx.tbl_Job_Tasks.Single(q => q.JobTaskID == taskid);
                        ctx.tbl_Job_Tasks.DeleteObject(task);
                        ctx.SaveChanges();
                    }

                    scope.Complete();
                }

                var customerid = ctx.tbl_Job.FirstOrDefault(q => q.JobID == jobid).CustomerID;
                InvoiceFinancialDetail finance = RecalcJobData(jobid, customerid);

                return Json(finance);
            }
        }

        public ActionResult SearchCode(int jobid, string searchstr, int pbid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var frid = (from j in ctx.tbl_Job
                            where j.JobID == jobid
                            select j.FranchiseID)
                            .Single();

                var codelist = (from b in ctx.tbl_PB_JobCodes
                                join c in ctx.tbl_PB_SubSection on b.SubSectionID equals c.SubsectionID
                                join d in ctx.tbl_PB_Section on c.SectionID equals d.SectionID
                                join e in ctx.tbl_PriceBook on d.PriceBookID equals e.PriceBookID
                                where e.FranchiseID == frid && e.ActiveBookYN && e.PriceBookID == pbid
                                orderby b.JobCode
                                select new
                                {
                                    b.JobCodeID,
                                    Code = b.JobCode + " - " + b.JobCodeDescription
                                });

                codelist = codelist.Where(q => q.Code.Contains(searchstr));

                return Json(codelist.ToList());
            }
        }

        public ActionResult SearchPart(int jobid, string searchstr, int pbid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var frid = (from j in ctx.tbl_Job
                            where j.JobID == jobid
                            select j.FranchiseID)
                            .Single();

                var partlist = (from s in ctx.tbl_PB_Parts
                                join t in ctx.tbl_PriceBook on s.PriceBookID equals t.PriceBookID
                                join m in ctx.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                                where m.FranchiseID == frid && t.ActiveBookYN && t.PriceBookID == pbid
                                orderby m.PartCode
                                select new
                                {
                                    s.PartID,
                                    Code = m.PartCode + " - " + m.PartName
                                });

                partlist = partlist.Where(q => q.Code.Contains(searchstr));

                return Json(partlist.ToList());
            }
        }

        public ActionResult GetJobCodeData(int jobcodeid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var codedata = ctx.tbl_PB_JobCodes.Select(b => new
                {
                    b.JobCodeID,
                    b.JobCode,
                    b.JobCodeDescription,
                    b.JobAddonMemberPrice
                })
                .FirstOrDefault(b => b.JobCodeID == jobcodeid);

                return Json(codedata);
            }
        }

        public ActionResult GetJobPartData(int partid)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var partdata = (from s in ctx.tbl_PB_Parts
                                join m in ctx.tbl_PB_MasterParts on s.MasterPartID equals m.MasterPartID
                                where s.PartID == partid
                                orderby m.PartCode
                                select new
                                {
                                    m.PartCode,
                                    m.PartName,
                                    s.PartAddonMemberPrice
                                })
                                .FirstOrDefault();

                return Json(partdata);
            }
        }

        [HttpPost]
        public ActionResult UpdateAccountCode(int taskid, string accode)
        {
            using (var ctx = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var task = ctx.tbl_Job_Tasks.Single(q => q.JobTaskID == taskid);

                task.AccountCode = accode;

                ctx.SaveChanges();

                return Json("success");
            }
        }
    }
}
