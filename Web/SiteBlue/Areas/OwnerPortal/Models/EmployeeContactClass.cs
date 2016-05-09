using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class EmployeeContactClass
    {
        public int EmpContactID { get; set; } 
        public int PhoneTypeID { get; set; }
        public string PhoneType { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployeeID { get; set; }
    }
}