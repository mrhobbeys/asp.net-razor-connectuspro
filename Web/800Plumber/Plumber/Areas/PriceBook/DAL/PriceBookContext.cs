using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SiteBlue.Areas.PriceBook.Models
{
    public class PriceBookContext : DbContext
    {
        public DbSet<Franchise> franchises { get; set; }
        public DbSet<PriceBooks> pricebooks { get; set; }
        public DbSet<Section> sections { get; set; }
        public DbSet<SubSection> subsections { get; set; }
        public DbSet<Task> tasks { get; set; }
        public DbSet<TaskDetail> taskdetails { get; set; }
        public DbSet<Parts> parts { get; set; }
        public DbSet<MasterParts> masterparts { get; set; }
        public DbSet<TaskImage> taskimage { get; set; }
        public DbSet<AccountCodes> accountcode { get; set; }
        public DbSet<PriceBookRates> pricebookrates { get; set; }
        public DbSet<Markups> markups { get; set; }
        public DbSet<PartCodes> partcodes { get; set; }
        public DbSet<LaborView> laborviews { get; set; }
        public DbSet<LaborSectionView> laborsectionviews { get; set; }
        public DbSet<LaborSubSectionView> laborsubsectionviews { get; set; }
        public DbSet<LaborTaskView> labortaskviews { get; set; }
        public DbSet<LaborPriceBookView> laborpricebookviews { get; set; }
    }
}