using System;
using System.Collections.Generic;
using System.Text;
using QBFC10Lib;

namespace QBEngine
{
  public class QBBase
    {

        protected QBSessionManager sessionManager = null;
        protected IMsgSetRequest requestMsgSet = null;
        protected IMsgSetResponse responseMsgSet = null;
        protected IManager Manager = null;
        


        public void setQBManager(IManager Manager)
        {
            this.Manager = Manager;
            this.sessionManager = Manager.sessionManager;
            this.requestMsgSet = Manager.requestMsg;
        }
    }
}
