using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Data;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Data.Audit
{
    internal interface IAudit<T> : IAudit where T: EntityObject
    {
    }

    internal interface IAudit
    {
        Guid AuditEntryID { get; set; }
        Guid AuditID { get; set; }
        string Attribute { get; set; }
        string OldValue { get; set; }
        string NewValue { get; set; }
        AuditLog AuditLog { get; set; }
    }
}
