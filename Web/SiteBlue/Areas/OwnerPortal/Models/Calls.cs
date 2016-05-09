using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

[Table("tbl_Calls")]
public class Calls
{
    [Key]
    [Display(Name = "CallID")]
    public int CallID { get; set; }

    [Required]
    [Display(Name = "CallTakerID")]
    public System.Nullable<int> CallTakerID { get; set; }

    [Display(Name = "CallInTime")]
    [MaxLength(50)]
    public string CallInTime { get; set; }

    [Display(Name = "CallCompletedTime")]
    [MaxLength(50)]
    public string CallCompletedTime { get; set; }

    [Required]
    [Display(Name = "ResultCodeID")]
    public System.Nullable<int> ResultCodeID { get; set; }

    [Required]
    [Display(Name = "CancelReasonID")]
    public System.Nullable<int> CancelReasonID { get; set; }

    [Display(Name = "CallReason")]
    [MaxLength(2147483647)]
    public string CallReason { get; set; }

    [Display(Name = "CallZipCode")]
    [MaxLength(12)]
    public string CallZipCode { get; set; }

    [Display(Name = "CallPhone")]
    [MaxLength(20)]
    public string CallPhone { get; set; }

    [Required]
    [Display(Name = "CallTypeID")]
    public System.Nullable<int> CallTypeID { get; set; }

    [Display(Name = "CallName")]
    [MaxLength(50)]
    public string CallName { get; set; }

    [Display(Name = "CallAddress")]
    [MaxLength(75)]
    public string CallAddress { get; set; }

    [Display(Name = "CompanyName")]
    [MaxLength(50)]
    public string CompanyName { get; set; }

    [Display(Name = "CompanyAddress")]
    [MaxLength(75)]
    public string CompanyAddress { get; set; }

    [Required]
    [Display(Name = "ConceptID")]
    public System.Nullable<int> ConceptID { get; set; }

    [Required]
    [Display(Name = "ConceptDetailID")]
    public System.Nullable<int> ConceptDetailID { get; set; }

    [Display(Name = "Owner")]
    public System.Nullable<bool> Owner { get; set; }

    [Display(Name = "CrossStreet")]
    [MaxLength(50)]
    public string CrossStreet { get; set; }

    [Display(Name = "Email")]
    [MaxLength(50)]
    public string Email { get; set; }

    [Required]
    [Display(Name = "ReferralID")]
    public System.Nullable<int> ReferralID { get; set; }

    [Display(Name = "CallReceivedDate")]
    public System.DateTime CallReceivedDate { get; set; }

    [Required]
    [Display(Name = "PriorityID")]
    public System.Nullable<int> PriorityID { get; set; }

    [Display(Name = "RequestedDate")]
    public System.DateTime? RequestedDate { get; set; }

    [Display(Name = "RequestedTime")]
    [MaxLength(50)]
    public string RequestedTime { get; set; }

    [Display(Name = "ServiceDate")]
    public System.DateTime? ServiceDate { get; set; }

    [Required]
    [Display(Name = "ServiceWindowID")]
    public System.Nullable<int> ServiceWindowID { get; set; }

    [Required]
    [Display(Name = "ExpectedPayTypeID")]
    public System.Nullable<int> ExpectedPayTypeID { get; set; }

    [Required]
    [Display(Name = "CustomerID")]
    public System.Nullable<int> CustomerID { get; set; }

    [Required]
    [Display(Name = "LocationID")]
    public System.Nullable<int> LocationID { get; set; }

    [Required]
    [Display(Name = "FranchiseID")]
    public System.Nullable<int> FranchiseID { get; set; }

    [Required]
    [Display(Name = "ConceptTypeID")]
    public System.Nullable<int> ConceptTypeID { get; set; }

    [Required]
    [Display(Name = "ServiceLength")]
    public System.Nullable<int> ServiceLength { get; set; }

    [Display(Name = "TimeStamp")]
    public byte[] TimeStamp { get; set; }

    [Display(Name = "CallCell")]
    [MaxLength(20)]
    public string CallCell { get; set; }

    [Display(Name = "CancelReason")]
    [MaxLength(2147483647)]
    public string CancelReason { get; set; }

}
