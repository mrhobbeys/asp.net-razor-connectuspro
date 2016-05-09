using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_CareerApplication")]
    public class CareerApplication
    {
        [Key]
        [ScaffoldColumn(false)]
        public int CareerApplicationId { get; set; }
        
        [DisplayName("First and Last Name")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        [Required(ErrorMessage = "Required")]
        public string FirstLastName { get; set; }

        [DisplayName("Address")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        [Required(ErrorMessage = "Required")]
        public string Address { get; set; }

        [DisplayName("City")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        [Required(ErrorMessage = "Required")]
        public string City { get; set; }

        [DisplayName("State")]
        [StringLength(2, ErrorMessage = "Validation error - Length: 2")]
        [Required(ErrorMessage = "Required")]
        public string State { get; set; }

        [DisplayName("Zip")]
        [StringLength(15, ErrorMessage = "Validation error - Length: 15")]
        [Required(ErrorMessage = "Required")]
        public string Zip { get; set; }

        [DisplayName("Phone")]
        [StringLength(20, ErrorMessage = "Validation error - Length: 20")]
        [Required(ErrorMessage = "Required")]
        public string Phone { get; set; }

        [DisplayName("Mobile Phone")]
        [StringLength(20, ErrorMessage = "Validation error - Length: 20")]
        public string MobilePhone { get; set; }

        [DisplayName("Email")]
        [StringLength(150, ErrorMessage = "Validation error - Length: 150")]
        [Required(ErrorMessage = "Required")]
        public string Email { get; set; }

        [DisplayName("Position Desired")]
        [StringLength(255, ErrorMessage = "Validation error - Length: 255")]
        public string PositionDesired { get; set; }

        [DisplayName("How many years of experience?")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string ExperienceYears { get; set; }

        [DisplayName("Do you have a plumbing license?")]
        [DefaultValue(false)]
        public bool? PlumbingLicense { get; set; }

        [DisplayName("Desired Salary")]
        public double? DesiredSalary { get; set; }

        [DisplayName("Best time to contact")]
        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string BestTimeToContact { get; set; }

        public int MessageStatusId { get; set; }

        public virtual MessageStatus MessageStatus { get; set; }
    }
}