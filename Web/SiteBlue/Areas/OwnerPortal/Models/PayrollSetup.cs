using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    /// <summary>
    /// View-Model for Payroll Setup Screen
    /// </summary>
    public class PayrollSetup
    {
        internal PayrollSetup(int franchiseID, decimal overtimeStarts, int overtimeMethodID, decimal overtimeMultiplier, IEnumerable<PayrollSpiff> payrollSpiffs,
                                 IEnumerable<OvertimeMethod> overtimeMethods, IEnumerable<JobCode> jobCodes, IEnumerable<Employees> employees, IEnumerable<SpiffPayType> payTypes)
        {
            this.FranchiseID = franchiseID;
            this.OvertimeStarts = overtimeStarts;
            this.OvertimeMethodID = overtimeMethodID;
            this.OTMultiplier = overtimeMultiplier;

            this.PayrollSpiffs = payrollSpiffs;
            if (payrollSpiffs == null)
                this.PayrollSpiffs = new List<PayrollSpiff>();
            else
                this.PayrollSpiffs = new List<PayrollSpiff>(payrollSpiffs);

            // load overtime methods
            this.OvertimeMethods = (from overtimeMethod in overtimeMethods
                                    select new SelectListItem()
                                     {
                                         Text = overtimeMethod.OvertimeMethodName,
                                         Value = overtimeMethod.OvertimeMethodID.ToString(),
                                         Selected = overtimeMethod.OvertimeMethodID == overtimeMethodID
                                     }).ToList<SelectListItem>();

            this.ServicePros = (
                                from servicePro in employees
                                select new SelectListItem()
                                {
                                    Text = servicePro.Employee,
                                    Value = servicePro.EmployeeID.ToString()
                                }
                            ).ToList<SelectListItem>();

            this.SpiffPayTypes = (
                    from spiffPayType in payTypes
                    select new SelectListItem()
                    {
                        Text = spiffPayType.SpiffPayTypeName,
                        Value = spiffPayType.SpiffPayTypeID.ToString()
                    }
                ).ToList<SelectListItem>();

            // Since job codes go under a price book I want the UI to be aware of the PriceBook ID on a job code
            var jobCodeList = (
                                from jobCode in jobCodes
                                select new
                                {
                                    PriceBookID = jobCode.PriceBookID,
                                    Text = jobCode.JobCodeName + "-" + jobCode.JobCodeDescription,
                                    Value = jobCode.JobCodeID.ToString()
                                }
                             ).ToList();
            this.JobCodes_JSON = (new JavaScriptSerializer()).Serialize(jobCodeList);

            this.PriceBooks =
                    (
                        from jobCode in jobCodes
                        select new
                        {
                            PriceBookName = jobCode.PriceBookName,
                            PriceBookID = jobCode.PriceBookID
                        }
                    ).Distinct().Select(x => new SelectListItem() 
                                            {
                                             Text = x.PriceBookName,
                                             Value = x.PriceBookID.ToString()
                                            }
                                        );
        }


        public int FranchiseID { get; internal set; }
        public int PayrollSetupID { get; internal set; }
        public decimal OvertimeStarts { get; internal set; }
        public int OvertimeMethodID { get; internal set; }
        public decimal OTMultiplier { get; internal set; }

        public IEnumerable<PayrollSpiff> PayrollSpiffs { get; internal set; }

        // JSon array for binding on front-end
        public object PayrollSpiffs_JSON
        {
            get
            {
                var serializeType = new
                {
                    rows = (
                                from PayrollSpiff spiff in this.PayrollSpiffs
                                select new {
                                                id = spiff.PayrollSpiffID,
                                                data = new string[]{
                                                                      spiff.JobCode,
                                                                      spiff.JobCodeDescription,
                                                                      spiff.Employee,
                                                                      spiff.PayType,
                                                                      spiff.Rate.ToString("C"),
                                                                      (spiff.AddOn == true) ? "Yes" : "No",
                                                                      (spiff.DateExpires.HasValue == true)?spiff.DateExpires.Value.ToShortDateString() : "N/A",
                                                                      spiff.Comments,
                                                                      (spiff.Active == true) ? "Yes" : "No"
                                                                   }
                                            }
                            )
                };
                return serializeType;
                //return (new JavaScriptSerializer()).Serialize(serializeType);
            }
        }
        public System.Collections.Generic.IEnumerable<SelectListItem> OvertimeMethods { get; internal set; }
        public System.Collections.Generic.IEnumerable<SelectListItem> ServicePros { get; internal set; }
        public System.Collections.Generic.IEnumerable<SelectListItem> SpiffPayTypes { get; internal set; }

        public string JobCodes_JSON { get; internal set; }
        public System.Collections.Generic.IEnumerable<SelectListItem> PriceBooks { get; internal set; }

    }
}
