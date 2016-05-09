using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Business;
using SiteBlue.Business.Employee;
using SiteBlue.Controllers;

namespace SiteBlue.Areas.Tech.Controllers
{
    [Authorize]
    public class BadgeController : SiteBlueBaseController
    {
        public FileResult Preview(int id)
        {
            return GetBatchInternal(id, true);
        }

        public FileResult View(int id)
        {
            return GetBatchInternal(id, false);
        }

        private FileResult GetBatchInternal(int id, bool preview)
        {
            var contentType = "application/pdf";
            var result = preview 
                        ? AbstractBusinessService.Create<EmployeeService>(UserInfo.UserKey).PreviewBio(id)
                        : AbstractBusinessService.Create<EmployeeService>(UserInfo.UserKey).GetBio(id);
            var fileName = result.ResultData.Key;
            var data = result.ResultData.Value;

            if (!result.Success)
            {
                contentType = "text/plain";
                fileName = "error.txt";
                data = Encoding.Unicode.GetBytes(result.Message);
            }

            return new FileStreamResult(new MemoryStream(data), contentType) { FileDownloadName = fileName };
        }

        [HttpPost]
        public JsonResult Approve(int id)
        {
            try
            {
                AbstractBusinessService.Create<EmployeeService>(UserInfo.UserKey).PublishBio(id);
                return Json(new { Success = true, Message = "Your new bio is now live!" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }

        }
    }
}