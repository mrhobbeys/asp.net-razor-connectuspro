using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.CallCenter.Models
{
    public class SaveResult
    {
        public bool Success { get; set; }
        public int? SavedId { get; set; }
        public string[] Messages { get; set; }
    }
}