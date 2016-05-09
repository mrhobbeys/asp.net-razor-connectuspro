using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class InvoiceDetailInfo
    {
        public bool IsCorporate { get; set; }
        public bool isOwner { get; set; }
        public bool isTicket { get; set; }
        public bool isCompany { get; set; }
        public bool Authorized { get; set; }

        public InvoiceDetailInfo()
        {
            Head = new InvoiceHeadDetail();
            JobInfo = new InvoiceJobInfoDetail();
            ServiceComment = new InvoiceServiceCommentDetail();
            CustomerSignature = new InvoiceCustomerSignatureDetail();
            Financial = new InvoiceFinancialDetail();
            MngJobTask = new ManageJobTask();
            Payment = new PaymentInfo();
            MemberInfo = new CustomerMembershipInfo();
        }

        public InvoiceHeadDetail Head { get; set; }
        public InvoiceJobInfoDetail JobInfo { get; set; }
        public InvoiceServiceCommentDetail ServiceComment { get; set; }
        public InvoiceCustomerSignatureDetail CustomerSignature { get; set; }
        public InvoiceFinancialDetail Financial;
        public SelectList EmployeeList { get; set; }
        public ManageJobTask MngJobTask { get; set; }
        public PaymentInfo Payment { get; set; }

        public string StyleFlag { get; set; }
        public string StyleFlag1 { get; set; }

        public string PreviousJobID { get; set; }
        public string CurrentJobID { get; set; }
        public string NextJobID { get; set; }

        public string IsItClose { get; set; }
        public string CloseTicketflag { get; set; }

        public string CustomerEmail { get; set; }

        public CustomerMembershipInfo MemberInfo { get; set; }
    }

    public class InvoiceHeadDetail
    {
        public string NPS_Score { get; set; }
        public string NPS_Comment { get; set; }
        public int Technician_Score { get; set; }
        public string Technician_Comment { get; set; }
        public int Sched_Score { get; set; }
        public string Sched_Comment { get; set; }
        public int IPad_Score { get; set; }
        public string IPad_Comment { get; set; }
    }

    public class InvoiceJobInfoDetail
    {
        public string DBAName { get; set; }

        //Job Info
        public string InvoiceNum { get; set; }
        public SelectList InvoiceStatusList { get; set; }
        public string JobType { get; set; }
        public SelectList BusinessTypeList { get; set; }
        public SelectList ServiceTypeList { get; set; }
        public string ServicePro { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public string WarrantyType1 { get; set; }
        public string WarrantyLength1 { get; set; }
        public string WarrantyType2 { get; set; }
        public string WarrantyLength2 { get; set; }
        public string CallReason { get; set; }
        public string LockDate { get; set; }
        public int? CustomerID { get; set; }


        //Bill
        public string CustomerBillTo { get; set; }
        public string JobLocation { get; set; }

        //Call Info
        public DateTime? CallReceived { get; set; }
        public string ReceivedBy { get; set; }
        public string CallMinutes { get; set; }
        public string Referral { get; set; }

        //Miscellaneous
        public string Requested { get; set; }
        public string DispatchedBy { get; set; }
        public string Dispatched { get; set; }
        public string JobCompleted { get; set; }
    }

    public class InvoiceServiceCommentDetail
    {
        public string Diagnosis { get; set; }
        public string Recommendations { get; set; }
        public string CustomerComments { get; set; }
        public string CallNotes { get; set; }
    }

    public class InvoiceCustomerSignatureDetail
    {
        public int Myjobid { get; set; }
        public int AcceptedpictureId { get; set; }
        public int AuthtostartpictureId { get; set; }
    }

    public class InvoiceFinancialDetail
    {
        public string Term { get; set; }
        public string SubTotal { get; set; }
        public SelectList TaxRateList { get; set; }
        public decimal CreditLimit { get; set; }
        public string CustomerBalance { get; set; }
        public string Tax { get; set; }
        public string TaxPartPercent { get; set; }
        public string TaxLaborPercent { get; set; }
        public string PaymentType { get; set; }
        public string Total { get; set; }
        public string TotalPaid { get; set; }
        public string Balance { get; set; }
    }

    public class ManageJobTask
    {
        public SelectList PriceBookList { get; set; }
        public SelectList TaskCodeList { get; set; }
        public SelectList PartList { get; set; }
        public SelectList AccountCodeList { get; set; }
    }

    public class PaymentInfo
    {
        public SelectList PaymentTypeList { get; set; }
        public int JobID { get; set; }
    }

    public class CustomerMembershipInfo
    {
        public SelectList MemberTypeList { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class JobHistoryInfo
    {
        public string FieldName { get; set; }
        public string TableName { get; set; }
        public string ChangeType { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string isTablet { get; set; }
        public string ChangedBy { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}