using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Questionnaire.Data;
using SiteBlue.Controllers;
using System.Text;
using SiteBlue.Data.EightHundred;
using System.Transactions;
using System.Data.Objects;

namespace SiteBlue.Areas.Admin.Controllers
{
    public class OnboardingController : SiteBlueBaseController
    {
        //
        // GET: /Admin/Onboarding/

        private QuestionnaireContext db = new QuestionnaireContext();
        private EightHundredEntities edb = new EightHundredEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClientApproval()
        {
            return View();
        }

        public JsonResult LoadQuestionnaires(long? questionnaireId, long? businessInformationId)
        {
            List<QuestionnaireInformation> questionnaires = new List<QuestionnaireInformation>();
            if ((questionnaireId == null) && (businessInformationId == null))
                questionnaires = db.QuestionnaireInformation.ToList();
            else
            {
                if ((questionnaireId.HasValue) && (businessInformationId.HasValue))
                    questionnaires = db.QuestionnaireInformation.Where(q => q.QuestionnaireId == questionnaireId.Value && q.BusinessInformationId == businessInformationId).ToList();
                else if (questionnaireId.HasValue)
                    questionnaires = db.QuestionnaireInformation.Where(q => q.QuestionnaireId == questionnaireId.Value).ToList();
                else if (businessInformationId.HasValue)
                    questionnaires = db.QuestionnaireInformation.Where(q => q.BusinessInformationId == businessInformationId.Value).ToList();
            }

            return Json(questionnaires, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadQuestionnaire(long questionnaireId)
        {
            var businessInformation = db.BusinessInformation.Where(b => b.QuestionnaireId == questionnaireId).ToList();
            if ((businessInformation != null) && (businessInformation.Count > 0))
                //return Json(new { Result = businessInformation, Message = "Success"}, JsonRequestBehavior.AllowGet);
                return Json(
                    new
                    {
                        BusinessInformationId = businessInformation[0].BusinessInformationId,
                        OfficePhone = businessInformation[0].OfficePhone,
                        CellPhone = businessInformation[0].CellPhone,
                        Website = businessInformation[0].Website,
                        TrucksService = businessInformation[0].TrucksService,
                        TrucksCommerial = businessInformation[0].TrucksCommercial,
                        TrucksInstallation = businessInformation[0].TrucksInstallation,
                        TrucksSales = businessInformation[0].TrucksSales,
                        TechniciansService = businessInformation[0].TechniciansService,
                        TechniciansCommercial = businessInformation[0].TechniciansCommercial,
                        TechniciansInstallation = businessInformation[0].TechniciansInstallation,
                        TechniciansSales = businessInformation[0].TechniciansSales,
                        GPS = businessInformation[0].GPS == true ? "Yes" : "No",
                        GPSSystemName = businessInformation[0].GPSSystem != null ? businessInformation[0].GPSSystem.GPSSystemName : "",
                        OtherGPSSystem = businessInformation[0].OtherGPSSystem,
                        GPSExpirationDate = businessInformation[0].GPSExpirationDate.HasValue ? businessInformation[0].GPSExpirationDate.Value.ToShortDateString() : "",
                        MondayFridayST = businessInformation[0].MondayFridayST,
                        MondayFridayET = businessInformation[0].MondayFridayET,
                        SaturdayST = businessInformation[0].SaturdayST,
                        SaturdayET = businessInformation[0].SaturdayET,
                        SundayST = businessInformation[0].SundayST,
                        SundayET = businessInformation[0].SundayET,
                        ServiceTripFees = businessInformation[0].ServiceTripFees == true ? "Yes" : "No",
                        ServiceTripFeesDescription = businessInformation[0].ServiceTripFeesDescription,
                        TechniciansAvailabilityNights = businessInformation[0].TechniciansAvailabilityNights == true ? "Nights," : "",
                        TechniciansAvailabilityWeekend = businessInformation[0].TechniciansAvailabilityWeekend == true ? "Weekend," : "",
                        TechniciansAvailabilityHolidays = businessInformation[0].TechniciansAvailabilityHolidays == true ? "Holidays." : "",
                        AdditionalDetails = businessInformation[0].AdditionalDetails,
                        AfterHoursHolidaysFees = businessInformation[0].AfterHoursHolidaysFees == true ? "Yes" : "No",
                        AfterHoursHolidaysFeesDescription = businessInformation[0].AfterHoursHolidaysFeesDescription,
                        ServiceCenterAgentsAnswer = businessInformation[0].ServiceCenterAgentsAnswer,
                        FlateRate = businessInformation[0].FlateRate == true ? "Yes" : "No",
                        TimeAndMaterial = businessInformation[0].TimeAndMaterial,
                        DoWarrantyWork = businessInformation[0].DoWarrantyWork,
                        ServicePlansOffer = businessInformation[0].ServicePlansOffer == true ? "Yes" : "No",
                        ServicePlanMembersDiscount = businessInformation[0].ServicePlanMembersDiscount,
                        ServicePlanMembersMaintenance = businessInformation[0].ServicePlanMembersMaintenance == true ? "Yes" : "No",
                        ServicePlanMembersTime = businessInformation[0].ServicePlanMembersTime,
                        ServicePlanName = businessInformation[0].ServicePlanName,
                        ServicePlanComment = businessInformation[0].ServicePlanComment,
                        ServicePlanPayTrip = businessInformation[0].ServicePlanPayTrip,
                        Message = "Success"
                    }, JsonRequestBehavior.AllowGet);

            return Json(new { Message = "No information found" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadZipCodes(long? businessInformationId)
        {
            try
            {
                List<ZipCode> zipCodes = db.ZipCode.Where(z => z.BusinessInformationId == businessInformationId).ToList();
                var sb = new StringBuilder();
                sb.Append("<rows>");
                foreach (var item in zipCodes)
                {
                    sb.AppendFormat("<row id='{0}'>", item.ZipCodeId);
                    sb.AppendFormat("<cell><![CDATA[{0}]]></cell>", item.ZipCodeNumber);
                    sb.AppendFormat("<cell><![CDATA[{0}]]></cell>", item.Comment);
                    sb.Append("</row>");
                }
                sb.Append("</rows>");
                return Json(new
                {
                    Message = "",
                    ResultData = sb.ToString(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = ex.Message,
                    ResultData = "",
                    Success = false
                });
            }
        }

        public JsonResult LoadDBAs(long? businessInformationId)
        {
            try
            {
                List<Dba> dbas = db.Dba.Where(z => z.BusinessInformationId == businessInformationId).ToList();
                var sb = new StringBuilder();
                sb.Append("<rows>");
                foreach (var item in dbas)
                {
                    sb.AppendFormat("<row id='{0}'>", item.DbaId);
                    sb.AppendFormat("<cell><![CDATA[{0}]]></cell>", item.DbaName);
                    sb.Append("</row>");
                }
                sb.Append("</rows>");
                return Json(new
                {
                    Message = "",
                    ResultData = sb.ToString(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = ex.Message,
                    ResultData = "",
                    Success = false
                });
            }
        }

        public JsonResult LoadServices()
        {
            try
            {
                List<tbl_Services> services = edb.tbl_Services.Where(s => s.ActiveYN == true).ToList();
                var sb = new StringBuilder();
                sb.Append("<rows>");
                int i = 0;
                foreach (var item in services)
                {
                    sb.AppendFormat("<row id='{0}'>", item.ServiceID);
                    sb.AppendFormat("<cell>{0}</cell>", "");
                    sb.AppendFormat("<cell><![CDATA[{0}]]></cell>", item.ServiceName);
                    sb.Append("</row>");
                    i++;
                }
                sb.Append("</rows>");
                return Json(new
                {
                    Message = "",
                    ResultData = sb.ToString(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = ex.Message,
                    ResultData = "",
                    Success = false
                });
            }
        }

        public JsonResult LoadTaxRates(long? businessInformationId)
        {
            try
            {
                var zipCodes = (from item in db.ZipCode
                                where (item.BusinessInformationId == businessInformationId)
                                select item.TaxRate).Distinct();
                var sb = new StringBuilder();
                sb.Append("<rows>");
                int i = 0;
                foreach (var item in zipCodes)
                {
                    sb.AppendFormat("<row id='{0}'>", i);
                    sb.AppendFormat("<cell><![CDATA[{0}]]></cell>", item);
                    sb.Append("</row>");
                    i++;
                }
                sb.Append("</rows>");
                return Json(new
                {
                    Message = "",
                    ResultData = sb.ToString(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = ex.Message,
                    ResultData = "",
                    Success = false
                });
            }
        }

        public JsonResult SaveInformationToDatabase(int franchiseId, long questionnaireId, long businessInformationId, string services, string cellPhone, string officePhone)
        {
            List<ZipCode> zipCodes = db.ZipCode.Where(z => z.BusinessInformationId == businessInformationId).ToList();
            List<Dba> dbas = db.Dba.Where(z => z.BusinessInformationId == businessInformationId).ToList();
            List<double?> itaxRates = new List<double?>();
            foreach (var z in zipCodes)
                if (z.TaxRate != null) itaxRates.Add(z.TaxRate);
            IEnumerable<double?> taxRates = itaxRates.Distinct();
            int insertedZipRows, skipedZipRows, insertedDBARows,
                skipedDBARows, insertedTaxRows, skipedTaxRows,
                insertedServiceRows, skipedServiceRows, insertedContactRows,
                skipedContactRows;
            int totalZipRows, totalDBARows, totalTaxRows, totalServiceRows, totalContactRows;
            string contactName = "";
            // Initialize static data
            insertedZipRows = 0;
            skipedZipRows = 0;
            insertedDBARows = 0;
            skipedDBARows = 0;
            insertedTaxRows = 0;
            skipedTaxRows = 0;
            insertedServiceRows = 0;
            skipedServiceRows = 0;
            insertedContactRows = 0;
            skipedContactRows = 0;

            totalZipRows = 0;
            totalDBARows = 0;
            totalTaxRows = 0;
            totalServiceRows = 0;
            totalContactRows = 0;

            string errorMessage = "";
            bool success = false;

            var franchise = edb.tbl_Franchise.Single(f => f.FranchiseID == franchiseId);
            // Contact information
            if (((cellPhone != null) && (cellPhone != "")) || ((officePhone != null) && (officePhone != "")))
            {
                var ownerInformation = db.OwnerInformation.Where(oi => oi.QuestionnaireId == questionnaireId).ToList();
                if ((ownerInformation != null) && (ownerInformation.Count > 0))
                    contactName = ownerInformation[0].FirstName + " " + ownerInformation[0].LastName;
            }

            using (TransactionScope transation = new TransactionScope())
            {
                try
                {
                    // Cell Phone
                    if ((cellPhone != null) && (cellPhone != ""))
                    {
                        string[] phoneElements = cellPhone.Split(new char[] { '-' });
                        cellPhone = string.Format("({0}) {1}-{2}", phoneElements[0], phoneElements[1], phoneElements[2]);
                        // Check if phone exists in tbl_Franchise_Contacts
                        totalContactRows = (from fphone in edb.tbl_Franchise_Contacts
                                            where (fphone.FranchiseID == franchiseId && fphone.PhoneNumber == cellPhone && fphone.PhoneTypeID == 4)
                                            select fphone.FranchiseContactID).Count();
                        if (totalContactRows == 0)
                        {
                            // Insert a new row
                            tbl_Franchise_Contacts contactToInsert = new tbl_Franchise_Contacts()
                            {
                                FranchiseID = franchise.FranchiseID,
                                ContactName = contactName,
                                PhoneTypeID = 4,
                                PhoneNumber = cellPhone
                            };
                            edb.tbl_Franchise_Contacts.AddObject(contactToInsert);
                            edb.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                            insertedContactRows++;
                        }
                        else
                            skipedContactRows++;
                    }
                    // Office Phone
                    if ((officePhone != null) && (officePhone != ""))
                    {
                        string[] phoneElements = officePhone.Split(new char[] { '-' });
                        officePhone = string.Format("({0}) {1}-{2}", phoneElements[0], phoneElements[1], phoneElements[2]);
                        // Check if phone exists in tbl_Franchise_Contacts
                        totalContactRows = (from fphone in edb.tbl_Franchise_Contacts
                                            where (fphone.FranchiseID == franchiseId && fphone.PhoneNumber == cellPhone && fphone.PhoneTypeID == 5)
                                            select fphone.FranchiseContactID).Count();
                        if (totalContactRows == 0)
                        {
                            // Insert a new row
                            tbl_Franchise_Contacts contactToInsert = new tbl_Franchise_Contacts()
                            {
                                FranchiseID = franchise.FranchiseID,
                                ContactName = contactName,
                                PhoneTypeID = 5,
                                PhoneNumber = officePhone
                            };
                            edb.tbl_Franchise_Contacts.AddObject(contactToInsert);
                            edb.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                            insertedContactRows++;
                        }
                        else
                            skipedContactRows++;
                    }
                    // Services
                    if ((services != null) && (services != ""))
                    {
                        string[] serviceElements = services.Split(new char[] { ',' });
                        for (int i = 0; i < serviceElements.Length; i++)
                        {
                            // Insert a new row
                            int serviceId = int.Parse(serviceElements[i]);
                            totalServiceRows = (from fservice in edb.tbl_Franchise_Services
                                                where (fservice.FranchiseID == franchiseId && fservice.ServiceID == serviceId)
                                                select fservice.FranchiseeServiceID).Count();
                            if (totalServiceRows == 0)
                            {
                                tbl_Franchise_Services serviceToInsert = new tbl_Franchise_Services()
                                {
                                    FranchiseID = franchise.FranchiseID,
                                    ServiceID = int.Parse(serviceElements[i].ToString())
                                };
                                edb.tbl_Franchise_Services.AddObject(serviceToInsert);
                                edb.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                                insertedServiceRows++;
                            }
                            else
                                skipedServiceRows++;
                        }
                    }

                    // ZIP
                    foreach (var zip in zipCodes)
                    {
                        // Check if ZipCode exists in tbl_Franchise_ZipList
                        totalZipRows = (from fzip in edb.tbl_Franchise_ZipList
                                        where (fzip.FranchiseID == franchiseId && fzip.FranchiseZipID == zip.ZipCodeNumber)
                                        select fzip.ZipID).Count();
                        if (totalZipRows == 0)
                        {
                            // Insert a new row
                            tbl_Franchise_ZipList zipToInsert = new tbl_Franchise_ZipList()
                            {
                                FranchiseZipID = zip.ZipCodeNumber,
                                FranchiseID = franchise.FranchiseID,
                                ActiveYN = true,
                                DateAdded = DateTime.Now,
                                DateRemoved = null,
                                OwnedYN = false,
                                ServicesYN = true,
                                City = zip.Comment.Length >= 50 ? zip.Comment.Substring(0, 47) + "..." : zip.Comment,
                                State = franchise.LegalState,
                                Country = "USA",
                                CallTakerMessage = null
                            };
                            edb.tbl_Franchise_ZipList.AddObject(zipToInsert);
                            edb.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                            insertedZipRows++;
                        }
                        else
                            skipedZipRows++;
                    }

                    // DBA
                    foreach (var dba in dbas)
                    {
                        // Check if DBA Name exists in tbl_Dispatch_DBA
                        totalDBARows = (from fdba in edb.tbl_Dispatch_DBA
                                        where (fdba.FranchiseID == franchiseId && fdba.DBAName == dba.DbaName)
                                        select fdba.DBAID).Count();
                        if (totalDBARows == 0)
                        {
                            // Insert a new row
                            tbl_Dispatch_DBA dbaToInsert = new tbl_Dispatch_DBA()
                            {
                                FranchiseID = franchise.FranchiseID,
                                DBAName = dba.DbaName,
                                DBAIMage = null
                            };
                            edb.tbl_Dispatch_DBA.AddObject(dbaToInsert);
                            edb.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                            insertedDBARows++;
                        }
                        else
                            skipedDBARows++;
                    }

                    // Tax Rate
                    foreach (var tax in taxRates)
                    {
                        // Check if Tax Rate exists in tbl_TaxRates
                        totalTaxRows = (from ftax in edb.tbl_TaxRates
                                        where (ftax.FranchiseId == franchiseId && ftax.LaborAmount == tax.Value)
                                        select ftax.TaxRateID).Count();
                        if (totalTaxRows == 0)
                        {
                            // Insert a new row
                            tbl_TaxRates taxToInsert = new tbl_TaxRates()
                            {
                                FranchiseId = franchise.FranchiseID,
                                TaxDescription = tax.Value.ToString(),
                                LaborAmount = (float)tax.Value,
                                PartsAmount = (float)tax.Value,
                                AccountCode = "20601", // TODO Anis: find what to insert into account code
                                ActiveYN = true
                            };
                            edb.tbl_TaxRates.AddObject(taxToInsert);
                            edb.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                            insertedTaxRows++;
                        }
                        else
                            skipedTaxRows++;
                    }

                    // Mark the transaction as complete
                    transation.Complete();
                    success = true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            if (success)
            {
                edb.AcceptAllChanges();
                return Json(new
                {
                    Message = "All data updated with success.",
                    ContactRowInserted = insertedContactRows,
                    ContactRowSkiped = skipedContactRows,
                    ZIPRowInserted = insertedZipRows,
                    ZIPRowSkiped = skipedZipRows,
                    DBARowInserted = insertedDBARows,
                    DBARowSkiped = skipedDBARows,
                    TaxRowInserted = insertedTaxRows,
                    TaxRowSkiped = skipedTaxRows,
                    ServiceRowInserted = insertedServiceRows,
                    ServiceRowSkiped = skipedServiceRows,
                    Success = true
                });
            }

            return Json(new
            {
                Message = errorMessage,
                Success = false
            });

        }

    }
}
