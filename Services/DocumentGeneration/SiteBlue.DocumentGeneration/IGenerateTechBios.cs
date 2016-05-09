using System.ServiceModel;
using SiteBlue.DocumentGeneration.TechBios;

namespace SiteBlue.DocumentGeneration
{
    [ServiceContract]
    public interface IGenerateTechBios
    {
        [OperationContract]
        RenderResult RenderBadge(Bio bio);

        [OperationContract]
        RenderResult RenderBio(Bio bio);

        [OperationContract]
        RenderResult RenderBioHtml(Bio bio);

        [OperationContract]
        RenderResult RenderBioCustom(Bio bio, string templatePath);

        [OperationContract]
        RenderResult RenderBioHtmlCustom(Bio bio, string templatePath);
    }
}
