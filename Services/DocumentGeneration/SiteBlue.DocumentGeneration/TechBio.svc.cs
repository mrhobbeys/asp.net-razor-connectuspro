using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using SiteBlue.DocumentGeneration.TechBios;

namespace SiteBlue.DocumentGeneration
{
    public class TechBio : IGenerateTechBios
    {
        private const string ResourcePath = "~/TechBios/";

        public RenderResult RenderBio(Bio bio)
        {
            return RenderInternal(bio, ResourcePath + "Template/DefaultBioTemplate.xslt", false);
        }

        public RenderResult RenderBioHtml(Bio bio)
        {
            return RenderInternal(bio, ResourcePath + "Template/DefaultBioTemplate.xslt", true);
        }

        public RenderResult RenderBioHtmlCustom(Bio bio, string templatePath)
        {
            return RenderInternal(bio, templatePath, true);
        }

        public RenderResult RenderBioCustom(Bio bio, string templatePath)
        {
            return RenderInternal(bio, templatePath, false);
        }

        public RenderResult RenderBadge(Bio bio)
        {
            return RenderInternal(bio, ResourcePath + "Template/DefaultBioTemplate.xslt", false);
        }

        public RenderResult RenderBadgeCustom(Bio bio, string templatePath)
        {
            return RenderInternal(bio, templatePath, false);
        }

        public RenderResult RenderBadgeHtml(Bio bio)
        {
            return RenderInternal(bio, ResourcePath + "Template/DefaultBioTemplate.xslt", true);
        }

        public RenderResult RenderBadgeHtmlCustom(Bio bio, string templatePath)
        {
            return RenderInternal(bio, templatePath, true);
        }

        private static RenderResult RenderInternal(Bio bio, string templatePath, bool asHtml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Bio));
                string html;

                using (var ms = new MemoryStream())
                {
                    serializer.Serialize(ms, bio);

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

                return result;
            }
            catch (Exception ex)
            {
                return new RenderResult { Success = false, ExceptionMessage = ex.Message, Data = null };
            }
        }
    }
}
