using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

[Table("tbl_Payments")]
public class Payments
{
       [Key]
       [Display(Name = "PaymentID")]
       public int PaymentID { get; set; } 

       [Required]
       [Display(Name = "JobID")]
       public int JobID { get; set; } 

       [Display(Name = "PaymentDate")]
       public System.Nullable<System.DateTime> PaymentDate { get; set; } 

       [Display(Name = "PaymentAmount")]
       public decimal PaymentAmount { get; set; } 

       [Required]
       [Display(Name = "DepositStatus")]
       public bool DepositStatus { get; set; } 

       [Required]
       [Display(Name = "DepositID")]
       public int DepositID { get; set; } 

       [Required]
       [Display(Name = "PaymentTypeID")]
       public int PaymentTypeID { get; set; } 

       [Display(Name = "CheckNumber")]
       [MaxLength(20)]
       public string CheckNumber { get; set; } 

       [Display(Name = "DriversLicNUm")]
       [MaxLength(50)]
       public string DriversLicNUm { get; set; } 

       [Required]
       [Display(Name = "FranchiseID")]
       public int FranchiseID { get; set; } 

       [Display(Name = "TimeStamp")]
       public byte[] timestamp { get; set; } 

}
