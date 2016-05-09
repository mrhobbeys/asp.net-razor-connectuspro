using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class EmployeeHistoryInfo
    {
        public string FieldName { get; set; }
        public string TableName { get; set; }
        public string ChangeType { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string isTablet { get; set; }
        public string ChangedBy { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}