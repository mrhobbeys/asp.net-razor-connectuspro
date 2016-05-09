using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace QBServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IQBOperation" in both code and config file together.
    [ServiceContract]
    public interface IQBOperation
    {
        [OperationContract]
        void DoWork();
    }
}
