using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.PriceBook.Models;
using SiteBlue.Areas.PriceBook.DAL;
using SecurityGuard.Core;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using SecurityGuard.ViewModels;
using System.Web.Security;
using SiteBlue.Core;
using SiteBlue.Data.EightHundred;
using SiteBlue.Controllers;
using System.IO;

namespace SiteBlue.Areas.PriceBook.Controllers
{
    [HandleError]
    //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class PriceBookController : SiteBlueBaseController
    {
        #region Private Members

        private PriceBookTreeView treeview;
        private MembershipConnection memberShipContext = new MembershipConnection();

        #endregion

        #region Constructors

        public PriceBookController()
        {
            this.treeview = new PriceBookTreeView(new PriceBookContext());
        }

        public PriceBookController(PriceBookTreeView treeview)
        {
            this.treeview = treeview;
        }

        #endregion

        #region Views

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult Index(string frid, FormCollection collection)
        {
            int franchiseID = 0;

            if (!Request.IsAjaxRequest())
            {
                ViewBag.ac = this.treeview.GetAccountCodes();

                if (frid != null)
                {
                    franchiseID = Convert.ToInt32(frid);
                }
                else
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var franchise = UserInfo.CurrentFranchise ??
                                        UserInfo.DefaultFranchise ?? UserInfo.Franchises.FirstOrDefault();

                        franchiseID = franchise == null ? 56 : franchise.FranchiseID;
                    }
                }
                
                ViewBag.PriceBooks = this.treeview.GetPriceBooksByFranchiseID(franchiseID);

                return View();
            }

            int pid = 0;
            bool mflag = true;

            int.TryParse(collection["PriceBookID"], out pid);
            bool.TryParse(collection["MFlag"], out mflag);

            var sections = this.treeview.GetPriceBookTree(pid, mflag);

            ViewBag.mflag = mflag;
            ViewBag.percent = this.treeview.GetMemberPricePercent(pid);

            return PartialView("Tree", sections);
        }

        public ActionResult GetPriceBook(int fid)
        {
            try
            {
                var pricebooklist = this.treeview.GetPriceBooks(fid)
                    .Select(c => new PriceBooks
                    {
                        PriceBookID = c.PriceBookID,
                        BookName = c.BookName
                    }).ToList();

                return Json(new { ok = true, data = pricebooklist, message = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }

        [OutputCache(Duration = 0)]
        public ActionResult TreeSubSections(int? Id, bool mflag)
        {
            return PartialView("TreeSubSections", this.treeview.GetSubSections(Id, mflag));
        }

        public ActionResult TreeTasks(int? Id, bool mflag)
        {
            return PartialView("TreeTasks", this.treeview.GetTasks(Id, mflag));
        }

        public ActionResult TreeTasksSearch(int? Id, string searchstr)
        {
            return PartialView("TreeTasksSearch", this.treeview.GetTasks(Id, searchstr));
        }

        public ActionResult TreeSubSectionsSearch(int? Id, string searchstr)
        {
            return PartialView("TreeSubSectionsSearch", this.treeview.GetSubSections(Id, searchstr));
        }

        public ActionResult AllTaskList(int? Id, bool mflag)
        {
            ViewBag.ac = this.treeview.GetAccountCodes();
            ViewBag.nlevel = 1;
            ViewBag.nid = Id;

            return PartialView("TaskList", this.treeview.GetAllTasks(Id, mflag));
        }

        public ActionResult TaskList(int? Id, bool mflag)
        {
            ViewBag.ac = this.treeview.GetAccountCodes();
            ViewBag.nlevel = 2;
            ViewBag.nid = Id;

            return PartialView("TaskList", this.treeview.GetTasks(Id, mflag));
        }

        public ActionResult SingleTask(int? Id)
        {
            ViewBag.ac = this.treeview.GetAccountCodes();
            ViewBag.nlevel = 3;
            ViewBag.nid = Id;

            return PartialView("TaskList", this.treeview.SingleTask(Id));
        }

        [OutputCache(Duration = 0)]
        public ActionResult TaskDetail(int? Id)
        {
            ViewBag.jobcodeid = Id;
            
            return PartialView("TaskDetail", this.treeview.GetTaskDetail(Id));
        }

        public EmptyResult ActivateSection(int? id, bool active)
        {
            this.treeview.ActivateSection(id, active);

            return new EmptyResult();
        }

        public EmptyResult ActivateSubSection(int? id, bool active)
        {
            this.treeview.ActivateSubSection(id, active);

            return new EmptyResult();
        }

        public EmptyResult ActivateTask(int? id, bool active)
        {
            this.treeview.ActivateTask(id, active);

            return new EmptyResult();
        }

        public ActionResult UpdateSection(int? id, string name)
        {
            if (this.treeview.UpdateSection(id, name))
            {
                return new EmptyResult();
            }
            else
            {
                throw new HttpException(500, "Error: Updating Section Name");
            }
        }

        public ActionResult UpdateSubSection(int? id, string name)
        {
            if (this.treeview.UpdateSubSection(id, name))
            {
                return new EmptyResult();
            }
            else
            {
                throw new HttpException(500, "Error: Updating Sub-Section Name");
            }
        }

        public ActionResult UpdateTask(int? id, string name)
        {
            if (this.treeview.UpdateTask(id, name))
            {
                return new EmptyResult();
            }
            else
            {
                throw new HttpException(500, "Error: Updating Task Name");
            }
        }

        #endregion

        #region Events

        protected override void Dispose(bool disposing)
        {
            treeview.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Image Section

        public string Upload(int? taskid, HttpPostedFileBase fileData)
        {
            string taskimg = this.treeview.GetTaskCode(taskid) + ".jpg";

            var fileName = this.Server.MapPath("~/uploads/" + taskimg);
            fileData.SaveAs(fileName);

            fileName = this.Server.MapPath("~/uploads/" + System.IO.Path.GetFileName(fileData.FileName));
            fileData.SaveAs(fileName);

            this.treeview.UpdateImage(taskid, taskimg);

            return taskimg;
        }

        public string GetTaskImage(int? taskid)
        {
            var imgname = this.treeview.GetTaskImage(taskid);

            var filename = this.Server.MapPath("~/uploads/" + imgname);

            if (System.IO.File.Exists(filename))
            {
                return imgname;
            }

            return "";
        }

        public string DeleteTempImage(string filename)
        {
            try
            {
                System.IO.File.Delete(this.Server.MapPath("~/uploads/" + filename));
            }
            catch
            {
                return "0";
            }

            return "1";
        }

        #endregion

        #region TaskDetail Management

        public ActionResult TaskDetailDlg(int? id, int? jid)
        {
            ViewBag.jobcodeid = jid;

            if (id == null)
            {
                ViewBag.action = "create";
                return PartialView("_TaskDetailDlg", new TaskDetailEx() { Qty = 1 });
            }

            ViewBag.action = "update";

            return PartialView("_TaskDetailDlg", this.treeview.GetSingleTaskDetail(id));
        }

        [HttpPost]
        public int CreateTaskDetail(TaskDetail detailData)
        {
            if (this.treeview.CreateTaskDetail(detailData))
            {
                return detailData.JobCodeID;
            }

            return 0;
        }

        [HttpPost]
        public int UpdateTaskDetail(TaskDetail detailData)
        {
            if (this.treeview.UpdateTaskDetail(detailData))
            {
                return detailData.JobCodeID;
            }

            return 0;
        }

        public string DeleteTaskDetail(int id)
        {
            if (this.treeview.DeleteTaskDetail(id))
            {
                return "success";
            }

            return "failed";
        }

        public string CalcPartPrices(int jobcodeid, string cost, string qty)
        {
            decimal v, v1;

            try
            {
                Decimal.TryParse(cost, out v);
                Decimal.TryParse(qty, out v1);

                return this.treeview.CalcPartPrices(jobcodeid, v, v1);
            }
            catch
            {
                return "-1";
            }
        }

        #endregion

        #region Section CRUD

        public ActionResult CreateSection(Section data)
        {
            var sid = this.treeview.CreateSection(data);
            var sections = this.treeview.GetPriceBookTree(data.PriceBookID, true).Where(q => q.SectionID == sid).First();

            return PartialView("SectionNode", sections);
        }

        public string DeleteSection(int id)
        {
            if (this.treeview.DeleteSection(id))
            {
                return "success";
            }

            return "failed";
        }

        public string CopySection(int id)
        {
            if (this.treeview.CopySection(id))
            {
                return "success";
            }

            return "failed";
        }

        public ActionResult LaborSection(int? Id, int tlevel)
        {
            ViewBag.codeid = Id.Value;
            ViewBag.tlevel = tlevel;

            return PartialView("LaborList", this.treeview.LaborSection(Id));
        }

        #endregion

        #region SubSection CRUD

        public ActionResult CreateSubSection(FormCollection collection)
        {
            string subsectioname = "";
            bool active = false;
            int sid = 0;

            if (collection.Keys.Count == 3)
            {
                int.TryParse(collection["hSectionID"], out sid);
                subsectioname = collection["SubSectionName"];
                active = collection["ActiveYN"].Contains("true");
            }

            SubSection data = new SubSection
            {
                SectionID = sid,
                SubSectionName = subsectioname,
                ActiveYN = active
            };

            var ssid = this.treeview.CreateSubSection(data);
            var subsections = this.treeview.GetSubSections(sid, true).Where(q => q.SubsectionID == ssid).First();

            return PartialView("SubSectionNode", subsections);
        }

        public string DeleteSubSection(int id)
        {
            if (this.treeview.DeleteSubSection(id))
            {
                return "success";
            }

            return "failed";
        }

        public string CopySubSection(int id)
        {
            if (this.treeview.CopySubSection(id))
            {
                return "success";
            }

            return "failed";
        }

        public ActionResult LaborSubSection(int? Id, int tlevel)
        {
            ViewBag.codeid = Id.Value;
            ViewBag.tlevel = tlevel;

            return PartialView("LaborList", this.treeview.LaborSubSection(Id));
        }

        #endregion

        #region Task Management

        public ActionResult CreateTask(FormCollection collection)
        {
            int ssid = 0;
            int.TryParse(collection["hSubSectionID"], out ssid);

            decimal cost, sprice, mprice, asprice, amprice;
            decimal.TryParse(collection["JobCost"], out cost);
            decimal.TryParse(collection["JobStdPrice"], out sprice);
            decimal.TryParse(collection["JobMemberPrice"], out mprice);
            decimal.TryParse(collection["JobAddonStdPrice"], out asprice);
            decimal.TryParse(collection["JobAddonMemberPrice"], out amprice);

            Task data = new Task
            {
                SubSectionID = ssid,
                ManualPricingYN = false,
                ActiveYN = true,
                JobCode = collection["JobCode"],
                JobCodeDescription = collection["JobCodeDescription"],
                JobCost = cost,
                JobStdPrice = sprice,
                JobMemberPrice = mprice,
                JobAddonStdPrice = asprice,
                JobAddonMemberPrice = amprice,
                ResAccountCode = collection["ResAccountCode"],
                ComAccountCode = collection["ComAccountCode"]
            };

            var taskid = this.treeview.CreateTask(data);

            return null;
        }

        public string DeleteTask(int id)
        {
            if (this.treeview.DeleteTask(id))
            {
                return "success";
            }

            return "failed";
        }

        public string CopyTask(int id)
        {
            if (this.treeview.CopyTask(id))
            {
                return "success";
            }

            return "failed";
        }

        public ActionResult LaborTask(int? Id, int tlevel)
        {
            ViewBag.codeid = Id.Value;
            ViewBag.tlevel = tlevel;

            return PartialView("LaborList", this.treeview.LaborTask(Id));
        }

        public ActionResult SubmitLabor(FormCollection collection)
        {
            if (collection.Keys.Count == 2)
                return null;

            int codeid, tlevel, pricebookid;
            int.TryParse(collection["codeid"], out codeid);
            int.TryParse(collection["pricebookid"], out pricebookid);
            int.TryParse(collection["tlevel"], out tlevel);

            string[] partids = collection["item.PartID"].Split(',');
            string[] costs = collection["item.PartCost"].Split(',');
            string[] stds = collection["item.PartStdPrice"].Split(',');
            string[] members = collection["item.PartMemberPrice"].Split(',');
            string[] astds = collection["item.PartAddonStdPrice"].Split(',');
            string[] amembers = collection["item.PartAddonMemberPrice"].Split(',');

            for (var i = 0; i < partids.Length; i++)
            {
                switch (tlevel)
                {
                    case 0 :
                        treeview.UpdatePricebookLabor(pricebookid, 
                                                        Convert.ToInt32(partids[i]), 
                                                        Convert.ToDecimal(costs[i]), 
                                                        Convert.ToDecimal(stds[i]), 
                                                        Convert.ToDecimal(members[i]),
                                                        Convert.ToDecimal(astds[i]),
                                                        Convert.ToDecimal(amembers[i]));
                        break;
                    case 1 :
                        treeview.UpdateSectionLabor(codeid, partids[i], costs[i], stds[i], members[i], astds[i], amembers[i]);
                        break;
                    case 2 :
                        treeview.UpdateSubSectionLabor(codeid, partids[i], costs[i], stds[i], members[i], astds[i], amembers[i]);
                        break;
                    case 3 :
                        treeview.UpdateTaskLabor(codeid, partids[i], costs[i], stds[i], members[i], astds[i], amembers[i]);
                        break;

                }
            }

            treeview.RecalculatePrices(0, pricebookid);

            //return PartialView("LaborList", this.treeview.LaborSubSection(codeid));
            return null;
        }

        public void UpdateAccountCode(int id, string acid, string acflag)
        {
            this.treeview.UpdateAccountCode(id, acid, acflag);
        }

        #endregion

        #region MasterPart Management

        public ActionResult MasterPartDlg(int? fid)
        {
            ViewBag.franchiseid = fid;

            return PartialView("_MasterPartDlg", this.treeview.GetPartCodes());
        }

        public bool CreateMasterPart(MasterParts mpData)
        {
            var pricebookId = Convert.ToInt32(RouteData.Values["id"]);
            return this.treeview.CreateMasterPart(pricebookId, mpData);
        }

        #endregion

        #region PriceBook CRUD

        public ActionResult LaborPriceBook(int? Id)
        {
            ViewBag.PriceBookId = Id.Value;
            return PartialView("LaborList", this.treeview.LaborPriceBook(Id));
        }

        public ActionResult RecalcMemberPrice(int id, double per)
        {
            if (this.treeview.UpdateMemberPricePercent(id, per))
            {
                this.treeview.RecalcMemberPrice(id, per);
            }

            return null;
        }

        [HttpPost]
        public JsonResult SendToiPad(int id, int techId)
        {
            var pricebookName = string.Empty;
            var tablet = string.Empty;

            string folder = null;

            using (var db = GetContext())
            {
                pricebookName = db.tbl_PriceBook.Single(pb => pb.PriceBookID == id).BookName;
            }

            using (var db = new EightHundredEntities())
            {
                
                var result = (from t in db.tbl_Employee
                        join tt in db.tbl_Franchise_Tablets
                        on t.EmployeeID equals tt.EmployeeID
                        join f in db.tbl_Franchise
                        on t.FranchiseID equals f.FranchiseID
                        where t.EmployeeID == techId
                        select new {TabletNumber = tt.TabletNumber}).Single();

                folder = Path.Combine(GlobalConfiguration.InitTabletDropPath, result.TabletNumber);
                tablet = result.TabletNumber;
            }
            
            using (var db = new PriceBookContext())
            {
                db.Database.ExecuteSqlCommand("EXEC [GetXMLByPriceBook] @PriceBookId, @Folder",
                                                   new System.Data.SqlClient.SqlParameter("PriceBookId", id),
                                                   new System.Data.SqlClient.SqlParameter("Folder", folder));
            }
            return Json(new {Success = true, Name = pricebookName, Tablet = tablet});
        }

        [HttpPost]
        public ActionResult CopyPriceBook(int? Id, string bookname)
        {
            string errMsg;
            var newId = treeview.CopyPriceBook(Id, bookname, out errMsg);

            return newId.HasValue
                    ? Json(new { ok = true, data = new { pid = newId.Value, pname = bookname }, message = "Success" })
                    : Json(new { ok = false, data = new { pid = 0, pname = "Error" }, message = errMsg });
        }

        [HttpPost]
        public bool RenamePriceBook(int? id, string renname)
        {
            return treeview.RenamePriceBook(id, renname);
        }

        [HttpPost]
        public bool DeletePriceBook(int? id)
        {
            return treeview.DeletePriceBook(id);
        }

        #endregion

        public ActionResult SubmitDatabase(int per, int pbid, int mpid, int nlevel, int nid, bool mflag)
        {
            try
            {
                string retmsg = this.treeview.SubmitDatabase(per, pbid, mpid, nlevel, nid, mflag);
                return Json(new { success = true, msg = retmsg });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "An exception occurred saving to database." });
            }
        }

        public ActionResult PartList(int pbid)
        {
            ViewBag.pbid = pbid;
            return PartialView("_PartList");
        }

        public ActionResult GetAjaxParts(JQueryDataTableParamModel param, int pbid)
        {
            var allpartlist = this.treeview.GetParts(pbid);
            IEnumerable<PartsModel> filteredpartlist;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isCodeSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var isNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]);

                filteredpartlist = allpartlist
                   .Where(c => isCodeSearchable && c.PartCode.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isNameSearchable && c.PartName.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredpartlist = allpartlist;
            }

            var isCodeSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isCostSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isStdSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var isMemSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var isAStdSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var isAMemSortable = Convert.ToBoolean(Request["bSortable_6"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PartsModel, string> orderingFunction =
                (c => sortColumnIndex == 0 && isCodeSortable ? c.PartCode :
                    sortColumnIndex == 1 && isNameSortable ? c.PartName :
                    sortColumnIndex == 2 && isCostSortable ? c.PartCost.ToString() :
                    sortColumnIndex == 3 && isStdSortable ? c.PartStdPrice.ToString() :
                    sortColumnIndex == 4 && isMemSortable ? c.PartMemberPrice.ToString() :
                    sortColumnIndex == 5 && isAStdSortable ? c.PartAddonStdPrice.ToString() :
                    sortColumnIndex == 6 && isAMemSortable ? c.PartAddonMemberPrice.ToString() :
                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredpartlist = filteredpartlist.OrderBy(orderingFunction);
            else
                filteredpartlist = filteredpartlist.OrderByDescending(orderingFunction);

            var partlist = filteredpartlist.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            var result = (from c in partlist
                          select new[] { 
                            c.PartCode, 
                            c.PartName, 
                            c.PartCost.ToString(), 
                            c.PartStdPrice.ToString(), 
                            c.PartMemberPrice.ToString(), 
                            c.PartAddonStdPrice.ToString(), 
                            c.PartAddonMemberPrice.ToString(), 
                            c.PartID.ToString() 
                         });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allpartlist.Count(),
                iTotalDisplayRecords = filteredpartlist.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Technicians(int id)
        {
            using (var db = new EightHundredEntities())
            {
                var technicians = from t in db.tbl_Franchise_Tablets
                                  join e in db.tbl_Employee
                                      on t.EmployeeID equals e.EmployeeID
                                  where e.FranchiseID == id
                                  orderby e.Employee, t.TabletNumber
                                  select new {key = t.EmployeeID, label = e.Employee + " (" + t.TabletNumber + ")"};
                    
                return Json(technicians.ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPartCategory()
        {
            using (var db = new EightHundredEntities())
            {
                var category = from pc in db.tbl_PB_Parts_Codes
                               select new
                               {
                                   key = pc.PartCodeID,
                                   label = pc.PartCode
                               };

                return Json(category.ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetPartAdjustList(int pbid, string catid)
        {
            using (var db = new EightHundredEntities())
            {
                var partadjustlist = (from p in db.tbl_PB_Parts
                                      join mp in db.tbl_PB_MasterParts on p.MasterPartID equals mp.MasterPartID
                                      where p.PriceBookID == pbid && mp.PartCodeID == catid
                                      select new PartAdjust
                                      {
                                          PartID = p.PartID,
                                          MasterPartID = mp.MasterPartID,
                                          PartCode = mp.PartCode,
                                          PartDescription = mp.PartName,
                                          FrequencyUsed = (from jd in db.tbl_PB_JobCodes_Details 
                                                           join j in db.tbl_PB_JobCodes on jd.JobCodeID equals j.JobCodeID
                                                           join ss in db.tbl_PB_SubSection on j.SubSectionID equals ss.SubsectionID
                                                           join s in db.tbl_PB_Section on ss.SectionID equals s.SectionID
                                                           where s.PriceBookID == p.PriceBookID && jd.PartID == p.PartID
                                                           select jd.Qty).Sum(),
                                          PartCost = p.PartCost,
                                          PartStdPrice = p.PartStdPrice,
                                          PartMemberPrice = p.PartMemberPrice,
                                          PartAddonStdPrice = p.PartAddonStdPrice,
                                          PartAddonMemberPrice = p.PartAddonMemberPrice
                                      }).ToList();

                return PartialView("_PartAdjustList", partadjustlist);
            }
        }

        [HttpPost]
        public ActionResult UpdatePart(PartAdjust partdata, int pbid)
        {
            using (var db = new EightHundredEntities())
            {
                var part = db.tbl_PB_Parts.Single(q => q.PartID == partdata.PartID);

                part.PartCost = partdata.PartCost.Value;
                part.PartStdPrice = partdata.PartStdPrice.Value;
                part.PartMemberPrice = partdata.PartMemberPrice.Value;
                part.PartAddonStdPrice = partdata.PartAddonStdPrice.Value;
                part.PartAddonMemberPrice = partdata.PartAddonMemberPrice.Value;

                var masterpart = db.tbl_PB_MasterParts.Single(q => q.MasterPartID == partdata.MasterPartID);

                masterpart.PartCode = partdata.PartCode;
                masterpart.PartName = partdata.PartDescription;

                db.SaveChanges();

                var taskdetail = db.tbl_PB_JobCodes_Details.Where(q => q.PartID == partdata.PartID).ToList();

                taskdetail.ForEach(q => q.PartCost = partdata.PartCost.Value);
                taskdetail.ForEach(q => q.PartStdPrice = partdata.PartStdPrice.Value);
                taskdetail.ForEach(q => q.PartMemberPrice = partdata.PartMemberPrice.Value);
                taskdetail.ForEach(q => q.PartAddonStdPrice = partdata.PartAddonStdPrice.Value);
                taskdetail.ForEach(q => q.PartAddonMemberPrice = partdata.PartAddonMemberPrice.Value);

                db.SaveChanges();

                //Recalcuate Task
                this.treeview.RecalculatePrices(0, pbid);

                return Json("success");
            }
        }
    }
}
