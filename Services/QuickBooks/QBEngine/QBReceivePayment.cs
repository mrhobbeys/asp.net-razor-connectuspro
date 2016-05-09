using System;
using System.Collections.Generic;
using System.Text;
using QBFC10Lib;

namespace QBEngine
{
    public class QBReceivePayment:QBBase
    {

        public string CreatePayment(ReceivePayment Payment)
        {


            requestMsgSet.ClearRequests();
            IReceivePaymentAdd PaymentAddRq = requestMsgSet.AppendReceivePaymentAddRq();
            
           
            PaymentAddRq.CustomerRef.FullName.SetValue(Payment.CustomerName);
            PaymentAddRq.Memo.SetValue(Payment.Memo);
            PaymentAddRq.TotalAmount.SetValue(Payment.TotalAmount) ;           
            
            //Adding Invoice Reference ......
            if (!string.IsNullOrEmpty(Payment.InvoiceTxnID))
            {

                IAppliedToTxnAdd TxnAdd = PaymentAddRq.ORApplyPayment.AppliedToTxnAddList.Append();
                TxnAdd.TxnID.SetValue(Payment.InvoiceTxnID);
                TxnAdd.PaymentAmount.SetValue(Payment.TotalAmount);
                // TxnAdd.PaymentAmount.SetValue(Payment.TotalAmount);
            }
            else {

                PaymentAddRq.ORApplyPayment.IsAutoApply.SetValue(true);
            }

            string xml = requestMsgSet.ToXMLString();
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            if (responseMsgSet.ResponseList.GetAt(0).StatusCode == 0)
            {
                IResponse response = responseMsgSet.ResponseList.GetAt(0);

                //the request-specific response is in the details, make sure we have some
                if (response.Detail != null)
                {
                    //make sure the response is the type we're expecting
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtReceivePaymentAddRs)
                    {
                        //upcast to more specific type here, this is safe because we checked with response.Type check above
                        IReceivePaymentRet PaymentRet = (IReceivePaymentRet)response.Detail;
                        if (PaymentRet != null)
                            return PaymentRet.EditSequence.GetValue();

                    }

                }
                return "-1";
            }
            else
            {
                throw new QBException(responseMsgSet.ResponseList.GetAt(0).StatusCode, "QBEngine :" + responseMsgSet.ResponseList.GetAt(0).StatusMessage,requestMsgSet.ToXMLString());
                return "-1";
            }

        }
    }
}
