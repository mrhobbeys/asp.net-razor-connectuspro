using System;
using System.Collections.Generic;

namespace SiteBlue.Areas.HVAC_App.Models
{
    public class AccessoryModel: JobModel
    {
        public string Code { get; set; }

        public AccessoryModel()
        {
            Code = "";
            Count = 0;
            Description = "";
            Price = 0;
        }
    }
     

    public class JobPart
    {
        public decimal PartCost { get; set; }
        public string PartCode{ get; set; }
        public string PartName{ get; set; }
        public decimal PartStdPrice{ get; set; }
        public int PartID{ get; set; }
        public decimal Qty { get; set; } 
    }
}