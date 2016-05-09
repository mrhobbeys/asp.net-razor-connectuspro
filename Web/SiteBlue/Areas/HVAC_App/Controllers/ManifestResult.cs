using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVACapp.Areas.HVAC_App.Controllers
{
    public class ManifestResult: ActionResult
    {
        private string _manifest;

        public ManifestResult(string textOfManifest)
        {
            _manifest = textOfManifest;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "text/cache-manifest";
            context.HttpContext.Response.Output.WriteLine("CACHE MANIFEST");
            context.HttpContext.Response.Output.WriteLine(_manifest);
        }
    }
}