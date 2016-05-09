using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    public class TaskDetailEx
    {
        public int JobCodeDetailsID { get; set; }
        public int JobCodeID { get; set; }
        public int PartID { get; set; }
        public decimal Qty { get; set; }
        public bool ManualPricingYN { get; set; }
        public decimal PartCost { get; set; }
        public decimal PartStdPrice { get; set; }
        public decimal PartMemberPrice { get; set; }
        public decimal PartAddonStdPrice { get; set; }
        public decimal PartAddonMemberPrice { get; set; }
        public string PartName { get; set; }
        public string VendorPartID { get; set; }
        public string PartCodeID { get; set; }
    }
}