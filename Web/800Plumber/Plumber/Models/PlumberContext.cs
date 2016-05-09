using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Plumber.Models
{
    public class PlumberContext: DbContext
    {

        public DbSet<CityServed> CityServed { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Management> Management { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<MediaExtension> MediaExtension { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<ServiceType> ServiceType { get; set; }
        public DbSet<SpecialOffer> SpecialOffer { get; set; }
        public DbSet<LocationService> LocationServices { get; set; }
        public DbSet<LocationCoupon> LocationCoupons { get; set; }
        public DbSet<LocationZip> LocationZip { get; set; }
        public DbSet<TerritoryOwner> TerritoryOwners { get; set; }
        public DbSet<CareerApplication> CareerApplication { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Franchise> Franchise { get; set; }
        public DbSet<ZipList> PostalCode { get; set; }
        public DbSet<ServiceCategory> ServiceCategory { get; set; }
        public DbSet<LocationServiceCategory> LocationServiceCategory { get; set; }
        public DbSet<FranchiseService> FranchiseService { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<ServiceFeedback> ServiceFeedback { get; set; }
        public DbSet<Template> Template { get; set; }
        public DbSet<LocationImage> LocationImage { get; set; }
        public DbSet<Testimonial> Testimonial { get; set; }
        public DbSet<NearestLocationNotification> NearestLocationNotification { get; set; }
        public DbSet<Site> Site { get; set; }
        public DbSet<TrainingType> TrainingType { get; set; }
        public DbSet<Training> Training { get; set; }
        public DbSet<Career> Career { get; set; }
    }
}