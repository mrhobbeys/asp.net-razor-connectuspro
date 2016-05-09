using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.HVAC_App.Models.SetupModels
{
    public class PackageVideoCode
    {
        public int Id { get; set; }
        public string PackageName { get; set; } 
        [Display(Name = "Enter YouTube video code")]
        public string Code { get; set; }
    }
}