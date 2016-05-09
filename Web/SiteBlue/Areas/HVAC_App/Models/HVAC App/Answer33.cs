using System.Collections.Generic;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.HVAC_App.Models.HVAC_App
{
    public class Answer33
    {
        public string email { get; set; }
        public List<HvacPaymentItem> payments { get; set; }
        public string total_amount { get; set; }
    }

    public class HvacPaymentItem
    {
        public long id { get; set; }
        public string type { get; set; }
        public int typeId { get; set; }
        public string payment { get; set; }
        public string code { get; set; }
    }

    public class HvacPaymentType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public HvacPaymentType() { }

        public HvacPaymentType(tbl_Payment_Types type)
        {
            Id = type.PaymentTypeId;
            Name = type.PaymentType;
        }
    }
}