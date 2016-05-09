using System;

namespace SiteBlue.Areas.CallCenter.Models
{
    public class CustomerInformation
    {
        public int ContactID { get; set; }
        public int SecondaryContactID { get; set; }
        public string SecondaryContactNumber { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public int LocationID { get; set; }
        public bool IsMember { get; set; }
        public bool IsPriority { get; set; }
        public DateTime? MemberFrom { get; set; }
        public DateTime? MemberTo { get; set; }
        public int FranchiseID { get; set; }

    }
}