using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_JobCodes")]
    public class Task
    {
        [Key]
        [Display(Name = "JobCode ID")]
        public int JobCodeID { get; set; }

        [Display(Name = "Sub Section ID")]
        [ForeignKey("SubSection")]
        public int SubSectionID { get; set; }

        public SubSection SubSection { get; set; }

        [Display(Name = "Manual Pricing")]
        public bool ManualPricingYN { get; set; }

        [Display(Name = "Active")]
        public bool ActiveYN { get; set; }

        [Display(Name = "Code")]
        [MaxLength(10)]
        public string JobCode { get; set; }

        [Display(Name = "Code Description")]
        [MaxLength(100)]
        public string JobCodeDescription { get; set; }

        [Display(Name = "Cost")]
        public decimal JobCost { get; set; }

        [Display(Name = "Standard Price")]
        public decimal JobStdPrice { get; set; }

        [Display(Name = "Member Price")]
        public decimal JobMemberPrice { get; set; }

        [Display(Name = "Add-On Standard Price")]
        public decimal JobAddonStdPrice { get; set; }

        [Display(Name = "Add-On Member Price")]
        public decimal JobAddonMemberPrice { get; set; }

        [Display(Name = "RES Account Code")]
        [MaxLength(5)]
        public string ResAccountCode { get; set; }

        [Display(Name = "COM AccountCode")]
        [MaxLength(5)]
        public string ComAccountCode { get; set; }

        public int MFlag { get; set; }

        public virtual ICollection<TaskDetail> TaskDetails { get; set; }
    }
}