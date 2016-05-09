using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SiteBlue.Areas.PriceBook.Models;

 [Table("tbl_Franchise_Contacts")]
public class Franchise_Contacts
{
       [Key]
       [Display(Name = "FranchiseContactID")]
       public int FranchiseContactID { get; set; } 

       [Required]
       [Display(Name = "FranchiseID")]
       public int FranchiseID { get; set; } 

       [Display(Name = "ContactName")]
       [MaxLength(50)]
       public string ContactName { get; set; } 

       [Required]
       [Display(Name = "PhoneTypeID")]
       public int PhoneTypeID { get; set; } 

       [Display(Name = "PhoneNumber")]
       [MaxLength(15)]
       public string PhoneNumber { get; set; }

       public virtual Franchise Franchisee { get; set; }
       public virtual PhoneTypes PhoneType { get; set; }

}
