using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using SiteBlue.Core;
using SiteBlue.Core.Email;
using SiteBlue.Data.EightHundred;

using InvoiceGen = SiteBlue.Business.Enterprise.InvoiceGeneration;
using SiteBlue.Business.Reporting;

namespace SiteBlue.Business.PayrollSetup
{
    public class PayrollSetupService : AbstractBusinessService
    {
        EightHundredEntities db = new EightHundredEntities();

        // instantiate through the AbstractBusinessService.Create
        public  PayrollSetupService(){}

        public OperationResult<PayrollSetup> SavePayrollSetup(int franchiseID,decimal overtimeStarts,int overtimeMethod,decimal overtimeMultiplier)
        {
            // Step 0: Business Validations
            StringBuilder sbExceptions = new StringBuilder();
            if (overtimeStarts < 0)
                sbExceptions.AppendLine("Overtime Start cannot be negative: " + overtimeStarts.ToString());
            if (overtimeMultiplier < 0)
                sbExceptions.AppendLine("Overtime Multiplier cannot be negative: " + overtimeMultiplier.ToString());
            if (sbExceptions.Length > 0)
                throw new ArgumentException(sbExceptions.ToString());

            // Step 1: inflate a POCO object with our parameters and do any validation
            PayrollSetup payrollSetupToReturn = new PayrollSetup()
                {
                    FranchiseID = franchiseID,
                    OvertimeStarts = overtimeStarts,
                    OvertimeMethodID = overtimeMethod,
                    OTMultiplier = overtimeMultiplier
                };

            // Step 2: Database Persistance
            // If there is already a Payroll Setup for the franchise, do UPDATE
            var existingPayroll = (from tbl_HR_PayrollSetup payrollSetup in db.tbl_HR_PayrollSetup
                                   where payrollSetup.FranchiseID == franchiseID
                                   select payrollSetup).FirstOrDefault<tbl_HR_PayrollSetup>();
            if (existingPayroll != null)
            {
                // do an update
                existingPayroll.OvertimeStarts = (float)overtimeStarts;
                existingPayroll.OvertimeMethod = overtimeMethod;
                existingPayroll.OTMultiplier = (float)overtimeMultiplier;
            }
            else
            {
                // Otherwise, do an INSERT
                tbl_HR_PayrollSetup newPayrollSetup = new tbl_HR_PayrollSetup()
                {
                    FranchiseID = franchiseID,
                    OvertimeStarts = (float)overtimeStarts,
                    OvertimeMethod = overtimeMethod,
                    OTMultiplier = (float)overtimeMultiplier
                };
                db.tbl_HR_PayrollSetup.AddObject(newPayrollSetup);
            }
            db.SaveChanges();

            return new OperationResult<PayrollSetup>() { Success = true, ResultData = payrollSetupToReturn };
        }

        public OperationResult<PayrollSpiff> PayrollSpiff_Add(int payrollSetupID, int serviceProID, int jobCodeID, int payType, decimal rate, DateTime? dateExpires, string comments, bool addOn, bool active)
        {
            // Step 0: Business Validation - make sure the payroll SETUP ID exists
            int existingCount = (from payrollSetup in db.tbl_HR_PayrollSetup
                                where payrollSetup.PayrollSetupID == payrollSetupID
                                select payrollSetup).Count();
            if (existingCount == 0)
                throw new PayrollSetupDoesNotExistException(payrollSetupID);

            // Step 1: inflate a POCO object with our parameters and do any validation
            PayrollSpiff toReturn = new PayrollSpiff()
            {
                PayrollSetupID = payrollSetupID,
                ServiceProID = serviceProID,
                JobCodeID = jobCodeID,
                PayType = payType,
                Rate = rate,
                DateExpires = dateExpires,
                Comments = comments,
                AddOn = addOn,
                Active = active
            };

            // Step 2: Save to DB
            tbl_HR_PayrollSpiffs newPayrollSpiff = new tbl_HR_PayrollSpiffs();
            newPayrollSpiff.PayrollSetupID = payrollSetupID;
            newPayrollSpiff.ServiceProID = serviceProID;
            newPayrollSpiff.JobCodeID = jobCodeID;
            newPayrollSpiff.PayTypeID = payType;
            newPayrollSpiff.Rate = rate;
            newPayrollSpiff.DateExpires = dateExpires;
            newPayrollSpiff.Comments = comments;
            newPayrollSpiff.AddonYN = addOn;
            newPayrollSpiff.ActiveYN = active;

            db.tbl_HR_PayrollSpiffs.AddObject(newPayrollSpiff);
            db.SaveChanges();

            return new OperationResult<PayrollSpiff>() { Success = true, ResultData = toReturn };
        }

        public OperationResult<PayrollSpiff> PayrollSpiff_Update(int payrollSpiffID, int serviceProID, int jobCodeID, int payType, decimal rate, DateTime? dateExpires, string comments, bool addOn, bool active)
        {
            // Step 1: Get existing Spiff from DB
            tbl_HR_PayrollSpiffs existingPayrollSpiff = (from tbl_HR_PayrollSpiffs payrollSpiff in db.tbl_HR_PayrollSpiffs
                                                        where payrollSpiff.PayrollSpiffID == payrollSpiffID
                                                         select payrollSpiff).FirstOrDefault<tbl_HR_PayrollSpiffs>();
            if (existingPayrollSpiff == null)
                throw new Exception("Cannot update SPIFF.  Record does not exist. ID: " + payrollSpiffID.ToString());

            // Step 2: inflate a POCO object with our parameters and do any validation
            PayrollSpiff toReturn = new PayrollSpiff()
            {
                PayrollSetupID = existingPayrollSpiff.PayrollSetupID,
                ServiceProID = serviceProID,
                JobCodeID = jobCodeID,
                PayType = payType,
                Rate = rate,
                DateExpires = dateExpires,
                Comments = comments,
                AddOn = addOn,
                Active = active
            };

            // Step 3: Save to DB
                existingPayrollSpiff.ServiceProID = serviceProID;
                existingPayrollSpiff.JobCodeID = jobCodeID;
                existingPayrollSpiff.PayTypeID = payType;
                existingPayrollSpiff.Rate = rate;
                existingPayrollSpiff.DateExpires = dateExpires;
                existingPayrollSpiff.Comments = comments;
                existingPayrollSpiff.AddonYN = addOn;
                existingPayrollSpiff.ActiveYN = active;
            db.SaveChanges();

            return new OperationResult<PayrollSpiff>() { Success = true, ResultData = toReturn };
        }

    }

    public class PayrollSetupDoesNotExistException : Exception
    {
        public int PayrollSetupID { get; private set; }
        public PayrollSetupDoesNotExistException(int payrollSetupID) 
            : base(string.Format("Payroll Setup with ID: {0} does not exist", payrollSetupID))
        {
            this.PayrollSetupID = payrollSetupID;
        }
    }
}
