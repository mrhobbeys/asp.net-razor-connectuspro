using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class JobStatusHistoryController : Controller
    {
        //
        // GET: /OwnerPortal/JobStatusHistory/
        EightHundredEntities DB = new EightHundredEntities();
       
        public ActionResult jobstatushistorydata(int jobsid)
        {
            JobStatusHistory objJobStatusHistory = new JobStatusHistory();
            List<JobStatusHistory> lststatushistory = new List<JobStatusHistory>();
            JobStatusHistory objsummary;
            DateTime lastDate = Convert.ToDateTime("1/1/2000").Date;
            int laststaus = 0;
            var joblist = (from j in DB.tbl_Job_Status_History
                           join S in DB.tbl_Job_Status on j.StatusID equals S.StatusID
                           where j.JobID == jobsid
                           orderby j.StatusDateChanged
                           select new { j, S }).ToList();

            foreach (var item in joblist)
            {
                objJobStatusHistory.statuses = item.S.Status;
                lastDate = item.j.StatusDateChanged.Value;
                objJobStatusHistory.statuschangeddate = item.j.StatusDateChanged.Value.ToShortDateString();
                objJobStatusHistory.time = item.j.StatusDateChanged.Value.ToShortTimeString();
                if (item.j.ChangedOnTabletYN != false)
                {
                    objJobStatusHistory.tablet = "YES";

                }
                else
                {
                    objJobStatusHistory.tablet = "NO";
                }
                objJobStatusHistory.changedto = item.j.ChangedfromTo;
                objJobStatusHistory.field = item.j.ChangedField;
                objJobStatusHistory.bywhom = item.j.ChangedBy;


                objsummary = new JobStatusHistory
                {
                    statuses = objJobStatusHistory.statuses,
                    statuschangeddate = objJobStatusHistory.statuschangeddate,
                    time = objJobStatusHistory.time,
                    changedto = objJobStatusHistory.changedto,
                    field = objJobStatusHistory.field,
                    bywhom = objJobStatusHistory.bywhom,
                    tablet = objJobStatusHistory.tablet
                };
                lststatushistory.Add(objsummary);
            }
            if (laststaus != 6 && laststaus != 7)
            {

                var jobrec = (from J in DB.tbl_Job where J.JobID == jobsid select new { J.CallCompleted, J.StatusID }).Single();

                if (jobrec.CallCompleted > lastDate)
                {


                    objJobStatusHistory.statuses = "Completed";
                    objJobStatusHistory.statuschangeddate = jobrec.CallCompleted.Value.ToShortDateString();
                    objJobStatusHistory.time = jobrec.CallCompleted.Value.ToShortTimeString();
                    objJobStatusHistory.tablet = "---";

                    objJobStatusHistory.changedto = "";
                    objJobStatusHistory.field = "";
                    objJobStatusHistory.bywhom = "";


                    objsummary = new JobStatusHistory
                    {
                        statuses = objJobStatusHistory.statuses,
                        statuschangeddate = objJobStatusHistory.statuschangeddate,
                        time = objJobStatusHistory.time,
                        changedto = objJobStatusHistory.changedto,
                        field = objJobStatusHistory.field,
                        bywhom = objJobStatusHistory.bywhom,
                        tablet = objJobStatusHistory.tablet
                    };
                    lststatushistory.Add(objsummary);
                }

            }



            return Json(lststatushistory);
        }
        public ActionResult JobStatusHistory()
        {
            return View();
            
        }

    }
}
