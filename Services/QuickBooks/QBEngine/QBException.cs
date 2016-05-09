using System;
using System.Text;
using AppEngine.Utilities;

namespace QBEngine
{
   public class QBException:System.Exception  
    {
       int statusCode;
       string error;
       string QBXml;

       public override string Message {
        get { return error; }
        }

       public QBException(int statusCode,string error) {
           this.statusCode = statusCode;
           this.error = error;

           if (LogManager.Instance != null)
               LogManager.Instance.Error(error);
           else
               LogManager.GetLogManager(null).Info(error);

       }
       public QBException(int statusCode, string error,string QBXml)
       {
           this.statusCode = statusCode;
           this.error = error;
           this.QBXml = QBXml;

           if (LogManager.Instance != null)
               LogManager.Instance.Error(QBXml);
           else 
               LogManager.GetLogManager(null).Info(QBXml);

       }
       public override string ToString() {

           return "QBException :"+error;
       }
    }
}
