using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using SiteBlue.Business.Enterprise.TechnicianBioGeneration;
using SiteBlue.Core.Email;
using SiteBlue.Core.Enterprise.DocumentGeneration;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Business.Employee
{
    public class EmployeeService : AbstractBusinessService
    {
        private class BioCacheItem
        {
            public string FileName { get; set; }
            public byte[] Data { get; set; }
            public DateTime Date { get; set; }
        }

        private const int CacheDurationInMinutes = 30;
        private static readonly Dictionary<int, BioCacheItem> BioCache = new Dictionary<int, BioCacheItem>();

        public void PublishBio(int techId)
        {
            if (BioCache.ContainsKey(techId)) BioCache.Remove(techId);
        }

        public OperationResult<KeyValuePair<string, byte[]>> PreviewBio(int techId)
        {
            var result = new OperationResult<KeyValuePair<string, byte[]>>();
            try
            {
                var renderResult = GetBioInternal(techId, true);
                result.Success = renderResult.Success;
                result.Message = renderResult.Message;

                if (result.Success)
                    result.ResultData = new KeyValuePair<string, byte[]>(renderResult.ResultData.FileName,
                                                                         renderResult.ResultData.Data);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public OperationResult<KeyValuePair<string, byte[]>> GetBio(int techId)
        {
            var result = new OperationResult<KeyValuePair<string, byte[]>>();
            try
            {
                var renderResult = GetBioInternal(techId, false);
                result.Success = renderResult.Success;
                result.Message = renderResult.Message;

                if (result.Success)
                    result.ResultData = new KeyValuePair<string, byte[]>(renderResult.ResultData.FileName,
                                                                         renderResult.ResultData.Data);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        private OperationResult<BioCacheItem> GetBioInternal(int techId, bool bypassCache)
        {
            var result = new OperationResult<BioCacheItem>();
            BioCacheItem item;

            if (bypassCache || !BioCache.ContainsKey(techId) || BioCache[techId] == null ||
                BioCache[techId].Date < DateTime.Now.AddMinutes(CacheDurationInMinutes))
            {
                Bio bio;
                using (var db = new EightHundredEntities(UserKey))
                {
                    var info = (from t in db.tbl_Employee
                                join b in db.tbl_Dispatch_TechBios
                                    on t.EmployeeID equals b.ServiceProID
                                join f in db.tbl_Franchise
                                    on t.FranchiseID equals f.FranchiseID
                                where t.EmployeeID == techId && t.ServiceProYN && t.ActiveYN
                                select new {Tech = t, Bio = b, Franchise = f}).SingleOrDefault();

                    if (info == null)
                    {
                        result.Message = "Tech not found or no bio defined.";
                        return result;
                    }

                    bio = new Bio
                              {
                                  Name = info.Tech.Employee,
                                  TechId = info.Tech.EmployeeID,
                                  Text = info.Bio.BioText,
                                  ClientId = info.Tech.FranchiseID,
                                  IsFranchise = info.Franchise.FranchiseTypeID == 6,
                                  LastDrugTest = info.Bio.LastDrugTest.GetValueOrDefault(),
                                  BackgroundCheckCompleted = info.Bio.BackgroundCheckCompleted.GetValueOrDefault()
                              };
                }

                RenderResult renderResult;
                using (var client = new GenerateTechBiosClient())
                {
                    renderResult = client.RenderBio(bio);
                }

                if (!renderResult.Success)
                {
                    result.Message = renderResult.ExceptionMessage;
                    return result;
                }

                item = new BioCacheItem
                           {
                               Date = DateTime.UtcNow,
                               Data = renderResult.Data,
                               FileName = BuildFileName(bio.Name) + ".pdf"
                           };

                if (!bypassCache)
                    BioCache.Add(techId, item);
            }
            else
                item = BioCache[techId];

            result.Success = true;
            result.ResultData = item;
            return result;
        }

        public OperationResult<bool> SendTechnicianBio(int techId, string recipients)
        {
            var result = new OperationResult<bool>();
            try
            {
                var renderResult = GetBioInternal(techId, false);

                if (!renderResult.Success)
                {
                    result.Message = renderResult.Message;
                    return result;
                }

                var e = new EmailEngine();
                var cacheItem = BioCache[techId];
                using (var s = new MemoryStream(cacheItem.Data))
                {
                    s.Seek(0, SeekOrigin.Begin);
                    s.Position = 0;

                    var att = new Attachment(s, cacheItem.FileName, "application/pdf");
                    e.Send(null, recipients, null, null, "Technician Bio", "", new[] {att}, true);
                }

                result.Success = true;
                result.ResultData = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }

            return result;
        }

        private static string BuildFileName(string fromString)
        {
            if (string.IsNullOrWhiteSpace(fromString))
                fromString = "Technician Bio";

            return Path.GetInvalidFileNameChars()
                       .Aggregate(fromString, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
