using System.ServiceModel;

namespace SiteBlue.DocumentGeneration
{
    [ServiceContract]
    public interface IRenderDocuments
    {
        [OperationContract]
        RenderResult PDFFromHtml(string html);
        [OperationContract]
        RenderResult PDFFromUrl(string url);
    }
    
}
