using System;
using System.Data;
using System.Linq;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Business.Job
{
    public class AuditedJobContext : EightHundredEntities
    {
        private readonly string _userName;
        private readonly bool _isTablet;

        public AuditedJobContext(Guid userKey, string userName, bool isTablet)
            : base(userKey)
        {
            _userName = userName;
            _isTablet = isTablet;
        }

        protected override void Audit(object sender, EventArgs e)
        {
            base.Audit(sender, e);

            var modifiedJobs = ObjectStateManager.GetObjectStateEntries(EntityState.Modified)
                                                    .Where(entity => entity.Entity is tbl_Job).ToArray();
            var deletedTasks = ObjectStateManager.GetObjectStateEntries(EntityState.Deleted)
                                                    .Where(entity => entity.Entity is tbl_Job_Tasks).ToArray();

            if (modifiedJobs.Count() == 0 && deletedTasks.Count() == 0) return;

            foreach (var ose in modifiedJobs)
            {
                var j = (tbl_Job)ose.Entity;
                ose.GetModifiedProperties().Where(fld => !Equals(ose.OriginalValues[fld], ose.CurrentValues[fld])).
                        Select(fld =>
                               new tbl_Job_Status_History
                               {
                                   JobID = j.JobID,
                                   StatusID = j.StatusID,
                                   StatusDateChanged = DateTime.Now,
                                   ChangedOnTabletYN = _isTablet,
                                   ChangedBy = _userName,
                                   ChangedField = fld,
                                   ChangedfromTo =
                                       string.Concat(ose.OriginalValues[fld] ?? "NULL", " => ",
                                                     ose.CurrentValues[fld] ?? "NULL")
                               }).ToList().ForEach(audit => tbl_Job_Status_History.AddObject(audit));
            }

            deletedTasks.Select(ose => ose.Entity).OfType<tbl_Job_Tasks>().Select(t => new tbl_Job_Status_History
            {
                JobID = t.JobID,
                StatusID = tbl_Job.Select(j => new { j.JobID, j.StatusID }).Single(j => j.JobID == t.JobID).StatusID,
                StatusDateChanged = DateTime.Now,
                ChangedOnTabletYN = _isTablet,
                ChangedBy = _userName,
                ChangedField = "Job Task Deleted",
                ChangedfromTo = string.Format("Task id {0} with job code {1} was deleted.", t.JobTaskID, t.JobCode)
            }).ToList().ForEach(audit => tbl_Job_Status_History.AddObject(audit));
        }
    }
}
