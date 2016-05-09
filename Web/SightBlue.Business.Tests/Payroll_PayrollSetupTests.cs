using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiteBlue.Business.Reporting;
using SiteBlue.Business.PayrollSetup;
using SiteBlue.Business;
using System.Data.SqlClient;
using System.Configuration;

namespace SightBlue.Business.Tests
{
    [TestClass]
    public class Payroll_PayrollSetupTests
    {

        // Scenario A1
        [TestMethod]
        public void PayrollSetup_001_GetPayrollSetupNoFranchise()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A1");
            int? franchiseID = null;

            // Act
            // Query reporting service for PayrollSetup
            ReportingService rs = ReportingService.Create<ReportingService>(Guid.NewGuid());
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);

            // Assert
            // I should at least get a non-null array, may not have anything in it
            Assert.IsNotNull(arrPayrollSetup);
        }

        // Scenario A2
        [TestMethod]
        public void PayrollSetup_002_GetPayrollSetupNoFranchise_AddAtLeastOnePayrollSetup()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A2");
            int? franchiseID = null;

            // Act
            // Query reporting service for PayrollSetup
            ReportingService rs = ReportingService.Create<ReportingService>(Guid.NewGuid());
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);

            // Assert
            // I should at least get a non-null array, may not have anything in it
            Assert.IsNotNull(arrPayrollSetup);
            Assert.IsTrue(arrPayrollSetup.Length > 0, "Should have at least one payroll setup item");
        }

        // Scenario A3
        [TestMethod]
        public void PayrollSetup_003_GetPayrollSetupSpecifyFranchise()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A3");
            int? franchiseID = 39;

            // Act
            // Query reporting service for PayrollSetup
            ReportingService rs = ReportingService.Create<ReportingService>(Guid.NewGuid());
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);

            // Assert
            // I should at least get a non-null array, may not have anything in it
            Assert.IsNotNull(arrPayrollSetup);
            Assert.IsTrue(arrPayrollSetup.Length == 1, "Should have one and only one payroll setup");
        }

        // Save Payroll Setup when not exists.  Should add it to the franchise
        // Scenario A4
        [TestMethod]
        public void PayrollSetup_004_SavePayrollSetup_New()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A4");
            int franchiseID = 39;
            decimal overtimeStarts = 100M;
            int overtimeMethod = 2;
            decimal overtimeMultiplier = 3.5M;
            string reportingOvertimeMethod = "Daily";

            // Act
            // call save payroll setup on business entity
            PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(Guid.NewGuid());
            OperationResult<SiteBlue.Business.PayrollSetup.PayrollSetup> result = payrollSetupService.SavePayrollSetup(franchiseID, overtimeStarts, overtimeMethod, overtimeMultiplier);

            // For checking on it after we've saved it
            ReportingService rs = ReportingService.Create<ReportingService>(Guid.NewGuid());


            // Assert
            // Check the POCO object returned
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.ResultData);
            Assert.AreEqual<int>(franchiseID, result.ResultData.FranchiseID);
            Assert.AreEqual<decimal>(overtimeStarts, result.ResultData.OvertimeStarts);
            Assert.AreEqual<int>(overtimeMethod, result.ResultData.OvertimeMethodID);
            Assert.AreEqual<decimal>(overtimeMultiplier, result.ResultData.OTMultiplier);

            // Can also check the data saved
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);
            Assert.AreEqual<int>(1, arrPayrollSetup.Length);
            SiteBlue.Business.Reporting.PayrollSetup reportingPayrollSetup = arrPayrollSetup[0];
            Assert.AreEqual<int>(franchiseID, reportingPayrollSetup.FranchiseID);
            Assert.AreEqual<decimal>(overtimeStarts, (decimal)reportingPayrollSetup.OvertimeStarts);
            Assert.AreEqual<int>(overtimeMethod, reportingPayrollSetup.OvertimeMethodID);
            Assert.AreEqual<decimal>(overtimeMultiplier, (decimal)reportingPayrollSetup.OTMultiplier);
            Assert.AreEqual<string>(reportingOvertimeMethod, reportingPayrollSetup.OvertimeMethod);
        }

        // Save Payroll Setup when already exists.  Should add it to the franchise
        // Scenario A5
        [TestMethod]
        public void PayrollSetup_005_SavePayrollSetup_ExistingPayrollSetup()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A5");
            int franchiseID = 39;
            decimal overtimeStarts = 100M;
            int overtimeMethod = 2;
            decimal overtimeMultiplier = 3.5M;
            string reportingOvertimeMethod = "Daily";

            // Pre-validation, ensure one payrollsetup DOES alredy exist
            ReportingService rs = ReportingService.Create<ReportingService>(Guid.NewGuid());
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);
            Assert.AreEqual<int>(1, arrPayrollSetup.Length);

            // Act
            // call save payroll setup on business entity
            PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(Guid.NewGuid());
            OperationResult<SiteBlue.Business.PayrollSetup.PayrollSetup> result = payrollSetupService.SavePayrollSetup(franchiseID, overtimeStarts, overtimeMethod, overtimeMultiplier);

            // Assert
            // Check the POCO object returned
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.ResultData);
            Assert.AreEqual<int>(franchiseID, result.ResultData.FranchiseID);
            Assert.AreEqual<decimal>(overtimeStarts, result.ResultData.OvertimeStarts);
            Assert.AreEqual<int>(overtimeMethod, result.ResultData.OvertimeMethodID);
            Assert.AreEqual<decimal>(overtimeMultiplier, result.ResultData.OTMultiplier);

            // Can also check the data saved
            arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);
            Assert.AreEqual<int>(1, arrPayrollSetup.Length);
            SiteBlue.Business.Reporting.PayrollSetup reportingPayrollSetup = arrPayrollSetup[0];
            Assert.AreEqual<int>(franchiseID, reportingPayrollSetup.FranchiseID);
            Assert.AreEqual<decimal>(overtimeStarts, (decimal)reportingPayrollSetup.OvertimeStarts);
            Assert.AreEqual<int>(overtimeMethod, reportingPayrollSetup.OvertimeMethodID);
            Assert.AreEqual<decimal>(overtimeMultiplier, (decimal)reportingPayrollSetup.OTMultiplier);
            Assert.AreEqual<string>(reportingOvertimeMethod, reportingPayrollSetup.OvertimeMethod);
        }

        // Save Payroll Setup.  Test business validations
        // Scenario A6
        [TestMethod]
        public void PayrollSetup_006_SavePayrollSetup_BusinessException_OvertimeStarts()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A6");
            int franchiseID = 39;
            decimal overtimeStarts = -10M;  // Expect this to cause an argument exception
            int overtimeMethod = 2;
            decimal overtimeMultiplier = 2.5M;

            // Act
            // call save payroll setup on business entity
            Exception caughtException = null;
            try
            {
                PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(Guid.NewGuid());
                OperationResult<SiteBlue.Business.PayrollSetup.PayrollSetup> result = payrollSetupService.SavePayrollSetup(franchiseID, overtimeStarts, overtimeMethod, overtimeMultiplier);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
            

            // Assert
            Assert.IsNotNull(caughtException);
            Assert.IsInstanceOfType(caughtException, typeof(ArgumentException));
        }

        // Save Payroll Setup.  Test business validations
        // Scenario A7
        [TestMethod]
        public void PayrollSetup_007_SavePayrollSetup_BusinessException_OvertimeMultiplier()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A7");
            int franchiseID = 39;
            decimal overtimeStarts = -10M;  
            int overtimeMethod = 2;
            decimal overtimeMultiplier = -2.5M; // Expect this to cause an argument exception

            // Act
            // call save payroll setup on business entity
            Exception caughtException = null;
            try
            {
                PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(Guid.NewGuid());
                OperationResult<SiteBlue.Business.PayrollSetup.PayrollSetup> result = payrollSetupService.SavePayrollSetup(franchiseID, overtimeStarts, overtimeMethod, overtimeMultiplier);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }


            // Assert
            Assert.IsNotNull(caughtException);
            Assert.IsInstanceOfType(caughtException, typeof(ArgumentException));
        }


        // Add a new Spiff to a PayrollSetup
        // Scenario A8
        [TestMethod]
        public void PayrollSetup_020_SaveNewSpiff()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A8");
            int franchiseID = 39;
            ReportingService rs = ReportingService.Create<ReportingService>(Guid.NewGuid());
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);
            Assert.AreEqual<int>(1, arrPayrollSetup.Length);          

            int payrollSetupID = arrPayrollSetup[0].PayrollSetupID;
            int serviceProID = 172;
            int jobCodeID = 65834;
            int payType = 0;
            decimal rate = 1.5M;
            DateTime? dateExpires = DateTime.Parse("5/2/2012");
            string comments = "Test comments";
            bool addOn = true;
            bool active = true;

            // Act
            // Save new Spiff to existing PayrollSetup
            PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(Guid.NewGuid());
            OperationResult<SiteBlue.Business.PayrollSetup.PayrollSpiff> result = payrollSetupService.PayrollSpiff_Add(payrollSetupID, serviceProID, jobCodeID, payType, rate, dateExpires, comments, addOn, active);

            // Assert
            SiteBlue.Business.PayrollSetup.PayrollSpiff newSpiff = result.ResultData;
            Assert.AreEqual<int>(payrollSetupID, newSpiff.PayrollSetupID);
            Assert.AreEqual<int>(serviceProID, newSpiff.ServiceProID);
            Assert.AreEqual<int>(jobCodeID, newSpiff.JobCodeID);
            Assert.AreEqual<int>(payType, newSpiff.PayType);
            Assert.AreEqual<decimal>(rate, newSpiff.Rate);
            Assert.AreEqual<DateTime?>(dateExpires, newSpiff.DateExpires);
            Assert.AreEqual<string>(comments, newSpiff.Comments);
            Assert.AreEqual<bool>(addOn, newSpiff.AddOn);
            Assert.AreEqual<bool>(active, newSpiff.Active);

            // Can also check the saved entity
            SiteBlue.Business.Reporting.PayrollSetup[] arrRptPayrollSetup = rs.GetPayrollSetupData(franchiseID);
            Assert.AreEqual<int>(1, arrRptPayrollSetup.Length);
            Assert.AreEqual<int>(1, arrRptPayrollSetup[0].PayrollSpiffs.Count());
            SiteBlue.Business.Reporting.PayrollSpiff reportSpiff = arrRptPayrollSetup[0].PayrollSpiffs.ToArray()[0];

            Assert.AreEqual<int>(payrollSetupID, reportSpiff.PayrollSetupID);
            Assert.AreEqual<int>(serviceProID, reportSpiff.ServiceProID);
            Assert.AreEqual<int>(jobCodeID, reportSpiff.JobCodeID);
            Assert.AreEqual<int>(payType, reportSpiff.PayTypeID);
            Assert.AreEqual<decimal>(rate, reportSpiff.Rate);
            Assert.AreEqual<DateTime?>(dateExpires, reportSpiff.DateExpires);
            Assert.AreEqual<string>(comments, reportSpiff.Comments);
            Assert.AreEqual<bool>(addOn, reportSpiff.AddOn);
            Assert.AreEqual<bool>(active, reportSpiff.Active);

        }

        // Scenario A9
        [TestMethod]
        public void PayrollSetup_021_UpdateSpiff()
        {
            // Arrange
            SQLHelper.RunSetupScript("UnitTest_PayrollSetup", "A9");
            int franchiseID = 39;
            ReportingService rs = ReportingService.Create<ReportingService>(Guid.NewGuid());
            SiteBlue.Business.Reporting.PayrollSetup[] arrPayrollSetup = rs.GetPayrollSetupData(franchiseID);
            Assert.AreEqual<int>(1, arrPayrollSetup.Length);

            int payrollSetupID = arrPayrollSetup[0].PayrollSetupID;
            int payrollSpiffID = arrPayrollSetup[0].PayrollSpiffs.ToArray()[0].PayrollSpiffID;
            int serviceProID = 172;
            int jobCodeID = 65834;
            int payType = 1;
            decimal rate = 3.5M;
            DateTime? dateExpires = DateTime.Parse("5/2/2014");
            string comments = "Test comments Update Spiff";
            bool addOn = true;
            bool active = true;

            // Act
            // Save Spiff to existing PayrollSetup
            PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(Guid.NewGuid());
            OperationResult<SiteBlue.Business.PayrollSetup.PayrollSpiff> result = payrollSetupService.PayrollSpiff_Update(payrollSpiffID, serviceProID, jobCodeID, payType, rate, dateExpires, comments, addOn, active);

            // Assert
            SiteBlue.Business.PayrollSetup.PayrollSpiff newSpiff = result.ResultData;
            Assert.AreEqual<int>(payrollSetupID, newSpiff.PayrollSetupID);
            Assert.AreEqual<int>(serviceProID, newSpiff.ServiceProID);
            Assert.AreEqual<int>(jobCodeID, newSpiff.JobCodeID);
            Assert.AreEqual<int>(payType, newSpiff.PayType);
            Assert.AreEqual<decimal>(rate, newSpiff.Rate);
            Assert.AreEqual<DateTime?>(dateExpires, newSpiff.DateExpires);
            Assert.AreEqual<string>(comments, newSpiff.Comments);
            Assert.AreEqual<bool>(addOn, newSpiff.AddOn);
            Assert.AreEqual<bool>(active, newSpiff.Active);

            // Can also check the saved entity
            SiteBlue.Business.Reporting.PayrollSetup[] arrRptPayrollSetup = rs.GetPayrollSetupData(franchiseID);
            Assert.AreEqual<int>(1, arrRptPayrollSetup.Length);
            Assert.AreEqual<int>(1, arrRptPayrollSetup[0].PayrollSpiffs.Count());
            SiteBlue.Business.Reporting.PayrollSpiff reportSpiff = arrRptPayrollSetup[0].PayrollSpiffs.ToArray()[0];

            Assert.AreEqual<int>(payrollSetupID, reportSpiff.PayrollSetupID);
            Assert.AreEqual<int>(serviceProID, reportSpiff.ServiceProID);
            Assert.AreEqual<int>(jobCodeID, reportSpiff.JobCodeID);
            Assert.AreEqual<int>(payType, reportSpiff.PayTypeID);
            Assert.AreEqual<decimal>(rate, reportSpiff.Rate);
            Assert.AreEqual<DateTime?>(dateExpires, reportSpiff.DateExpires);
            Assert.AreEqual<string>(comments, reportSpiff.Comments);
            Assert.AreEqual<bool>(addOn, reportSpiff.AddOn);
            Assert.AreEqual<bool>(active, reportSpiff.Active);
        }

        [TestMethod]
        public void PayrollSetup_021_UpdateSpiff_ThatDoesNotExist()
        {
            // Arrange
            int payrollSpiffID = -1; // not supposed to exist
            int serviceProID = 172;
            int jobCodeID = 65834;
            int payType = 1;
            decimal rate = 3.5M;
            DateTime? dateExpires = DateTime.Parse("5/2/2014");
            string comments = "Test comments Update Spiff";
            bool addOn = true;
            bool active = true;

            // Act
            // Save Spiff to existing PayrollSetup
            Exception caughtException = null;
            try
            {
                PayrollSetupService payrollSetupService = PayrollSetupService.Create<PayrollSetupService>(Guid.NewGuid());
                OperationResult<SiteBlue.Business.PayrollSetup.PayrollSpiff> result = payrollSetupService.PayrollSpiff_Update(payrollSpiffID, serviceProID, jobCodeID, payType, rate, dateExpires, comments, addOn, active);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsNotNull(caughtException);
        }

    }
}
