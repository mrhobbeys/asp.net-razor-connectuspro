using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

 [Table("tbl_Referral")]
public class Referral
{
       [Key]
       [Display(Name = "referralID")]
       public int referralID { get; set; } 

       [Display(Name = "ReferralType")]
       [MaxLength(50)]
       public string ReferralType { get; set; } 

       [Required]
       [Display(Name = "activeYN")]
       public bool activeYN { get; set; }

       [Display(Name = "Franchise ID")]
       public System.Nullable<int> FranchiseID { get; set; }

}
