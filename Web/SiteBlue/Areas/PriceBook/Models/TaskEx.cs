using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    public class TaskEx
    {
        public int JobCodeID { get; set; }
        public int SubSectionID { get; set; }
        public bool ManualPricingYN { get; set; }
        public bool ActiveYN { get; set; }
        public string JobCode { get; set; }
        public string JobCodeDescription { get; set; }
        public decimal JobCost { get; set; }
        public decimal JobStdPrice { get; set; }
        public decimal JobMemberPrice { get; set; }
        public decimal JobAddonStdPrice { get; set; }
        public decimal JobAddonMemberPrice { get; set; }
        public string ResAccountCode { get; set; }
        public string ComAccountCode { get; set; }
        public string ResAccountName { get; set; }
        public string ComAccountName { get; set; }

        public string ImgName { get; set; }
        public int MFlag { get; set; }
        public decimal LaborPercentage { get; set; }
    }
}