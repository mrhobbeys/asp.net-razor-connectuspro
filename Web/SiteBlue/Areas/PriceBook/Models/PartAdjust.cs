using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.PriceBook.Models
{
    public class PartAdjust
    {
        public int PartID { get; set; }
        public int MasterPartID { get; set; }
        public string PartCode { get; set; }
        public string PartDescription { get; set; }
        public decimal? FrequencyUsed { get; set; }
        public decimal? PartCost { get; set; }
        public decimal? PartStdPrice { get; set; }
        public decimal? PartMemberPrice { get; set; }
        public decimal? PartAddonStdPrice { get; set; }
        public decimal? PartAddonMemberPrice { get; set; }
    }
}