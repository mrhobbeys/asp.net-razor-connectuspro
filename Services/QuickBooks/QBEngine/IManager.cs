using System;
using System.Text;
using QBFC10Lib;

namespace QBEngine
{
   public interface IManager
    {
        
        
       string Country
       {
           get;
          

       }
       short MjrVersion
       {

           get;
       }
       short MnrVersion
       {
           get;
       }

       QBSessionManager sessionManager
        {
            get;
            
        }
         IMsgSetRequest requestMsg
        {
            get;

        }
    }
}
