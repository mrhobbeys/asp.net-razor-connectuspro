using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_JobCodes_Details")]
    public class TaskDetail
    {

        [Key]
        [Display(Name = "TaskDetail ID")]
        public int JobCodeDetailsID { get; set; }

        [ForeignKey("Task")]
        [Display(Name = "Task ID")]
        public int JobCodeID { get; set; }

        public virtual Task Task { get; set; }

        public int PartID { get; set; }
        public decimal Qty { get; set; }
        public bool ManualPricingYN { get; set; }
        public decimal PartCost { get; set; }
        public decimal PartStdPrice { get; set; }
        public decimal PartMemberPrice { get; set; }
        public decimal PartAddonStdPrice { get; set; }
        public decimal PartAddonMemberPrice { get; set; }
    }
}