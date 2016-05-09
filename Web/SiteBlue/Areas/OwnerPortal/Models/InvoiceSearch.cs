using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class InvoiceSearch
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SelectList StatusList { get; set; } 
    }
}