using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.HVAC_App.Models.SetupModels
{
    public class SetupQuestionsModel
    {
        public List<SetupQuestion> QuestionsForChoose { get; set; }

        public List<SetupQuestion> QuestionsSelected { get; set; }

        public List<int> NotDeleteIds { get; set; }

        public List<int> NotReorderIds { get; set; }
    }
}