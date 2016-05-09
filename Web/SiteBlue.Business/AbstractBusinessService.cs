using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business
{
    public abstract class AbstractBusinessService
    {
        public Guid UserKey { get; protected set; }

        public static T Create<T>(Guid userKey) where T : AbstractBusinessService
        {
            var svc = (T) Activator.CreateInstance(typeof (T));
            svc.UserKey = userKey;

            return svc;
        }
    }
}
