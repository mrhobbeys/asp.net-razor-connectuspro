using System.ComponentModel.DataAnnotations;
using SiteBlue.Areas.OwnerPortal.Models;

 [Table("tbl_Customer")]
public class Customers
{
       [Key]
       [Display(Name = "CustomerID")]
       [ForeignKey("BilltoLocation")]
       public int CustomerID { get; set; }
     
       public Locations BilltoLocation { get; set; }

       [Display(Name = "FindByName")]
       [MaxLength(150)]
       public string FindByName { get; set; } 

       [Display(Name = "CustomerName")]
       [MaxLength(50)]
       public string CustomerName { get; set; } 

       [Display(Name = "CompanyName")]
       [MaxLength(50)]
       public string CompanyName { get; set; } 

       [Display(Name = "EMail")]
       [MaxLength(150)]
       public string EMail { get; set; } 

       [Display(Name = "TimeStamp")]
       public byte[] timestamp { get; set; } 

}
