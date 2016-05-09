using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SiteBlue.Core.Enterprise.DocumentGeneration;
using System.Security.Principal;

namespace SiteBlue.Core.DocumentGeneration
{
    public static class PDFEngine
    {
        private static IRenderDocuments CreateClient()
        {
            var svc = new RenderDocumentsClient();

            svc.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(GlobalConfiguration.DocumentGenerationUser, GlobalConfiguration.DocumentGenerationPassword, string.Empty);
            svc.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Identification;  


            //svc.ClientCredentials.UserName.UserName = GlobalConfiguration.DocumentGenerationUser;
            //svc.ClientCredentials.UserName.Password = GlobalConfiguration.DocumentGenerationPassword;
            
            return svc;
        }

        /// <summary>
        /// Renders a PDF from a HTML string.
        /// </summary>
        /// <param name="html">The HTML string to render into PDF.</param>
        /// <returns>A stream containing the PDF data.</returns>
        public static DocumentResult GetPDF(string html)
        {
            DocumentResult result;

            try
            {
                var svc = CreateClient();
                var svcResult = svc.PDFFromHtml(html);
                result = !svcResult.Success 
                    ? new DocumentResult {Success = false, Message = svcResult.ExceptionMessage} 
                    : new DocumentResult {Success = true, Data = new MemoryStream(svcResult.Data)};
            }
            catch (Exception ex)
            {
                //TODO: Log exception
                return new DocumentResult {Success = false, Message = ex.Message};
            }

            if (result.Data != null && result.Data.CanSeek)
            {
                result.Data.Seek(0, SeekOrigin.Begin);
                result.Data.Position = 0;
            }

            return result;
        }

        /// <summary>
        /// Renders a PDF from a URL.
        /// </summary>
        /// <param name="html">The URL to render into PDF.</param>
        /// <returns>A stream containing the PDF data.</returns>
        public static DocumentResult GetPDF(Uri uri)
        {
            DocumentResult result;

            try
            {
                var svc = CreateClient();
                var svcResult = svc.PDFFromUrl(uri.ToString());
                result = !svcResult.Success
                    ? new DocumentResult { Success = false, Message = svcResult.ExceptionMessage }
                    : new DocumentResult { Success = true, Data = new MemoryStream(svcResult.Data) };
            }
            catch (Exception ex)
            {
                //TODO: Log exception
                return new DocumentResult { Success = false, Message = ex.Message };
            }

            if (result.Data != null && result.Data.CanSeek)
            {
                result.Data.Seek(0, SeekOrigin.Begin);
                result.Data.Position = 0;
            }

            return result;
        }

        /// <summary>
        /// Renders a PDF from an HTML string and saves it to the local file system.
        /// </summary>
        /// <param name="html">The HTML string to render into PDF.</param>
        /// <param name="path">The full path and file name of the file to save.</param>
        /// <returns>A flag indicating whether the render/save was successful or not.</returns>
        public static bool StorePDF(string html, string path)
        {
            try
            {
                var svc = CreateClient();
                var result = svc.PDFFromHtml(html);
                
                if (result.Success)
                    File.WriteAllBytes(path, result.Data);

                return result.Success;
            }
            catch (Exception)
            {
                //TODO: Log exception
                return false;
            }
        }

        ///// <summary>
        ///// Renders a PDF from a URL and saves it to the local file system.
        ///// </summary>
        ///// <param name="html">The URL to render into PDF.</param>
        ///// <param name="path">The full path and file name of the file to save.</param>
        ///// <returns>A flag indicating whether the render/save was successful or not.</returns>
        public static bool StorePDF(Uri uri, string path)
        {
            try
            {
                var svc = CreateClient();
                var result = svc.PDFFromUrl(uri.ToString());

                if (result.Success)
                    File.WriteAllBytes(path, result.Data);

                return result.Success;
            }
            catch (Exception)
            {
                //TODO: Log exception
                return false;
            }
        }

      
    }
}
