using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class JobStatusHistory
    {
        public string statuses { get; set; }
        public string statuschangeddate { get; set; }
        public string time { get; set; }
        public string tablet { get; set; }
        public string bywhom { get; set; }
        public string field { get; set; }
        public string changedto { get; set; }
    }
}