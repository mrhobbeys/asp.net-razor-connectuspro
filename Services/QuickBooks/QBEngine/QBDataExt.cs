using System;
using System.Collections.Generic;
using System.Text;
using Common ;
using QBFC10Lib;
namespace QBEngine
{
    public class QBDataExt :QBBase
    {
        string VaultID = "VaultID";

        public DataExt CreateDataExt(DataExt dataExt) {

            requestMsgSet.ClearRequests();
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
            IDataExtDefAdd DataDef = requestMsgSet.AppendDataExtDefAddRq() ;
            DataDef.OwnerID.SetValue("0");
            DataDef.DataExtName.SetValue(VaultID);
            DataDef.DataExtType.SetValue(ENDataExtType.detSTR255TYPE) ;
            DataDef.AssignToObjectList.Add(ENAssignToObject.atoCustomer) ;
            IDataExtAdd DataExtAddRq = requestMsgSet.AppendDataExtAddRq();
            
            DataExtAddRq.OwnerID.SetValue("0");            
            DataExtAddRq.DataExtName.SetValue(VaultID);                
            DataExtAddRq.ORListTxnWithMacro.ListDataExt.ListDataExtType.SetValue(ENListDataExtType.ldetCustomer);                
            DataExtAddRq.ORListTxnWithMacro.ListDataExt.ListObjRef.ListID.SetValue(dataExt.ListID);
            //DataExtAddRq.ORListTxnWithMacro.ListDataExt.ListObjRef.FullName.SetValue("ab");
            //Set field value for DataExtValue
            DataExtAddRq.DataExtValue.SetValue(dataExt.DataExtValue);

            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            IResponseList responseList = responseMsgSet.ResponseList;
            IResponse response = responseMsgSet.ResponseList.GetAt(0);
            if (response.StatusCode == 0)
            {
                //the request-specific response is in the details, make sure we have some
                if (response.Detail != null)
                {
                    //make sure the response is the type we're expecting
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtDataExtDefAddRs)
                    {

                        IDataExtDefRet DataExtRet = (IDataExtDefRet)response.Detail;
                        if (DataExtRet == null) return null;


                        if (DataExtRet.OwnerID != null)
                        {
                            dataExt.OwnerID = (string)DataExtRet.OwnerID.GetValue();
                        }
                        //Get value of DataExtName
                        dataExt.DataExtName = (string)DataExtRet.DataExtName.GetValue();
                        //Get value of DataExtValue
                        //dataExt.DataExtValue = (string)DataExtRet. .DataExtValue.GetValue();
                    }
                    
                }                
            }
            else
            {

                throw new QBException(response.StatusCode, response.StatusMessage.ToString(), requestMsgSet.ToXMLString());
            }

            return dataExt;
        }


        public DataExt UpdateDataExt(DataExt dataExt) {

            requestMsgSet.ClearRequests();
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
            IDataExtMod DataExtMod = requestMsgSet.AppendDataExtModRq();

            
            DataExtMod.DataExtName.SetValue(VaultID);
            DataExtMod.DataExtValue.SetValue(dataExt.DataExtValue);
            DataExtMod.OwnerID.SetValue( "0");
            DataExtMod.ORListTxn.ListDataExt.ListDataExtType.SetValue(ENListDataExtType.ldetCustomer);
            DataExtMod.ORListTxn.ListDataExt.ListObjRef.ListID.SetValue(dataExt.ListID);

            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            IResponseList responseList = responseMsgSet.ResponseList;
            IResponse response = responseMsgSet.ResponseList.GetAt(0);
            if (response.StatusCode == 0)
            {
                //the request-specific response is in the details, make sure we have some
                if (response.Detail != null)
                {
                    //make sure the response is the type we're expecting
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtDataExtModRs)
                    {

                        //IDataExtMod DataExtRet = (IDataExtMod)response.Detail;
                        //if (DataExtRet == null) return null;


                        //if (DataExtRet.OwnerID != null)
                        //{
                        //    dataExt.OwnerID = (string)DataExtRet.OwnerID.GetValue();
                        //}
                        ////Get value of DataExtName
                        //dataExt.DataExtName = (string)DataExtRet.DataExtName.GetValue();
                        ////Get value of DataExtValue
                        ////dataExt.DataExtValue = (string)DataExtRet. .DataExtValue.GetValue();
                    }

                }
            }
            else
            {

                throw new QBException(response.StatusCode, response.StatusMessage.ToString(), requestMsgSet.ToXMLString());
            }

            return dataExt;
        }
    }
}
