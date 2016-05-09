using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using EO.Pdf;

namespace SiteBlue.DocumentGeneration
{
    public class Render : IRenderDocuments
    {
        public Render()
        {
            Runtime.AddLicense(ConfigurationManager.AppSettings["PDFLicense"]);
        }

        public RenderResult PDFFromUrl(string url)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    HtmlToPdf.ConvertUrl(url, stream);
                    return new RenderResult { Success = true, Data = stream.GetBuffer() };
                }
            }
            catch (Exception ex)
            {
                return new RenderResult { Success = false, ExceptionMessage = ex.Message };
            }
        }

        public RenderResult PDFFromHtml(string html)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    HtmlToPdf.ConvertHtml(html, stream);
                    return new RenderResult { Success = true, Data = stream.GetBuffer() };
                }
            }
            catch (Exception ex)
            {
                return new RenderResult { Success = false, ExceptionMessage = ex.Message };
            }
        }

        internal byte[] MergePdfDocuments(params byte[][] docs)
        {
            if (docs == null || docs.Length == 0) return null;
            if (docs.Length == 1) return docs[0];

            var docList = new List<PdfDocument>();

            foreach (var doc in docs)
            {
                using (var toAdd = new MemoryStream(doc))
                {
                    docList.Add(new PdfDocument(toAdd));
                }
            }

            var mergedDoc = PdfDocument.Merge(docList.ToArray());

            using (var merged = new MemoryStream())
            {
                mergedDoc.Save(merged);
                return merged.GetBuffer();
            }
        }
    }
}
