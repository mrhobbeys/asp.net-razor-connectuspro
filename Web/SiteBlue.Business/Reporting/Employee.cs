using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Reporting
{
    /// <summary>
    /// Reporting View-Model
    /// </summary>
    public class Employee
    {
	    public int FranchiseID              {get;internal set;}
	    public string FranchiseNUmber       {get;internal set;}
	    public string FranchiseLegalName    {get;internal set;}
	    public int EmployeeID               {get;internal set;}
	    public string EmployeeName          {get;internal set;}
	    public decimal CommissionRate       {get;internal set;}
	    public bool ServiceProYN            {get;internal set;}
	    public bool ActiveYN                {get;internal set;}
    }
}
