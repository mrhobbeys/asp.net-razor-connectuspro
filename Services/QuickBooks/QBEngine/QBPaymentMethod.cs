using System;
using System.Collections.Generic;
using System.Text;
using Common;
using QBFC10Lib ;

namespace QBEngine
{
    public class QBPaymentMethod : QBBase
    {
        List<PaymentMethod> MethodList = null;
        public List<PaymentMethod> GetPaymentMethod()
        {
            requestMsgSet.ClearRequests();
            IPaymentMethodQuery PaymentMethodQueryRq = requestMsgSet.AppendPaymentMethodQueryRq();
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            WalkPaymentMethodQueryRs(responseMsgSet);
            return MethodList;

        }


        void WalkPaymentMethodQueryRs(IMsgSetResponse responseMsgSet)
        {
            if (responseMsgSet == null) return;
            IResponseList responseList = responseMsgSet.ResponseList;
            if (responseList == null) return;
            //if we sent only one request, there is only one response, we'll walk the list for this sample
            for (int i = 0; i < responseList.Count; i++)
            {
                MethodList = new List<PaymentMethod>();
                IResponse response = responseList.GetAt(i);
                //check the status code of the response, 0=ok, >0 is warning
                if (response.StatusCode >= 0)
                {
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType == ENResponseType.rtPaymentMethodQueryRs)
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            IPaymentMethodRetList PaymentMethodRet = (IPaymentMethodRetList)response.Detail;
                            int count = PaymentMethodRet.Count;
                            for (int a = 0; a < count; a++)
                                MethodList.Add(WalkPaymentMethodRet(PaymentMethodRet.GetAt(a)));
                        }
                    }
                }
            }
        }



        PaymentMethod WalkPaymentMethodRet(IPaymentMethodRet PaymentMethodRet)
        {

            if (PaymentMethodRet == null) return null;
            PaymentMethod Method = new PaymentMethod();
            Method.ListID = (string)PaymentMethodRet.ListID.GetValue();
            Method.TimeCreated = (DateTime)PaymentMethodRet.TimeCreated.GetValue();            
            Method.TimeModified = (DateTime)PaymentMethodRet.TimeModified.GetValue();            
            Method.EditSequence = (string)PaymentMethodRet.EditSequence.GetValue();            
            Method.Name = (string)PaymentMethodRet.Name.GetValue();            
            if (PaymentMethodRet.IsActive != null)
            {
                Method.IsActive = (bool)PaymentMethodRet.IsActive.GetValue();
            }
            //Get value of PaymentMethodType
            //if (PaymentMethodRet.PaymentMethodType != null)
            //{
            //ENPaymentMethodType PaymentMethodType270 = (ENPaymentMethodType)PaymentMethodRet.PaymentMethodType.GetValue();
            //}

            return Method;
        }
    }
}
