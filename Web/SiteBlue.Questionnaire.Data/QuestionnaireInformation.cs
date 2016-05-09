using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("VW_Questionnaire")]
    public class QuestionnaireInformation
    {
        [Key]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long QuestionnaireId { get; set; }
        public long? OwnerInformationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessName { get; set; }
        public string OIHomeAddress { get; set; }
        public string OICity { get; set; }
        public string OIState { get; set; }
        public string OIZipCode { get; set; }
        public string OICellPhone { get; set; }
        public string OIHomePhone { get; set; }
        public string OIEmail { get; set; }
        public long? BusinessInformationId { get; set; }
        public string OfficePhone { get; set; }
        public string CellPhone { get; set; }
        public string Website { get; set; }
        public string BusinessAddress { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public string BSState { get; set; }
        public string ZipCode { get; set; }
        public string TrucksService { get; set; }
        public string TrucksCommercial { get; set; }
        public string TrucksInstallation { get; set; }
        public string TechniciansService { get; set; }
        public string TechniciansCommercial { get; set; }
        public string TechniciansInstallation { get; set; }
        public bool? GPS { get; set; }
        public int? GPSSystemId { get; set; }
        public string GPSSystemName { get; set; }
        public string OtherGPSSystem { get; set; }
        public string Estimators { get; set; }
        public string MondayFridayST { get; set; }
        public string MondayFridayET { get; set; }
        public string SaturdayST { get; set; }
        public string SaturdayET { get; set; }
        public string SundayST { get; set; }
        public string SundayET { get; set; }
        public bool? ServiceTripFees { get; set; }
        public string ServiceTripFeesDescription { get; set; }
        public bool? TechniciansAvailabilityNights { get; set; }
        public bool? TechniciansAvailabilityWeekend { get; set; }
        public bool? TechniciansAvailabilityHolidays { get; set; }
        public bool? TechniciansAvailability { get; set; }
        public string AdditionalDetails { get; set; }
        public bool? AfterHoursHolidaysFees { get; set; }
        public string AfterHoursHolidaysFeesDescription { get; set; }
        public string ServiceCenterAgentsAnswer { get; set; }
        public bool? FlateRate { get; set; }
        public bool? TimeAndMaterial { get; set; }
        public bool? DoWarrantyWork { get; set; }
        public bool? ServicePlansOffer { get; set; }
        public string ServicePlanMembersDiscount { get; set; }
        public bool? ServicePlanMembersMaintenance { get; set; }
        public string ServicePlanMembersTime { get; set; }
        public string KeyOfficeFirstName { get; set; }
        public string KeyOfficeLastName { get; set; }
        public string KeyOfficeTitle { get; set; }
        public string KeyOfficeCellPhone { get; set; }
        public string KeyOfficeOfficePhone { get; set; }
        public string ServicePlanName { get; set; }
        public string ServicePlanComment { get; set; }
        public string TrucksSales { get; set; }
        public string TechniciansSales { get; set; }
        public DateTime? GPSExpirationDate { get; set; }
        public bool? ServicePlanPayTrip { get; set; }
        public long? AccountingInformationId { get; set; }
        public string CurrentAccountingSoftware { get; set; }
        public string CurrentAccountingSoftwareUsingTime { get; set; }
        public DateTime? LastCompletedDate { get; set; }
        public bool? ChargeSalesTax { get; set; }
        public double? SalesTaxCharge { get; set; }
        public string TaxId { get; set; }
        public string RegistredMunicipality { get; set; }
        public string UseQuickBooks { get; set; }
        public DateTime? FiscalYearEnd { get; set; }
        public string CalendarType { get; set; }
        public string OtherCalendarType { get; set; }
        public DateTime? ReconciledBankDate { get; set; }
        public DateTime? ReconsiledClosedMonthEnd { get; set; }
        public string QuickBooksTransitionName { get; set; }
        public string QuickBooksTransitionPhone { get; set; }
        public string AccountingKnowledgeRate { get; set; }
        public string FinancialTransactionRecord { get; set; }
        public string OtherFinancialTransactionRecord { get; set; }
        public string AccountsChart { get; set; }
        public string GeneralJourneyEntries { get; set; }
        public string AccountingSystemAddons { get; set; }
        public string AccountingSystemAddonsList { get; set; }
        public string AchWithdrawls { get; set; }

    }
}
