using System;
using System.Collections.Generic;

namespace SiteBlue.Areas.MyCalls.Models
{
    public class GridModel
    {
        public GridHeader Header { get; set; }
        public Dictionary<InfoNumber, List<KeyValuePair<string, int>>> GridData { get; set; }
        public int? opt { get; set; }
    }

    public class GridHeader
    {
        public List<string> Options { get; set; } 
    }

    public class InfoNumber
    {
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }
}