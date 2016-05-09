using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("BusinessInformation")]
    public class BusinessInformation
    {
        [Key]
        [ScaffoldColumn(false)]
        public long BusinessInformationId { get; set; }

        [DisplayName("Questionnaire")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public long QuestionnaireId { get; set; }

        [DisplayName("Office phone")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string OfficePhone { get; set; }

        [DisplayName("Cell phone")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string CellPhone { get; set; }

        [DisplayName("Same as owner address / phone")]
        [DefaultValue(false)]
        public bool SameAsOwnerInfo { get; set; }

        [DisplayName("Website")]
        //[Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string Website { get; set; }

        [DisplayName("Business address")]
        //[Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string BusinessAddress { get; set; }

        [DisplayName("City")]
        [Column("City")]
        //[Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string BusinessCity { get; set; }

        [DisplayName("State")]
        [Column("StateId")]
        //[Required(ErrorMessage = "Required")]
        public int? StateId { get; set; }

        [DisplayName("ZIP")]
        [Column("ZipCode")]
        //[Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string BusinessZipCode { get; set; }

        //[DisplayName("Number of trucks")]
        [DisplayName("Service")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TrucksService { get; set; }

        [DisplayName("Commercial")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TrucksCommercial { get; set; }

        [DisplayName("Installation")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TrucksInstallation { get; set; }

        [DisplayName("Sales")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TrucksSales { get; set; }

        //[DisplayName("Number of technicians")]
        [DisplayName("Service")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TechniciansService { get; set; }

        [DisplayName("Commercial")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TechniciansCommercial { get; set; }

        [DisplayName("Installation")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TechniciansInstallation { get; set; }

        [DisplayName("Sales")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string TechniciansSales { get; set; }

        [DisplayName("Do You have GPS")]
        [DefaultValue(false)]
        public bool GPS { get; set; }

        [DisplayName("What system?")]
        public int? GPSSystemId { get; set; }

        [DisplayName("Other system")]
        public string OtherGPSSystem { get; set; }

        [DisplayName("GPS Plan Expiration Date")]
        public DateTime? GPSExpirationDate { get; set; }

        [DisplayName("If you do HVAC work, how many estimators do you have on staff?")]
        public string Estimators { get; set; }

        //[DisplayName("Hours of operation")]
        //public string OperationHours { get; set; }

        [DisplayName("Start hour")]
        public string MondayFridayST { get; set; }

        [DisplayName("End hour")]
        public string MondayFridayET { get; set; }

        [DisplayName("Start hour")]
        public string SaturdayST { get; set; }

        [DisplayName("End hour")]
        public string SaturdayET { get; set; }

        [DisplayName("Start hour")]
        public string SundayST { get; set; }

        [DisplayName("End hour")]
        public string SundayET { get; set; }

        [DisplayName("Do you charge service trip fees?")]
        [DefaultValue(false)]
        public bool ServiceTripFees { get; set; }

        [DisplayName("Please describe those fees. Do your service plan members pay this fee?")]
        public string ServiceTripFeesDescription { get; set; }

        //[DisplayName("Are technician available nights, weekend and holidays?")]
        [DisplayName("Nights")]
        [DefaultValue(false)]
        public bool TechniciansAvailabilityNights { get; set; }

        [DisplayName("Weekend")]
        [DefaultValue(false)]
        public bool TechniciansAvailabilityWeekend { get; set; }

        [DisplayName("Holidays")]
        [DefaultValue(false)]
        public bool TechniciansAvailabilityHolidays { get; set; }

        [DisplayName("Additional details")]
        public string AdditionalDetails { get; set; }

        [DisplayName("Do you have extra charges for service afterhours or on holidays?")]
        [DefaultValue(false)]
        public bool AfterHoursHolidaysFees { get; set; }

        [DisplayName("If so, please describe those fees:")]
        public string AfterHoursHolidaysFeesDescription { get; set; }

        [DisplayName("Please provide a script of how you would like your phones answered.")]
        [DefaultValue("Plumbing Service Department, how may we help you?")]
        [MaxLength(5000, ErrorMessage = "Maximum characters allowed: 500")]
        public string ServiceCenterAgentsAnswer { get; set; }

        [DisplayName("FlateRate")]
        [DefaultValue(false)]
        public bool FlateRate { get; set; }

        [DisplayName("Time and Material")]
        [DefaultValue(false)]
        public bool TimeAndMaterial { get; set; }

        [DisplayName("Do you do warranty work for any outside provider?")]
        [DefaultValue(false)]
        public bool DoWarrantyWork { get; set; }

        [DisplayName("Do you offer service plans?")]
        [DefaultValue(false)]
        public bool ServicePlansOffer { get; set; }

        [DisplayName("What is the discount for service plan members?")]
        public string ServicePlanMembersDiscount { get; set; }

        [DisplayName("Do service plan members receive maintenance visits?")]
        [DefaultValue(false)]
        public bool ServicePlanMembersMaintenance { get; set; }

        [DisplayName("How often?")]
        public string ServicePlanMembersTime { get; set; }

        [DisplayName("Do service plan members pay trip charges?")]
        [DefaultValue(false)]
        public bool ServicePlanPayTrip { get; set; }

        [DisplayName("First name")]
        public string KeyOfficeFirstName { get; set; }

        [DisplayName("Last name")]
        public string KeyOfficeLastName { get; set; }

        [DisplayName("Title")]
        public string KeyOfficeTitle { get; set; }

        [DisplayName("Cell phone")]
        public string KeyOfficeCellPhone { get; set; }

        [DisplayName("Office phone")]
        public string KeyOfficeOfficePhone { get; set; }

        [DisplayName("Name of Service Plan")]
        public string ServicePlanName { get; set; }

        [DisplayName("Plan Description")]
        public string ServicePlanComment { get; set; }

        public virtual Questionnaire Questionnaire { get; set; }

        public virtual State State { get; set; }

        public virtual GPSSystem GPSSystem { get; set; }

        public virtual List<AdvertisedPhoneNumber> AdvertisedPhoneNumbers { get; set; }

        public virtual List<Dba> Dbas { get; set; }

        public virtual List<Discount> Discounts { get; set; }

        public virtual List<LicenseNumber> LicenseNumbers { get; set; }

        public virtual List<MarketingType> MarketingTypes { get; set; }

        public virtual List<ReferralCode> ReferralCodes { get; set; }

        public virtual List<ServicePlan> ServicePlans { get; set; }

        public virtual List<WarrantyWork> WarrantyWorks { get; set; }

        public virtual List<ZipCode> ZipCodes { get; set; }

        public virtual List<Address> Addresses { get; set; }

        public virtual List<OfficePersonnel> OfficePersonnels { get; set; }
    }
}
