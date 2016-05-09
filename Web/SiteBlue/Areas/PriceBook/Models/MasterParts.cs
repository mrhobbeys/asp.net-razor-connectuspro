using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_PB_MasterParts")]
    public class MasterParts
    {
        [Key]
        [Display(Name = "MasterPart Id")]
        public int MasterPartID { get; set; }

        public int ConceptID { get; set; }
        public int FranchiseID { get; set; }
        public string PartCodeID { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public decimal PartCost { get; set; }
        public string VendorPartID { get; set; }
        public bool ActiveYN { get; set; }
    }
}