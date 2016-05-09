using System.ServiceModel;
namespace SiteBlue.DocumentGeneration
{
    [ServiceContract]
    public interface IGenerateInvoices
    {
        [OperationContract]
        RenderResult Render(Invoices.Invoice invoice, byte[][] docsToAppend);

        [OperationContract]
        RenderResult RenderHtml(Invoices.Invoice invoice);

        [OperationContract]
        RenderResult RenderCustom(Invoices.Invoice invoice, string templatePath, byte[][] docsToAppend);

        [OperationContract]
        RenderResult RenderHtmlCustom(Invoices.Invoice invoice, string templatePath);

    }
}
