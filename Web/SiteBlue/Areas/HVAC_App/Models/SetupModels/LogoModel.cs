using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.HVAC_App.Models.SetupModels
{
    public class LogoModel
    {
        private readonly string _url = "";

        [Display(Name = "Backgroung image url")]
        public string Url { get { return _url; } }

        public LogoModel(int configId, EightHundredBaseContext context)
        {
            var listofurls = context.tbl_HVAC_ConfigLogoUrl.Where(item => item.ConfigID == configId).ToList();
            _url = listofurls.Count != 0 ? listofurls.First().Logourl : "../../../Areas/hvac_app/content/bigimage.png";
        }

        public static void UpdateOrAddedUrl(string url, EightHundredBaseContext context, int configId)
        {
            var listofurls = context.tbl_HVAC_ConfigLogoUrl.Where(item => item.ConfigID == configId).ToList();
            if (listofurls.Count != 0)
            {
                var urlTemp = listofurls.First();
                urlTemp.Logourl = url;
            }
            else
            {
                var urlTemp = new tbl_HVAC_ConfigLogoUrl
                                  {
                                      ConfigID = configId,
                                      Logourl = url
                                  };
                context.tbl_HVAC_ConfigLogoUrl.AddObject(urlTemp);
            }
        }
    }
}