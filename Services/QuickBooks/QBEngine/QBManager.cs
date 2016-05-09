using System;
using System.Text;
using QBFC10Lib;

namespace QBEngine
{
  

  public class QBManager:IManager {


      static QBManager Instance;

      bool connectionOpen;
      bool sessionBegun;
      
      string AppID;
      string Application;

      QBSessionManager sessionManager = null;
      IMsgSetRequest requestMsgSet = null;
      IMsgSetResponse responseMsgSet = null;

      string country;
      short mjrVersion;
      short mnrVersion;

    
       QBSessionManager IManager.sessionManager {

          get {
              if (sessionBegun)
                  return sessionManager;
              else
                  return null;
          }
      }
       IMsgSetRequest IManager.requestMsg {
          get { return requestMsgSet; }
      }

      string IManager.Country {

          get {
             return  this.country;
          }

                
      }

      short IManager.MjrVersion
      {

          get
          {
             return this.mjrVersion;
          }


      }

      short IManager.MnrVersion
      {

          get
          {
             return this.mnrVersion;
          }


      }

      private QBManager() {
          
          }
      public bool Init(string AppID, string Application,string CompanyFile) {
          
          this.AppID = AppID;
          this.Application = Application;
          
              try
              {
                  if (!sessionBegun)
                  {
                      sessionManager = new QBSessionManager();
                      sessionManager.OpenConnection(AppID, Application);
                      connectionOpen = true;
                      sessionManager.BeginSession(CompanyFile, ENOpenMode.omDontCare);
                      sessionBegun = true;
                  }
              }
              catch (Exception e)
              {
                  if (sessionBegun)
                      sessionManager.EndSession();
                  if (connectionOpen)
                      sessionManager.CloseConnection();
                  throw new QBException(00, e.ToString());
              }
          


          return sessionBegun;
  }
      public bool Init(string AppID, string Application)
      {   
          this.AppID = AppID;
          this.Application = Application;
              try
              {
                  if (!sessionBegun)
                  {
                      sessionManager = new QBSessionManager();
                      sessionManager.OpenConnection(AppID, Application);
                      connectionOpen = true;
                      sessionManager.BeginSession("", ENOpenMode.omDontCare);
                      sessionBegun = true;
                  }
              }
              catch (Exception e)
              {
                  if (sessionBegun)
                      sessionManager.EndSession();
                  if (connectionOpen)
                      sessionManager.CloseConnection();
                  throw new QBException(00, e.ToString());
              }
          return sessionBegun;
      }

      /// <summary>
      ///  Don't require Major and Minor version 
      /// </summary>
      /// <param name="country"></param>
      /// <returns></returns>
      public bool connect(string country) { 
       
          try
          {
              Array array = this.getSupportedVersion();
              mjrVersion =(short) double.Parse(array.GetValue(array.Length - 1).ToString());
              mnrVersion = 0;//double.Parse(array.GetValue(0).ToString());
              this.connect(country, this.mjrVersion, this.mnrVersion);
              requestMsgSet = sessionManager.CreateMsgSetRequest(this.country, this.mjrVersion, this.mnrVersion);
          }
          catch (Exception e) {
              return false;
              throw new QBException(01, e.ToString());
          }
          return true;
      
      }
      /// <summary>
      /// Obsolete Method 
      /// </summary>
      /// <param name="country"></param>
      /// <param name="mjrVersion"></param>
      /// <param name="mnrVersion"></param>
      /// <returns></returns>
      public bool connect(string country, short mjrVersion, short mnrVersion) {
          this.country = country;
          this.mjrVersion = mjrVersion;
          this.mnrVersion = mnrVersion;
          try
          {
              requestMsgSet = sessionManager.CreateMsgSetRequest(this.country, this.mjrVersion, this.mnrVersion);
          }
          catch (Exception e) {
              return false;
              throw new QBException(01, e.ToString());
          }
          return true;
      }
      public Array getSupportedVersion() { 
      
          
            Array array =sessionManager.QBXMLVersionsForSession;

            return array;
       
      
      }

      public bool disconnect() {

          if(sessionBegun)
          sessionManager.EndSession();
          sessionBegun = false;
          if(connectionOpen)
          sessionManager.CloseConnection();
          connectionOpen = false;

          return !connectionOpen;
      }
      public string GetCompanyName()
      {


          ICompanyQuery CompanyQueryRq = requestMsgSet.AppendCompanyQueryRq();
          responseMsgSet = sessionManager.DoRequests(requestMsgSet);
          ICompanyRet CompanyRet = null;
          if (responseMsgSet == null) return null;
          IResponseList responseList = responseMsgSet.ResponseList;
          if (responseList == null) return null;
          for (int i = 0; i < responseList.Count; i++)
          {
              IResponse response = responseList.GetAt(i);

              if (response.StatusCode >= 0)
              {
                  if (response.Detail != null)
                  {
                      ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                      if (responseType == ENResponseType.rtCompanyQueryRs)
                      {

                          CompanyRet = (ICompanyRet)response.Detail;
                          
                      }

                  }

              }

          }

          return (string)CompanyRet.CompanyName.GetValue();

      }


      public static QBManager getManager() {
          if(Instance == null)
          Instance = new QBManager();

          return Instance;
      }


    }
}
