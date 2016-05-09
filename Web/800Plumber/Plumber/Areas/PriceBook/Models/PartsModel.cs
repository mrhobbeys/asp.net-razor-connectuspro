using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    public class PartsModel
    {
        public int PartID { get; set; }

        public decimal PartCost { get; set; }
        public decimal PartStdPrice { get; set; }
        public decimal PartMemberPrice { get; set; }
        public decimal PartAddonStdPrice { get; set; }
        public decimal PartAddonMemberPrice { get; set; }

        public string PartCode { get; set; }
        public string PartName { get; set; }
    }
}