using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteBlue.Questionnaire.Data;

namespace SiteBlue.Models
{
    public static class QuestionnaireModel
    {
        /// <summary>
        /// Create an empty object from <see cref="SiteBlue.Questionnaire.Data.OwnerInformation"/> 
        /// class, with a reference to <see cref="SiteBlue.Questionnaire.Data.Questionnaire"/>
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaire ID</param>
        /// <returns></returns>
        public static OwnerInformation InitializeOwnerInformation(long QuestionnaireId)
        {
            OwnerInformation ownerInformation = new OwnerInformation();
            ownerInformation.QuestionnaireId = QuestionnaireId;
            ownerInformation.FirstName = "";
            ownerInformation.LastName = "";
            ownerInformation.BusinessName = "";
            ownerInformation.HomeAddress = "";
            ownerInformation.City = "";
            ownerInformation.StateId = 1;
            ownerInformation.ZipCode = "";
            ownerInformation.CellPhone = "";
            ownerInformation.HomePhone = "";
            ownerInformation.EmailAddress = "";
            return ownerInformation;
        }

        public static BusinessInformation InitializeBusinessInformation(long QuestionnaireId)
        {
            BusinessInformation businessInformation = new BusinessInformation();
            businessInformation.QuestionnaireId = QuestionnaireId;
            businessInformation.OfficePhone = "";
            businessInformation.CellPhone = "";
            //businessInformation.Website = "";
            //businessInformation.BusinessAddress = "";
            //businessInformation.BusinessCity = "";
            //businessInformation.StateId = 1;
            //businessInformation.BusinessZipCode = "";
            businessInformation.TrucksService = "";
            businessInformation.TrucksCommercial = "";
            businessInformation.TrucksInstallation = "";
            businessInformation.TrucksSales = "";
            businessInformation.TechniciansService = "";
            businessInformation.TechniciansCommercial = "";
            businessInformation.TechniciansInstallation = "";
            businessInformation.TechniciansSales = "";
            //businessInformation.Addresses = new List<Address>();
            //businessInformation.OfficePersonnels = new List<OfficePersonnel>();
            return businessInformation;
        }

        public static AccountingInformation InitializeAccountingInformation(long QuestionnaireId)
        {
            AccountingInformation accountingInformation = new AccountingInformation();
            accountingInformation.QuestionnaireId = QuestionnaireId;
            accountingInformation.CurrentAccountingSoftware = "";
            accountingInformation.CurrentAccountingSoftwareUsingTime = "";
            accountingInformation.LastCompletedDate = DateTime.Now;

            return accountingInformation;
        }
    }
}