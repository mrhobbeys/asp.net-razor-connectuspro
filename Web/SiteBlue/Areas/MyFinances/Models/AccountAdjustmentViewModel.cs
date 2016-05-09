using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteBlue.Areas.MyFinances.Models
{
    public class AccountAdjustmentViewModel
    {
        public int? JobID { get; set; }
        public decimal? AdjustmentAmount { get; set; }
        public SelectList AdjustmentTypeList { get; set; }
        public string Comment { get; set; }
    }
}