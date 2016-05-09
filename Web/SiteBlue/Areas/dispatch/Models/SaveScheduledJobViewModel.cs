using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.dispatch.Models
{
    public class SaveScheduledJobViewModel
    {
        public int JobId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TechnicianId { get; set; }
    }
}