using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SiteBlue.Questionnaire.Data
{
    public class QuestionnaireContext : DbContext
    {
        public DbSet<AccountingInformation> AccountingInformation { get; set; }
        public DbSet<AdvertisedPhoneNumber> AdvertisedPhoneNumber { get; set; }
        public DbSet<BusinessInformation> BusinessInformation { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Day> Day { get; set; }
        public DbSet<Dba> Dba { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<GPSSystem> GPSSystem { get; set; }
        public DbSet<Hvac> Hvac { get; set; }
        public DbSet<Level> Level { get; set; }
        public DbSet<LicenseNumber> LicenseNumber { get; set; }
        public DbSet<LicenseType> LicenseType { get; set; }
        public DbSet<MarketingType> MarketingType { get; set; }
        public DbSet<OwnerInformation> OwnerInformation { get; set; }
        public DbSet<Plumbing> Plumbing { get; set; }
        public DbSet<Questionnaire> Questionnaire { get; set; }
        public DbSet<ReferralCode> ReferralCode { get; set; }
        public DbSet<ServicePlan> ServicePlan { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<TechnicianInformation> TechnicianInformation { get; set; }
        public DbSet<WarrantyWork> WarrantyWork { get; set; }
        public DbSet<ZipCode> ZipCode { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<OfficePersonnel> OfficePersonnel { get; set; }
        public DbSet<AddressType> AddressType { get; set; }
        public DbSet<QuestionnaireInformation> QuestionnaireInformation { get; set; }
    }
}
