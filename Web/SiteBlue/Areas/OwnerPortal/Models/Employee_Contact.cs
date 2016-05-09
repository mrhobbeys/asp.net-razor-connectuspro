using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

[Table("tbl_Employee_Contact")]
public class Employee_Contact
{
    [Key]
    [Display(Name = "EmpContactID")]
    public int EmpContactID { get; set; }

    [Required]
    [Display(Name = "PhoneTypeID")]
    public int PhoneTypeID { get; set; }

    [Display(Name = "PhoneNumber")]
    [MaxLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "EmployeeID")]
    public int EmployeeID { get; set; }

}
