using System.Collections.Generic;
using System.Linq;

namespace SiteBlue.Areas.HVAC_App.Models
{
    public class JobModel
    {
        public int JobCodeId { get; set; }
        public string JobCode { get; set; }
        public string ResAccountCode { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public virtual decimal TotalPrice { get { return Price*Count; } }
    }

    public class SystemModel : JobModel
    {
        public string id { get; set; }
        public string AFUE { get; set; }
        public string SEER { get; set; } 
    }

    public class SystemInfoModelWithParts : SystemModel
    {
        public SystemInfoModelWithParts()
        {
            Parts = new List<JobPart>();
        }
        public List<JobPart> Parts { get; set; }
        public override decimal TotalPrice
        {
            get
            {
                var total = 0m;
                total = Parts.Aggregate(total, (current, part) => current + part.PartStdPrice*part.Qty);
                return total;
            }
        }
    }

    public class TotalInfo
    {
        public TotalInfo()
        {
            Jobs = new List<JobModel>();
        }

        public SystemInfoModelWithParts MainSystem { get; set; }
        public ICollection<JobModel> Jobs { get; private set; }

        public string TotalDescription { 
            get
            {
                var desription = "";
                desription = Jobs.Aggregate(desription, (current, accessoryModel) => current + accessoryModel.Description + "<br/>" );
                return desription;
            }
        }

        public decimal TotalAmount
        {
            get
            {
                var amount = 0m;
                amount = Jobs.Aggregate(amount, (current, accessoryModel) => current + accessoryModel.Count * accessoryModel.TotalPrice);
                return amount;
            }
        }

        public decimal Tax { get; set; }

        public decimal GrandTotal
        {
            get
            {
                var ta = TotalAmount;
                return ta + ta*Tax;
            }
        }
        
        public void AddMainSystem(SystemInfoModelWithParts system)
        {
            //if (system.Count != 0)
                Jobs.Add(system);
        }

        public void AddAccessory(AccessoryModel accessory)
        {
            if (accessory.Count!=0)
                Jobs.Add(accessory);
        }
    }
}