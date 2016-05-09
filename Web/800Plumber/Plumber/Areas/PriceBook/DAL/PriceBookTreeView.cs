using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SiteBlue.Areas.PriceBook.Models;
using System.Transactions;

namespace SiteBlue.Areas.PriceBook.DAL
{
    public class PriceBookTreeView : IDisposable
    {
        #region Private Members
        private PriceBookContext context;
        private bool disposed = false;
        #endregion

        #region constructors
        public PriceBookTreeView(PriceBookContext context)
        {
            this.context = context;
        }
        #endregion

        #region Private Methods
        private static int CopareFranchiseForSort(Franchise f1, Franchise f2)
        {
            return f1.FranchiseNumber.CompareTo(f2.FranchiseNumber);
        }
        private static int CopareSectionForSort(Section s1, Section s2)
        {
            try
            {
                return s1.SectionName.CompareTo(s2.SectionName);
            }
            catch { return 0; }
        }

        public static int CopareSubSectionForSort(SubSection s1, SubSection s2)
        {
            return s1.SubSectionName.CompareTo(s2.SubSectionName);
        }
        #endregion

        #region Public Methods/Properties
        public IEnumerable<PriceBooks> GetPriceBooks()
        {
            List<PriceBooks> lst = context.pricebooks.Where(q => q.ActiveBookYN).ToList();
            lst.Insert(0, new PriceBooks{ PriceBookID = -1, BookName = "Select PriceBook" });

            return lst;
        }

        public IEnumerable<PriceBooks> GetPriceBooksByFranchiseID(int franchiseID)
        {
            List<PriceBooks> lst = context.pricebooks.Where(q => q.FranchiseID == franchiseID).ToList();
            lst.Insert(0, new PriceBooks { PriceBookID = -1, BookName = "Select PriceBook" });

            return lst;
        }

        public IEnumerable<PriceBooks> GetPriceBooks(int fid)
        {
            var lst = (from p in context.pricebooks
                       join f in context.franchises on p.FranchiseID equals f.FranchiseID
                       where f.FranchiseID == fid && p.ActiveBookYN
                       select p).ToList();

            return lst;
        }

        public IEnumerable<Franchise> GetFranchises()
        {
            List<Franchise> lst = context.franchises.ToList();
            lst.Sort(CopareFranchiseForSort);
            Franchise f = new Franchise();
            f.FranchiseID = -1;
            f.FranchiseNumber = "Select Franchise";
            lst.Insert(0, f);
            return lst;
        }

        public List<Franchise> GetFranchiseByFranchiseID(int franchiseID)
        {
            List<Franchise> lst = context.franchises.Where(f => f.FranchiseID == franchiseID).ToList();
            return lst;
        }

        public IEnumerable<AccountCodes> GetAccountCodes()
        {
            StringBuilder sbquery = new StringBuilder();

            sbquery.Append("SELECT AccountCode, ('['+AccountCode+'] '+AccountName) AS AccountName, AccountType FROM tbl_account_codes");
            List<AccountCodes> lst = context.accountcode.SqlQuery(sbquery.ToString()).ToList();
            lst.Insert(0, new AccountCodes { AccountCode = "", AccountName = "Select Account Code" });
            
            return lst;
        }

        public IEnumerable<SubSection> GetSubSections(int? sid, bool mflag)
        {
            var list = context.subsections.OrderBy(t => t.SubSectionName).Where(t => sid.HasValue && t.SectionID == sid.Value).ToList();

            if (!mflag)
                return list.Where(q => q.MFlag == 1 || q.MFlag == 2).ToList();

            return list.Where(q => q.MFlag != 2).ToList();
        }

        public IEnumerable<Task1> GetTasks(int? pricebookid, string tasksearchstring)
        {
            StringBuilder sbquery = new StringBuilder();

            //Get List of Sections by joining to PriceBook and filtering on Franchise 
            sbquery.Append("select distinct top 100 t1.* from tbl_PB_Section s ");
            sbquery.Append(" Inner Join tbl_PB_SubSection sb ON sb.SectionID  = s.SectionID ");
            sbquery.Append(" Inner Join tbl_PB_JobCodes t ON sb.SubSectionID  = t.SubSectionID ");
            sbquery.Append(" AND (t.JobCodeDescription like '%" + tasksearchstring + "%' Or t.JobCode like '%" + tasksearchstring + "%')");
            sbquery.Append(" Inner Join tbl_PB_JobCodes t1 ON t.SubSectionID  = t1.SubSectionID ");
            sbquery.Append(" Where s.PriceBookID = @param1");
            sbquery.Append(" Order By t1.SubSectionID");

            int localfranchiseid = (pricebookid.HasValue) ? pricebookid.Value : 0;

            IEnumerable<TaskImage> ti = context.taskimage.ToList();

            var x = context.tasks.SqlQuery(sbquery.ToString(), new SqlParameter("param1", localfranchiseid))
                .Select(c => new Task1 {
                    JobCodeID = c.JobCodeID,
                    SubSectionID = c.SubSectionID,
                    ManualPricingYN = c.ManualPricingYN,
                    ActiveYN = c.ActiveYN,
                    JobCode = c.JobCode,
                    JobCodeDescription = c.JobCodeDescription,
                    JobCost = c.JobCost,
                    JobStdPrice = c.JobStdPrice,
                    JobMemberPrice = c.JobMemberPrice,
                    JobAddonStdPrice = c.JobAddonStdPrice,
                    JobAddonMemberPrice = c.JobAddonMemberPrice,
                    ResAccountCode = c.ResAccountCode,
                    ComAccountCode = c.ComAccountCode,
                    ImgName = (ti.Where(cc => cc.JobCodeID == c.JobCodeID).Count() == 0) ? "" : ti.Where(cc => cc.JobCodeID == c.JobCodeID).First().ImgName
                }).ToList();

            return x;
        }

        public IEnumerable<SubSection> GetSubSections(int? pricebookid, string tasksearchstring)
        {
            StringBuilder sbquery = new StringBuilder();

            //Get List of Sections by joining to PriceBook and filtering on Franchise 
            sbquery.Append("select distinct sb1.* from ( ");
            sbquery.Append("select distinct top 100 t1.* from tbl_PB_Section s ");
            sbquery.Append(" Inner Join tbl_PB_SubSection sb ON sb.SectionID  = s.SectionID ");
            sbquery.Append(" Inner Join tbl_PB_JobCodes t ON sb.SubSectionID  = t.SubSectionID ");
            sbquery.Append(" AND (t.JobCodeDescription like '%" + tasksearchstring + "%' Or t.JobCode like '%" + tasksearchstring + "%')");
            sbquery.Append(" Inner Join tbl_PB_JobCodes t1 ON t.SubSectionID  = t1.SubSectionID ");
            sbquery.Append(" Where s.PriceBookID = @param1");
            sbquery.Append(" Order By t1.SubSectionID ) t1 ");
            sbquery.Append(" Inner Join tbl_PB_SubSection sb1 ON sb1.SubSectionID  = t1.SubSectionID ");

            int localfranchiseid = (pricebookid.HasValue) ? pricebookid.Value : 0;

            return context.subsections.SqlQuery(sbquery.ToString(), new SqlParameter("param1", localfranchiseid)).ToList();
        }

        public IEnumerable<Task1> GetAllTasks(int? sbid, bool mflag)
        {
            var list = (from t in context.tasks
                        join s1 in context.subsections on t.SubSectionID equals s1.SubsectionID
                        join s in context.sections on s1.SectionID equals s.SectionID
                        join im in context.taskimage on t.JobCodeID equals im.JobCodeID into tt
                        from im in tt.DefaultIfEmpty()
                        join lv  in context.laborviews on t.JobCodeID  equals lv.JobCodeID into tlv
                        from lv in tlv.DefaultIfEmpty()
                        where s.SectionID == sbid.Value
                        select new Task1
                        {
                            JobCodeID = t.JobCodeID,
                            SubSectionID = t.SubSectionID,
                            ManualPricingYN = t.ManualPricingYN,
                            ActiveYN = t.ActiveYN,
                            JobCode = t.JobCode,
                            JobCodeDescription = t.JobCodeDescription,
                            JobCost = t.JobCost,
                            JobStdPrice = t.JobStdPrice,
                            JobMemberPrice = t.JobMemberPrice,
                            JobAddonStdPrice = t.JobAddonStdPrice,
                            JobAddonMemberPrice = t.JobAddonMemberPrice,
                            ResAccountCode = t.ResAccountCode,
                            ComAccountCode = t.ComAccountCode,
                            LaborPercentage = ((decimal?)lv.LaborPercentage) ?? 0,
                            MFlag = t.MFlag,
                            ImgName = im.ImgName
                        }).ToList();

            if (!mflag)
                return list.Where(q => q.MFlag == 1 || q.MFlag == 2).ToList();

            return list.Where(q => q.MFlag != 2).ToList();
        }

        public IEnumerable<Task1> GetTasks(int? sbid, bool mflag)
        {
            var list = (from t in context.tasks
                        join im in context.taskimage on t.JobCodeID equals im.JobCodeID into tt
                        from im in tt.DefaultIfEmpty()
                        join lv in context.laborviews on t.JobCodeID equals lv.JobCodeID into tlv
                        from lv in tlv.DefaultIfEmpty()
                        where t.SubSectionID == sbid.Value
                        orderby t.JobCode
                        select new Task1
                        {
                            JobCodeID = t.JobCodeID,
                            SubSectionID = t.SubSectionID,
                            ManualPricingYN = t.ManualPricingYN,
                            ActiveYN = t.ActiveYN,
                            JobCode = t.JobCode,
                            JobCodeDescription = t.JobCodeDescription,
                            JobCost = t.JobCost,
                            JobStdPrice = t.JobStdPrice,
                            JobMemberPrice = t.JobMemberPrice,
                            JobAddonStdPrice = t.JobAddonStdPrice,
                            JobAddonMemberPrice = t.JobAddonMemberPrice,
                            ResAccountCode = t.ResAccountCode,
                            ComAccountCode = t.ComAccountCode,
                            LaborPercentage = ((decimal?)lv.LaborPercentage) ?? 0,
                            MFlag = t.MFlag,
                            ImgName = im.ImgName
                        }).ToList();

            if (!mflag)
                return list.Where(q => q.MFlag == 1 || q.MFlag == 2).ToList();

            return list.Where(q => q.MFlag != 2).ToList();
        }

        public IEnumerable<Task1> SingleTask(int? jid)
        {
            return (from t in context.tasks
                    join im in context.taskimage on t.JobCodeID equals im.JobCodeID into tt
                    from im in tt.DefaultIfEmpty()
                    join lv in context.laborviews on t.JobCodeID equals lv.JobCodeID into tlv
                    from lv in tlv.DefaultIfEmpty()
                    where t.JobCodeID == jid.Value
                    select new Task1
                    {
                        JobCodeID = t.JobCodeID,
                        SubSectionID = t.SubSectionID,
                        ManualPricingYN = t.ManualPricingYN,
                        ActiveYN = t.ActiveYN,
                        JobCode = t.JobCode,
                        JobCodeDescription = t.JobCodeDescription,
                        JobCost = t.JobCost,
                        JobStdPrice = t.JobStdPrice,
                        JobMemberPrice = t.JobMemberPrice,
                        JobAddonStdPrice = t.JobAddonStdPrice,
                        JobAddonMemberPrice = t.JobAddonMemberPrice,
                        ResAccountCode = t.ResAccountCode,
                        ComAccountCode = t.ComAccountCode,
                        LaborPercentage = ((decimal?)lv.LaborPercentage) ?? 0,
                        MFlag = t.MFlag,
                        ImgName = im.ImgName
                    }).ToList();
        }

        /*public IEnumerable<SelectListItem> GetParts(int? id)
        {
            var lst = (from t0 in context.parts
                       join t1 in context.masterparts on t0.MasterPartID equals t1.MasterPartID
                       select new
                       {
                           PartID = t0.PartID,
                           PartName = "[" + SqlFunctions.StringConvert((decimal)t0.PartID) + "] " + t1.PartName
                       })
                        .Distinct()
                        .Take(100)
                        .ToList();

            return lst.Select(c => new SelectListItem
            {
                Value = c.PartID.ToString(),
                Text = c.PartName
            });
        }*/

        public IEnumerable<PartsModel> GetParts(int pbid)
        {
            return (from t0 in context.parts
                       join t1 in context.masterparts on t0.MasterPartID equals t1.MasterPartID
                       where t0.PriceBookID == pbid
                       select new PartsModel
                        {
                           PartID = t0.PartID,
                           PartCode = t1.PartCode,
                           PartName = t1.PartName,
                           PartCost = t0.PartCost,
                           PartAddonMemberPrice = t0.PartAddonMemberPrice,
                           PartAddonStdPrice = t0.PartAddonStdPrice,
                           PartMemberPrice = t0.PartMemberPrice,
                           PartStdPrice = t0.PartStdPrice
                       })
                        .ToList();
        }

        public IEnumerable<TaskDetail1> GetTaskDetail(int? taskid)
        {
            return (from v in context.taskdetails
                    join v1 in context.parts on v.PartID equals v1.PartID
                    join v2 in context.masterparts on v1.MasterPartID equals v2.MasterPartID
                    where v.JobCodeID == taskid.Value
                    select new TaskDetail1
                    {
                        JobCodeDetailsID = v.JobCodeDetailsID,
                        JobCodeID = v.JobCodeID,
                        PartID = v.PartID,
                        Qty = v.Qty,
                        ManualPricingYN = v.ManualPricingYN,
                        PartCost = v.PartCost,
                        PartAddonMemberPrice = v.PartAddonMemberPrice,
                        PartAddonStdPrice = v.PartAddonStdPrice,
                        PartMemberPrice = v.PartMemberPrice,
                        PartStdPrice = v.PartStdPrice,
                        PartName = v2.PartName,
                        VendorPartID = v2.VendorPartID,
                        PartCodeID = v2.PartCodeID
                    }).ToList();
        }

        public TaskDetail1 GetSingleTaskDetail(int? id)
        {
            return (from v in context.taskdetails
                    join v1 in context.parts on v.PartID equals v1.PartID
                    join v2 in context.masterparts on v1.MasterPartID equals v2.MasterPartID
                    where v.JobCodeDetailsID == id.Value
                    select new TaskDetail1
                    {
                        JobCodeDetailsID = v.JobCodeDetailsID,
                        Qty = v.Qty,
                        PartCost = v.PartCost,
                        PartAddonMemberPrice = v.PartAddonMemberPrice,
                        PartAddonStdPrice = v.PartAddonStdPrice,
                        PartMemberPrice = v.PartMemberPrice,
                        PartStdPrice = v.PartStdPrice,
                        PartName = v2.PartName
                    }).Single();
                    
        }

        public IEnumerable<Section> GetPriceBookTree(int pricebookid, bool mflag)
        {
            StringBuilder sbquery = new StringBuilder();

            //Get List of Sections by joining to PriceBook and filtering on Franchise 
            sbquery.Append("select s.* from tbl_PB_Section s ");
            sbquery.Append(" Where s.PriceBookID = @param1");

            if (!mflag)
            {
                sbquery.Append(" AND (s.MFlag = 1 OR s.MFlag = 2)");
            }
            else
            {
                sbquery.Append(" AND s.MFlag != 2 ");
            }

            sbquery.Append(" Order By s.SectionName");

            List<Section> lst = context.sections.SqlQuery(sbquery.ToString(), new SqlParameter("param1", pricebookid)).ToList();

            //lst.Sort(CopareSectionForSort);
            
            return lst;
        }

        public double GetMemberPricePercent(int pricebookid)
        {
            return context.pricebooks.SingleOrDefault(q => q.PriceBookID == pricebookid).MemberPricePercent ?? 0;
        }

        public void ActivateSection(int? id, bool active)
        {
            int lid = (id.HasValue) ? id.Value : 0;
            context.sections.Find(id).ActiveYN = active;
            context.SaveChanges();
        }

        public void ActivateSubSection(int? id, bool active)
        {
            int lid = (id.HasValue) ? id.Value : 0;
            context.subsections.Find(id).ActiveYN = active;
            context.SaveChanges();
        }

        public void ActivateTask(int? id, bool active)
        {
            int lid = (id.HasValue) ? id.Value : 0;
            context.tasks.Find(id).ActiveYN = active;
            context.SaveChanges();
        }

        public bool UpdateSection(int? id, string sectionname)
        {
            int lid = (id.HasValue) ? id.Value : 0;
            Section s = context.sections.Find(lid);
            
            if (s != null)
            {
                try
                {
                    s.SectionName = sectionname;
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
                
            }
            else
            {
                return false;
            }
            
        }

        public bool UpdateSubSection(int? id, string subsectionname)
        {
            int lid = (id.HasValue) ? id.Value : 0;
            SubSection s = context.subsections.Find(lid);

            if (s != null)
            {
                try
                {
                    s.SubSectionName = subsectionname;
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        public bool UpdateTask(int? id, string jobcodedescription)
        {
            int lid = (id.HasValue) ? id.Value : 0;
            Task t = context.tasks.Find(lid);

            if (t != null)
            {
                try
                {
                    t.JobCodeDescription = jobcodedescription;
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }
        #endregion

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Image Section
        public bool UpdateImage(int? taskid, string imagename)
        {
            int lid = (taskid.HasValue) ? taskid.Value : 0;
            var t = context.taskimage.SingleOrDefault(s => s.JobCodeID == lid);

            if (t != null)
            {
                try
                {
                    t.ImgName = imagename;
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try 
                {
                    context.taskimage.Add(new TaskImage { ImgName = imagename, JobCodeID = taskid.Value });
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public string GetTaskImage(int? taskid)
        {
            int tid = (taskid.HasValue) ? taskid.Value : 0;

            var t = context.taskimage.SingleOrDefault(q => q.JobCodeID == tid);

            if (t != null)
            {
                return t.ImgName;
            }

            return context.tasks.SingleOrDefault(q => q.JobCodeID == tid).JobCode + ".jpg";
        }

        public string GetTaskCode(int? taskid)
        {
            return context.tasks.SingleOrDefault(q => q.JobCodeID == taskid.Value).JobCode;
        }
        #endregion

        #region TaskDetail Management
        public bool CreateTaskDetail(TaskDetail detailData)
        {
            try
            {
                context.taskdetails.Add(detailData);
                context.SaveChanges();

                RecalculatePrices(3, detailData.JobCodeID);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateTaskDetail(TaskDetail detailData)
        {
            var td = context.taskdetails.Find(detailData.JobCodeDetailsID);

            try
            {
                td.JobCodeID = detailData.JobCodeID;
                td.Qty = detailData.Qty;
                td.ManualPricingYN = false;
                td.PartCost = detailData.PartCost;
                td.PartStdPrice = detailData.PartStdPrice;
                td.PartMemberPrice = detailData.PartMemberPrice;
                td.PartAddonStdPrice = detailData.PartAddonStdPrice;
                td.PartAddonMemberPrice = detailData.PartAddonMemberPrice;

                context.SaveChanges();

                RecalculatePrices(3, detailData.JobCodeID);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTaskDetail(int id)
        {
            var t = context.taskdetails.Find(id);

            try
            {
                context.taskdetails.Remove(t);
                context.SaveChanges();

                RecalculatePrices(3, t.JobCodeID);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string CalcPartPrices(int jobcodeid, decimal cost, decimal qty)
        {
            var markupObj = context.markups.SingleOrDefault(mu => mu.Lowerbound <= cost && cost <= mu.Upperbound);

            if (markupObj == null || markupObj.Markup == 0)
                return "0";

            var markup = markupObj.Markup;
            var discount = (from pbr in context.pricebookrates
                                join s in context.sections on pbr.PriceBookID equals s.PriceBookID
                                join ss in context.subsections on s.SectionID equals ss.SectionID
                                join jc in context.tasks on ss.SubsectionID equals jc.SubSectionID
                                where jc.JobCodeID == jobcodeid
                                select pbr.MemberDiscountRate).SingleOrDefault();

            var retail = decimal.Round(markup * cost, 2);
            var member = decimal.Round((1 - discount / 100) * retail, 2);

            return string.Format("{0},{1}", retail, member);
        }

        public string GetPartPrices(int? partid)
        {
            int pid = (partid.HasValue) ? partid.Value : 0;

            var v = context.parts.SingleOrDefault(q => q.PartID == partid);

            return decimal.Round(v.PartCost, 2) + "," + decimal.Round(v.PartStdPrice, 2)
                + "," + decimal.Round(v.PartMemberPrice, 2) + "," + decimal.Round(v.PartAddonStdPrice, 2)
                + "," + decimal.Round(v.PartAddonMemberPrice, 2);
        }

        public void RecalculatePrices(int level, int levelId)
        {
            context.Database.ExecuteSqlCommand(
                "EXEC [RecalculateTree] @Level, @LevelId"
                , new SqlParameter("Level", level)
                , new SqlParameter("LevelId", levelId)
                );
        }

        #endregion

        #region Section CRUD
        public int CreateSection(Section data)
        {
            try
            {
                data.MFlag = 0;

                context.sections.Add(data);
                context.SaveChanges();

                return data.SectionID;
            }
            catch
            {
                return 0;
            }
        }

        public bool DeleteSection(int id)
        {
            var s = context.sections.Find(id);

            try
            {
                if (s.MFlag == 0)
                {
                    context.sections.Remove(s);
                }
                else
                {
                    s.MFlag = 2;
                }
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CopySection(int id)
        {
            var s = context.sections.Find(id);

            try
            {
                Section newdata = new Section
                {
                    ActiveYN = s.ActiveYN,
                    MFlag = 0,
                    PriceBookID = s.PriceBookID,
                    SectionName = s.SectionName + " - Copy",
                };

                Section newsection = context.sections.Add(newdata);
                context.SaveChanges();

                var ss = context.subsections.Where(q => q.SectionID == s.SectionID).ToList();

                foreach (var item in ss)
                {
                    SubSection newsubdata = new SubSection
                    {
                        SectionID = newsection.SectionID,
                        ActiveYN = item.ActiveYN,
                        MFlag = 0,
                        SubSectionName = item.SubSectionName
                    };

                    SubSection newsubsection = context.subsections.Add(newsubdata);
                    context.SaveChanges();

                    var task = context.tasks.Where(q => q.SubSectionID == item.SubsectionID).ToList();

                    foreach (var item1 in task)
                    {
                        Task newtdata = new Task
                        {
                            ActiveYN = item1.ActiveYN,
                            ComAccountCode = item1.ComAccountCode,
                            ResAccountCode = item1.ResAccountCode,
                            JobAddonMemberPrice = item1.JobAddonMemberPrice,
                            JobAddonStdPrice = item1.JobAddonStdPrice,
                            JobCode = item1.JobCode,
                            JobCodeDescription = item1.JobCodeDescription,
                            JobCost = item1.JobCost,
                            JobMemberPrice = item1.JobMemberPrice,
                            JobStdPrice = item1.JobStdPrice,
                            ManualPricingYN = item1.ManualPricingYN,
                            MFlag = 0,
                            SubSectionID = newsubsection.SubsectionID
                        };

                        Task newtask = context.tasks.Add(newtdata);
                        context.SaveChanges();

                        var taskdetail = context.taskdetails.Where(q => q.JobCodeID == item1.JobCodeID).ToList();

                        foreach (var tditem in taskdetail)
                        {
                            TaskDetail newtddata = new TaskDetail
                            {
                                JobCodeID = newtask.JobCodeID,
                                ManualPricingYN = tditem.ManualPricingYN,
                                PartAddonMemberPrice = tditem.PartAddonMemberPrice,
                                PartAddonStdPrice = tditem.PartAddonStdPrice,
                                PartCost = tditem.PartCost,
                                PartID = tditem.PartID,
                                PartMemberPrice = tditem.PartMemberPrice,
                                PartStdPrice = tditem.PartStdPrice,
                                Qty = tditem.Qty
                            };

                            context.taskdetails.Add(newtddata);
                            context.SaveChanges();
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Labor> LaborSection(int? id)
        {
            return (from l in context.laborsectionviews
                    where l.SectionID == id.Value
                    select new Labor
                    {
                        PartID = l.PartID,
                        LaborPrice = l.LaborPrice,
                        PartName = l.PartName,
                        PartCost = l.PartCost,
                        PartStdPrice = l.PartStdPrice,
                        PartMemberPrice = l.PartMemberPrice,
                        PartAddonStdPrice = l.PartAddonStdPrice,
                        PartAddonMemberPrice = l.PartAddonMemberPrice
                    })
                    .ToList();
        }

        public void UpdateSectionLabor(int codeid, string partid, string cost, string std, string member, string astd, string amember)
        {
            var tasklist = (from t in context.tasks
                            join ss in context.subsections on t.SubSectionID equals ss.SubsectionID
                            join s in context.sections on ss.SectionID equals s.SectionID
                            where s.SectionID == codeid
                            select t)
                            .Distinct()
                            .ToList();

            CalcLabor(tasklist, partid, cost, std, member, astd, amember);
        }

        public void UpdatePricebookLabor(int pricebookid, int partid, decimal cost, decimal std, decimal member, decimal astd, decimal amember)
        {
            context.Database.ExecuteSqlCommand(
                "EXEC [PB_UpdatePartPrices] @PriceBookId, @PartId, @PartCost, @StandardPrice, @AddOnPrice, @MemberPrice, @MemberAddOnPrice, @UpdatePriceForPriceBook",
                new SqlParameter("PriceBookId", pricebookid),
                new SqlParameter("PartId", partid),
                new SqlParameter("PartCost", cost),
                new SqlParameter("StandardPrice", std),
                new SqlParameter("AddOnPrice", astd),
                new SqlParameter("MemberPrice", member),
                new SqlParameter("MemberAddOnPrice", amember),
                new SqlParameter("UpdatePriceForPriceBook", true)
                );
        }

        #endregion

        #region SubSection CRUD
        public int CreateSubSection(SubSection data)
        {
            try
            {
                data.MFlag = 0;

                context.subsections.Add(data);
                context.SaveChanges();

                return data.SubsectionID;
            }
            catch
            {
                return 0;
            }
        }

        public bool DeleteSubSection(int id)
        {
            var s = context.subsections.Find(id);

            try
            {
                if (s.MFlag == 0)
                {
                    context.subsections.Remove(s);
                }
                else
                {
                    s.MFlag = 2;
                }
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CopySubSection(int id)
        {
            var s = context.subsections.Find(id);

            try
            {
                SubSection newdata = new SubSection
                {
                    ActiveYN = s.ActiveYN,
                    MFlag = 0,
                    SectionID = s.SectionID,
                    SubSectionName = s.SubSectionName + " - Copy",
                };

                SubSection newsubsection = context.subsections.Add(newdata);
                context.SaveChanges();

                var task = context.tasks.Where(q => q.SubSectionID == s.SubsectionID).ToList();

                foreach (var item1 in task)
                {
                    Task newtdata = new Task
                    {
                        ActiveYN = item1.ActiveYN,
                        ComAccountCode = item1.ComAccountCode,
                        ResAccountCode = item1.ResAccountCode,
                        JobAddonMemberPrice = item1.JobAddonMemberPrice,
                        JobAddonStdPrice = item1.JobAddonStdPrice,
                        JobCode = item1.JobCode,
                        JobCodeDescription = item1.JobCodeDescription,
                        JobCost = item1.JobCost,
                        JobMemberPrice = item1.JobMemberPrice,
                        JobStdPrice = item1.JobStdPrice,
                        ManualPricingYN = item1.ManualPricingYN,
                        MFlag = 0,
                        SubSectionID = newsubsection.SubsectionID
                    };

                    Task newtask = context.tasks.Add(newtdata);
                    context.SaveChanges();

                    var taskdetail = context.taskdetails.Where(q => q.JobCodeID == item1.JobCodeID).ToList();

                    foreach (var tditem in taskdetail)
                    {
                        TaskDetail newtddata = new TaskDetail
                        {
                            JobCodeID = newtask.JobCodeID,
                            ManualPricingYN = tditem.ManualPricingYN,
                            PartAddonMemberPrice = tditem.PartAddonMemberPrice,
                            PartAddonStdPrice = tditem.PartAddonStdPrice,
                            PartCost = tditem.PartCost,
                            PartID = tditem.PartID,
                            PartMemberPrice = tditem.PartMemberPrice,
                            PartStdPrice = tditem.PartStdPrice,
                            Qty = tditem.Qty
                        };

                        context.taskdetails.Add(newtddata);
                        context.SaveChanges();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Labor> LaborSubSection(int? id)
        {
            return (from l in context.laborsubsectionviews
                    where l.SubsectionID == id.Value
                    select new Labor
                    {
                        PartID = l.PartID,
                        LaborPrice = l.LaborPrice,
                        PartName = l.PartName,
                        PartCost = l.PartCost,
                        PartStdPrice = l.PartStdPrice,
                        PartMemberPrice = l.PartMemberPrice,
                        PartAddonStdPrice = l.PartAddonStdPrice,
                        PartAddonMemberPrice = l.PartAddonMemberPrice
                    })
                    .ToList();
        }

        public void UpdateSubSectionLabor(int codeid, string partid, string cost, string std, string member, string astd, string amember)
        {
            var tasklist = (from t in context.tasks
                            join ss in context.subsections on t.SubSectionID equals ss.SubsectionID
                            where ss.SubsectionID == codeid
                            select t)
                            .Distinct()
                            .ToList();

            CalcLabor(tasklist, partid, cost, std, member, astd, amember);
        }
        #endregion

        #region Task CRUD
        public int CreateTask(Task data)
        {
            try
            {
                data.MFlag = 0;

                context.tasks.Add(data);
                context.SaveChanges();

                return data.JobCodeID;
            }
            catch
            {
                return 0;
            }
        }

        public bool DeleteTask(int id)
        {
            var t = context.tasks.Find(id);

            try
            {
                if (t.MFlag == 0)
                {
                    context.tasks.Remove(t);
                }
                else
                {
                    t.MFlag = 2;
                }
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CopyTask(int id)
        {
            var t = context.tasks.Find(id);

            try
            {
                Task newdata = new Task
                {
                    ActiveYN = t.ActiveYN,
                    MFlag = 0,
                    SubSectionID = t.SubSectionID,
                    JobCodeDescription = t.JobCodeDescription + " - Copy",
                    JobCode = t.JobCode,
                    ManualPricingYN = t.ManualPricingYN,
                    JobCost = t.JobCost,
                    JobStdPrice = t.JobStdPrice,
                    JobMemberPrice = t.JobMemberPrice,
                    JobAddonStdPrice = t.JobAddonStdPrice,
                    JobAddonMemberPrice = t.JobAddonMemberPrice,
                    ResAccountCode = t.ResAccountCode,
                    ComAccountCode = t.ComAccountCode
                };

                Task newtask = context.tasks.Add(newdata);
                context.SaveChanges();

                var taskdetail = context.taskdetails.Where(q => q.JobCodeID == t.JobCodeID).ToList();

                foreach (var tditem in taskdetail)
                {
                    TaskDetail newtddata = new TaskDetail
                    {
                        JobCodeID = newtask.JobCodeID,
                        ManualPricingYN = tditem.ManualPricingYN,
                        PartAddonMemberPrice = tditem.PartAddonMemberPrice,
                        PartAddonStdPrice = tditem.PartAddonStdPrice,
                        PartCost = tditem.PartCost,
                        PartID = tditem.PartID,
                        PartMemberPrice = tditem.PartMemberPrice,
                        PartStdPrice = tditem.PartStdPrice,
                        Qty = tditem.Qty
                    };

                    context.taskdetails.Add(newtddata);
                    context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Labor> LaborTask(int? id)
        {
            return (from l in context.labortaskviews
                    where l.JobCodeID == id.Value
                    select new Labor
                    {
                        PartID = l.PartID,
                        LaborPrice = l.LaborPrice,
                        PartName = l.PartName,
                        PartCost = l.PartCost,
                        PartStdPrice = l.PartStdPrice,
                        PartMemberPrice = l.PartMemberPrice,
                        PartAddonStdPrice = l.PartAddonStdPrice,
                        PartAddonMemberPrice = l.PartAddonMemberPrice
                    })
                    .ToList();
        }

        public void UpdateTaskLabor(int codeid, string partid, string cost, string std, string member, string astd, string amember)
        {
            var tasklist = (from t in context.tasks
                            where t.JobCodeID == codeid
                            select t)
                            .Distinct()
                            .ToList();

            CalcLabor(tasklist, partid, cost, std, member, astd, amember);
        }

        public bool UpdateResAccountCode(int id, string rac)
        {
            var t = context.tasks.Find(id);

            try
            {
                t.ResAccountCode = rac;
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateComAccountCode(int id, string cac)
        {
            var t = context.tasks.Find(id);

            try
            {
                t.ComAccountCode = cac;
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region CRUD part

        public IEnumerable<PartCodes> GetPartCodes()
        {
            var lst = context.partcodes.OrderByDescending(q => q.PartCode).ToList();
            lst.Insert(0, new PartCodes { PartCodeID = "", PartCode = "Select PartCode" });

            return lst;
        }

        public bool CreateMasterPart(int pricebookId, MasterParts mpData)
        {
            try
            {
                mpData.ConceptID = 1;
                mpData.ActiveYN = true;

                using (var tScope = new TransactionScope())
                {
                    context.masterparts.Add(mpData);
                    context.SaveChanges();

                    var part = new Parts()
                                   {
                                       PartAddonMemberPrice = 0,
                                       PartAddonStdPrice = 0,
                                       PartCost = mpData.PartCost,
                                       MasterPartID = mpData.MasterPartID,
                                       Markup = 0,
                                       PartMemberPrice = 0,
                                       PartStdPrice = 0,
                                       PriceBookID = pricebookId
                                   };

                    context.parts.Add(part);
                    context.SaveChanges();

                    tScope.Complete();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region PriceBook CRUD
        public IEnumerable<Labor> LaborPriceBook(int? id)
        {
            return (from l in context.laborpricebookviews
                    where l.PriceBookID == id.Value
                    select new Labor
                    {
                        PartID = l.PartID,
                        LaborPrice = l.LaborPrice,
                        PartName = l.PartName,
                        PartCost = l.PartCost,
                        PartStdPrice = l.PartStdPrice,
                        PartMemberPrice = l.PartMemberPrice,
                        PartAddonStdPrice = l.PartAddonStdPrice,
                        PartAddonMemberPrice = l.PartAddonMemberPrice
                    })
                    .ToList();
        }


        public bool UpdateMemberPricePercent(int id, double per)
        {
            var p = context.pricebooks.Find(id);

            try
            {
                p.MemberPricePercent = per;

                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void RecalcMemberPrice(int id, double per)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    per = per / 100;
                    StringBuilder sbquery = new StringBuilder();

                    sbquery.Append("UPDATE t SET t.JobMemberPrice=(1-" + per + ")*t.JobStdPrice, t.JobAddonMemberPrice=(1-" + per + ")*t.JobAddonStdPrice ");
                    sbquery.Append("FROM tbl_PB_JobCodes t, tbl_PB_SubSection ss, tbl_PB_Section s ");
                    sbquery.Append("WHERE t.SubSectionID=ss.SubSectionID AND ss.SectionID=s.SectionID AND s.PriceBookID = @param1");

                    context.Database.ExecuteSqlCommand(sbquery.ToString(), new SqlParameter("param1", id));

                    sbquery.Clear();
                    sbquery.Append("UPDATE td SET td.PartMemberPrice=(1-" + per + ")*td.PartStdPrice, td.PartAddonMemberPrice=(1-" + per + ")*td.PartAddonStdPrice ");
                    sbquery.Append("FROM tbl_PB_JobCodes_Details td, tbl_PB_JobCodes t, tbl_PB_SubSection ss, tbl_PB_Section s ");
                    sbquery.Append("WHERE td.JobCodeID=t.JobCodeID AND t.SubSectionID=ss.SubSectionID AND ss.SectionID=s.SectionID AND s.PriceBookID = @param1");

                    context.Database.ExecuteSqlCommand(sbquery.ToString(), new SqlParameter("param1", id));

                    scope.Complete();
                }
            }
            catch
            {
            }
        }

        public int? CopyPriceBook(int? id, string bookname, out string msg)
        {
            var p = context.pricebooks.Find(id.GetValueOrDefault());

            if (p == null)
            {
                msg = "Source price book not found.";
                return null;
            }

            try
            {
                var newdata = (PriceBooks)context.Entry(p).GetDatabaseValues().ToObject();
                newdata.BookName = bookname;
                newdata = context.pricebooks.Add(newdata);

                context.SaveChanges();
                context.Database.ExecuteSqlCommand("EXEC [CopyPriceBook] @PriceBookId, @NewPriceBookId",
                                                   new SqlParameter("PriceBookId", p.PriceBookID),
                                                   new SqlParameter("NewPriceBookId", newdata.PriceBookID));
                msg = null;
                return newdata.PriceBookID;
            }
            catch (Exception ex)
            {
                msg = "An exception occurred while copying price book: " + ex.Message;
                return null;
            }
        }

        #endregion

        public void CalcLabor(List<Task> tasklist, string partid, string cost, string std, string member, string astd, string amember)
        {
            foreach (var tt in tasklist)
            {
                int tpartid = int.Parse(partid);
                decimal tcost = decimal.Parse(cost);
                decimal tstd = decimal.Parse(std);
                decimal tmember = decimal.Parse(member);
                decimal tastd = decimal.Parse(astd);
                decimal tamember = decimal.Parse(amember);

                var tmp = context.taskdetails.Where(q => q.JobCodeID == tt.JobCodeID && q.PartID == tpartid).ToList();

                if (tmp.Count == 0)
                {
                    // This logic is not necessary for labor change
                    // when no labor part, there is no labor update
                    // RJ
                    //
                    //var td = context.taskdetails.Where(q => q.JobCodeID == tt.JobCodeID).ToList();

                    //if (td.Count != 0)
                    //{
                    //    tamember -= td.Sum(t => t.PartAddonMemberPrice);
                    //    tastd -= td.Sum(t => t.PartAddonStdPrice);
                    //    tmember -= td.Sum(t => t.PartMemberPrice);
                    //    tstd -= td.Sum(t => t.PartStdPrice);
                    //}

                    //TaskDetail newtd = new TaskDetail
                    //{
                    //    JobCodeID = tt.JobCodeID,
                    //    ManualPricingYN = false,
                    //    PartAddonMemberPrice = tamember,
                    //    PartAddonStdPrice = tastd,
                    //    PartMemberPrice = tmember,
                    //    PartStdPrice = tstd,
                    //    PartCost = 0,
                    //    PartID = tpartid,
                    //    Qty = 1
                    //};

                    //context.taskdetails.Add(newtd);
                    //context.SaveChanges();
                }
                else
                {
                    var tmp1 = tmp.First();
                    tmp1.PartAddonMemberPrice = tamember;
                    tmp1.PartAddonStdPrice = tastd;
                    tmp1.PartMemberPrice = tmember;
                    tmp1.PartStdPrice = tstd;
                    tmp1.PartCost = tcost;

                    context.SaveChanges();
                    RecalculatePrices(3, tmp1.JobCodeID);
                }
            }
        }

        public string SubmitDatabase(int per, int pbid, int mpid, int nlevel, int nid, bool mflag)
        {
            var part = context.parts.SingleOrDefault(q => q.PriceBookID == pbid && q.MasterPartID == mpid);

            if (part == null)
                return "Cannot find adjustment part for this price book!";

            try
            {
                context.Database.ExecuteSqlCommand(
                    "EXEC ApplyMarkup @Level, @LevelId, @MarkupAsPercentage, @MarkupMasterPartId"
                    , new SqlParameter("Level", nlevel)
                    , new SqlParameter("LevelId", nid)
                    , new SqlParameter("MarkupAsPercentage", ((per/100m) - 1) * 100)
                    , new SqlParameter("MarkupMasterPartId", mpid)
                    );
            }
            catch
            {
                return "An exception occurred saving to database. No changes were made.";
            }

            return "Successfully updated!";
        }
    }
}