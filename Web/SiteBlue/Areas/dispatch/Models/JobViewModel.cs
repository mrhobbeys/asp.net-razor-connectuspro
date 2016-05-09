using System;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.dispatch.Models
{
    public class JobViewModel
    {
        public int JobID { get; set; }
        public int FranchiseID { get; set; }
        public string CallReason { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public tbl_Job_Status Status { get; set; }
        public string StatusText { get; set; }
        public string ServiceProName { get; set; }
        public string CallName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public string DiagnosisNotes { get; set; }
        public string OwnerNotes { get; set; }

        public tbl_Customer Customer { get; set; }
        public tbl_Locations JobLocation { get; set; }
        public tbl_Contacts JobLocationContact { get; set; }
        public tbl_Employee PrimaryTechnician { get; set; }
        public tbl_Employee[] SecondaryTechnicians { get; set; }
        public tbl_Contacts BillToContact { get; set; }
        public tbl_Contacts BillToCell { get; set; }
        public tbl_Locations BillToLocation { get; set; }
        public tbl_Call_CancelReasons CancelReason { get; set; }
        
        public string DisplayName
        {
            get
            {
                var name = CallName ?? ((Customer == null || string.IsNullOrEmpty(Customer.CustomerName)) ? Phone ?? JobID.ToString() : Customer.CustomerName ?? JobID.ToString());
                return string.IsNullOrWhiteSpace(name) ? JobID.ToString() : name;
            }
        }
    }
}