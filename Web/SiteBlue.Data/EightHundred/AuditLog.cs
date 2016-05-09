using System.Text;
using System.Data.Objects.DataClasses;
using System.Data;

namespace SiteBlue.Data.EightHundred
{
    partial class AuditLog
    {
        internal EntityObject EntityBeingAudited { get; set; }
        internal EntityKey KeyValue { get; set; }
    }
}
