using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteBlue.Areas.HVAC_App.Models.SetupModels
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
    }

    public class QuestionsComparer : IEqualityComparer<SetupQuestion>
    {
        public bool Equals(SetupQuestion x, SetupQuestion y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(SetupQuestion obj)
        {
            return obj.Id;
        }
    }

    public class SetupQuestion : Question
    {
        public List<int> LinkedIds { get; set; }
        public List<int> NotLessIndexIds { get; set; }
    }
}