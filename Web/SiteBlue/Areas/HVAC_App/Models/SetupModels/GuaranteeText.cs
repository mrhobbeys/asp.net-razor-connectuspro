using System.Linq;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.HVAC_App.Models.SetupModels
{
    public class GuaranteeModel
    {
        public int Id { get; set; }
        public string GuaranteeName { get; set; }
        public string GuaranteeText { get; set; }

        public static GuaranteeModel[] GuaranteeModels(EightHundredEntities context, int configId)
        {
            var listOfGuaranteeIds = context.tbl_HVAC_Guarantees.Select(item => item.GuaranteeID).ToList();
            var listOfselectedGuarantees =
                context.tbl_HVAC_ConfigQuestions.Where(item => item.ConfigID == configId && listOfGuaranteeIds.Contains(item.QuestionID)).Select(i => i.QuestionID).ToArray();
            var gt = context.tbl_HVAC_ConfigGuaranteeTexts.Where(item => item.ConfigID == configId).ToList();
            var texts = new GuaranteeModel[listOfselectedGuarantees.Length];
            for (var i = 0; i < texts.Length; ++i)
            {
                var grId = listOfselectedGuarantees[i];
                var gr = context.tbl_HVAC_Guarantees.Single(item => item.GuaranteeID == grId);
                texts[i] = new GuaranteeModel
                {
                    Id = gr.GuaranteeID,
                    GuaranteeName = gr.GuaranteeName,
                    GuaranteeText = ""
                };
                if (gt.Any(item => item.GuaranteeID == gr.GuaranteeID))
                {
                    texts[i].GuaranteeText = gt.Single(item => item.GuaranteeID == gr.GuaranteeID).GuaranteeText;
                }
            }
            return texts;
        }
    }
}