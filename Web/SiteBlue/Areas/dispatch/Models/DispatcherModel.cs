using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.Dispatch.Models
{
    public class DispatcherModel
    {
        [Key]
        public int ServiceProID { get; set; }
        public string Employee { get; set; }
        public int EmployeeID { get; set; }
        public int FranchiseID { get; set; }

        public List<DispatcherModel> lst { get; set; }
    }
}