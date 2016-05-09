using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("AccountingInformation")]
    public class AccountingInformation
    {
        [Key]
        [ScaffoldColumn(false)]
        public long AccountingInformationId { get; set; }

        [DisplayName("Questionnaire")]
        public long QuestionnaireId { get; set; }

        [DisplayName("What is your current accounting software?")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string CurrentAccountingSoftware { get; set; }

        [DisplayName("How long have you been using your current accounting software?")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        public string CurrentAccountingSoftwareUsingTime { get; set; }

        [DisplayName("What is your last completed month – the date that everything has been entered and all check processed?")]
        [Required(ErrorMessage = "Required", AllowEmptyStrings = true)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/DD/YYYY}")]
        public DateTime? LastCompletedDate { get; set; }

        [DisplayName("Do you charge sales tax?")]
        [DefaultValue(false)]
        public bool ChargeSalesTax { get; set; }

        [DisplayName("If so, what is the sales tax rate?")]
        public double? SalesTaxCharge { get; set; }

        [DisplayName("What is your tax id/account number for your local municipality?")]
        public string TaxId { get; set; }

        [DisplayName("What is the name of the municipality with who your tax id/account number is registered?")]
        public string RegistredMunicipality { get; set; }


        [DisplayName("Do you plan to use the QuickBooks accounting software provided by ConnectusPro for full integration?")]
        public string UseQuickBooks { get; set; }

        [DisplayName("When is your fiscal year end?")]
        public DateTime? FiscalYearEnd { get; set; }

        [DisplayName("Do you use a standard calendar month or a retail period (4/4/5) calendar?")]
        public string CalendarType { get; set; }

        //[DisplayName("")]
        public string OtherCalendarType { get; set; }

        [DisplayName("What is the date of your last fully reconciled bank statement?")]
        public DateTime? ReconciledBankDate { get; set; }

        [DisplayName("What is the date of your last fully reconciled/closed month end?")]
        public DateTime? ReconsiledClosedMonthEnd { get; set; }

        [DisplayName("What is the name and contact number of the individual who will be managing your transition to QuickBooks?")]
        public string QuickBooksTransitionName { get; set; }

        //[DisplayName("")]
        public string QuickBooksTransitionPhone { get; set; }

        [DisplayName("On a scale of 1 – 10 – rate your accounting knowledge ")]
        public string AccountingKnowledgeRate { get; set; }

        [DisplayName("Do you have an accounting staff or do you use an outside accountant to record your financial transactions?")]
        public string FinancialTransactionRecord { get; set; }

        //[DisplayName("")]
        public string OtherFinancialTransactionRecord { get; set; }

        [DisplayName("Are you familiar with your chart of accounts?")]
        public string AccountsChart { get; set; }

        [DisplayName("Are you familiar with general journal entries?")]
        public string GeneralJourneyEntries { get; set; }

        [DisplayName("Do you use any other software add-ons with your accounting system (example: Price Point)")]
        public string AccountingSystemAddons { get; set; }

        //[DisplayName("")]
        public string AccountingSystemAddonsList { get; set; }

        [DisplayName("Have your processed ACH withdrawals from your current checking account in the past?")]
        public string AchWithdrawls { get; set; }

        public virtual Questionnaire Questionnaire { get; set; }
    }
}
