using System;
using System.ComponentModel.DataAnnotations;
using SiteBlue.Areas.Dispatch.Models;
using SiteBlue.Areas.PriceBook.Models;
using SiteBlue.Areas.OwnerPortal.Models;

[Table("tbl_Job")]
public class Job_dis
{
    [Key]
    [Display(Name = "JobID")]
    public int JobID { get; set; }

    [Required]
    [Display(Name = "FranchiseID")]
    public int FranchiseID { get; set; }

    [ForeignKey("FranchiseID")]
    public Franchise Franchise { get; set; }

    [Required]
    [Display(Name = "CustomerID")]
    public int CustomerID { get; set; }

    [ForeignKey("CustomerID")]
    public Customers Customer { get; set; }

    [Required]
    [Display(Name = "LocationID")]
    public int LocationID { get; set; }
    
    [ForeignKey("LocationID")]
    public Locations Location { get; set; }

    [Required]
    [Display(Name = "CallTakerID")]
    public int CallTakerID { get; set; }

    [Required]
    [Display(Name = "DispatcherID")]
    public int DispatcherID { get; set; }

    [Required]
    [Display(Name = "JobTypeID")]
    public int JobTypeID { get; set; }

    [ForeignKey("JobTypeID")]
    public Customers JobType { get; set; }

    [Required]
    [Display(Name = "ServiceProID")]
    public int ServiceProID { get; set; }

    [ForeignKey("ServiceProID")]
    public Employees ServicePro { get; set; }

    [Required]
    [Display(Name = "CallSourceID")]
    public int CallSourceID { get; set; }

    [Display(Name = "PromoCode")]
    [MaxLength(5)]
    public string PromoCode { get; set; }

    [Display(Name = "CallTaken")]
    public DateTime? CallTaken { get; set; }

    [Display(Name = "CallCompleted")]
    public DateTime? CallCompleted { get; set; }

    [Display(Name = "CallDispatched")]
    public DateTime? CallDispatched { get; set; }

    [Display(Name = "TravelStarted")]
    public DateTime? TravelStarted { get; set; }

    [Display(Name = "JobStarted")]
    public DateTime? JobStarted { get; set; }

    [Display(Name = "JobWrapup")]
    public DateTime? JobWrapup { get; set; }

    [Display(Name = "JobEnded")]
    public DateTime? JobEnded { get; set; }

    [Display(Name = "CallReason")]
    [MaxLength(2147483647)]
    public string CallReason { get; set; }

    [Display(Name = "Diagnostics")]
    [MaxLength(2147483647)]
    public string Diagnostics { get; set; }

    [Display(Name = "Recommendations")]
    [MaxLength(2147483647)]
    public string Recommendations { get; set; }

    [Required]
    [Display(Name = "Balance")]
    public decimal Balance { get; set; }

    [Required]
    [Display(Name = "StatusID")]
    public int StatusID { get; set; }

    [ForeignKey("StatusID")]
    public Job_Status Status { get; set; }

    [Required]
    [Display(Name = "TotalSales")]
    public decimal TotalSales { get; set; }

    [Display(Name = "InvoiceNumber")]
    [MaxLength(15)]
    public string InvoiceNumber { get; set; }

    [Display(Name = "CustomerComments")]
    [MaxLength(2147483647)]
    public string CustomerComments { get; set; }

    [Display(Name = "InvoicedDate")]
    public System.Nullable<System.DateTime> InvoicedDate { get; set; }

    [Required]
    [Display(Name = "JobTypeDetailID")]
    public int JobTypeDetailID { get; set; }

    [Required]
    [Display(Name = "JobPriorityID")]
    public int JobPriorityID { get; set; }

    [Display(Name = "RequestedDate")]
    public System.Nullable<System.DateTime> RequestedDate { get; set; }

    [Display(Name = "RequestedTime")]
    [MaxLength(25)]
    public string RequestedTime { get; set; }

    [Display(Name = "ServiceDate")]
    public System.Nullable<System.DateTime> ServiceDate { get; set; }

    [Required]
    [Display(Name = "ServiceWindowID")]
    public int ServiceWindowID { get; set; }

    [Required]
    [Display(Name = "ExpectedPayTypeID")]
    public int ExpectedPayTypeID { get; set; }

    [Required]
    [Display(Name = "ConceptID")]
    public int ConceptID { get; set; }

    [Required]
    [Display(Name = "AreaID")]
    public int AreaID { get; set; }

    [Required]
    [Display(Name = "BusinessTypeID")]
    public int BusinessTypeID { get; set; }

    [Display(Name = "ScheduleStart")]
    public System.Nullable<System.DateTime> ScheduleStart { get; set; }

    [Display(Name = "ScheduleEnd")]
    public System.Nullable<System.DateTime> ScheduleEnd { get; set; }

    [Display(Name = "ServiceLength")]
    public int ServiceLength { get; set; }

    [Required]
    [Display(Name = "SubTotal")]
    public decimal SubTotal { get; set; }

    [Required]
    [Display(Name = "TaxLaborPercentage")]
    public float TaxLaborPercentage { get; set; }

    [Required]
    [Display(Name = "TaxPartPercentage")]
    public float TaxPartPercentage { get; set; }

    [Required]
    [Display(Name = "TaxAmount")]
    public decimal TaxAmount { get; set; }

    [Required]
    [Display(Name = "TaxAuthorityID")]
    public int TaxAuthorityID { get; set; }

    [Display(Name = "DispatchedDate")]
    public System.Nullable<System.DateTime> DispatchedDate { get; set; }

    [Display(Name = "ActualStart")]
    public System.Nullable<System.DateTime> ActualStart { get; set; }

    [Display(Name = "ActualEnd")]
    public System.Nullable<System.DateTime> ActualEnd { get; set; }

    [Required]
    [Display(Name = "CallID")]
    public int CallID { get; set; }
    public virtual Calls Call { get; set; }

    [Required]
    [Display(Name = "RescheduleReasonID")]
    public int RescheduleReasonID { get; set; }

    [Display(Name = "TimeStamp")]
    public byte[] TimeStamp { get; set; }

    [Required]
    [Display(Name = "LockedYN")]
    public bool LockedYN { get; set; }

    [Display(Name = "PayrollCompletedDate")]
    public System.Nullable<System.DateTime> PayrollCompletedDate { get; set; }

    [Display(Name = "QBCompletedDate")]
    public System.Nullable<System.DateTime> QBCompletedDate { get; set; }

    [Display(Name = "WSRCompletedDate")]
    public System.Nullable<System.DateTime> WSRCompletedDate { get; set; }

    [Required]
    [Display(Name = "WarrantyType1")]
    public int WarrantyType1 { get; set; }

    [Required]
    [Display(Name = "WarrantyType2")]
    public int WarrantyType2 { get; set; }

    [Required]
    [Display(Name = "WarrantyLen1")]
    public int WarrantyLen1 { get; set; }

    [Required]
    [Display(Name = "WarrantyLen2")]
    public int WarrantyLen2 { get; set; }

    [Display(Name = "AccountingRefNum")]
    [MaxLength(50)]
    public string AccountingRefNum { get; set; }

    [Display(Name = "AccountingLoggedInName")]
    [MaxLength(50)]
    public string AccountingLoggedInName { get; set; }

    [Display(Name = "PONumber")]
    [MaxLength(50)]
    public string PONumber { get; set; }

    [Display(Name = "AuthorizedBy")]
    [MaxLength(50)]
    public string AuthorizedBy { get; set; }

    [Display(Name = "AuthorizationToStart")]
    [MaxLength(2147483647)]
    public byte[] AuthorizationToStart { get; set; }

    [Display(Name = "AcceptedBy")]
    [MaxLength(2147483647)]
    public byte[] AcceptedBy { get; set; }

    [Display(Name = "Cancelledby")]
    [MaxLength(50)]
    public string Cancelledby { get; set; }

    [Display(Name = "cancelleddate")]
    public System.Nullable<System.DateTime> cancelleddate { get; set; }

    [Required]
    [Display(Name = "CancelReasonID")]
    public int CancelReasonID { get; set; }

    [Display(Name = "CancelReason")]
    [MaxLength(2147483647)]
    public string CancelReason { get; set; }

    [Display(Name = "RescheduleReason")]
    [MaxLength(2147483647)]
    public string RescheduleReason { get; set; }

}
