using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.HVAC_App.Models.SetupModels
{
    public class FinanceOptionsModel:PackageModel
    {
        public List<FinanceOption> FinanceOptions { get; set; } 
    }

    public class FinanceOption
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}