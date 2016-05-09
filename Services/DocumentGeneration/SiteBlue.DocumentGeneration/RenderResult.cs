using System.Runtime.Serialization;

namespace SiteBlue.DocumentGeneration
{
    [DataContract]
    public class RenderResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public byte[] Data { get; set; }

        [DataMember]
        public string ExceptionMessage { get; set; }
    }
}