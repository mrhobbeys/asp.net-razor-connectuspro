using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    public class Labor
    {
        public int PartID { get; set; }
        public string PartName { get; set; }
        public decimal LaborPrice { get; set; }
        public decimal PartCost { get; set; }
        public decimal PartStdPrice { get; set; }
        public decimal PartMemberPrice { get; set; }
        public decimal PartAddonStdPrice { get; set; }
        public decimal PartAddonMemberPrice { get; set; }
    }
}