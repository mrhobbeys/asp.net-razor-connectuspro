using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace SiteBlue.DocumentGeneration
{
    public class Invoice : IGenerateInvoices
    {
        private const string ResourcePath = "~/Invoices/";

        public RenderResult Render(Invoices.Invoice invoice, byte[][] docsToAppend)
        {
            return RenderCustom(invoice, ResourcePath + "Template/DefaultInvoiceTemplate.xslt", docsToAppend);
        }

        public RenderResult RenderHtml(Invoices.Invoice invoice)
        {
            return RenderHtmlCustom(invoice, ResourcePath + "Template/DefaultInvoiceTemplate.xslt");
        }

        public RenderResult RenderHtmlCustom(Invoices.Invoice invoice, string templatePath)
        {
            return RenderInternal(invoice, templatePath, null, true);
        }

        public RenderResult RenderCustom(Invoices.Invoice invoice, string templatePath, byte[][] docsToAppend)
        {
            return RenderInternal(invoice, templatePath, docsToAppend, false);
        }

        private static RenderResult RenderInternal(Invoices.Invoice invoice, string templatePath, ICollection<byte[]> docsToAppend, bool asHtml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Invoices.Invoice));
                string html;

                using (var ms = new MemoryStream())
                {
                    serializer.Serialize(ms, invoice);

                    if (ms.CanSeek)
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.Position = 0;
                    }

                    var xmlDoc = new XPathDocument(ms);
                    var xsl = new XslCompiledTransform();

                    var path = templatePath.StartsWith("~/") ? HostingEnvironment.MapPath(templatePath) : templatePath;
                    xsl.Load(path);

                    using (var outstream = new MemoryStream())
                    {
                        using (var writer = XmlWriter.Create(outstream, xsl.OutputSettings))
                        {

                            xsl.Transform(xmlDoc, writer);

                            if (outstream.CanSeek)
                            {
                                outstream.Seek(0, SeekOrigin.Begin);
                                outstream.Position = 0;
                            }

                            var sr = new StreamReader(outstream);
                            html = sr.ReadToEnd();
                        }
                    }
                }

                var renderer = new Render();
                var result = asHtml
                                 ? new RenderResult { Success = true, Data = Encoding.Unicode.GetBytes(html) }
                                 : renderer.PDFFromHtml(html);

                if (!asHtml)
                {
                    var allDocs = new List<byte[]> { result.Data };

                    var termsPath = HostingEnvironment.MapPath(string.Format("{0}{1}/{2}_terms.pdf", ResourcePath, "Terms", invoice.FranchiseId));

                    if (File.Exists(termsPath))
                        allDocs.Add(File.ReadAllBytes(termsPath));

                    if (docsToAppend != null && docsToAppend.Count > 0)
                        allDocs.AddRange(docsToAppend);

                    result.Data = renderer.MergePdfDocuments(allDocs.ToArray());
                }

                return result;
            }
            catch (Exception ex)
            {
                return new RenderResult { Success = false, ExceptionMessage = ex.Message, Data = null };
            }
        }
    }
}
