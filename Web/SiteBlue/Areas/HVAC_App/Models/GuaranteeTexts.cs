using System.Collections.Generic;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.HVAC_App.Models
{
    public class GuaranteeTexts
    {
        //26 - Comfort Guarantee (cg)
        //27 - Lemon Free Guarantee (lfg)
        //28 - Never Undersold Guarantee (nug)
        //29 - Home Respect Guarantee (hrg)
        //30 - Complete Satisfaction Guarantee (csg)
        //36 -
        //37 -
        public string cg { get; set; }
        public string lfg { get; set; }
        public string nug { get; set; }
        public string hrg { get; set; }
        public string csg { get; set; }
        public string npp { get; set; }
        public string ns { get; set; }

        public void SetValuesFromList(List<tbl_HVAC_ConfigGuaranteeTexts> list)
        {
            foreach (var textse in list)
            {
                switch (textse.GuaranteeID)
                {
                    case 26:
                        {
                            cg = textse.GuaranteeText;
                            break;
                        }
                    case 27:
                        {
                            lfg = textse.GuaranteeText;
                            break;
                        }
                    case 28:
                        {
                            nug = textse.GuaranteeText;
                            break;
                        }
                    case 29:
                        {
                            hrg = textse.GuaranteeText;
                            break;
                        }
                    case 30:
                        {
                            csg = textse.GuaranteeText;
                            break;
                        }

                    case 36:
                        {
                            npp = textse.GuaranteeText;
                            break;
                        }
                    case 37:
                        {
                            ns = textse.GuaranteeText;
                            break;
                        }
                }
            }
        }
    }
}