using System;
using System.Collections.Generic;
using System.Linq;
using SiteBlue.Areas.HVAC_App.Models.SetupModels;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.HVAC_App.Models
{
    public class VerificationResult
    {
        public string NameOfVerification { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class VerificationHvacData
    {
        private readonly EightHundredEntities _context;
        private readonly int _franchiseId;
        private readonly List<VerificationResult> _listOfVerification;
        private readonly int _configId;

        private readonly string[] _systemcodes = new[] { "ACAH1", "ACGFV", "ACAH1", "HPGFV", "HPAH1", "PKGFV", "PKHE1", "PKAC1" };
        private readonly int[] _tonscodes = new[] { 18, 24, 30, 36, 42, 48, 60 };

        public VerificationHvacData(EightHundredEntities context, int frId, int configId)
        {
            _context = context;
            _franchiseId = frId;
            _configId = configId;
            _listOfVerification = new List<VerificationResult>();
        }

        public List<VerificationResult> RunVerification()
        {
            var verPriceBookId = VerificationPriceBookId();
            _listOfVerification.Add(verPriceBookId);
            var verSelectedSystems = VerificationSelectedSystem();
            _listOfVerification.Add(verSelectedSystems);
            var verJobCodes = VerificationExistCodes((string[])verSelectedSystems.Data, (int)verPriceBookId.Data);
            _listOfVerification.Add(verJobCodes);
            var verPartsCodes = VerificationExistPartCodes((string[])verSelectedSystems.Data, (int)verPriceBookId.Data);
            _listOfVerification.Add(verPartsCodes);
            _listOfVerification.Add(VerificationDuplicationPartsIntoJobs((string[])verSelectedSystems.Data, (int)verPriceBookId.Data));
            _listOfVerification.Add(VerificationLogoImage());
            _listOfVerification.Add(VerificationGuaranteeTexts());
            return _listOfVerification;
        }

        private int GetPriceBookId()
        {
            var priceBook =
                _context.tbl_HVAC_ConfigFranchise.First(item => item.FranchiseID == _franchiseId).PricebookID;
            var priceBookId = priceBook.HasValue ? priceBook.Value : -1;
            return priceBookId;
        }

        public VerificationResult VerificationPriceBookId()
        {
            var pricebookId = GetPriceBookId();
            var verPriceBookId = new VerificationResult
                                     {
                                         NameOfVerification = "Company linked to HVAC Pricebook:",
                                         Result = true,
                                         Message =
                                             String.Format("Company linked to {0} Pricebook", pricebookId),
                                         Data = pricebookId
                                     };
            if (pricebookId == -1)
            {
                verPriceBookId.Result = false;
                verPriceBookId.Message = "The company is NOT linked to a HVAC Pricebook";
            }
            return verPriceBookId;
        }

        private VerificationResult VerificationSelectedSystem()
        {
            var list = GetSelectedSystems();
            var verfication = new VerificationResult
                                  {
                                      NameOfVerification = "Systems selected",
                                      Result = false,
                                      Message = String.Format("No packages selected"),
                                      Data = new string[0]
                                  };
            if (list.Any())
            {
                verfication.Result = true;
                verfication.Message = list.Aggregate("", (t, i) => t + " " + i) + " have been selected";
                verfication.Data = list.ToArray();
            }
            return verfication;
        }

        private List<string> GetSelectedSystems()
        {
            var list = _context.tbl_HVAC_ConfigSystems.Where(item => item.ConfigID == _configId).OrderBy(i => i.OrderNum).Select(i=>i.tbl_HVAC_SystemType.SystemTypeName).ToList();
            return list;
        }

        private VerificationResult VerificationExistCodes(string[] systemsId, int pricebookId)
        {
            var getAllPackages = _context.View_HVAC_APP.Where(item => item.PriceBookID == pricebookId).Select(i => i.JobCode).ToList();
            var verfication = new VerificationResult
                                  {
                                      NameOfVerification = "Package codes verification",
                                      Result = true,
                                      Message = String.Format("All codes have info")
                                  };
            var missInfo = from sys in systemsId
                           from systemcode in _systemcodes
                           from tonscode in _tonscodes
                           select string.Format("{0}-{1}{2}", sys, systemcode, tonscode.ToString())
                               into code
                               where !getAllPackages.Contains(code)
                               select code;
            if (missInfo.Count() > 0)
            {
                verfication.Result = false;
                verfication.Message = "This package does not exist in your pricebook.";
            }
            verfication.Data = missInfo.ToList();
            return verfication;
        }

        private VerificationResult VerificationExistPartCodes(string[] systemsId,int pricebookId)
        {
            var getAllPackages = _context.View_HVAC_APP_Parts.Where(item => item.PriceBookID == pricebookId).Select(i => i.JobCode).ToList();
            var verfication = new VerificationResult
                                  {
                                      NameOfVerification = "Verification part codes",
                                      Result = true,
                                      Message = String.Format("All jobs have one or more parts")
                                  };
            var missInfo = from systemcode in _systemcodes
                           from sys in systemsId
                           from tonscode in _tonscodes
                           select string.Format("{0}-{1}{2}", sys, systemcode, tonscode.ToString())
                           into code
                           where !getAllPackages.Contains(code)
                           select code;
            if (missInfo.Any())
            {
                verfication.Result = false;
                verfication.Message = "No parts info for jobs: ";
                foreach (var code in missInfo)
                {
                    verfication.Message += code + " ";
                }
            }
            return verfication;
        }

        private VerificationResult VerificationDuplicationPartsIntoJobs(string[] systemsId,int pricebookId)
        {
            var getAllPackages = _context.View_HVAC_APP_Parts.Where(item => item.PriceBookID == pricebookId).GroupBy(i=>i.JobCode).ToList();

            var verfication = new VerificationResult
            {
                NameOfVerification = "Verification duplication part codes",
                Result = true,
                Message = String.Format("Duplication parts:\n")
            };
            foreach (var package in getAllPackages)
            {
                foreach (var part in package.GroupBy(i=>i.PartID))
                {
                    if (part.Count()>1)
                    {
                        verfication.Result = false;
                        verfication.Message += String.Format("JobCode {2} have duplicated Part ID = {0} ({1})\n",
                                                             part.First().PartID, part.First().PartName,
                                                             part.First().JobCode);
                    }
                }
                
            }
            if (verfication.Result)
            {
                verfication.Message = "No duplication parts in jobs";
            }
            return verfication;
        }

        private VerificationResult VerificationLogoImage()
        {
            var logo = new LogoModel(_configId, _context);
            var result = new VerificationResult
                             {
                                 NameOfVerification = "Company Logo linked",
                                 Result = false,
                                 Message = "No URL is linked to this company"
                             };
            if (!String.IsNullOrEmpty(logo.Url))
            {
                result.Result = true;
                result.Message = String.Format("The logo URL is {0}", logo.Url);
            }
            return result;
        }

        private VerificationResult VerificationGuaranteeTexts()
        {
            var texts = GuaranteeModel.GuaranteeModels(_context, _configId);
            var result = new VerificationResult
                             {
                                 NameOfVerification = "Guarantee Text Validation",
                                 Result = true,
                                 Message = ""
                             };
            foreach (var guaranteeModel in texts)
            {
                if (String.IsNullOrEmpty(guaranteeModel.GuaranteeText))
                {
                    result.Result = false;
                    result.Message += String.Format("Guarantee {0} has no text\n", guaranteeModel.GuaranteeName);
                }
            }
            if (result.Result)
                result.Message = "All guarantees have texts";
            return result;
        }
    }

    
}
