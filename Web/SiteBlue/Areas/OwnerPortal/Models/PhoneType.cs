using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

 [Table("tbl_PhoneType")]
public class PhoneTypes
{
       [Key]
       [Display(Name = "PhoneTypeID")]
       public int PhoneTypeID { get; set; } 

       [Display(Name = "PhoneType")]
       [MaxLength(50)]
       public string PhoneType { get; set; }

       public virtual ICollection<Franchise_Contacts> FranchiseContacts { get; set; }  

}
