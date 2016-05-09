using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Controllers;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    public class SignatureController : SiteBlueBaseController
    {
        public ActionResult Get(int id, int PictureID)
        {
            byte[] imageBytes;
            using (var db = new EightHundredEntities(Guid.NewGuid()))
            {
                var sigs = db.tbl_Job.Select(j => new { j.JobID, j.AuthorizationToStart, j.AcceptedBy }).SingleOrDefault(j => j.JobID == id);

                imageBytes = PictureID == 0 ? sigs.AuthorizationToStart : sigs.AcceptedBy;
            }

            if (imageBytes == null || imageBytes.Length == 0)
                imageBytes = System.IO.File.ReadAllBytes(Server.MapPath("/Content/images/noimage.png"));

            var ms = new MemoryStream(imageBytes);
            Response.Clear();
            Response.Expires = 0;
            Response.AddHeader("Content-Length", Convert.ToString(ms.Length));
            Response.BufferOutput = false;

            return new FileStreamResult(ms, "image/png");
        }
    }
}