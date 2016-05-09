namespace HVACapp.Areas.HVAC_App.Models
{
    public class AnswerModel
    {
        public int AnswerID { get; set; }
        public int UserID { get; set; } //salesman ID
        public int CustomerID { get; set; }
        public int QuestionID { get; set; }
        public string Answer { get; set; }
    }
}