using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.DocumentGeneration.TechBios
{
    public class Bio
    {
        public int TechId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int ClientId { get; set; }
        public bool IsFranchise { get; set; }
        public DateTime LastDrugTest { get; set; }
        public DateTime BackgroundCheckCompleted { get; set; }
    }
}