using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace SiteBlue.Areas.OwnerPortal.Models
{
    [Table("tbl_Employee")]
    public class Employees
    {
        [Key]
        [Display(Name = "Employee ID")]
        public int EmployeeID { get; set; }

        [Required]
        [Display(Name = "Franchise ID")]
        public int FranchiseID { get; set; }

        [Display(Name = "Employee")]
        [MaxLength(50)]
        public string Employee { get; set; }

        [Required]
        [Display(Name = "ServicePro YN")]
        public bool ServiceProYN { get; set; }

        [Required]
        [Display(Name = "CallTaker YN")]
        public bool CallTakerYN { get; set; }

        [Required]
        [Display(Name = "CallMaker YN")]
        public bool CallMakerYN { get; set; }

        [Required]
        [Display(Name = "Dispatcher YN")]
        public bool DispatcherYN { get; set; }

        [Required]
        [Display(Name = "Invoicer YN")]
        public bool InvoicerYN { get; set; }

        [Required]
        [Display(Name = "Reportmaker YN")]
        public bool ReportmakerYN { get; set; }

        [Required]
        [Display(Name = "WorkStatus ID")]
        public int WorkStatusID { get; set; }

        [Display(Name = "Primary Phone")]
        [MaxLength(15)]
        public string PrimaryPhone { get; set; }

        [Display(Name = "Address")]
        [MaxLength(50)]
        public string Address { get; set; }

        [Display(Name = "City")]
        [MaxLength(50)]
        public string City { get; set; }

        [Display(Name = "State")]
        [MaxLength(2)]
        public string State { get; set; }

        [Display(Name = "Postal")]
        [MaxLength(15)]
        public string Postal { get; set; }

        [Display(Name = "Country")]
        [MaxLength(50)]
        public string Country { get; set; }

        [Required]
        [Display(Name = "ActiveYN")]
        public bool ActiveYN { get; set; }

        [Display(Name = "BirthDate")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Anniversary")]
        public System.Nullable<System.DateTime> Anniversary { get; set; }

        [Display(Name = "Spouse")]
        [MaxLength(50)]
        public string Spouse { get; set; }

        [Display(Name = "EmerContact")]
        [MaxLength(50)]
        public string EmerContact { get; set; }

        [Display(Name = "EmerAddress")]
        [MaxLength(50)]
        public string EmerAddress { get; set; }

        [Display(Name = "EmerCity")]
        [MaxLength(50)]
        public string EmerCity { get; set; }

        [Display(Name = "EmerState")]
        [MaxLength(2)]
        public string EmerState { get; set; }

        [Display(Name = "EmerPostal")]
        [MaxLength(15)]
        public string EmerPostal { get; set; }

        [Display(Name = "EmerCountry")]
        [MaxLength(50)]
        public string EmerCountry { get; set; }

        [Display(Name = "EmerPrimaryPhone")]
        [MaxLength(15)]
        public string EmerPrimaryPhone { get; set; }

        [Display(Name = "EmerSecondaryPhone")]
        [MaxLength(15)]
        public string EmerSecondaryPhone { get; set; }

        [Required]
        [Display(Name = "PayTypeID")]
        public int PayTypeID { get; set; }

        [Required]
        [Display(Name = "LaborRate")]
        public float LaborRate { get; set; }

        [Required]
        [Display(Name = "PartRate")]
        public float PartRate { get; set; }

        [Required]
        [Display(Name = "ExemptYN")]
        public bool ExemptYN { get; set; }

        [Display(Name = "DriversLic")]
        [MaxLength(50)]
        public string DriversLic { get; set; }

        [Display(Name = "SS Number")]
        [Column("SS#")]
        [MaxLength(16)]
        public string SSNumber { get; set; }

        [Display(Name = "HiredDate")]
        public System.Nullable<System.DateTime> HiredDate { get; set; }

        [Display(Name = "TerminatedDate")]
        public System.Nullable<System.DateTime> TerminatedDate { get; set; }

        [Display(Name = "Email")]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "VacationEarned")]
        public float VacationEarned { get; set; }

        [Required]
        [Display(Name = "VacationUsed")]
        public float VacationUsed { get; set; }

        [Required]
        [Display(Name = "SickEarned")]
        public float SickEarned { get; set; }

        [Required]
        [Display(Name = "SickUsed")]
        public float SickUsed { get; set; }

        [Required]
        [Display(Name = "PersonalEarned")]
        public float PersonalEarned { get; set; }

        [Required]
        [Display(Name = "PersonalUsed")]
        public float PersonalUsed { get; set; }

        [Required]
        [Display(Name = "OtherEarned")]
        public float OtherEarned { get; set; }

        [Required]
        [Display(Name = "OtherUsed")]
        public float OtherUsed { get; set; }

        [Required]
        [Display(Name = "HourlyRate")]
        public float HourlyRate { get; set; }

        [Required]
        [Display(Name = "WeeklySalary")]
        public float WeeklySalary { get; set; }

        [Display(Name = "DLExpDate")]
        public System.Nullable<System.DateTime> DLExpDate { get; set; }

        [Display(Name = "ProLisc")]
        [MaxLength(50)]
        public string ProLisc { get; set; }

        [Display(Name = "ProLiscExpDate")]
        public System.Nullable<System.DateTime> ProLiscExpDate { get; set; }

        [Display(Name = "ProLevel")]
        [MaxLength(50)]
        public string ProLevel { get; set; }

        [Display(Name = "TimeStamp")]
        public byte[] timestamp { get; set; }

    }
}